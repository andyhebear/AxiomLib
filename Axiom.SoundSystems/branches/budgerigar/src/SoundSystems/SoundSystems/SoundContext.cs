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

using Axiom.Collections;
using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.SoundSystems.Effects;
using Axiom.Utilities;

namespace Axiom.SoundSystems
{
    /// <summary>
    /// Manages sounds. Updates sound replay per frame.
    /// </summary>
    public abstract class SoundContext : IDisposable
    {
        #region Static

        /// <summary>
        /// The default speed of sound in metres per second, equals 343.5f 
        /// </summary>
        public static float DefaultSpeedOfSound
        {
            get { return 343.5f; }
        }

        /// <summary>
        /// Helper method to copy values from one to another <see cref="ISoundEmitter"/>, most likely from a <see cref="Sound"/> to a <see cref="SoundInstance"/>.
        /// </summary>
        public static void CopySettings(ISoundEmitter from, ISoundEmitter to)
        {
            to.Kind = from.Kind;
            to.Loop = from.Loop;
            to.DistanceScale = from.DistanceScale;
            to.DopplerScale = from.DopplerScale;
            to.SpeedOfSound = from.SpeedOfSound;
            to.Forward = from.Forward;
            to.Up = from.Up;
            to.Position = from.Position;
            to.Velocity = from.Velocity;
            to.Volume = from.Volume;
            to.Pan = from.Pan;
            to.Pitch = from.Pitch;

            to.Effects = (EffectList)from.Effects.Clone();
        }

        #endregion

        #region Fields

        /// <summary>
        /// Used to track sounds paused with <see cref="PauseReplay"/> so they can be resumed with <see cref="ResumeReplay"/>
        /// </summary>
        protected List<SoundInstance> pausedSounds = new List<SoundInstance>();

        #endregion

        #region Events

        /// <summary>
        /// Raised when disposing the instance
        /// </summary>
        public event EventHandler Disposing;

        #endregion

        #region Nested

        public class WindowEventListener : IWindowEventListener
        {
            public WindowEventListener(SoundContext context)
            {
                mContext = context;
            }

            SoundContext mContext;

            /// <summary>
            /// Window has moved position
            /// </summary>
            /// <param name="rw">The RenderWindow which created this event</param>
            public void WindowMoved(RenderWindow rw)
            {
            }

            /// <summary>
            /// Window has resized
            /// </summary>
            /// <param name="rw">The RenderWindow which created this event</param>
            public void WindowResized(RenderWindow rw)
            {
            }

            /// <summary>
            /// Window has closed, the associated <see cref="SoundContext"/> will be disposed
            /// </summary>
            /// <param name="rw">The RenderWindow which created this event</param>
            public void WindowClosed(RenderWindow rw)
            {
                if (mContext != null)
                    mContext.Dispose();

                mContext = null;
            }

            /// <summary>
            /// Window lost/regained the focus
            /// </summary>
            /// <remarks>
            /// We read the <see cref="RenderWindow.IsActive"/> property to determine whether the associated context shall be
            /// active or not. 
            /// </remarks>
            /// <param name="rw">The RenderWindow which created this event</param>
            public void WindowFocusChange(RenderWindow rw)
            {
                Contract.RequiresNotNull(rw, "RenderWindow");

                // pause/resume replay
                if (mContext != null)
                    mContext.Active = rw.IsActive;                    
            }
        }

        #endregion

        #region Properties

        private Dictionary<string, Sound> _soundMap = new Dictionary<string, Sound>();
        /// <summary>
        /// Collection of all loaded sounds where key is the file name used to load it
        /// </summary>
        public Dictionary<string, Sound> SoundMap
        {
            get { return _soundMap; }
        }

