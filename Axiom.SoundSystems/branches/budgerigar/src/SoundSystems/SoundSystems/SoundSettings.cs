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
    /// The settings to be used when creating new sounds.
    /// </summary>
    public sealed class SoundSettings : ISoundEmitter
    {
        #region Constructor

        public SoundSettings()
        {
            // ISoundEmitter

            _kind = SoundKind.Simple;
            _loop = false;
            _distanceScale = 1.0f;
            _dopplerScale = 1.0f;
            _speedOfSound = SoundContext.DefaultSpeedOfSound;
            _position = Vector3.Zero;
            _orientation = Quaternion.Identity;
            _forward = Vector3.NegativeUnitZ;
            _up = Vector3.UnitY;
            _velocity = Vector3.Zero;
            _volume = 1.0f;
            _pan = 0.0f;
            _pitch = 0.0f;
            _effects = new EffectList();
        }

        #endregion

        #region ISoundEmitter

        private SoundKind _kind;
        /// <summary>
        /// 
        /// </summary>
        public SoundKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        private bool _loop;
        /// <summary>
        /// 
        /// </summary>
        public bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        private float _distanceScale;
        /// <summary>
        /// 
        /// </summary>
        public float DistanceScale
        {
            get { return _distanceScale; }
            set { _distanceScale = value; }
        }

        private float _dopplerScale;
        /// <summary>
        /// 
        /// </summary>
        public float DopplerScale
        {
            get { return _dopplerScale; }
            set { _dopplerScale = value; }
        }

        private float _speedOfSound;
        /// <summary>
        /// 
        /// </summary>
        public float SpeedOfSound
        {
            get { return _speedOfSound; }
            set { _speedOfSound = value; }
        }

        private Vector3 _position;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        private Quaternion _orientation;
        /// <summary>
        /// 
        /// </summary>
        public Quaternion Orientation
        {
            get { return _orientation; }
            set { _orientation = value; }
        }

        private Vector3 _forward;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Forward
        {
            get { return _forward; }
            set { _forward = value; }
        }

        private Vector3 _up;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Up
        {
            get { return _up; }
            set { _up = value; }
        }

        private Vector3 _velocity;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        private float _volume;
        /// <summary>
        /// 
        /// </summary>
        public float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        private float _pan;
        /// <summary>
        /// 
        /// </summary>
        public float Pan
        {
            get { return _pan; }
            set { _pan = value; }
        }

        private float _pitch;
        /// <summary>
        /// 
        /// </summary>
        public float Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        private EffectList _effects;
        /// <summary>
        /// 
        /// </summary>
        public EffectList Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        #endregion

        #region Methods

        public void CopyTo(ISoundEmitter emitter)
        {
            SoundContext.CopySettings(this, emitter);
        }

        #endregion
    }
}
