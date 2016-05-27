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
    /// Allows to define a sequence of <see cref="IEmitterEffect"/> effects that are applied to
    /// a specified set of <see cref="ISoundEmitter"/> objects in a sequence. 
    /// I.e. next effect will be applied when a previous has finished.
    /// </summary>
    /// <remarks>
    /// A simple example of a sequence would be a volume-fade-in/no-effect/volume-fade-out sequence,
    /// that can make a sound start softly, play it for a specified amount of time and softly end the replay.
    /// </remarks>
    /// <todo>
    /// TODO Implement
    /// </todo>
    public class EffectSequence
    {
        private List<ISoundEmitter> _emitters;
        /// <summary>
        /// List of emitters to apply the sequence to.
        /// </summary>
        public List<ISoundEmitter> Emitters
        {
            get { return _emitters; }
            set { _emitters = value; }
        }

        private EffectList _sequence;
        /// <summary>
        /// Defines the effect sequence.
        /// </summary>
        public EffectList Sequence
        {
            get { return _sequence; }
            set { _sequence = value; }
        }

        ///// <summary>
        ///// Updates
        ///// </summary>
        //public void Update()
        //{
        //}
    }
}
