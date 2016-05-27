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
using Axiom.Core;
using Axiom.Math;
using Axiom.SoundSystems.Effects;

namespace Axiom.SoundSystems
{
    /// <summary>
    /// A sound that can be attached to a scene node.
    /// </summary>
    /// <remarks>
    /// The world position and world orientation of the sound are transparent to the transforms of the <see cref="Node"/> the sound is attached to.
    /// </remarks>
    public abstract class SoundInstance : MovableObject, ISoundEmitter, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <remarks>
        /// Users shall initialize the instance through <see cref="Axiom.SoundSystems.Sound"> methods
        /// </remarks>
        /// <param name="creator">The <see cref="Sound"/> that created this sound instance</param>
        /// <param name="name">The custom name of the sound</param>
        /// <param name="kind">The kind of the sound</param>
        protected internal SoundInstance(Sound creator, string name, SoundKind kind)
            : base()
        {
            this.name = name;
            this.isVisible = false;
            _parentSound = creator;
            _kind = kind;
            _state = SoundState.Stopped;
        }

        #endregion

        #region Fields

        /// <summary>
        /// Used to determine whether to dispose the instance
        /// </summary>
        protected bool _explicitStop;

        #endregion

        #region ISoundEmitter

        protected SoundKind _kind;
        /// <summary>
        /// Gets or sets the kind of the sound.
        /// </summary>
        public SoundKind Kind
        {
            get { return _kind; }
            set { _kind = value; }
        }

        protected bool _loop;
        /// <summary>
        /// 
        /// </summary>
        public virtual bool Loop
        {
            get { return _loop; }
            set { _loop = value; }
        }

        protected float _distanceScale;
        /// <summary>
        /// 
        /// </summary>
        public virtual float DistanceScale
        {
            get { return _distanceScale; }
            set { _distanceScale = value; }
        }

        protected float _dopplerScale;
        /// <summary>
        /// 
        /// </summary>
        public virtual float DopplerScale
        {
            get { return _dopplerScale; }
            set { _dopplerScale = value; }
        }

        protected float _speedOfSound;
        /// <summary>
        /// 
        /// </summary>
        public virtual float SpeedOfSound
        {
            get { return _speedOfSound; }
            set { _speedOfSound = value; }
        }

        protected Vector3 _position;
        /// <summary>
        /// The world position of this sound emitter
        /// </summary>
        /// <remarks>
        /// The value is transparent to the derived position of the underlying scene node if any.
        /// </remarks>
        public virtual Vector3 Position
        {
            get
            {
                if (ParentNode != null && _position != ParentNode.DerivedPosition)
                {
                    _position = ParentNode.DerivedPosition;
                }

                return _position;
            }
            set
            {
                if (_position != value)
                {
                    _position = value;

                    if (ParentNode != null)
                    {
                        // TODO: set derived position of the parent node
                    }
                }
            }
        }

        protected Quaternion _orientation;
        /// <summary>
        /// The world orientation of this emitter
        /// </summary>
        public virtual Quaternion Orientation
        {
            get 
            {
                if (ParentNode != null)
                {
                    UpdateFromParent(ParentNode);
                }

                return _orientation; 
            }
            set 
            {
                if (value != _orientation)
                {
                    _orientation = value;

                    if (ParentNode != null)
                    {
                        // TODO: set derived orientation of the parent node
                    }
                }
            }
        }

        protected Vector3 _forward;
        /// <summary>
        /// Forward orientation vector of this emitter in world coordinates
        /// </summary>
        /// <remarks>
        /// The value points in the direction of the negative Z-Axis of the underlying scene node if any.
        /// The values of the <see cref="Forward"/> and <see cref="Up"/> vectors must be orthonormal (at right angles to one another). 
        /// Behavior is undefined if these vectors are not orthonormal. The preferred way might be to use the <see cref="Orientation"/> property instead.
        /// </remarks>
        public virtual Vector3 Forward
        {
            get 
            {
                if (ParentNode != null)
                {
                    UpdateFromParent(ParentNode);
                }

                return _forward; 
            }
            set 
            {
                if (value != _forward)
                {
                    _forward = value;

                    if (ParentNode != null)
                    {
                        // TODO: set derived orientation of the parent node
                    }
                }
            }
        }

        protected Vector3 _up;
        /// <summary>
        /// The upward orientation vector of this emitter in world coordinates
        /// </summary>
        /// <remarks>
        /// The value maps to the Y-Axis of the underlying scene node if any.
        /// The values of the <see cref="Forward"/> and <see cref="Up"/> vectors must be orthonormal (at right angles to one another). 
        /// Behavior is undefined if these vectors are not orthonormal. The preferred way might be to use the <see cref="Orientation"/> property instead.
        /// </remarks>
        public virtual Vector3 Up
        {
            get 
            {
                if (ParentNode != null)
                {
                    UpdateFromParent(ParentNode);
                }

                return _up; 
            }
            set 
            {
                if (_up != value)
                {
                    _up = value;

                    if (ParentNode != null)
                    {
                        // TODO: set derived orientation of the parent node
                    }
                }
            }
        }

