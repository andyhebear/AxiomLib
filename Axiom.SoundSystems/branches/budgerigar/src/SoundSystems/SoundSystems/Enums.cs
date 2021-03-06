﻿#region MIT License
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

namespace Axiom.SoundSystems
{
    /// <summary>
    /// The kind of a <see cref="SoundInstance"/>
    /// </summary>
    public enum SoundKind
    {
        /// <summary>
        /// A simple sound, usable for ambient effects and music replay
        /// </summary>
        Simple = 1,
        /// <summary>
        /// A spatially positionable sound
        /// </summary>
        Spatial = 2
    }

    /// <summary>
    /// State of a <see cref="SoundInstance"/>
    /// </summary>
    public enum SoundState
    {
        /// <summary>
        /// The sound is invalidated. For example after disposal.
        /// </summary>
        Invalid,

        /// <summary>
        /// The sound is stopped
        /// </summary>
        Stopped,

        /// <summary>
        /// The sound is paused
        /// </summary>
        Paused,

        /// <summary>
        /// The sound is playing
        /// </summary>
        Playing
    }
}
