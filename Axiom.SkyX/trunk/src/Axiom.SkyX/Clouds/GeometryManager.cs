#region MIT/X11 License
// This file is part of the Axiom.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Axiom.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using Axiom.Math;
using Axiom.Core;

namespace Axiom.SkyX.Clouds
{
    public class GeometryManager : IDisposable
    {
        private bool _isCreated;
        private Vector2 _height;
        private Radian _alpha;
        private Radian _beta;
        private float _radius;
        private Radian _phie;
        private int _numberOfBlocks;
        private int _na;
        private int _nb;
        private int _nc;
        private Vector2 _worldOffset;
        private List<GeometryBlock> _geometryBlocks = new List<GeometryBlock>();
        private SceneNode _sceneNode;
        private Vector3 _lastCameraPosition;
        private VClouds _vclouds;

        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            private set { _isCreated = value; }
            get { return _isCreated; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vc"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="numberOfBlocks"></param>
        /// <param name="na"></param>
        /// <param name="nb"></param>
        /// <param name="nc"></param>
        public GeometryManager(VClouds vc,
            Vector2 height, float radius,
            Radian alpha, Radian beta,
            int numberOfBlocks, int na, int nb, int nc)
        {
            _vclouds = vc;
            this.IsCreated = false;
            _height = height;
            _radius = radius;
            _alpha = alpha;
            _beta = beta;
            _phie = Utility.TWO_PI / (float)numberOfBlocks;
            _numberOfBlocks = numberOfBlocks;
            _na = na;
            _nb = nb;
            _nc = nc;
            _worldOffset = new Vector2(0, 0);
            _lastCameraPosition = Vector3.Zero;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSinceLastFrame"></param>
        public void Update(float timeSinceLastFrame)
        {
            if (!this.IsCreated)
            {
                return;
            }

            _sceneNode.Position = new Vector3(_vclouds.Camera.DerivedPosition.x, _height.x, _vclouds.Camera.DerivedPosition.z);
           
            UpdateGeometry(timeSinceLastFrame);
            _lastCameraPosition = _vclouds.Camera.DerivedPosition;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            Remove();

            _sceneNode = _vclouds.SceneManager.RootSceneNode.CreateChildSceneNode();
            CreateGeometry();
            this.IsCreated = true;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (!this.IsCreated)
            {
                return;
            }

            _sceneNode.DetachAllObjects();
            _sceneNode.Parent.RemoveAndDestroyChild(_sceneNode.Name);
            _sceneNode = null;

            _geometryBlocks.Clear();
            this.IsCreated = false;
        }
        /// <summary>
        /// 
        /// </summary>
        private void CreateGeometry()
        {
            for (int k = 0; k < _numberOfBlocks; k++)
            {
                _geometryBlocks.Add(new GeometryBlock(_vclouds, _height.y, _alpha, _beta, _radius, _phie, _na, _nb, _nc, k));
                _geometryBlocks[k].Create();
                // Each geometry block must be in a different scene node, See: GeometryBlock::isInFrustum(Ogre::Camera *c)
                SceneNode sn = _sceneNode.CreateChildSceneNode();
                sn.AttachObject(_geometryBlocks[k].Entity);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeSinceLastFrame"></param>
        public void UpdateGeometry(float timeSinceLastFrame)
        {
            // Calculate wind offset
            Vector2 cameraDirection = new Vector2(_vclouds.Camera.DerivedDirection.x, _vclouds.Camera.DerivedDirection.z);
            float offset = -cameraDirection.Dot(_vclouds.WindDirectionAsVector2) * _vclouds.WindSpeed * timeSinceLastFrame;
            _worldOffset += _vclouds.WindDirectionAsVector2 * _vclouds.WindSpeed * timeSinceLastFrame;

            // Calculate camera offset
            Vector2 cameraOffset = new Vector2(_vclouds.Camera.DerivedPosition.x - _lastCameraPosition.x, _vclouds.Camera.DerivedPosition.z - _lastCameraPosition.z);
            offset -= cameraOffset.Dot(cameraDirection);
            _worldOffset += cameraOffset;

            for (int k = 0; k < _numberOfBlocks; k++)
            {
                _geometryBlocks[k].Update(offset);
                _geometryBlocks[k].WorldOffset = _worldOffset;
            }
        }
    }
}
