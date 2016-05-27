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
using Axiom.Graphics;
using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// A screen is a container class for all 2D operations upon the RenderWindow, it is comparible<para></para>
    /// in function to the SceneManager. Each Screen may use a seperate TextureAtlas or share one <para></para>
    /// together, but each Screen rendered seperately as one batch. So five screens is five batches.<para></para>
    /// <para></para>      
    /// Typically it is normal just to have one screen, or a second one for debug output. Screens need a <para></para>
    /// Viewport to render too, and will get the SceneManager from the Viewport's Camera to listen for <para></para>
    /// drawing updates. All Screen drawings are done after the RENDER_QUEUE_OVERLAY renderqueue.<para></para>
    /// </summary>
    public class Screen : LayerContainer, IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        protected RenderSystem _renderSystem;
        protected SceneManager _sceneManager;
        protected Viewport _viewport;
        protected Real _viewportWidth;
        protected Real _viewportHeight;
        protected Real _invViewportWidth;
        protected Real _invViewportHeight;
        /// <summary>
        /// Gets the width of the viewport that the screen uses.
        /// </summary>
        public Real ViewportWidth
        {
            get { return _viewportWidth; }
        }
        /// <summary>
        /// Gets the height of the viewport that the screen uses.
        /// </summary>
        public Real ViewportHeight
        {
            get { return _viewportHeight; }
        }
        /// <summary>
        /// Gets the horizontal offset provided by the RenderSystem for texels.
        /// </summary>
        public override Real TexelOffsetX
        {
            get { return _renderSystem.HorizontalTexelOffset; }
        }
        /// <summary>
        /// Gets the vertical offset provided by the RenderSystem for texels.
        /// </summary>
        public override Real TexelOffsetY
        {
            get { return _renderSystem.VerticalTexelOffset; }
        }
        /// <summary>
        /// Gets the rendersystem that the Screen and Axiom is using.
        /// </summary>
        public RenderSystem RenderSystem
        {
            get { return _renderSystem; }
        }
        public Real Width
        {
            get { return _viewportWidth; }
        }
        public Real Height
        {
            get { return _viewportHeight; }
        }
        /// <summary>
        /// Default ctor
        /// </summary>
        /// <param name="viewport"></param>
        /// <param name="atlas"></param>
        internal Screen(Viewport viewport, TextureAtlas atlas)
            :base(atlas)
        {
            _viewport = viewport;
            _sceneManager = _viewport.Camera.SceneManager;
            _renderSystem = Root.Instance.RenderSystem;
            _sceneManager.QueueEnded += new EventHandler<SceneManager.EndRenderQueueEventArgs>(QueueEnded);
            _viewportWidth = _viewport.ActualWidth;
            _viewportHeight = _viewport.ActualHeight;
            _invViewportHeight = 1.0f / _viewportHeight;
            _invViewportWidth = 1.0f / _viewportWidth;

            CreateVertexBuffer();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            if (_sceneManager != null)
                _sceneManager.QueueEnded -= QueueEnded;
        }
        void QueueEnded(object sender, SceneManager.EndRenderQueueEventArgs e)
        {
            if (e.RenderQueueId != RenderQueueGroupID.Overlay)
                return;

            RenderOnce();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RenderOnce()
        {
            bool force = false;
            // force == true if viewport size changed.
            RenderVertices(force);
            if (_renderOp.vertexData.vertexCount != 0)
            {
                PrepareRenderSystem();
                _renderSystem.Render(_renderOp);
				Silverback.RenderCalls++;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
		public override void Transform( ref DynamicBuffer<Vertex> vertices, int begin, int end )
		{
			for ( int i = begin; i < end; i++ )
			{

				float x = ( ( vertices[ i ].Position.x ) * _invViewportWidth ) * 2 - 1;
				float y = ( ( vertices[ i ].Position.y ) * _invViewportHeight ) * -2 + 1;
				Vertex v = vertices[ i ];
				v.Position = new Vector3( x, y, 0 );
				vertices[ i ] = v;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        protected void PrepareRenderSystem()
        {
            _renderSystem.WorldMatrix = Matrix4.Identity;
            _renderSystem.ProjectionMatrix = Matrix4.Identity;
            _renderSystem.ViewMatrix = Matrix4.Identity;
            _sceneManager.SetPass(_atlas.Pass2D);
        }
    }
}
