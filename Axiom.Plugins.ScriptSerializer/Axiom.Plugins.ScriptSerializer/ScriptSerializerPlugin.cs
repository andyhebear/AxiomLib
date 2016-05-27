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

using Axiom.Core;

#endregion Namespace Declarations

namespace Axiom.Plugins.ScriptSerializer
{
    /// <summary>
    /// Plugin instance for Script Serializer
    /// </summary>
    public class ScriptSerializerPlugin : IPlugin
    {
        #region Plugin fields

        private ScriptSerializerManager _scriptSerializerManager;

        #endregion Plugin fields

        #region IPlugin Members

        public void Initialize()
        {
            this._scriptSerializerManager = new ScriptSerializerManager();
        }

        public void Shutdown()
        {
            if ( this._scriptSerializerManager != null )
            {
                if ( !this._scriptSerializerManager.IsDisposed )
                    this._scriptSerializerManager.Dispose();
            }
            this._scriptSerializerManager = null;
        }

        #endregion IPlugin Members
    }
}