        protected Vector3 _velocity;
        /// <summary>
        /// 
        /// </summary>
        public virtual Vector3 Velocity
        {
            get { return _velocity; }
            set { _velocity = value; }
        }

        protected float _volume;
        /// <summary>
        /// 
        /// </summary>
        public virtual float Volume
        {
            get { return _volume; }
            set { _volume = value; }
        }

        protected float _pan;
        /// <summary>
        /// 
        /// </summary>
        public virtual float Pan
        {
            get { return _pan; }
            set { _pan = value; }
        }

        protected float _pitch;
        /// <summary>
        /// 
        /// </summary>
        public virtual float Pitch
        {
            get { return _pitch; }
            set { _pitch = value; }
        }

        private EffectList _effects;
        /// <summary>
        /// 
        /// </summary>
        public virtual EffectList Effects
        {
            get { return _effects; }
            set { _effects = value; }
        }

        #endregion

        #region MovableObject

        /// <summary>
        /// Unused
        /// </summary>
        /// <param name="camera"></param>
        public override void NotifyCurrentCamera(Camera camera)
        {
        }

        /// <summary>
        /// Unused
        /// </summary>
        /// <param name="queue"></param>
        public override void UpdateRenderQueue(Axiom.Graphics.RenderQueue queue)
        {
        }

        /// <summary>
        /// Null bounding box (as the sound source is a point)
        /// </summary>
        public override AxisAlignedBox BoundingBox
        {
            get
            {
                return AxisAlignedBox.Null;
            }
        }

        /// <summary>
        /// Bounding radius. Returns NaN.
        /// </summary>
        public override float BoundingRadius
        {
            get
            {
                return float.NaN;
            }
        }

        #endregion

        #region Protected

        /// <summary>
        /// Update properties that depend on the orientation of the sound source from the transforms of a <see cref="Node"/>
        /// </summary>
        /// <param name="parent"></param>
        protected void UpdateFromParent(Node parent)
        {
            if (parent.DerivedOrientation != _orientation)
            {
                _orientation = parent.DerivedOrientation;
                _forward = (- _orientation.ZAxis);
                _up = _orientation.YAxis;
            }
        }

        #endregion

        #region Properties
        
        protected Sound _parentSound;
        /// <summary>
        /// The <see cref="Sound"/> that created this sound instance.
        /// </summary>
        public Sound ParentSound
        {
            get { return _parentSound; }
        }

        private SoundState _state;
        /// <summary>
        /// Current state of the sound
        /// </summary>
        public SoundState State
        {
            get { return _state; }
            set { _state = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        ///	Start playing the sound
        /// </summary>
        public virtual void Play()
        {
            State = SoundState.Playing;
            _explicitStop = false;
        }

        /// <summary>
        /// Pause the replay, calling <see cref="Play"> again will start playing from the position where <see cref="Pause"/> was called
        /// </summary>
        public virtual void Pause()
        {
            State = SoundState.Paused;
        }

        /// <summary>
        /// Stop playing this sound and set the replay position to the start of the buffer
        /// </summary>
        public virtual void Stop()
        {
            State = SoundState.Stopped;
            _explicitStop = true;
        }

        /// <summary>
        /// Update sound replay
        /// </summary>
        /// <remarks>
        /// This method is called by the <see cref="SoundContext"/> on frame update.
        /// Implementors must a.o. check whether the sound was played to the end and dispose this instance if so.
        /// </remarks>
        public virtual void Update()
        {
            foreach (IEmitterEffect fx in Effects)
                fx.UpdateEffect();
        }

        /// <summary>
        /// Adds a <see cref="IEmitterEffect"/> to the playing instance. It takes care for initializing the effect.
        /// </summary>
        /// <param name="fx"></param>
        public virtual void AddEffect(IEmitterEffect fx)
        {
            Effects.Add(fx);
            fx.InitializeEffect(this);
        }

        /// <summary>
        /// Removes a <see cref="IEmitterEffect"/> from the playing instance. It takes care for disposing the effect.
        /// </summary>
        /// <param name="fx"></param>
        public virtual void RemoveEffect(IEmitterEffect fx)
        {
            Effects.Remove(fx);
            fx.Dispose();
        }

        #endregion

        #region IDisposable

        protected bool _isDisposed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        protected virtual void _dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                State = SoundState.Invalid;

                if (disposeManagedResources)
                {
                    Effects.Clear();

                    // remove the instance from sound manager
                    SoundContext sm = ParentSound.Creator;
                    if (sm.SoundInstanceMap.ContainsKey(this.Name))
                        sm.SoundInstanceMap.Remove(this.Name);
                }

                _isDisposed = true;
            }
        }

        /// <summary>
        /// Implementation of <see cref="IDisposable"/>
        /// </summary>
        public virtual void Dispose()
        {
            _dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
