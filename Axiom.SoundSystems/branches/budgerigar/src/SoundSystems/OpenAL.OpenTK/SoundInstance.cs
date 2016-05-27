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
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Axiom.SoundSystems.OpenAL.OpenTK
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
            AL.GenSources(1, out _soundSource);

            // link the buffer and sound source
            AL.Source(_soundSource, ALSourcei.Buffer, _sound);

            if (_kind == SoundKind.Simple)
            {
                AL.Source(_soundSource, (ALSourcei)AL_DISTANCE_MODEL, 0);
                AL.Source(_soundSource, ALSourcef.RolloffFactor, 0);
                AL.Source(_soundSource, ALSourceb.SourceRelative, false);
                AL.Source(_soundSource, ALSource3f.Position, 0, 0, 0);
            }
            else if (_kind == SoundKind.Spatial)
            {
                AL.Source(_soundSource, (ALSourcei)AL_DISTANCE_MODEL, AL_LINEAR_DISTANCE);
            }

        }

        #endregion

        #region Const

        // the following values are not wrapped by OpenTK.dll 1.0.0.201
        public const int AL_DISTANCE_MODEL = 0xD000;
        public const int AL_LINEAR_DISTANCE = 0xD003;
        public const int AL_NONE = 0;
        public const int AL_DOPPLER_FACTOR = 0xC000;
        public const int AL_SPEED_OF_SOUND = 0xC003;
        
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
                AL.Source(_soundSource, ALSourceb.Looping, value ? true : false);
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
                AL.Source(_soundSource, ALSourcef.RolloffFactor, Tools.XnaDistanceScaleToOpenALRollOff(value));
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
                AL.Source(_soundSource, (ALSourcef)AL_DOPPLER_FACTOR, Tools.XnaDopplerScaleToOpenALDopplerFactor(value));
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
                AL.Source(_soundSource, (ALSourcef)AL_SPEED_OF_SOUND, value);
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

                AL.Source(_soundSource, ALSource3f.Velocity, _vector3[0], _vector3[1], _vector3[2]);
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
                AL.Source(_soundSource, ALSourcef.Gain, Tools.XnaToOpenALVolume(value));
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
                AL.Source(_soundSource, ALSourcef.Pitch, Tools.XnaToOpenALPitch(value));
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
                AL.GetSource(_soundSource, (ALGetSourcei)ALSourcef.ConeInnerAngle, out inside);
                AL.GetSource(_soundSource, (ALGetSourcei)ALSourcef.ConeOuterAngle, out outside);
                return new Tuple<int, int>(inside, outside);
            }
            set
            {
                AL.Source(_soundSource, ALSourcef.ConeInnerAngle, value.First);
                AL.Source(_soundSource, ALSourcef.ConeOuterAngle, value.Second);
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
                AL.GetSource(_soundSource, ALSourcef.ConeOuterGain, out val);
                return Tools.OpenALToXnaVolume(val);
            }
            set
            {
                AL.Source(_soundSource, ALSourcef.ConeOuterGain, Tools.XnaToOpenALVolume(value));
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

            AL.SourcePlay(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Pause()
        {
            base.Pause();
            AL.SourcePause(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Stop()
        {
            base.Stop();
            AL.SourceStop(_soundSource);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Update()
        {
            // whether played to the end
            int state;
            AL.GetSource(_soundSource, ALGetSourcei.SourceState, out state);
            if (state == (int)ALSourceState.Stopped && !_explicitStop)
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

                    AL.Source(_soundSource, ALSource3f.Position, _vector3[0], _vector3[1], _vector3[2]);
                }

                if (emitter.Forward != _lastDirection)
                {
                    // if direction has changed
                    _lastDirection = emitter.Forward;

                    _vector3[0] = _lastDirection.x;
                    _vector3[1] = _lastDirection.y;
                    _vector3[2] = _lastDirection.z;

                    AL.Source(_soundSource, ALSource3f.Direction, _vector3[0], _vector3[1], _vector3[2]);
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

                AL.DeleteSources(1, ref _soundSource);

                base._dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
