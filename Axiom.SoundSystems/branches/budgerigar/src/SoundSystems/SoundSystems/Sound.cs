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
using System.Diagnostics;
using System.Globalization;
using Axiom.Core;
using Axiom.Math;
using Axiom.SoundSystems.Effects;
using System.Collections.Generic;

namespace Axiom.SoundSystems
{

    /// <summary>
    /// Represents a sound loaded from a resource
    /// </summary>
    /// <remarks>
    /// To get a playing instance of this sound call <see cref="Play"/>.
    /// </remarks>
    public abstract class Sound : ISoundEmitter, ICloneable, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Internal constructor.
        /// </summary>
        /// <remarks>
        /// Users shall initialize the instance through <see cref="Axiom.SoundSystems.SoundContext.LoadSound">
        /// </remarks>
        /// <param name="creator">The <see cref="SoundContext"/> that creates this sound</param>
        /// <param name="filename">The file name of the sound</param>
        /// <param name="kind">The kind of the sound</param>
        protected internal Sound(SoundContext creator, string fileName, SoundKind kind)
        {
            _creator = creator;
            _name = GetUniqueName(fileName);
            _fileName = fileName;
            _kind = kind;
        }

        #endregion

        #region ISoundEmitter

        protected SoundKind _kind;
        /// <summary>
        /// The kind of the sound, ambient or spatial.
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

        private Vector3 _position;
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Position
        {
            get { return _position; }
            set { _position = value; }
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

        #region ICloneable

        public abstract object Clone();

        #endregion

        #region Properties

        private SoundContext _creator;
        /// <summary>
        /// The <see cref="SoundContext"/> that created this sound
        /// </summary>
        public SoundContext Creator
        {
            get { return _creator; }
        }

        private string _name;
        /// <summary>
        /// Name of the sound. 
        /// </summary>
        /// <remarks>
        /// The name is automatically assigned. It can be used as key to access sounds from <see cref="SoundMap"/>
        /// </remarks>
        public string Name
        {
            get { return _name; }
        }

        protected string _fileName;
        /// <summary>
        /// The file name of the resource
        /// </summary>
        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public SoundInstance Play()
        {
            return Play(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPlaying"></param>
        /// <returns></returns>
        public SoundInstance Play(bool startPlaying)
        {
            return Play(startPlaying, GenerateUniqueKey());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SoundInstance Play(string instanceKey)
        {
            return Play(true, instanceKey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPlaying"></param>
        /// <param name="instanceKey"></param>
        /// <returns></returns>
        public SoundInstance Play(bool startPlaying, string instanceKey)
        {
            Debug.Assert(instanceKey != null);

            SoundInstance sound = CreateSoundInstance(instanceKey);

            _creator.SoundInstanceMap.Add(instanceKey, sound);

            if (startPlaying)
            {
                sound.Play();
            }

            return sound;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <returns></returns>
        public SoundInstance Play(SceneNode node)
        {
            return Play(GenerateUniqueKey(), node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPlaying"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public SoundInstance Play(bool startPlaying, SceneNode node)
        {
            return Play(startPlaying, GenerateUniqueKey(), node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="instanceKey"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public SoundInstance Play(string instanceKey, SceneNode node)
        {
            return Play(true, instanceKey, node);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startPlaying"></param>
        /// <param name="instanceKey"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        public SoundInstance Play(bool startPlaying, string instanceKey, SceneNode node)
        {
            Debug.Assert(instanceKey != null && node != null);

            SoundInstance sound = CreateSoundInstance(instanceKey);

            _creator.SoundInstanceMap.Add(instanceKey, sound);

            node.AttachObject(sound);

            if (startPlaying)
            {
                sound.Play();
            }

            return sound;
        }

        #endregion

        #region Abstract

        /// <summary>
        /// Implement loading from the file specified by <see cref="FileName"/>
        /// </summary>
        public virtual void Load()
        {
            LogManager.Instance.Write("Axiom.SoundSystems: Loading sound '{0}'", FileName);
        }

        /// <summary>
        /// Create a <see cref="SoundInstance"/> to be ready for replay
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected abstract SoundInstance CreateSoundInstance(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sound"></param>
        protected void NotifyCreated(SoundInstance sound)
        {
            foreach (IEmitterEffect fx in sound.Effects)
            {
                fx.InitializeEffect(sound);
            }
        }

        #endregion

        #region Unique keying

        private static int _uniqueKeyCounter;

        private string GenerateUniqueKey()
        {
            return String.Format("soundInstance_{0}", _uniqueKeyCounter++);
        }

        private string GetUniqueName(string prefix)
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}_{1}", prefix, GetNextId());
        }

        private static int _id;

        private int GetNextId()
        {
            return _id++;
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
                if (disposeManagedResources)
                {
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
