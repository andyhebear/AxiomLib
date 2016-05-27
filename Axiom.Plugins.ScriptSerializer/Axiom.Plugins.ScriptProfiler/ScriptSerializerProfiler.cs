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

using System.Diagnostics;
using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Plugins.ScriptProfiler
{
    internal class ScriptSerializerProfiler : DisposableObject, IResourceGroupListener
    {
        #region ScriptSerializerProfiler fields

        private const string serializerLogName = "SerializerProfiler.log";
        private int _scriptCount;
        private Stopwatch _scriptCompileStartTime;

        #endregion ScriptSerializerProfiler fields

        internal ScriptSerializerProfiler()
            : base()
        {
            _scriptCount = 0;
            _scriptCompileStartTime = new Stopwatch();
            LogManager.Instance.CreateLog( serializerLogName );
            ResourceGroupManager.Instance.AddResourceGroupListener( this );
        }

        protected override void dispose( bool disposeManagedResources )
        {
            if ( !this.IsDisposed )
            {
                if ( disposeManagedResources )
                {
                    ResourceGroupManager.Instance.RemoveResourceGroupListener( this );
                    _scriptCompileStartTime.Stop();
                    _scriptCompileStartTime = null;
                }
            }

            base.dispose( disposeManagedResources );
        }

        private void _loadStats()
        {
        }

        private void _logMessage( string message )
        {
            LogManager.Instance.GetLog( serializerLogName ).Write( "Serializer log: " + message );
        }

        #region IResourceGroupListener Members
        
        public void ScriptParseStarted( string scriptName, ref bool skipThisScript )
        {
            // DO NOTHING
        }

        public void ScriptParseEnded( string scriptName, bool skipped )
        {
            // DO NOTHING
        }

        public void ResourceGroupScriptingStarted( string groupName, int scriptCount )
        {
            _scriptCount = scriptCount;
            _scriptCompileStartTime.Start();
        }

        public void ResourceGroupScriptingEnded( string groupName )
        {
            _scriptCompileStartTime.Stop();
            string message = string.Format( "[{0}] {1} scripts parsed in {2} milliseconds.", groupName, _scriptCount, _scriptCompileStartTime.ElapsedMilliseconds );
            _logMessage( message );
            _scriptCompileStartTime.Reset();
        }

        public void ResourceGroupPrepareStarted( string groupName, int resourceCount )
        {
            // DO NOTHING
        }

        public void ResourcePrepareStarted( Resource resource )
        {
            // DO NOTHING
        }

        public void ResourcePrepareEnded()
        {
            // DO NOTHING
        }

        public void ResourceGroupPrepareEnded( string groupName )
        {
            // DO NOTHING
        }

        public void ResourceGroupLoadStarted( string groupName, int resourceCount )
        {
            // DO NOTHING
        }

        public void ResourceLoadStarted( Resource resource )
        {
            // DO NOTHING
        }

        public void ResourceLoadEnded()
        {
            // DO NOTHING
        }

        public void WorldGeometryStageStarted( string description )
        {
            // DO NOTHING
        }

        public void WorldGeometryStageEnded()
        {
            // DO NOTHING
        }

        public void ResourceGroupLoadEnded( string groupName )
        {
            // DO NOTHING
        }

        #endregion
    }
}
