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
namespace Axiom.SkyX.Clouds
{
    public class VClouds : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        private bool _IsCreated;
        /// <summary>
        /// 
        /// </summary>
        private Vector2 _height;
        /// <summary>
        /// 
        /// </summary>
        private Radian _alpha;
        /// <summary>
        /// 
        /// </summary>
        private Radian _beta;
        /// <summary>
        /// 
        /// </summary>
        private float _radius;
        /// <summary>
        /// 
        /// </summary>
        private int _numberOfBlocks;
        /// <summary>
        /// 
        /// </summary>
        private int _na;
        /// <summary>
        /// 
        /// </summary>
        private int _nb;
        /// <summary>
        /// 
        /// </summary>
        private int _nc;
        /// <summary>
        /// 
        /// </summary>
        private Radian _windDirection;
        /// <summary>
        /// 
        /// </summary>
        private float _windSpeed;
        /// <summary>
        /// 
        /// </summary>
        private Vector2 _weather;
        /// <summary>
        /// 
        /// </summary>
        private int _numberOfForcedUpdates;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 _sunDirection;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 _sunColor;
        /// <summary>
        /// 
        /// </summary>
        private Vector3 _ambientColor;
        /// <summary>
        /// 
        /// </summary>
        private Vector4 _lightResponse;
        /// <summary>
        /// 
        /// </summary>
        private Vector4 _ambientFactors;
        /// <summary>
        /// 
        /// </summary>
        private float _globalOpacity;
        /// <summary>
        /// 
        /// </summary>
        private float _cloudFieldScale;
        /// <summary>
        /// 
        /// </summary>
        private float _noiseScale;
        /// <summary>
        /// 
        /// </summary>
        private DataManager _dataManager;
        /// <summary>
        /// 
        /// </summary>
        private GeometryManager _geometryManager;
        /// <summary>
        /// 
        /// </summary>
        private SceneManager _sceneManager;
        /// <summary>
        /// 
        /// </summary>
        private Camera _camera;

        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get { return _IsCreated; }
            private set { _IsCreated = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2 Height
        {
            get { return _height; }
            private set { _height = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float Radius
        {
            get { return _radius; }
            private set { _radius = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Radian WindDirection
        {
            set { _windDirection = value; }
            get { return _windDirection; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2 WindDirectionAsVector2
        {
            get { return new Vector2(Utility.Cos(_windDirection), Utility.Sin(_windDirection)); }
        }
        /// <summary>
        /// 
        /// </summary>
        public float WindSpeed
        {
            set { _windSpeed = value; }
            get { return _windSpeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 SunDirection
        {
            get { return _sunDirection; }
            set { _sunDirection = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 SunColor
        {
            get { return _sunColor; }
            set 
            { 
                _sunColor = value;
                if (!this.IsCreated)
                {
                    return;
                }

                Material mat = GetMaterial();
                mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uSunColor", _sunColor);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 AmbientColor
        {
            set 
            { 
                _ambientColor = value;
                if (!this.IsCreated)
                {
                    return;
                }

                Material mat = GetMaterial();
                mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uAmbientColor", _ambientColor);
            }
            get { return _ambientColor; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector4 LightResponse
        {
            get { return _lightResponse; }
            set 
            { 
                _lightResponse = value;

                if (!this.IsCreated)
                {
                    return;
                }

                Material mat = GetMaterial();
                mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uLightResponse", _lightResponse);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector4 AmbientFactors
        {
            set 
            { 
                _ambientFactors = value;

                if (!this.IsCreated)
                {
                    return;
                }

                Material mat = GetMaterial();
                mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uAmbientFactors", _ambientFactors);
            }
            get { return _ambientFactors; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float GlobalOpacity
        {
            get { return _globalOpacity; }
            set { _globalOpacity = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float CloudFieldScale
        {
            set { _cloudFieldScale = value; }
            get { return _cloudFieldScale; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float NoiseScale
        {
            get { return _noiseScale; }
            set { _noiseScale = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneManager SceneManager
        {
            get { return _sceneManager; }
            private set { _sceneManager = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Camera Camera
        {
            get { return _camera; }
            private set { _camera = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public DataManager DataManager
        {
            get { return _dataManager; }
            private set { _dataManager = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public GeometryManager GeometryManager
        {
            get { return _geometryManager; }
            private set { _geometryManager = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="c"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        /// <param name="alpha"></param>
        /// <param name="beta"></param>
        /// <param name="numberOfBlocks"></param>
        /// <param name="na"></param>
        /// <param name="nb"></param>
        /// <param name="nc"></param>
        public VClouds(SceneManager sm, Camera c,
            Vector2 height, float radius, Radian alpha, Radian beta,
            int numberOfBlocks, int na, int nb, int nc)
        {
            this.SceneManager = sm;
            this.Camera = c;
            this.IsCreated = false;
            this.Height = height;
            _alpha = alpha;
            _beta = beta;
            this.Radius = radius;
            _numberOfBlocks = numberOfBlocks;
            _na = na;
            _nb = nb;
            _nc = nc;
            this.WindDirection = new Degree((Real)0);
            this.WindSpeed = 80f;
            _weather = new Vector2(1.0f, 1.0f);
            _numberOfForcedUpdates = -1;
            this.SunDirection = new Vector3(0, -1, 0);
            this.SunColor = new Vector3(1, 1, 1);
            this.AmbientColor = new Vector3(0.63f, 0.63f, 0.7f);
            this.LightResponse = new Vector4(0.25f, 0.2f, 1.0f, 0.1f);
            this.AmbientFactors = new Vector4(0.4f, 1.0f, 1.0f, 1.0f);
            this.GlobalOpacity = 1.0f;
            this.CloudFieldScale = 1.0f;
            this.NoiseScale = 4.2f;
            this.DataManager = null;
            this.GeometryManager = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sm"></param>
        /// <param name="c"></param>
        /// <param name="height"></param>
        /// <param name="radius"></param>
        public VClouds(SceneManager sm, Camera c,
            Vector2 height, float radius)
        {
            this.SceneManager = sm;
            this.Camera = c;
            this.IsCreated = false;
            this.Height = height;
            _alpha = new Radian(new Degree((Real)12));
            _beta = new Radian(new Degree((Real)40));
            this.Radius = radius;
            _numberOfBlocks = 12;
            _na = 10;
            _nb = 8;
            _nc = 6;
            this.WindDirection = new Degree((Real)0);
            this.WindSpeed = 80f;
            _weather = new Vector2(1.0f, 1.0f);
            _numberOfForcedUpdates = -1;
            this.SunDirection = new Vector3(0, -1, 0);
            this.SunColor = new Vector3(1, 1, 1);
            this.AmbientColor = new Vector3(0.63f, 0.63f, 0.7f);
            this.LightResponse = new Vector4(0.25f, 0.2f, 1.0f, 0.1f);
            this.AmbientFactors = new Vector4(0.4f, 1.0f, 1.0f, 1.0f);
            this.GlobalOpacity = 1.0f;
            this.CloudFieldScale = 1.0f;
            this.NoiseScale = 4.2f;
            this.DataManager = null;
            this.GeometryManager = null;
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
        public void Create()
        {
            Remove();

            //Data manager
            this.DataManager = new DataManager(this);
            this.DataManager.Create(128, 128, 20);

            //Geometry manager
            this.GeometryManager = new GeometryManager(this, _height, _radius, _alpha, _beta, _numberOfBlocks, _na, _nb, _nc);
            this.GeometryManager.Create();
            Material mat = (Material)MaterialManager.Instance.GetByName("SkyX_VolClouds");
            mat.GetTechnique(0).GetPass(0).VertexProgramParameters.SetNamedConstant("uRadius", _radius);

            this.IsCreated = true;

            // Update material parameters
            this.SunColor = _sunColor;
            this.AmbientColor = _ambientColor;
            this.LightResponse = _lightResponse;
            this.AmbientFactors = _ambientFactors;

            // Set current wheater
            int nforced = (_numberOfForcedUpdates == -1) ? 2 : _numberOfForcedUpdates;
            SetWeather(_weather.x, _weather.y, nforced);
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
            _dataManager = null;
            _geometryManager = null;
            this.IsCreated = false;
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
            _dataManager.Update(timeSinceLastFrame);
            _geometryManager.Update(timeSinceLastFrame);
            Material mat = (Material)MaterialManager.Instance.GetByName("SkyX_VolClouds");
            mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uInterpolation", _dataManager.Interpolation);

            mat.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uSunDirection", -_sunDirection);
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="humidity"></param>
        /// <param name="averageCloudSize"></param>
        public void SetWeather(float humidity, float averageCloudSize)
        {
            SetWeather(humidity, averageCloudSize, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="humidity"></param>
        /// <param name="averageCloudSize"></param>
        /// <param name="numberOfForcedUpdates"></param>
        public void SetWeather(float humidity, float averageCloudSize, int numberOfForcedUpdates)
        {
            _weather = new Vector2(humidity, averageCloudSize);
            _numberOfForcedUpdates = numberOfForcedUpdates;
            if (!this.IsCreated)
            {
                return;
            }

            _dataManager.SetWeather(_weather.x, _weather.y, _numberOfForcedUpdates);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Material GetMaterial()
        {
            return (Material)MaterialManager.Instance.GetByName("SkyX_VolClouds");
        }
    }
}
