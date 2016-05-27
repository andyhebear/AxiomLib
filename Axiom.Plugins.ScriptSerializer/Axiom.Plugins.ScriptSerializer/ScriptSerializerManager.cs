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
using System.IO;
using Axiom.Core;
using Axiom.FileSystem;
using Axiom.Plugins.ScriptSerializer.ScriptBlock;
using Axiom.Scripting.Compiler;

using AbstractNodeList = System.Collections.Generic.IList<Axiom.Scripting.Compiler.AST.AbstractNode>;

#endregion Namespace Declarations

namespace Axiom.Plugins.ScriptSerializer
{
    /// <summary>
    /// This class registers and listens to various resource and compilation events and 
    /// handles loading/saving of the binary script from disk.  
    /// A cache folder is created in the working directory where the compiled scripts are dumped 
    /// to disk for later use. Whenever a text bases script is compiled, its AST in memory is saved to disk.
    /// Later when this script needs to be compiled, it would be skipped in favour of its corresponding binary version
    /// which is be directly loaded from disk and sent to the translators.
    /// The text based scripts can also be replaced with the binary version by removing them 
    /// and registering the cache folder in resources.cfg
    /// </summary>
    public sealed class ScriptSerializerManager : ScriptCompilerListener, IResourceGroupListener, IDisposable
    {
        #region ScriptSerializerManager fields

        private ScriptCompiler _compiler;
        private string _activeScriptName;
        private string _activeResourceGroup;
        private Archive _cacheArchive;

#if USE_MICROCODE_SHADERCACHE
        private ShaderSerializer _shaderSerializer;
#endif

        #endregion ScriptSerializerManager fields

        /// <summary>
        /// The binary file's extension that would be appended to original filename
        /// </summary>
        public readonly string binaryScriptExtension = ".sbin";

        /// <summary>
        /// The folder to save the compiled scripts in binary format
        /// </summary>
        public readonly string scriptCacheFolder = ".scriptCache";

        /// <summary>
        /// The filename to cache the shader microcodes
        /// </summary>
        public readonly string shaderCacheFilename = "ShaderCache";

        public ScriptSerializerManager()
            : base()
        {
            this.IsDisposed = false;
            this._compiler = new ScriptCompiler();
            this._initializeArchive( scriptCacheFolder );
            this._initializeShaderCache();
            ResourceGroupManager.Instance.AddResourceGroupListener( this );
            ScriptCompilerManager.Instance.OnPostConversion += this.PostConversion;

            // Register the binary extension in the proper load order
            ScriptCompilerManager.Instance.ScriptPatterns.Add( "*.program" + binaryScriptExtension );
            ScriptCompilerManager.Instance.ScriptPatterns.Add( "*.material" + binaryScriptExtension );
            ScriptCompilerManager.Instance.ScriptPatterns.Add( "*.particle" + binaryScriptExtension );
            ScriptCompilerManager.Instance.ScriptPatterns.Add( "*.compositor" + binaryScriptExtension );
        }

        private void _initializeArchive( string archiveName )
        {
            // Check if the directory exists
            if ( !Directory.Exists( archiveName ) )
            {
                // Directory does not exist. Create one
                Directory.CreateDirectory( archiveName );
            }

            this._cacheArchive = ArchiveManager.Instance.Load( archiveName, "Folder" );
        }

        private void _initializeShaderCache()
        {
#if USE_MICROCODE_SHADERCACHE
            this.mShaderSerializer = new ShaderSerializer();

            // Check if we the shaders have previously been cached
            if (this.mCacheArchive.Exists(shaderCacheFilename))
            {
                // A previously cached shader file exists.  load it
                using (Stream shaderCache = this.mCacheArchive.Open(shaderCacheFilename))
                {
                    this.mShaderSerializer.LoadCache(shaderCache);
                }
            }

            this.mShaderSerializer.Caching = true;
#endif
        }

        private void _saveShaderCache()
        {
#if USE_MICROCODE_SHADERCACHE
            // A previously cached shader file exists.  load it
            using (Stream shaderCache = this.mCacheArchive.Create(shaderCacheFilename))
            {
                this.mShaderSerializer.SaveCache(shaderCache);
            }
#endif
        }

        private bool _isBinaryScript( string scriptName )
        {
            string ext = Path.GetExtension( scriptName );

            if ( !string.IsNullOrEmpty( ext ) )
                return ext == binaryScriptExtension;

            return false;
        }

        private DateTime _getBinaryTimeStamp( string binaryFilename )
        {
            ScriptHeader header;

            using ( Stream stream = this._cacheArchive.Open( binaryFilename ) )
            {
                ScriptSerializer serializer = new ScriptSerializer();
                serializer.GetHeader( stream, out header );
            }

            return header.LastModifiedTime;
        }