        private Dictionary<string, SoundInstance> _soundInstanceMap = new Dictionary<string, SoundInstance>();
        /// <summary>
        /// Collection of all playing or stopped <see cref="SoundInstance"/> objects 
        /// where key is either automatically generated one or the instance key passed with <see cref="Sound.Play"/>
        /// </summary>
        public Dictionary<string, SoundInstance> SoundInstanceMap
        {
            get { return _soundInstanceMap; }
        }

        /// <summary>
        /// The main listener. Usually a <see cref="CameraListener"/>
        /// </summary>
        public virtual ISoundListener Listener
        {
            get;
            set;
        }

        private SoundSettings _defaultSoundSettings = new SoundSettings();
        /// <summary>
        /// Default values to be used to initialise properties of each newly loaded <see cref="Sound"/>
        /// </summary>
        /// <remarks>
        /// This is intended to have a place where most likely environmental properties of a <see cref="ISoundEmitter"/> can be set prior to loading the sounds, 
        /// for example set the <see cref="ISoundEmitter.DistanceScale"/> to be getting the same distance scale for all loading sounds.
        /// To modify the settings of an already loaded <see cref="Sound"/> or also a launched <see cref="SoundInstance"/> you can use <see cref="SoundSettings.CopyTo"/>
        /// </remarks>
        public SoundSettings DefaultSoundSettings
        {
            get { return _defaultSoundSettings; }
            set { _defaultSoundSettings = value; }
        }

        private List<IDisposable> _queuedForDisposal = new List<IDisposable>(256);
        /// <summary>
        /// List of objects that were queued for disposal during update, see <see cref="Update"/>
        /// </summary>
        public List<IDisposable> QueuedForDisposal
        {
            get { return _queuedForDisposal; }
        }

        private bool mActive = true;
        /// <summary>
        /// Gets or sets whether the context is active or not.
        /// </summary>
        /// <remarks>
        /// This can be used to pause and resume current sound replay. 
        /// Note that an inactive sound context doesn't prevent newly launched sound instances from sounding.
        /// </remarks>
        public bool Active
        {
            get
            {
                return mActive;
            }
            set
            {
                if (mActive != value)
                {
                    if (mActive)
                        PauseReplay();
                    else
                        ResumeReplay();

                    mActive = value;
                }
            }
        }

        private RenderWindow mWindow;
        /// <summary>
        /// Specifies the optional <see cref="RenderWindow"/> associated with this context.
        /// </summary>
        /// <remarks>
        /// This is mainly used to catch window events to determine whether the application is active or not and pause/resume replay according to it's state.
        /// The render window needs not to be specified, but if it isn't, this behavior cannot be automated and user needs to do an explicit call to <see cref="SoundsRoot.PauseReplay"/>
        /// It is allowed to use the same <see cref="RenderWindow"/> with multiple <see cref="SoundContext"/> instances.
        /// </remarks>
        public RenderWindow Window
        {
            get 
            { 
                return mWindow; 
            }
            set 
            {
                if (mWindow != value)
                {
                    if (mWindow != null && WindowListener != null)
                    {
                        // unregister old binding
                        WindowEventMonitor.Instance.UnregisterListener(mWindow, WindowListener);
                    }

                    mWindow = value;

                    if (mWindow != null)
                    {
                        if (mWindowListener == null)
                            mWindowListener = new WindowEventListener(this);

                        // register
                        WindowEventMonitor.Instance.RegisterListener(Window, mWindowListener);
                    }
                }
            }
        }

