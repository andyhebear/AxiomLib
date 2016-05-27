#region License
/*
    Gorilla
    -------
    
    Copyright (c) 2010 Robin Southern
 
    This is a c# (Axiom) port of Gorrilla, developed by Robin Southern, ported by me (bostich)

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

#region SVN Version Information
// <file>
//     <id value="$Id: 2118 2010-09-26 23:56:56Z bostich $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations
using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// Main singleton class for Gorilla
    /// </summary>
    public class Silverback : IDisposable
    {
		public static long RenderCalls = 0;
        protected Dictionary<string, TextureAtlas> _atlases;
        protected List<Screen> _screens;
        protected List<ScreenRendable> _screenRendables;
        /// <summary>
        /// Silverback constructor.
        /// </summary>
        public Silverback()
        {
            _atlases = new Dictionary<string, TextureAtlas>();
            _screens = new List<Screen>();
            _screenRendables = new List<ScreenRendable>();
            Root.Instance.FrameStarted += new EventHandler<FrameEventArgs>(FrameStarted);
        }
        public void Dispose()
        {
            Root.Instance.FrameStarted -= FrameStarted;
        }
        void FrameStarted(object sender, FrameEventArgs e)
        {
            foreach (ScreenRendable rend in _screenRendables)
                rend.FrameStarted();
        }

        /// <summary>
        /// Create a TextureAtlas from a ".gorilla" file. 
        /// </summary>
        /// <param name="name">Name is the name of the TextureAtlas, as well as the first part of the filename
        /// of the gorilla file; i.e. name.gorilla, the gorilla file can be loaded from a different
        /// resource group if you give that name as the second argument, otherwise it will assume 
        /// to be "General".</param>
        public void LoadAtlas( string name )
        {
            LoadAtlas( name, ResourceGroupManager.DefaultResourceGroupName );
        }
        /// <summary>
        /// Create a TextureAtlas from a ".gorilla" file. 
        /// </summary>
        /// <param name="name">Name is the name of the TextureAtlas, as well as the first part of the filename
        /// of the gorilla file; i.e. name.gorilla, the gorilla file can be loaded from a different
        /// resource group if you give that name as the second argument, otherwise it will assume 
        /// to be "General".</param>
        /// <param name="group">group to be loaded from, default is 'General'</param>
        public void LoadAtlas(string name, string group )
        {
            TextureAtlas atlas = new TextureAtlas(name + ".gorilla", group);
            _atlases[name] = atlas;
        }
        /// <summary>
        /// Create a Screen using a Viewport and a name of a previously loaded TextureAtlas.
        /// Both must exist. The screen will register itself as a RenderQueueListener to the
        /// SceneManager that has the Camera which is tied to the Viewport.
        /// </summary>
        /// <param name="viewport">viewport where we 'attach' to</param>
        /// <param name="atlas">name of the atlas texture, must be allready loaded</param>
        /// <returns>new screen</returns>
        /// <note>
        /// Each screen is considered a new batch. To reduce your batch count in Gorilla, 
        /// reduce the number of screens you use.
        /// </note>
        public Screen CreateScreen(Viewport viewport, string atlas)
        {
            TextureAtlas tatlas = _atlases[atlas];
            Screen screen = new Screen(viewport, tatlas);
            _screens.Add(screen);
            return screen;
        }
        /// <summary>
        /// Creates a new Screen renabdle object.
        /// </summary>
        /// <param name="maxSize"></param>
        /// <param name="atlas"></param>
        /// <returns></returns>
        public ScreenRendable CreateScreenRendable(Vector2 maxSize, string atlas)
        {
            TextureAtlas attlas = _atlases[atlas];
            ScreenRendable screen = new ScreenRendable(maxSize, attlas);
            _screenRendables.Add(screen);
            return screen;
        }
        /// <summary>
        /// estroy an existing screen, its layers and the contents of those layers.
        /// </summary>
        /// <param name="screen">screen to destroy</param>
        public void DestroyScreen(Screen screen)
        {
        }
        /// <summary>
        /// Destroy an existing screen, its layers and the contents of those layers.
        /// </summary>
        /// <param name="rendable"></param>
        public void DestroyScreenRendable(ScreenRendable rendable)
        {
            _screenRendables.Remove(rendable);
        }
    }
}
