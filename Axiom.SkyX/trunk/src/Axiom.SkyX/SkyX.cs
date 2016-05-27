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
using System.Diagnostics;
using Axiom.Core;
using Axiom.Math;

namespace Axiom.SkyX
{
    /// <summary>
    /// Main controller for SkyX
    /// </summary>
    public sealed class SkyX
    {
        #region - constants -
        public const string SkyXResourceGroup = "SkyX";
        public const int VersionMajor = 0;
        public const int VersionMinor = 1;
        public const int VersionPatch = 0;
        #endregion

        #region Fields and Properties
        /// <summary>
        /// Lighting mode
        /// </summary>
        private LightingMode _lightingMode;
        /// <summary>
        /// Time offset
        /// </summary>
        private float _timeOffset;
        /// <summary>
        /// Time multiplier
        /// </summary>
        private float _timeMultiplier;
        /// <summary>
        /// Enable starfield?
        /// </summary>
        private bool _IsStarFieldEnabled;
        /// <summary>
        /// Main axiom camera
        /// </summary>
        private Camera _camera;
        /// <summary>
        /// reference to the scenemanager
        /// </summary>
        private SceneManager _sceneManager;
        /// <summary>
        /// volumetric cloudsmanager
        /// </summary>
        private VCloudsManager _vcloudsManager;
        /// <summary>
        /// Clouds manager
        /// </summary>
        private CloudsManager _cloudsManager;
        /// <summary>
        /// Moon manager
        /// </summary>
        private MoonManager _moonManager;
        /// <summary>
        /// 
        /// </summary>
        private GpuManager _gpuManager;
        /// <summary>
        /// 
        /// </summary>
        private AtmosphereManager _atmosphereManager;
        /// <summary>
        /// Mesh manager
        /// </summary>
        private MeshManager _meshManager;
        /// <summary>
        /// is skyx created?
        /// </summary>
        private bool _isCreated;
        /// <summary>
        /// Has <see cref="SkyX.Create"/> been called?
        /// </summary>
        public Boolean IsCreated
        {
            get { return _isCreated; }
            private set { _isCreated = value; }
        }

        /// <summary>
        /// Get/Set the time multiplier
        /// </summary>
        /// <remarks>
        /// The time multiplier can be a negative number, 0 will disable auto-updating
        /// For setting a custom time of day, <see cref="AtmosphereManager.Options.Time"/> 
        /// </remarks>
        public Real TimeMultiplier
        {
            get { return _timeMultiplier; }
            set { _timeMultiplier = value; }
        }

        /// <summary>
        /// Get/Set the TimeOffset
        /// </summary>
        /// <remarks>
        /// For internal use only
        /// </remarks>
        internal Real TimeOffset
        {
            get { return _timeOffset; }
            private set { _timeOffset = value; }
        }

        /// <summary>
        /// Get the Mesh Manager
        /// </summary>
        public MeshManager MeshManager
        {
            get { return _meshManager; }
            private set { _meshManager = value; }
        }

        /// <summary>
        /// Get the Atmosphere Manager
        /// </summary>
        public AtmosphereManager AtmosphereManager
        {
            get { return _atmosphereManager; }
            private set { _atmosphereManager = value; }
        }

        /// <summary>
        /// Get the Gpu Manager
        /// </summary>
        public GpuManager GpuManager
        {
            get { return _gpuManager; }
            private set { _gpuManager = value; }
        }

        /// <summary>
        /// Get the Moon Manager
        /// </summary>
        public MoonManager MoonManager
        {
            get { return _moonManager; }
            private set { _moonManager = value; }
        }

        /// <summary>
        /// Get the Clouds Manager
        /// </summary>
        public CloudsManager CloudsManager
        {
            get { return _cloudsManager; }
            private set { _cloudsManager = value; }
        }

        /// <summary>
        /// Get the Volumetric Clouds Manager
        /// </summary>
        public VCloudsManager VCloudsManager
        {
            get { return _vcloudsManager; }
            private set { _vcloudsManager = value; }
        }

        /// <summary>
        /// Get/Sets the lighting mode
        /// </summary>
        /// <remarks>
        /// SkyX is designed for true HDR rendering, but there're a lot of applications
        /// that doesn't use HDR rendering, due to this a little exponential tone-mapping 
        /// algoritm is applied to SkyX materials if LightingMode.Ldr is selected. <see cref="AtmosphereManager.Options.Exposure"/>
        /// Select LightingMode.Hdr if your app is designed for true HDR rendering.
        /// </remarks>
        public LightingMode LightingMode
        {
            get { return _lightingMode; }
            set
            {
                _lightingMode = value;
                if (!this.IsCreated)
                {
                    return;
                }

                // Update skydome material
                _meshManager.MaterialName = _gpuManager.SkydomeMaterialName;
                // Update layered clouds material
                _cloudsManager.RegisterAll();
                // Update ground passes materials
                _gpuManager.UpdateFP();

                // Update parameters
                _atmosphereManager.Update(_atmosphereManager.Options, true);
            }
        }