        private AbstractNodeList loadAstFromDisk( string binaryFilename )
        {
            AbstractNodeList ast = null;

            using ( Stream stream = this._cacheArchive.Open( binaryFilename ) )
            {
                ScriptSerializer serializer = new ScriptSerializer();
                ast = serializer.Deserialize( stream );
            }

            return ast;
        }

        private void _saveAstToDisk( string binaryFilename, AbstractNodeList ast )
        {
            // A text script was just parsed. Save the compiled AST to disk
            using ( Stream stream = this._cacheArchive.Create( binaryFilename, true ) )
            {
                ScriptSerializer serializer = new ScriptSerializer();
                DateTime scriptTimestamp = ResourceGroupManager.Instance.ResourceModifiedTime( this._activeResourceGroup, this._activeScriptName );
                serializer.Serialize( stream, ast, scriptTimestamp );
            }
        }

        /// <summary>
        /// Interface ScriptCompilerListener
        /// </summary>
        /// <param name="compiler"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        public override bool PostConversion( ScriptCompiler compiler, AbstractNodeList nodes )
        {
            if ( !this._isBinaryScript( this._activeScriptName ) )
            {
                // A text script was just parsed. Save the compiled AST to disk
                string binaryFilename = this._activeScriptName + binaryScriptExtension;
                this._saveAstToDisk( binaryFilename, nodes );
            }

            return true;
        }

        #region IResourceGroupListener Members

        public void ResourceGroupScriptingStarted( string groupName, int scriptCount )
        {
            this._activeResourceGroup = groupName;
        }

        public void ScriptParseStarted( string scriptName, ref bool skipThisScript )
        {
            this._activeScriptName = scriptName;

            // Check if the binary version is being requested for parsing
            string binaryFilename;
            if ( this._isBinaryScript( scriptName ) )
            {
                // The script ends with the binary extension. fetch it from the cache folder
                binaryFilename = scriptName;
            }
            else
            {
                // This is a text based script.  Check if the compiled version is unavailable
                binaryFilename = scriptName + binaryScriptExtension;
                if ( !this._cacheArchive.Exists( binaryFilename ) )
                {
                    // A compiled version of this script doesn't exist in the cache.  Continue with regular text parsing
                    skipThisScript = false;
                    return;
                }

                // Check if this script was modified it was last compiled
                DateTime binaryTimestamp = this._getBinaryTimeStamp( binaryFilename );
                DateTime scriptTimestamp = ResourceGroupManager.Instance.ResourceModifiedTime( this._activeResourceGroup, scriptName );

                if ( scriptTimestamp > binaryTimestamp )
                {
                    LogManager.Instance.Write( "File Changed. Re-parsing file: " + scriptName );
                    skipThisScript = false;
                    return;
                }
            }

            // Load the compiled AST from the binary script file
            AbstractNodeList ast = this.loadAstFromDisk( binaryFilename );
            LogManager.Instance.Write( "Processing binary script: " + binaryFilename );
            this._compiler.Compile( ast, this._activeResourceGroup, false, false, false );

            // Skip further parsing of this script since its already been compiled
            skipThisScript = true;
        }

        public void ScriptParseEnded( string scriptName, bool skipped )
        {
            //DO NOTHING
        }

        public void ResourceGroupScriptingEnded( string groupName )
        {
            // Scripts for this resource group where just parsed.  save the shader cache to disk
            this._saveShaderCache();
        }

        public void ResourceGroupLoadStarted( string groupName, int resourceCount )
        {
            //DO NOTHING
        }

        public void ResourceLoadStarted( Resource resource )
        {
            //DO NOTHING
        }

        public void ResourceLoadEnded()
        {
            //DO NOTHING
        }

        public void WorldGeometryStageStarted( string description )
        {
            //DO NOTHING
        }

        public void WorldGeometryStageEnded()
        {
            //DO NOTHING
        }

        public void ResourceGroupLoadEnded( string groupName )
        {
            //DO NOTHING
        }

        public void ResourceGroupPrepareStarted( string groupName, int resourceCount )
        {
            //DO NOTHING
        }

        public void ResourcePrepareStarted( Resource resource )
        {
            //DO NOTHING
        }

        public void ResourcePrepareEnded()
        {
            //DO NOTHING
        }

        public void ResourceGroupPrepareEnded( string groupName )
        {
            //DO NOTHING
        }

        #endregion IResourceGroupListener Members

        #region IDisposable Members

        /// <summary>
        /// Determines if this instance has been disposed of already.
        /// </summary>
        public bool IsDisposed { get; private set; }

        public void Dispose()
        {
            if ( this.IsDisposed )
                return;

            ResourceGroupManager.Instance.RemoveResourceGroupListener( this );
            ScriptCompilerManager.Instance.OnPostConversion -= this.PostConversion;
            if ( this._cacheArchive != null )
            {
                this._cacheArchive.Unload();
                this._cacheArchive.Dispose();
            }
            this._cacheArchive = null;

            this._compiler = null;

            GC.SuppressFinalize( this );
            this.IsDisposed = true;
        }

        #endregion
    }
}
