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
using System.Collections.Generic;
using Axiom.SoundSystems.Decoders;
using Axiom.Graphics;
using Axiom.Core;
using Axiom.Utilities;

namespace Axiom.SoundSystems
{
    public sealed class SoundsRoot : Singleton<SoundsRoot>, IDisposable
    {
        #region Constructor

        /// <summary>
        /// Non public constructor, the static <see cref="Instance"/> property will return an unique instance of this singleton class
        /// </summary>
        private SoundsRoot()
        {
        }

        #endregion

        #region Properties

        private Dictionary<string, Type> _soundContextTypeMap = new Dictionary<string, Type>();
        /// <summary>
        /// Collection of registered sound manager types.
        /// </summary>
        /// <summary>
        /// Here's the place where plugins shall register sound manager classes.
        /// The string key is the full namespace name of a registered sound manager type, for example "Axiom.SoundSystems.Xna.Simple"
        /// The <see cref="Type"/> is the sound manager's type that can be instantiated.
        /// </remarks>
        /// </summary>
        public Dictionary<string, Type> SoundContextTypeMap
        {
            get { return _soundContextTypeMap; }
        }

        private List<SoundContext> _soundContexts = new List<SoundContext>();
        /// <summary>
        /// Collection of instantiated sound managers.
        /// </summary>
        /// <remarks>You can dispose a sound manager if you no longer need it by a call to <see cref="SoundContext.Dispose"/>, it will be removed from this list automatically.</remarks>
        public List<SoundContext> SoundContexts
        {
            get { return _soundContexts; }
        }

        private SoundContext _defaultSoundContext;
        /// <summary>
        /// Any available sound context or null. Or user specified context.
        /// </summary>
        /// <remarks>
        /// By default the property searches the first available sound context and returns an instance of it, or null if not found.
        /// The property can also be set to keep reference to a user selected sound context.
        /// </remarks>
        public SoundContext DefaultSoundContext
        {
            get
            {
                if (_defaultSoundContext == null && _soundContextTypeMap.Count > 0)
                {
                    // get the first entry from the collection
                    foreach (string key in _soundContextTypeMap.Keys)
                    {
                        _defaultSoundContext = CreateSoundContext(key);
                        break;
                    }
                }

                return _defaultSoundContext;
            }
            set
            {
                _defaultSoundContext = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Create a concrete <see cref="SoundContext"/> instance from the list of registered context types
        /// </summary>
        /// <param name="subsystemName">The name of the subsystem as registered by its plugin interface</param>
        /// <returns></returns>
        public SoundContext CreateSoundContext(string subsystemName)
        {
            return CreateSoundContext(subsystemName, null, null);
        }

        /// <summary>
        /// Create a concrete <see cref="SoundContext"/> instance from the list of registered context types
        /// </summary>
        /// <param name="subsystemName">The name of the subsystem as registered by its plugin interface</param>
        /// <param name="args">
        /// The default optional parameters can be:
        /// A <see cref="Camera"/> to be used with the <see cref="SoundContext.Listener"/> object</param>
        /// A <see cref="RenderWindow"/> to be associated with this context.
        /// For a list of subsystem specific parameters see comments of the subsystem's SoundContext class.
        /// </param>
        /// <returns></returns>
        public SoundContext CreateSoundContext(string subsystemName, params object[] args)
        {
            Contract.RequiresNotEmpty(subsystemName, "subsystemName");
            Contract.Requires(SoundContextTypeMap.ContainsKey(subsystemName));

            SoundContext ctx = (SoundContext)Activator.CreateInstance(SoundContextTypeMap[subsystemName]);
            SoundContexts.Add(ctx);
            ctx.Disposing += new EventHandler(ContextDisposing);

            ctx.Initialize(args);        
            
            return ctx;
        }

        /// <summary>
        /// Callback to catch disposal of a <see cref="SoundContext"/> instance
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContextDisposing(object sender, EventArgs e)
        {
            if (SoundContexts.Contains((SoundContext)sender))
            {
                SoundContexts.Remove((SoundContext)sender);
            }
        }

        #endregion

        #region IDisposable

        protected override void dispose(bool disposeManagedResources)
        {
            if (!isDisposed)
            {
                if (disposeManagedResources)
                {
                    foreach (SoundContext manager in SoundContexts)
                    {
                        manager.Dispose();
                    }
                    SoundContexts.Clear();

                    DecoderManager.Instance.Dispose();
                }

                base.dispose(disposeManagedResources);
            }
        }

        #endregion
    }
}
