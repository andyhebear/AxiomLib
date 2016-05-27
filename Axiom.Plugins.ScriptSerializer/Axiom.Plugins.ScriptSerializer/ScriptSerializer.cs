#region MIT License
/*
-----------------------------------------------------------------------------
This source file is part of Axiom ScriptSerializer Plugin
Copyright © 2011 Ali Akbar

This is a C# port for Axiom of Ogre ScriptSerializer plugin,
developed by Ali Akbar and ported by Francesco Guastella (aka romeoxbm).
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
                                                                              
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
                                                                              
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#endregion

#region SVN Version Information
// <file>
//     <id value="$Id$"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using Axiom.Core;
using Axiom.Plugins.ScriptSerializer.ScriptBlock;
using Axiom.Scripting.Compiler.AST;

using AbstractNodeList = System.Collections.Generic.IList<Axiom.Scripting.Compiler.AST.AbstractNode>;
using BlockEntryList = System.Collections.Generic.IList<Axiom.Plugins.ScriptSerializer.ScriptBlock.BlockEntry>;
using ParentEntry = System.Collections.Generic.KeyValuePair<Axiom.Scripting.Compiler.AST.AbstractNode, Axiom.Plugins.ScriptSerializer.ScriptBlock.ObjectASNTransitionType>;
using ParentStack = System.Collections.Generic.Stack<System.Collections.Generic.KeyValuePair<Axiom.Scripting.Compiler.AST.AbstractNode, Axiom.Plugins.ScriptSerializer.ScriptBlock.ObjectASNTransitionType>>;
using ResourceID = System.UInt32;
using SerializeStack = System.Collections.Generic.Stack<Axiom.Plugins.ScriptSerializer.ScriptBlock.BlockEntry>;

#endregion Namespace Declarations

namespace Axiom.Plugins.ScriptSerializer
{
    namespace ScriptBlock
    {
        enum ScriptBlockType
        {
            /// <summary>
            /// Block type containing the node data
            /// </summary>
            Node = 0x01,

            /// <summary>
            /// Block type containing the tree transition data
            /// </summary>
            Transition = 0x02,

            StringTable = 0x03
        };

        enum TreeTransitionDirection
        {
            Up = 0x01,
            Down = 0x02
        };

        enum ObjectASNTransitionType
        {
            Children = 0x01,
            Values = 0x02,
            Overrides = 0x03
        };

        struct ScriptHeader
        {
            internal UInt32 Magic;
            internal UInt16 Version;
            internal DateTime LastModifiedTime;
            internal Int64 StringTableOffset;
        };

        struct ScriptBlockHeader
        {
            internal ScriptBlockType BlockClass; //it was uint8
            internal AbstractNodeType BlockType;
            internal UInt32 BlockID;
        };

        struct TransitionBlock
        {
            internal TreeTransitionDirection Direction;
            internal ObjectASNTransitionType UserData;
        };

        struct AbstractNodeBlock
        {
            internal UInt32 LineNumber;
        };

        struct AtomAbstractNodeBlock
        {
            internal AbstractNodeBlock NodeInfo;
            internal UInt32 Id;
            internal ResourceID Value;
        };

        struct PropertyAbstractNodeBlock
        {
            internal AbstractNodeBlock NodeInfo;
            internal UInt32 Id;
            internal ResourceID Name;
        };

        struct StringTableBlock
        {
            internal int Count;
        };

        struct ResourceArrayHeader
        {
            internal int Count;
        };

        struct ObjectAbstractNodeBlock
        {
            internal AbstractNodeBlock NodeInfo;
            internal ResourceID Name;
            internal ResourceID Cls;
            internal UInt32 Id;
            internal bool IsAbstract;
            internal ResourceArrayHeader Bases;
            internal ResourceArrayHeader EnvironmentVars;
        };

        abstract class BlockEntry
        {
            internal ScriptBlockType BlockClass { get; set; }
        };

        class NodeBlockEntry : BlockEntry
        {
            internal AbstractNode Node { get; private set; }

            internal NodeBlockEntry( AbstractNode node )
                : base()
            {
                this.Node = node;
                this.BlockClass = ScriptBlockType.Node;
            }
        };

        class TransitionBlockEntry : BlockEntry
        {
            internal TreeTransitionDirection Direction { get; private set; }
            internal ObjectASNTransitionType UserData { get; private set; }

            internal TransitionBlockEntry( TreeTransitionDirection direction, ObjectASNTransitionType userData )
                : base()
            {
                this.Direction = direction;
                this.UserData = userData;
                this.BlockClass = ScriptBlockType.Transition;
            }
        };

        class StringTable
        {
            #region Fields

            private Dictionary<ResourceID, string> _reverseLookup;
            private Dictionary<string, ResourceID> _table;
            private uint _idCounter;

            #endregion Fields

            internal StringTable()
            {
                this._reverseLookup = new Dictionary<ResourceID, string>();
                this._table = new Dictionary<string, ResourceID>();
                this._idCounter = 0;
            }

            public ResourceID RegisterString( string data )
            {
                if ( string.IsNullOrEmpty( data ) )
                    return 0;

                if ( this._table.ContainsKey( data ) )
                    return this._table[ data ];

                ResourceID id = ++this._idCounter;
                this._table.Add( data, id );
                this._reverseLookup.Add( id, data );
                return id;
            }

            public void SetKeyValue( ResourceID id, string data )
            {
                this._table[ data ] = id;
                this._reverseLookup[ id ] = data;
            }

            public void Clear()
            {
                this._reverseLookup.Clear();
                this._table.Clear();
                this._idCounter = 0;
            }

            public string GetString( ResourceID id )
            {
                if ( id == 0 )
                    return string.Empty;

                if ( !this._reverseLookup.ContainsKey( id ) )
                    throw new AxiomException( "Cannot find resource string with specified id" );

                return this._reverseLookup[ id ];
            }

            public Dictionary<string, ResourceID> GetTable()
            {
                return this._table;
            }
        };
    }; // End of ScriptBlock Namespace

    internal class ScriptSerializer
    {
        #region Fields

        private readonly UInt32 magicCode = ( 'A' | 'X' << 8 | 'I' << 16 | 'O' << 24 | 'M' << 32 );
        private readonly UInt16 version = 0x0001;
        private UInt32 _blockIdCounter;
        private StringTable _stringTable;

        #endregion Fields

        internal ScriptSerializer()
        {
            this._blockIdCounter = 0;
            this._stringTable = new StringTable();
        }

        internal void Serialize( Stream stream, AbstractNodeList ast, DateTime scriptTimestamp )
        {
            this._blockIdCounter = 0;
            this._stringTable.Clear();

            using ( BinaryWriter writer = new BinaryWriter( stream ) )
            {
                ScriptHeader header;
                header.Magic = magicCode;
                header.Version = version;
                header.LastModifiedTime = scriptTimestamp;
                header.StringTableOffset = 0; // Will be overwritten later
                this._writeToStream( writer, header );

                SerializeStack s = new SerializeStack();
                this._writeStackChildren( ref s, ast );

                // Traverse all the trees and their nodes in depth-first order
                while ( s.Count != 0 )
                {
                    BlockEntry entry = s.Peek();
                    s.Pop();

                    this._writeBlock( writer, entry );

                    if ( entry.BlockClass == ScriptBlockType.Node )
                    {
                        NodeBlockEntry nodeEntry = (NodeBlockEntry)entry;
                        AbstractNode node = nodeEntry.Node;
                        if ( node is PropertyAbstractNode )
                        {
                            PropertyAbstractNode propertyNode = (PropertyAbstractNode)node;
                            this._writeStackChildren( ref s, propertyNode.Values );
                        }
                        else if ( node is ObjectAbstractNode )
                        {
                            ObjectAbstractNode objectNode = (ObjectAbstractNode)node;
                            this._writeStackChildren( ref s, objectNode.Children, ObjectASNTransitionType.Children );
                            this._writeStackChildren( ref s, objectNode.Values, ObjectASNTransitionType.Values );
                            this._writeStackChildren( ref s, objectNode.Overrides, ObjectASNTransitionType.Overrides );
                        }
                    }
                }

                // Write the string table
                header.StringTableOffset = stream.Position;
                this._writeStringTable( writer );

                // Re-write the header with the correct string table offset
                writer.Seek( 0, SeekOrigin.Begin );
                this._writeToStream( writer, header );
            }
        }

        internal AbstractNodeList Deserialize( Stream stream )
        {
            AbstractNodeList trees = new List<AbstractNode>();

            using ( BinaryReader reader = new BinaryReader( stream ) )
            {
                ScriptHeader header;
                this._readFromStream( reader, out header );

                if ( header.Magic != magicCode )
                    throw new AxiomException( "Binary file is not in correct format!" );

                else if ( header.Version != version )
                    throw new AxiomException( "Binary script is in an older format.  Please reparse the script" );

                // Seek to the string table
                stream.Seek( header.StringTableOffset, SeekOrigin.Begin );
                this._stringTable.Clear();
                this._readStringTable( reader );

                // Seek back to the start for reading the node data
                stream.Seek( Memory.SizeOf( typeof( ScriptHeader ) ), SeekOrigin.Begin );

                ParentStack parentStack = new ParentStack();
                AbstractNode previousNode = null;

                string filename = stream is FileStream ? Path.GetFileName( ( (FileStream)stream ).Name ) : string.Empty;

                while ( true )
                {
                    ScriptBlockHeader blockHeader = new ScriptBlockHeader();
                    this._readFromStream( reader, out blockHeader );
                    //int headerSize = Memory.SizeOf( typeof( ScriptBlockHeader ) );
                    //stream.Seek( -headerSize, SeekOrigin.Current );

                    if ( blockHeader.BlockClass == ScriptBlockType.Transition )
                    {
                        TransitionBlock block = new TransitionBlock();
                        this._readFromStream( reader, out block );

                        if ( block.Direction == TreeTransitionDirection.Down )
                        {
                            parentStack.Push( new ParentEntry( previousNode, block.UserData ) );
                        }
                        else if ( block.Direction == TreeTransitionDirection.Up )
                        {
                            previousNode = parentStack.Peek().Key;
                            parentStack.Pop();
                        }
                        else
                            throw new AxiomException( "Unsupported direction type" );
                    }
                    else if ( blockHeader.BlockClass == ScriptBlockType.Node )
                    {
                        AbstractNode parent = parentStack.Peek().Key;
                        AbstractNode asn = null;

                        if ( blockHeader.BlockType == AbstractNodeType.Atom )
                        {
                            AtomAbstractNodeBlock block = new AtomAbstractNodeBlock();
                            this._readFromStream( reader, out block );

                            AtomAbstractNode impl = new AtomAbstractNode( parent );
                            impl.File = filename;
                            impl.Line = block.NodeInfo.LineNumber;
                            impl.Value = this._stringTable.GetString( block.Value );
                            impl.Id = block.Id;
                            previousNode = impl;
                            asn = impl;
                        }
                        else if ( blockHeader.BlockType == AbstractNodeType.Property )
                        {
                            PropertyAbstractNodeBlock block = new PropertyAbstractNodeBlock();
                            this._readFromStream( reader, out block );

                            PropertyAbstractNode impl = new PropertyAbstractNode( parent );
                            impl.File = filename;
                            impl.Line = block.NodeInfo.LineNumber;
                            impl.Name = this._stringTable.GetString( block.Name );
                            impl.Id = block.Id;
                            previousNode = impl;
                            asn = impl;
                        }
                        else if ( blockHeader.BlockType == AbstractNodeType.Object )
                        {
                            ObjectAbstractNodeBlock block = new ObjectAbstractNodeBlock(); ;
                            this._readFromStream( reader, out block );

                            ObjectAbstractNode impl = new ObjectAbstractNode( parent );
                            impl.File = filename;
                            impl.Line = block.NodeInfo.LineNumber;
                            impl.Name = this._stringTable.GetString( block.Name );
                            impl.Cls = this._stringTable.GetString( block.Cls );
                            impl.Id = block.Id;
                            impl.IsAbstract = block.IsAbstract;

                            int baseCount = block.Bases.Count;
                            int envCount = block.EnvironmentVars.Count;

                            for ( int i = 0; i < baseCount; i++ )
                            {
                                ResourceID id;
                                this._readFromStream( reader, out id );
                                string Base = this._stringTable.GetString( id );
                                impl.Bases.Add( Base );
                            }

                            for ( int i = 0; i < envCount; i++ )
                            {
                                ResourceID keyId, valueId;
                                this._readFromStream( reader, out keyId );
                                this._readFromStream( reader, out valueId );

                                string key = this._stringTable.GetString( keyId );
                                string value = this._stringTable.GetString( valueId );

                                impl.SetVariable( key, value );
                            }

                            previousNode = impl;
                            asn = impl;
                        }

                        // Attach the node to the parent's appropriate child list
                        if ( parent != null )
                        {
                            if ( parent is PropertyAbstractNode )
                            {
                                PropertyAbstractNode propertyNode = (PropertyAbstractNode)parent;
                                propertyNode.Values.Add( asn );
                            }
                            else if ( parent is ObjectAbstractNode )
                            {
                                ObjectAbstractNode objectNode = (ObjectAbstractNode)parent;
                                ObjectASNTransitionType userData = parentStack.Peek().Value;

                                switch ( userData )
                                {
                                    case ObjectASNTransitionType.Children:
                                        objectNode.Children.Add( asn );
                                        break;

                                    case ObjectASNTransitionType.Overrides:
                                        objectNode.Overrides.Add( asn );
                                        break;

                                    case ObjectASNTransitionType.Values:
                                        objectNode.Values.Add( asn );
                                        break;

                                    default:
                                        throw new AxiomException( "Unsupported child type" );
                                }
                            }
                        }
                        else
                        {
                            // This node has no parent and is the root node
                            trees.Add( asn );
                        }
                    }
                    else if ( blockHeader.BlockClass == ScriptBlockType.StringTable )
                    {
                        break;
                    }
                }
            }

            return trees;
        }

        internal void GetHeader( Stream stream, out ScriptHeader header )
        {
            using ( BinaryReader reader = new BinaryReader( stream ) )
            {
                this._readFromStream( reader, out header );
            }
        }

        private void _readStringTable( BinaryReader reader )
        {
            ScriptBlockHeader blockHeader;
            this._readFromStream( reader, out blockHeader );

            if ( blockHeader.BlockClass != ScriptBlockType.StringTable )
                throw new AxiomException( "Error reading String Table" );

            StringTableBlock block;
            this._readFromStream( reader, out block );

            for ( int i = 0; i < block.Count; i++ )
            {
                ResourceID id = reader.ReadUInt32();
                string value = reader.ReadString();
                this._stringTable.SetKeyValue( id, value );
            }
        }

        private void _writeStringTable( BinaryWriter writer )
        {
            ScriptBlockHeader blockHeader;
            blockHeader.BlockID = ++this._blockIdCounter;
            blockHeader.BlockClass = ScriptBlockType.StringTable;
            blockHeader.BlockType = 0; // Not used

            StringTableBlock block;
            Dictionary<string, ResourceID> table = this._stringTable.GetTable();
            block.Count = table.Count;

            this._writeToStream( writer, blockHeader );
            this._writeToStream( writer, block );

            foreach ( KeyValuePair<string, ResourceID> currentPair in table )
            {
                ResourceID id = currentPair.Value;
                string data = currentPair.Key;

                writer.Write( id );
                writer.Write( data );
            }
        }

        private void _writeStackChildren( ref SerializeStack s, AbstractNodeList ast )
        {
            this._writeStackChildren( ref s, ast, 0 );
        }

        private void _writeStackChildren( ref SerializeStack s, AbstractNodeList children, ObjectASNTransitionType transitionUserdata )
        {
            BlockEntryList entries = new List<BlockEntry>();
            entries.Add( new TransitionBlockEntry( TreeTransitionDirection.Down, transitionUserdata ) );

            foreach ( AbstractNode currentNode in children )
            {
                BlockEntry entry = new NodeBlockEntry( currentNode );
                entries.Add( entry );
            }

            entries.Add( new TransitionBlockEntry( TreeTransitionDirection.Up, transitionUserdata ) );

            // pushing entries reversing order
            for ( int i = entries.Count - 1; i >= 0; i-- )
                s.Push( entries[ i ] );
        }

        private void _writeToStream<T>( BinaryWriter writer, T t )
        {
            byte[] buffer = new byte[ Memory.SizeOf( typeof( T ) ) ];
            IntPtr addr = Memory.PinObject( buffer );
            Marshal.StructureToPtr( t, addr, false );
            writer.Write( buffer );
            Memory.UnpinObject( buffer );
        }

        private void _readFromStream<T>( BinaryReader reader, out T t )
        {
            Type currentType = typeof( T );
            byte[] buffer = reader.ReadBytes( Memory.SizeOf( currentType ) );
            IntPtr addr = Memory.PinObject( buffer );
            t = (T)Marshal.PtrToStructure( addr, currentType );
            Memory.UnpinObject( buffer );
        }

        private void _writeBlock( BinaryWriter writer, BlockEntry entry )
        {
            if ( entry.BlockClass == ScriptBlockType.Transition )
            {
                TransitionBlockEntry transitionEntry = (TransitionBlockEntry)entry;
                ScriptBlockHeader blockHeader;
                blockHeader.BlockID = ++this._blockIdCounter;
                blockHeader.BlockClass = ScriptBlockType.Transition;
                blockHeader.BlockType = 0; // Not used

                TransitionBlock block;
                block.Direction = transitionEntry.Direction;
                block.UserData = transitionEntry.UserData;
                this._writeToStream( writer, blockHeader );
                this._writeToStream( writer, block );
            }
            else if ( entry.BlockClass == ScriptBlockType.Node )
            {
                NodeBlockEntry nodeEntry = (NodeBlockEntry)entry;
                AbstractNode node = nodeEntry.Node;
                if ( node is AtomAbstractNode )
                {
                    ScriptBlockHeader blockHeader;
                    AtomAbstractNode atomNode = (AtomAbstractNode)node;
                    blockHeader.BlockClass = ScriptBlockType.Node;
                    blockHeader.BlockType = AbstractNodeType.Atom;
                    blockHeader.BlockID = ++this._blockIdCounter;

                    AtomAbstractNodeBlock block;
                    block.NodeInfo.LineNumber = atomNode.Line;
                    block.Id = atomNode.Id;
                    block.Value = this._stringTable.RegisterString( atomNode.Value );

                    this._writeToStream( writer, blockHeader );
                    this._writeToStream( writer, block );
                }
                else if ( node is PropertyAbstractNode )
                {
                    ScriptBlockHeader blockHeader;
                    PropertyAbstractNode propertyNode = (PropertyAbstractNode)node;
                    blockHeader.BlockClass = ScriptBlockType.Node;
                    blockHeader.BlockType = AbstractNodeType.Property;
                    blockHeader.BlockID = ++this._blockIdCounter;

                    PropertyAbstractNodeBlock block;
                    block.NodeInfo.LineNumber = propertyNode.Line;
                    block.Id = propertyNode.Id;
                    block.Name = this._stringTable.RegisterString( propertyNode.Name );

                    this._writeToStream( writer, blockHeader );
                    this._writeToStream( writer, block );
                }
                else if ( node is ObjectAbstractNode )
                {
                    ScriptBlockHeader blockHeader;
                    ObjectAbstractNode objectNode = (ObjectAbstractNode)node;
                    blockHeader.BlockClass = ScriptBlockType.Node;
                    blockHeader.BlockType = AbstractNodeType.Object;
                    blockHeader.BlockID = ++this._blockIdCounter;

                    ObjectAbstractNodeBlock block;
                    block.NodeInfo.LineNumber = objectNode.Line;
                    block.Name = this._stringTable.RegisterString( objectNode.Name );
                    block.Cls = this._stringTable.RegisterString( objectNode.Cls );
                    block.Id = objectNode.Id;
                    block.IsAbstract = objectNode.IsAbstract;
                    block.Bases.Count = objectNode.Bases.Count;
                    block.EnvironmentVars.Count = objectNode.Variables.Count;

                    this._writeToStream( writer, blockHeader );
                    this._writeToStream( writer, block );

                    // Write out the "bases" list
                    foreach ( string it in objectNode.Bases )
                    {
                        ResourceID id = _stringTable.RegisterString( it );
                        _writeToStream( writer, id );
                    }

                    // Write the environment variables
                    foreach ( KeyValuePair<string, string> currentPair in objectNode.Variables )
                    {
                        ResourceID keyID = this._stringTable.RegisterString( currentPair.Key );
                        ResourceID valueID = this._stringTable.RegisterString( currentPair.Value );

                        writer.Write( keyID );
                        writer.Write( valueID );
                    }
                }
            }
            else
            {
                throw new AxiomException( "Cannot serialize block of type:" + entry.BlockClass );
            }
        }
    }
}