        /// <summary>
        /// Enable/disable the starfield
        /// </summary>
        public bool IsStarFieldEnabled
        {
            get { return _IsStarFieldEnabled; }
            set
            {
                _IsStarFieldEnabled = value;
                if (!this.IsCreated)
                {
                    return;
                }
                // Update skydome material
                _meshManager.MaterialName = _gpuManager.SkydomeMaterialName;
                // Update parameters
                _atmosphereManager.Update(_atmosphereManager.Options, true);
            }
        }

        /// <summary>
        /// Get the <see cref="SceneManager"/>
        /// </summary>
        public SceneManager SceneManager
        {
            get { return _sceneManager; }
            private set { _sceneManager = value; }
        }

        /// <summary>
        /// Get the <see cref="Camera"/>
        /// </summary>
        public Camera Camera
        {
            get { return _camera; }
            private set { _camera = value; }
        }

        /// Last camera position
        private Vector3 _lastCameraPosition;
        /// Last camera far clip distance
        private Real _lastCameraFarClipDistance;

        #endregion Fields and Properties

        #region Construction & Destruction

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="sceneManager">Axiom SceneManager</param>
        /// <param name="camera">Axiom Camera</param>
        public SkyX( SceneManager sceneManager, Camera camera )
        {
            if ( sceneManager == null )
                throw new ArgumentNullException( "sceneManager", "A valid SceneManager must be provided." );
            if ( camera == null )
                throw new ArgumentNullException( "camera", "A valid Camera must be provided." );

            this.SceneManager = sceneManager;
            this.Camera = camera;

            this.MeshManager = new MeshManager(this);
            this.GpuManager = new GpuManager(this);
            this.AtmosphereManager = new AtmosphereManager(this);
            this.MoonManager = new MoonManager(this);
            this.CloudsManager = new CloudsManager(this);
            this.VCloudsManager = new VCloudsManager(this);

		    this.IsCreated = false;
            this._lastCameraPosition = new Vector3(0, 0, 0);
            this._lastCameraFarClipDistance = 0;
            this.LightingMode = LightingMode.Ldr;
            this.IsStarFieldEnabled = true;
            this.TimeMultiplier = 0.1f;
            this.TimeOffset = 0.0f;

        }

        #endregion Construction & Destruction

        #region SkyX Management

        /// <summary>
        /// Create SkyX and add to the scene
        /// </summary>
        public void Create()
        {
            if ( IsCreated )
            {
                return;
            }

            MeshManager.Create();

            MeshManager.MaterialName = GpuManager.SkydomeMaterialName;

            AtmosphereManager.Update( AtmosphereManager.Options, true );

            _lastCameraPosition = Camera.DerivedPosition;
            _lastCameraFarClipDistance = Camera.Far;

            MoonManager.Create();

            IsCreated = true;
        }

        /// <summary>
        /// Remove SkyX, you can call this method to remove SkyX from the scene
        /// or release (secondary) SkyX memory, call <see cref="Create"/> to return SkyX to the scene.
        /// </summary>
        public void Remove()
        {
            if ( IsCreated )
            {
                return;
            }

            CloudsManager.RemoveAll();
            MeshManager.Remove();
            MoonManager.Remove();
            VCloudsManager.Remove();

            IsCreated = false;
        }

        /// <summary>
        /// Call every frame to update SkyX 
        /// </summary>
        /// <param name="timeSinceLastFrame">time elapsed since last frame</param>
        public void Update( Real timeSinceLastFrame )
        {
            if (!IsCreated)
            {
                return;
            }
            if (TimeMultiplier != 0)
            {
                AtmosphereManager.AtmosphereOptions opt = AtmosphereManager.Options;

                float timemultiplied = timeSinceLastFrame * TimeMultiplier;
                opt.Time.x += timemultiplied;
                TimeOffset += timemultiplied;
                if (opt.Time.x > 24)
                {
                    opt.Time.x -= 24;
                }
                else if (opt.Time.x < 0)
                {
                    opt.Time.x += 24;
                }

                AtmosphereManager.Options = opt;
            }

            if (_lastCameraPosition != Camera.DerivedPosition)
            {
                MeshManager.SceneNode.Position = Camera.DerivedPosition;
                MoonManager.MoonSceneNode.Position = Camera.DerivedPosition;

                _lastCameraPosition = Camera.DerivedPosition;
            }

            if (_lastCameraFarClipDistance != Camera.Far)
            {
                MeshManager.UpdateGeometry();
                MoonManager.Update();

                _lastCameraFarClipDistance = Camera.Far;
            }

            VCloudsManager.Update(timeSinceLastFrame);
        }

        #endregion SkyX Management
    }
}