        IWindowEventListener mWindowListener;
        /// <summary>
        /// The <see cref="IWindowEventListener"/> that is listening to events from <see cref="Window"/> if any.
        /// </summary>
        /// <remarks>
        /// A listener is automatically created when setting <see cref="Window"/> if necessary, 
        /// but you may provide your own implementation by setting this property. That may be useful for extra logic,
        /// for example to play some notification sounds on event invocation. It is highly recomended to derive from the provided <see cref="WindowEventListener"/>
        /// class since it handles automation on window focus change and closing already.
        /// </remarks>
        public IWindowEventListener WindowListener
        {
            get 
            { 
                return mWindowListener; 
            }
            set 
            {
                if (mWindowListener != null && Window != null)
                {
                    // unregister old binding
                    WindowEventMonitor.Instance.UnregisterListener(Window, mWindowListener);
                }

                mWindowListener = value;

                if (mWindowListener != null && Window != null)
                {
                    // register
                    WindowEventMonitor.Instance.RegisterListener(Window, mWindowListener);
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Load a sound from the common resource
        /// </summary>
        /// <remarks>
        /// Implementors shall create new instance of a <see cref="Sound"/> and call the <see cref="NotifyLoaded"/> method
        /// </remarks>
        /// <param name="filename">The sound's filename, extension is used to determine the encoding</param>
        /// <param name="type">The kind of the sound (simple or 3D)</param>
        /// <returns>The loaded sound</returns>
        public abstract Sound LoadSound(string filename, SoundKind kind);

        /// <summary>
        /// Create a <see cref="CameraListener"/> instance to be set for <see cref="Listener"/>
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public virtual CameraListener CreateCameraListener(Camera camera)
        {
            return new CameraListener(camera);
        }

        /// <summary>
        /// Update all sound replay
        /// </summary>
        /// <remarks>
        /// It is necessary to call this method each frame so that sound replay can be updated according to scene changes.
        /// This can be registered as a frame event handler in Axiom, most likely for the <see cref="Axiom.Core.Root.Instance.FrameStarted"/> event.
        /// It is also possible to call it explicitly from another frame event handler. 
        /// </remarks>
        /// <param name="source"></param>
        /// <param name="e"></param>
        public virtual void Update(object source, FrameEventArgs e)
        {
            if (Active)
            {
                foreach (SoundInstance sound in _soundInstanceMap.Values)
                    sound.Update();

                foreach (IDisposable obj in QueuedForDisposal)
                    obj.Dispose();

                QueuedForDisposal.Clear();
            }
        }

        /// <summary>
        /// Pause the replay of all playing sounds. This doesn't prevent new sound instances from sounding.
        /// </summary>
        public void PauseReplay()
        {
            foreach (SoundInstance si in SoundInstanceMap.Values)
                PauseSound(si);
        }

        /// <summary>
        /// Resume the replay of all sounds that were previously paused with <see cref="PauseReplay"/>.
        /// </summary>
        public void ResumeReplay()
        {
            foreach (SoundInstance si in pausedSounds)
                si.Play();

            pausedSounds.Clear();
        }

        /// <summary>
        /// Stop and dispose all used <see cref="SoundInstance"/> objects.
        /// </summary>
        public void StopReplay()
        {
        	List<SoundInstance> list = new List<SoundInstance>(SoundInstanceMap.Values);
        	foreach (SoundInstance si in list)
        	{
        		si.Dispose();
        	}
        	pausedSounds.Clear();
        }
                
        #endregion

        #region Protected Methods

        /// <summary>
        /// Initialize the instance
        /// </summary>
        /// <param name="args"></param>
        protected internal virtual void Initialize(params object[] args)
        {
        	Camera camera = FindArgument<Camera>(args);
            if (camera != null)
                this.Listener = this.CreateCameraListener(camera);

            this.Window = FindArgument<RenderWindow>(args);
        }
        
        /// <summary>
        /// Iterate the arguments and look for an entry of the type or derived type specified by the generic parameter.
        /// Returns the value on success.
        /// Returns default(T1) if a value of the desired type was not found. 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        protected T1 FindArgument<T1>(params object[] args)
        {
        	if (args == null)
        		return default(T1);
        	
        	foreach (object o in args)
        		if (o is T1)
        			return (T1)o;
        	
        	return default(T1);
        }
        
        /// <summary>
        /// Notify the manager that a sound has been loaded.
        /// </summary>
        /// <remarks>
        /// Derived class must call this method from a <see cref="LoadSound"/> implementation
        /// </remarks>
        /// <param name="sound"></param>
        protected virtual void NotifyLoaded(Sound sound)
        {
            SoundKind kind = sound.Kind;
            DefaultSoundSettings.CopyTo(sound);
            sound.Kind = kind;

            SoundMap.Add(sound.FileName, sound);
        }

        /// <summary>
        /// Pause a sound and add it to the list of paused sounds to be resumed on <see cref="ResumeReplay"/>
        /// </summary>
        /// <param name="sound"></param>
        protected void PauseSound(SoundInstance sound)
        {
            if (sound.State == SoundState.Playing)
            {
                sound.Pause();
                if (!pausedSounds.Contains(sound))
                    pausedSounds.Add(sound);
            }
        }

        #endregion

        #region Threaded sound loading

        // TODO: currently commented out, needs review, think how to be notyfying of succesful sound load

        //#region Nested

        ///// <summary>
        ///// Used for threaded sound loading
        ///// </summary>
        //protected struct SoundLoadInfo
        //{
        //    public SoundLoadInfo(string fileName, int soundId, SoundKind soundType)
        //    {
        //        this.FileName = fileName;
        //        this.SoundId = soundId;
        //        this.SoundKind = soundType;
        //    }

        //    public string FileName;

        //    public int SoundId;

        //    public SoundKind SoundKind;
        //}

        //#endregion

        ///// <summary>
        ///// Decode a sound from the common resource in a paralel thread.
        ///// (Currently only for DirectSound)
        ///// </summary>
        ///// <param name="filename">The sound's filename, extension is used to determine the encoding</param>
        ///// <param name="type">The sound type</param>
        ///// <returns>The Id of the sound, so it can be called using <see cref="GetSound"></returns>
        //public virtual int PreLoadSound(string filename, SoundKind type)
        //{
        //    SoundLoadInfo info = new SoundLoadInfo(filename, _lastId, type);

        //    if (!ThreadPool.QueueUserWorkItem(new WaitCallback(ThreadedLoadSound), info))
        //    {
        //        ThreadPool.QueueUserWorkItem
        //        throw new SoundSystemsException("Unable to queue thread for preloading");
        //    }

        //    // return the sound Id and increment the counter
        //    return (_lastId++);
        //}

        ///// <summary>
        ///// Entry point for the loading thread
        ///// </summary>
        ///// <param name="stateInfo">State information, used to access the filename, id and type of the new sound</param>
        //protected abstract void ThreadedLoadSound(object stateInfo);

        ///// <summary>
        ///// Get a previously loaded sound
        ///// </summary>
        ///// <param name="Id">The sound's Id</param>
        ///// <returns>The requested sound or null if it's not found</returns>
        //public virtual SoundInstance GetSound(int Id)
        //{
        //    if (Id < _soundMap.Count)
        //    {
        //        return _soundMap[Id];
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        #endregion

        #region IDisposable

        // A finalizer in this style shall be provided in a derived class if there are any unmanaged resources allocated
        ///// <summary>
        ///// Finalizer
        ///// </summary>
        //~SoundContext()
        //{
        //    dispose(false);
        //}

        protected bool _isDisposed;
        /// <summary>
        /// 
        /// </summary>
        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        protected virtual void dispose(bool disposeManagedResources)
        {
            if (!_isDisposed)
            {
                // dispose managed resources

                if (disposeManagedResources)
                {
                    if (Disposing != null)
                        Disposing(this, new EventArgs());

                    Axiom.Core.Root.Instance.FrameStarted -= new EventHandler<FrameEventArgs>(Update);

                    foreach (SoundInstance s in SoundInstanceMap.Values)
                        s.Dispose();

                    SoundInstanceMap.Clear();

                    foreach (Sound s in SoundMap.Values)
                        s.Dispose();

                    SoundMap.Clear();

                    pausedSounds.Clear();
                }

                // dispose unmanaged resources

                // call base
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
