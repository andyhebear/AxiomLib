#region MIT License
/*
The MIT License

Copyright (c) 2010 Axiom Contrib Developers

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
*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Axiom.SoundSystems.Controllers
{
    /// <summary>
    /// Method delegate to be invoked when a value meets an extent
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void EmitterValueEventHandler(object sender, EmitterValueEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class EmitterValueEventArgs : EventArgs
    {
        public EmitterValueEventArgs()
            : base()
        {
        }

        public EmitterValueEventArgs(ValueExtent extent)
            : base()
        {
            _extent = extent;
        }

        private ValueExtent _extent;
        /// <summary>
        /// The value extent reached
        /// </summary>
        public ValueExtent Extent
        {
            get { return _extent; }
            set { _extent = value; }
        }
    }
}
