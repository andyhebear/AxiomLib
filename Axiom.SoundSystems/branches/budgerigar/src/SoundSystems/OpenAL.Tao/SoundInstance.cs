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
using Tao.OpenAl;

namespace Axiom.SoundSystems.OpenAL.Tao
{
    /// <summary>
    /// OpenAL sound instance.
    /// </summary>
    public class SoundInstance : Axiom.SoundSystems.SoundInstance
    {
        #region Constructor

        internal SoundInstance(Sound creator, string name, int openALSound, SoundKind kind)
            : base(creator, name, kind)
        {
            _sound = openALSound;

            // create a sound source for the buffer
            Al.alGenSources(1, out _soundSource);

            // link the buffer and sound source
            Al.alSourcei(_soundSource, Al.AL_BUFFER, _sound);

            if (_kind == SoundKind.Simple)
            {
                Al.alSourcei(_soundSource, Al.AL_DISTANCE_MODEL, Al.AL_NONE);
                Al.alSourcef(_soundSource, Al.AL_ROLLOFF_FACTOR, 0);
                Al.alSourcei(_soundSource, Al.AL_SOURCE_RELATIVE, Al.AL_FALSE); // absolute coords in relation to listener
                Al.alSourcefv(_soundSource, Al.AL_POSITION, new float[] { 0, 0, 0 });
            }
            else if (_kind == SoundKind.Spatial)
            {
                Al.alSourcei(_soundSource, Al.AL_DISTANCE_MODEL, Al.AL_LINEAR_DISTANCE /*Al.AL_INVERSE_DISTANCE_CLAMPED*/ /*Al.AL_EXPONENT_DISTANCE_CLAMPED*/ /*Al.AL_INVERSE_DISTANCE*/);
            }
        }

        #endregion

        #region Fields

        /// <summary>
        /// OpenAL sound buffer id
        /// </summary>
        private int _sound;

        /// <summary>
        /// Vector3 array storage
        /// </summary>
        private float[] _vector3 = new float[3];

        private Vector3 _lastPosition;

        private Vector3 _lastDirection;

        #endregion

        #region ISoundEmitter

        /// <summary>
        /// 
        /// </summary>
        public override bool Loop
        {
            set
            {
                Al.alSourcei(_soundSource, Al.AL_LOOPING, value ? Al.AL_TRUE : Al.AL_FALSE);

                base.Loop = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override float DistanceScale
        {
            set
            {
            	Al.alSourcef(_soundSource, Al.AL_ROLLOFF_FACTOR, Tools.XnaDistanceScaleToOpenALRollOff(value));

                base.DistanceScale = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override float DopplerScale
        {
            set
            {
            	Al.alSourcef(_soundSource, Al.AL_DOPPLER_FACTOR, Tools.XnaDopplerScaleToOpenALDopplerFactor(value));

            	base.DopplerScale = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override float SpeedOfSound
        {
            set
            {
                Al.alSourcef(_soundSource, Al.AL_SPEED_OF_SOUND, value);

                base.SpeedOfSound = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override Vector3 Velocity
        {
            set
            {
                _vector3[0] = value.x;
                _vector3[1] = value.y;
                _vector3[2] = value.z;

                Al.alSourcefv(_soundSource, Al.AL_VELOCITY, _vector3);

                base.Velocity = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override float Volume
        {
            set
            {
                Al.alSourcef(_soundSource, Al.AL_GAIN, Tools.XnaToOpenALVolume(value));

                base.Volume = value;
            }
        }

        /// <summary>
        /// No effect for OpenAL since it doesn't support stereo panning, so far.
        /// </summary>
        public override float Pan
        {
            get { return base.Pan; }
            set { base.Pan = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override float Pitch
        {
            set
            {
                Al.alSourcef(_soundSource, Al.AL_PITCH, Tools.XnaToOpenALPitch(value));

                base.Pitch = value;
            }
        }

        #endregion

        #region Properties

        private int _soundSource;
        /// <summary>
        /// OpenAL sound source. 
        /// </summary>
        /// <remarks>
        /// Can be used to directly access properties of the playing OpenAL sound that the common interface doesn't support
        /// </remarks>
        public int SoundSource
        {
            get { return _soundSource; }
            set { _soundSource = value; }
        }

        /// <summary>
        /// Sound cone angles in degrees, the accepted range is 0 to 360, the default value is 360.
        /// </summary>
        public virtual Tuple<int, int> ConeAngles
        {
            get
            {
                int inside = 0, outside = 0;
                Al.alGetSourcei(_soundSource, Al.AL_CONE_INNER_ANGLE, out inside);
                Al.alGetSourcei(_soundSource, Al.AL_CONE_OUTER_ANGLE, out outside);

                return new Tuple<int, int>(inside, outside);
            }
            set
            {
                Al.alSourcei(_soundSource, Al.AL_CONE_INNER_ANGLE, value.First);
                Al.alSourcei(_soundSource, Al.AL_CONE_OUTER_ANGLE, value.Second);
            }
        }

        /// <summary>
        /// Sound cone outside volume
        /// </summary>
        public virtual float OutsideVolume
        {
            get
            {
                float val;
                Al.alGetSourcef(_soundSource, Al.AL_CONE_OUTER_GAIN, out val);

                return Tools.OpenALToXnaVolume(val);
            }
            set
            {
            	Al.alSourcef(_soundSource, Al.AL_CONE_OUTER_GAIN, Tools.XnaToOpenALVolume(value));
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        public override void Play()
        {
            base.Play();

            // update spatial settings before the first go
            Update();

            Al.alSourcePlay(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            Al.alSourcePause(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            Al.alSourceStop(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            // whether played to the end
            int state;
            Al.alGetSourcei(_soundSource, Al.AL_SOURCE_STATE, out state);
            if (state == Al.AL_STOPPED && !_explicitStop)
            {
                ParentSound.Creator.QueuedForDisposal.Add(this);
                return;
            }

            // update spatial params
            if (Kind == SoundKind.Spatial && State == SoundState.Playing)
            {
            	ISoundEmitter emitter = this;

                // for a spatial sound, peek current emitter transforms
                // that makes the emitter update transforms from the underlying scene node (if any) 

                if (emitter.Position != _lastPosition)
                {
                    // if position has changed
                    _lastPosition = emitter.Position;

                    _vector3[0] = _lastPosition.x;
                    _vector3[1] = _lastPosition.y;
                    _vector3[2] = _lastPosition.z;

                    Al.alSourcefv(_soundSource, Al.AL_POSITION, _vector3);
                }

                if (emitter.Forward != _lastDirection)
                {
                    // if direction has changed
                    _lastDirection = emitter.Forward;

                    _vector3[0] = _lastDirection.x;
                    _vector3[1] = _lastDirection.y;
                    _vector3[2] = _lastDirection.z;

                    Al.alSourcefv(_soundSource, Al.AL_DIRECTION, _vector3);
                }
            }

            base.Update();
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SoundInstance()
        {
            _dispose(false);
        }

        protected override void _dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // do nothing
                }

                Al.alDeleteSources(1, ref _soundSource);

                base._dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
