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
using Axiom.Media;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.SkyX
{
    /// <summary>
    /// 
    /// </summary>
    public partial class CloudLayer : IDisposable
    {
        /// <summary>
        /// cloud layer options.
        /// </summary>
        private Options _options;
        /// <summary>
        /// Ambient color gradient
        /// </summary>
        private ColorGradient _ambientGradiant;
        /// <summary>
        /// Sun color gradient
        /// </summary>
        private ColorGradient _sunGradiant;
        /// <summary>
        ///  Cloud layer pass
        /// </summary>
        private Pass _cloudLayerPass;
        /// <summary>
        /// SkyX parent reference
        /// </summary>
        private SkyX _parent;

        /// <summary>
        /// Get's the options.
        /// </summary>
        public Options LayerOptions
        {
            get { return _options; }
            private set { _options = value; }
        }
        /// <summary>
        /// Get's or set's the ambient gradient.
        /// </summary>
        public ColorGradient AmbientGradient
        {
            set { _ambientGradiant = value; }
            get { return _ambientGradiant; }
        }
        /// <summary>
        /// Get's or set's the sun gradient.
        /// </summary>
        public ColorGradient SunGradient
        {
            get { return _sunGradiant; }
            set { _sunGradiant = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SkyX SkyX
        {
            get { return _parent; }
            private set { _parent = value; }
        }
        /// <summary>
        /// Default onstructor
        /// </summary>
        /// <param name="skyX">skyX reference</param>
        public CloudLayer(SkyX skyX)
            : this(skyX, new Options())
        { }
        
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="skyX">skyX reference</param>
        /// <param name="options">Cloud layer options</param>
        public CloudLayer(SkyX skyX, Options options)
        {
            this.SkyX = skyX;
            this.LayerOptions = options;

            this.AmbientGradient = new ColorGradient();
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.95f, 1.0f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.7f, 0.7f, 0.65f), 0.625f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.55f, 0.4f), 0.5625f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.45f, 0.3f) * 0.4f, 0.5f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.5f, 0.25f, 0.25f) * 0.1f, 0.45f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.2f, 0.2f, 0.3f) * 0.1f, 0.35f));
            this.AmbientGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.2f, 0.2f, 0.5f) * 0.15f, 0));

            this.SunGradient = new ColorGradient();
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.95f, 1.0f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(1, 1, 1) * 0.8f, 0.75f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.8f, 0.75f, 0.55f) * 1.3f, 0.5625f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.5f, 0.2f) * 0.75f, 0.5f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.6f, 0.5f, 0.2f) * 0.35f, 0.4725f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.5f, 0.5f, 0.5f) * 0.15f, 0.45f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.2f, 0.2f, 0.25f) * 0.5f, 0.3f));
            this.SunGradient.AddFrame(new KeyValuePair<Vector3, float>(new Vector3(0.5f, 0.5f, 0.5f) * 0.35f, 0.0f));
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Unregister();
        }
        /// <summary>
        ///  Register layer
        /// </summary>
        /// <param name="cloudLayerPass">Pass where register the cloud layer</param>
        public void RegisterCloudLayer(Pass cloudLayerPass)
        {
            Unregister();

            cloudLayerPass.SetSceneBlending(SceneBlendType.TransparentAlpha);
            cloudLayerPass.CullingMode = CullingMode.None;
            cloudLayerPass.LightingEnabled = false;
            cloudLayerPass.DepthWrite = false;

            cloudLayerPass.SetVertexProgram("SkyX_Clouds_VP");
            if (this.SkyX.LightingMode == LightingMode.Ldr)
            {
                cloudLayerPass.SetFragmentProgram("SkyX_Clouds_LDR_FP");
            }
            else
            {
                cloudLayerPass.SetFragmentProgram("SkyX_Clouds_HDR_FP");
            }

            //TODO
            cloudLayerPass.CreateTextureUnitState("Cloud1.png").TextureAddressing = TextureAddressing.Wrap;
            cloudLayerPass.CreateTextureUnitState("c22n.png").TextureAddressing = TextureAddressing.Wrap;
            cloudLayerPass.CreateTextureUnitState("c22.png").TextureAddressing = TextureAddressing.Wrap;

            _cloudLayerPass = cloudLayerPass;
            UpdatePassParameters();
        }
        /// <summary>
        /// Unregister cloud pass
        /// </summary>
        public void Unregister()
        {
            if (_cloudLayerPass != null)
            {
                _cloudLayerPass.Parent.RemovePass(_cloudLayerPass);
                _cloudLayerPass = null;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdatePassParameters()
        {
            if (_cloudLayerPass == null)
            {
                return;
            }

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uScale", _options.Scale);
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uHeight", _options.Height);

            float[] windDirection = { _options.WindDirection.x, _options.WindDirection.y };

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uWindDirection", windDirection);

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uCloudLayerHeightVolume", _options.HeightVolume);
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uCloudLayerVolumetricDisplacement", _options.VolumetricDisplacement);
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uNormalMultiplier", _options.NormalMultiplier);
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uDetailAttenuation", _options.DetailAttenuation);
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uDistanceAttenuation", _options.DistanceAttenuation);
        }
        /// <summary>
        /// Update internal cloud pass parameters
        /// </summary>
        public void UpdateInternalPassParameters()
        {
            if (_cloudLayerPass == null)
            {
                return;
            }

            if (this.SkyX.LightingMode == LightingMode.Ldr)
            {
                _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uExposure", this.SkyX.AtmosphereManager.Options.Exposure);
            }

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uTime", this.SkyX.TimeOffset * _options.TimeMultiplier);

            Vector3 sunDir = this.SkyX.AtmosphereManager.SunDirection;
            if (sunDir.y > 0.15f)
            {
                sunDir = -sunDir;
            }

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uSunPosition", -sunDir * this.SkyX.MeshManager.SkydomeRadius);

            float point = (-this.SkyX.AtmosphereManager.SunDirection.y + 1.0f) / 2.0f;

            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uSunColor", _sunGradiant.GetColor(point));
            _cloudLayerPass.FragmentProgramParameters.SetNamedConstant("uAmbientLuminosity", _ambientGradiant.GetColor(point));
        }
    }
}