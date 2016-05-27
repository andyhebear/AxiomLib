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

using Axiom.Math;

namespace Axiom.SoundSystems
{
    /// <summary>
    /// Represents a 3D sound listener. Used in combination with a <see cref="ISoundEmitter"/> the <see cref="SoundContext"/> can simulate 3D audio effects.
    /// </summary>
    public interface ISoundListener
    {
        /// <summary>
        /// Gets or sets the position of this listener. 
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the orientation of this listener
        /// </summary>
        Quaternion Orientation { get; set; }

        /// <summary>
        /// Gets or sets the forward orientation vector for this listener. 
        /// </summary>
        Vector3 Forward {get;set;}

        /// <summary>
        /// Gets or sets the upward orientation vector for this listener. 
        /// </summary>
        Vector3 Up {get;set;}
 
        /// <summary>
        /// Gets or sets the velocity vector of this listener.
        /// </summary>
        Vector3 Velocity {get;set;}

    }
}
