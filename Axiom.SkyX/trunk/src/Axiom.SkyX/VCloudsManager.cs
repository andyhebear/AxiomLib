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
using Axiom.Graphics;
using Axiom.Media;
using Axiom.SkyX.Clouds;
namespace Axiom.SkyX
{
    public class VCloudsManager : IDisposable
    {

        private ColorGradient _ambientGradient;
        private ColorGradient _sunGradient;
        private VClouds _clouds;
        private Vector2 _height;
        private bool _autoUpdate;
        private float _windSpeed;
        private bool _isCreated;
        private SkyX _skyX;

        /// <summary>
        /// 
        /// </summary>
        public ColorGradient AmbientGradient
        {
            set { _ambientGradient = value; }
            get { return _ambientGradient; }
        }
        /// <summary>
        /// 
        /// </summary>
        public ColorGradient SunGradient
        {
            set { _sunGradient = value; }
            get { return _sunGradient; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2 Height
        {
            set { _height = value; }
            get { return _height; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoUpdate
        {
            get { return _autoUpdate; }
            set
            {
                _autoUpdate = value;
                UpdateWindSpeedConfig();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public float WindSpeed
        {
            get { return _windSpeed; }
            set
            {
                _windSpeed = value;
                UpdateWindSpeedConfig();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public VClouds VClouds
        {
            get { return _clouds; }
            private set { _clouds = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get { return _isCreated; }
            private set { _isCreated = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SkyX SkyX
        {
            get { return _skyX; }
            private set { _skyX = value; }
        }
        
        #region Construction and Destruction

        public VCloudsManager(SkyX skyX)
        {
            this.SkyX = skyX;
            _height = new Vector2(-1, -1);
            _windSpeed = 800.0f;
            _autoUpdate = true;
            _isCreated = false;

            _ambientGradient = new ColorGradient();
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.9f, 1.0f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.7f, 0.7f, 0.65f), 0.625f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.55f, 0.4f) * 0.5f, 0.5625f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.55f, 0.4f) * 0.2f, 0.475f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.45f, 0.3f) * 0.1f, 0.5f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.2f, 0.2f, 0.3f) * 0.25f, 0.35f));
            _ambientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.2f, 0.2f, 0.3f) * 0.3f, 0));

            _sunGradient = new ColorGradient();
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.9f, 1.0f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.8f, 0.75f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.8f, 0.75f, 0.55f) * 1.3f, 0.5625f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.5f, 0.2f) * 1.5f, 0.5f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.5f, 0.2f) * 0.2f, 0.4725f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.5f, 0.5f, 0.5f) * 0.05f, 0.45f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.25f, 0.25f, 0.25f), 0.3725f));
            _sunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.5f, 0.5f, 0.5f), 0.0f));

        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }
        #endregion Construction and Destruction

        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            if (this.IsCreated)
            {
                return;
            }

            float radius = this.SkyX.Camera.Far;
            // Use default options if the user haven't set any specific Height parameters
            Vector2 defaultHeight = new Vector2(radius * 0.03f, radius * 0.0525f);
            Vector2 height = (_height.x == -1 || _height.y == -1) ? defaultHeight : _height;

            this.VClouds = new VClouds(this.SkyX.SceneManager, this.SkyX.Camera, height, radius * 0.8f);
            this.VClouds.SetWeather(0.5f, 1, 4);
            this.VClouds.Create();

            this.IsCreated = true;

            UpdateWindSpeedConfig();

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

            SetLightParameters();

            this.VClouds.Update(timeSinceLastFrame);
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

            this.VClouds.Dispose();
            this.VClouds = null;
            this.IsCreated = false;
        }

        /// <summary>
        /// 
        /// </summary>
        private void SetLightParameters()
        {
            Vector3 sunDir = this.SkyX.AtmosphereManager.SunDirection;
            if (sunDir.y > 0.1f)
            {
                sunDir = -sunDir;
            }

            this.VClouds.SunDirection = sunDir;

            float point = (-this.SkyX.AtmosphereManager.SunDirection.y + 1.0f) / 2.0f;

            this.VClouds.AmbientColor = _ambientGradient.GetColor(point);
            this.VClouds.SunColor = _sunGradient.GetColor(point);
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdateWindSpeedConfig()
        {
            if (!this.IsCreated)
            {
                return;
            }

            if (this.AutoUpdate)
            {
                this.VClouds.WindSpeed = this.SkyX.TimeMultiplier * this.WindSpeed;
            }
            else
            {
                this.VClouds.WindSpeed = this.WindSpeed;
            }
        }
    }
}
