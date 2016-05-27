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

using Axiom.Utilities;
using System;
using Axiom.Core;
using Axiom.Math;
using Tao.OpenAl;

namespace Axiom.SoundSystems.OpenAL.Tao
{
	/// <summary>
	/// OpenAL sound context.
	/// </summary>
    public class SoundContext : Axiom.SoundSystems.SoundContext, IDisposable
    {
        #region Constructor

        /// <summary>
        /// 
        /// </summary>
        public SoundContext()
            : base()
        {
        	LogManager.Instance.Write(this.GetType().FullName + " created");
        }

        #endregion

        #region Static

        /// <summary>
        /// Used to track the number of requested instances of the OpenAL context.
        /// When it reaches zero while disposing, the OpenAL context can be freed.
        /// </summary>
        static int __numInitializations;

        #endregion

        #region Fields

        private Vector3 _lastListenerPosition;

        private Quaternion _lastListenerOrientation;

        private float[] _vector3 = new float[3];

        private float[] _vector6 = new float[6];

        #endregion

        #region Methods

		protected override void Initialize(params object[] args)
		{
            if (__numInitializations == 0)
            {
                int success = Alut.alutInit();

                if (success == Al.AL_FALSE)
                    throw new SoundSystemsException(Tools.BuildAlutErrorMessage("Creating OpenAL context failed"));                
            }
            __numInitializations++;

            LogManager.Instance.Write("OpenAL sound system #{0} initialized", __numInitializations);
            
            base.Initialize(args);
		}
		
        public override Axiom.SoundSystems.Sound LoadSound(string filename, SoundKind kind)
        {
            // create the sound
            Sound sound = new Sound(this, filename, kind);
            sound.Load();
            NotifyLoaded(sound);

            return sound;
        }

        public override Axiom.SoundSystems.CameraListener CreateCameraListener(Camera camera)
        {
            return new CameraListener(camera);
        }

        public override void Update(Object source, FrameEventArgs e)
        {
            base.Update(source, e);

            if (e.StopRendering)
                return;

            // update OpenAL listener

            if (Listener.Position != _lastListenerPosition)
            {
                // if the listener moved
                _lastListenerPosition = Listener.Position;

                _vector3[0] = _lastListenerPosition.x;
                _vector3[1] = _lastListenerPosition.y;
                _vector3[2] = _lastListenerPosition.z;

                Al.alListenerfv(Al.AL_POSITION, _vector3);
            }

            if (Listener.Orientation != _lastListenerOrientation)
            {
                // if the listener turned
                _lastListenerOrientation = Listener.Orientation;

                Vector3 forward = Listener.Forward;
                Vector3 up = Listener.Up;

                _vector6[0] = forward.x;
                _vector6[1] = forward.y;
                _vector6[2] = forward.z;

                _vector6[3] = up.x;
                _vector6[4] = up.y;
                _vector6[5] = up.z;

                Al.alListenerfv(Al.AL_ORIENTATION, _vector6);
            }
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// Finalizer
        /// </summary>
        ~SoundContext()
        {
            dispose(false);
        }

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposeManagedResources)
                {
                    // do nothing
                }

                if (--__numInitializations == 0)
                {
                    int success = Alut.alutExit();

                    if (success == Al.AL_FALSE)
                    {
                        // not safe to be called at this point unless we can check whether the LogManager has been disposed.
                        //LogManager.Instance.Write(Tools.BuildAlutErrorMessage("Freeing OpenAL context failed"));
                    }
                }

                if (__numInitializations < 0)
                	__numInitializations = 0; // should never happen, but appeared when finalizing
                
                base.dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
