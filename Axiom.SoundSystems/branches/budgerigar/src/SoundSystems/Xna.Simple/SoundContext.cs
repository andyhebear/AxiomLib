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

using Axiom.SoundSystems.Tools;
using System;
using Axiom.Collections;
using Axiom.Core;
using Axiom.Utilities;
using Microsoft.Xna.Framework.Content;

namespace Axiom.SoundSystems.Xna.Simple
{

	/// <summary>
	/// Xna sound context.
	/// </summary>
	/// <remarks>
    /// By default, sound resources are looked up in Axiom's <see cref="Axiom.Core.ResourceGroupManager"/>,
    /// if there's a need to use a specific content path, the following optional parameters can be used with 
    /// <see cref="SoundsRoot.CreateSoundContext"/> when creating an instance of this class:
	/// A <see cref="ContentManager"/> - A custom content manager instance to supply the implicit one.
	/// A <see cref="PathString"/> - A content path for an implicitly created <see cref="ContentManager"/>
	/// That way a single and exclusive sound content path can be specified.
	/// </remarks>
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
        /// Used to keep track of how many times <see cref="Initialize"/> was called,
        /// not to be initializing the xna's content manager more than once.
        /// When it reaches zero while disposing, the content manager can be freed.
        /// </summary>
        static int __numInitializations;

        /// <summary>
        /// The content manager used with instances of this class
        /// </summary>
        static ContentManager __content;

        /// <summary>
        /// If a custom <see cref="ContentManager"/> instance was specified, we must not explicitly dispose it.
        /// </summary>
        static bool __canDisposeContentManager;
        
        #endregion

        #region Nested

        private class SimpleServiceProvider : IServiceProvider
        {
            #region IServiceProvider

            public object GetService(Type serviceType)
            {
                return null;
            }

            #endregion
        }

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public ContentManager Content
        {
            get { return __content; }
        }

        #endregion

        #region Methods

        /// <summary>
        /// By default, sound resources are looked up in Axiom's <see cref="Axiom.Core.ResourceGroupManager"/>.
        /// This override allows to specify either a custom <see cref="ContentManager"/> or a custom content path
        /// by passing a <see cref="ContentManager"/> or a <see cref="PathString"/> object
        /// in the optional parameters. 
        /// </summary>
        /// <param name="args"></param>
        protected override void Initialize(params object[] args)
		{
            if (__numInitializations == 0)
            {
            	string contentPath = FindArgument<PathString>(args);
            	ContentManager cm = FindArgument<ContentManager>(args);
            	Contract.Requires(!(contentPath != null && cm != null), "Specifying both the content manager and the path is not allowed.");
            	
            	if (cm != null)
            		__content = cm;
            	else if (contentPath != null)
            		__content = new ContentManager(new SimpleServiceProvider(), contentPath);
            	else
					__content = new SoundSystemsContentManager(new SimpleServiceProvider(), string.Empty);
                
            	__canDisposeContentManager = cm == null;
            }
            __numInitializations++;

            LogManager.Instance.Write("Xna.Simple sound system #{0} initialized", __numInitializations);

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

        #endregion

        #region IDisposable

        protected override void dispose(bool disposeManagedResources)
        {
            if (!IsDisposed)
            {
            	if (disposeManagedResources)
                {
                    if (--__numInitializations == 0)
                    {
                    	if (__canDisposeContentManager)
                        	__content.Dispose();
                    	
                        __content = null;
                    }
                }

                base.dispose(disposeManagedResources);
            }
        }
        
        #endregion
    }
}
