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
using Axiom.SoundSystems.Effects;

namespace Axiom.SoundSystems
{
    /// <summary>
    /// Represents a 3D sound emitter. Used in combination with a <see cref="ISoundListener"/> the <see cref="SoundContext"/> can simulate 3D audio effects.
    /// </summary>
    public interface ISoundEmitter
    {
        /// <summary>
        /// Gets or sets the kind of the sound, simple or spatial.
        /// </summary>
        SoundKind Kind { get; set; }

        /// <summary>
        /// Gets or sets whether the sound is looping infinitely
        /// </summary>
        bool Loop { get; set; }

        /// <summary>
        /// Gets or sets a scalar that adjusts the effect of distance calculations on the sound
        /// </summary>
        /// <remarks>
        /// If sounds are attenuating too fast, which means the sounds get quiet too quickly as they move away from the listener, 
        /// you need to increase the DistanceScale. If sounds are not attenuating fast enough, decrease the DistanceScale.
        /// </remarks>
        float DistanceScale { get; set; }

        /// <summary>
        /// Gets or sets a scalar applied to the level of Doppler effect calculated between this and any <see cref="ISoundListener"/>. 
        /// </summary>
        float DopplerScale { get; set; }

        /// <summary>
        /// Gets or sets the speed of sound used with this emitter. In metres per second
        /// </summary>
        /// <remarks>
        /// Use to simulate different environments. The default value shall be 343.5f
        /// </remarks>
        float SpeedOfSound { get; set; }

        /// <summary>
        /// Gets or sets the position of this emitter. 
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// Gets or sets the orientation of this emitter
        /// </summary>
        Quaternion Orientation { get; set; }

        /// <summary>
        /// Gets or sets the forward orientation vector for this emitter
        /// </summary>
        Vector3 Forward { get; set; }

        /// <summary>
        /// Gets or sets the upward orientation vector for this emitter. 
        /// </summary>
        Vector3 Up { get; set; }

        /// <summary>
        /// Gets or sets the velocity vector of this emitter. Used for Doppler effect calculation.
        /// </summary>
        Vector3 Velocity { get; set; }

        /// <summary>
        /// Gets or sets the volume of this emitter.
        /// </summary>
        float Volume { get; set; }

        /// <summary>
        /// Gets or sets the panning for the emitter
        /// </summary>
        float Pan { get; set; }

        /// <summary>
        /// Gets or sets pitch value for the emitter
        /// </summary>
        float Pitch { get; set; }

        /// <summary>
        /// Gets a collection of <see cref="IEmitterEffect"/> effects for this emitter
        /// </summary>
        EffectList Effects { get; set; }
    }
}
