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
    public enum EffectKind
    {
        Once,
        Loop,
        PingPongOnce,
        PingPong
    }

    public enum EffectEventKind
    {
        /// <summary>
        /// The effect has started or resumed from disabled state
        /// </summary>
        Started,
        /// <summary>
        /// The effect has paused or was disabled
        /// </summary>
        Paused,
        /// <summary>
        /// The effect has finished
        /// </summary>
        Finished,
        /// <summary>
        /// The effect has changed it's evaluation, like changing direction in a <see cref="EffectKind.PingPong"/> operation
        /// </summary>
        Changed
    }
}
