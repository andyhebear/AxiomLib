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

namespace Axiom.SoundSystems.Effects
{
    /// <summary>
    /// Method delegate that is invoked when a <see cref="IEmitterEffect"/> changes it's state
    /// </summary>
    /// <param name="extent"></param>
    /// <param name="function"></param>
    public delegate void EffectEventHandler(object sender, EffectEventArgs e);

    /// <summary>
    /// 
    /// </summary>
    public class EffectEventArgs : EventArgs
    {
        public EffectEventArgs()
            : base()
        {
        }

        public EffectEventArgs(EffectEventKind eventKind, int numChanges)
            : base()
        {
            _eventKind = eventKind;
            _numChanges = numChanges;
        }

        private EffectEventKind _eventKind;
        /// <summary>
        /// 
        /// </summary>
        public EffectEventKind EventKind
        {
            get { return _eventKind; }
            set { _eventKind = value; }
        }

        private int _numChanges;
        /// <summary>
        /// The number of changes that have happened in the effect so far.
        /// </summary>
        /// <remarks>
        /// This won't be incremented when enabling/disabling the effect
        /// </remarks>
        public int NumChanges
        {
            get { return _numChanges; }
            set { _numChanges = value; }
        }
    }
}
