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
using Axiom.Graphics;
using Axiom.Math;
namespace Axiom.SkyX
{
    public class GpuManager
    {
        /// <summary>
        /// Ground pass list.
        /// </summary>
        private List<Pass> _groundPasses = new List<Pass>();
        /// <summary>
        /// Reference to skyX parent.
        /// </summary>
        private SkyX _skyX;
        /// <summary>
        /// Gpu program enum
        /// </summary>
        [Flags]
        public enum GpuProgram
        {
            /// <summary>
            /// Vertex program
            /// </summary>
            Vertex = 0,
            /// <summary>
            /// Fragment program
            /// </summary>
            Fragment = 1,
        }
        /// <summary>
        /// Get's a reference to the parent SkyX.
        /// </summary>
        public SkyX SkyX
        {
            get { return _skyX; }
            private set { _skyX = value; }
        }
        /// <summary>
        /// Get skydome material name
        /// </summary>
        public string SkydomeMaterialName
        {
            get
            {
                string starfield = (this.SkyX.IsStarFieldEnabled ? "STARFIELD_" : "");
                return (this.SkyX.LightingMode == LightingMode.Ldr) ? "SkyX_Skydome_" + starfield + "LDR" : "SkyX_Skydome_" + starfield + "HDR";
            }
        }
        /// <summary>
        /// Get moon material name
        /// </summary>
        public string MoonMaterialName
        {
            get { return "SkyX_Moon"; }
        }

        #region Construction and Destruction

        public GpuManager( SkyX skyX )
        {
            this.SkyX = skyX;
        }

        #endregion Construction and Destruction


        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, int value)
        {
            SetGpuProgramParameter(gpup, name, value, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="updateGroundPasses"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, int value, bool updateGroundPasses)
        {
            if (!this.SkyX.MeshManager.IsCreated)
            {
                return;
            }

            GpuProgramParameters parameters = null;

            switch (gpup)
            {
                case GpuProgram.Vertex:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.Fragment:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException("this type is not supported");
                    }
            }

            parameters.SetNamedConstant(name, value);

            if (!updateGroundPasses)
            {
                return;
            }

            foreach (Pass iter in _groundPasses)
            {
                switch (gpup)
                {
                    case GpuProgram.Vertex:
                        {
                            parameters = iter.VertexProgramParameters;
                        }
                        break;
                    case GpuProgram.Fragment:
                        {
                            parameters = iter.FragmentProgramParameters;
                        }
                        break;
                }

                parameters.SetNamedConstant(name, value);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, float value)
        {
            SetGpuProgramParameter(gpup, name, value, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="updateGroundPasses"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, float value, bool updateGroundPasses)
        {
            if (!this.SkyX.MeshManager.IsCreated)
            {
                return;
            }

            GpuProgramParameters parameters = null;

            switch (gpup)
            {
                case GpuProgram.Vertex:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.Fragment:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException("this type is not supported");
                    }
            }

            parameters.SetNamedConstant(name, value);

            if (!updateGroundPasses)
            {
                return;
            }

            foreach (Pass iter in _groundPasses)
            {
                switch (gpup)
                {
                    case GpuProgram.Vertex:
                        {
                            parameters = iter.VertexProgramParameters;
                        }
                        break;
                    case GpuProgram.Fragment:
                        {
                            parameters = iter.FragmentProgramParameters;
                        }
                        break;
                }

                parameters.SetNamedConstant(name, value);
            }

            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, Vector2 value)
        {
            SetGpuProgramParameter(gpup, name, value, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="updateGroundPasses"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, Vector2 value, bool updateGroundPasses)
        {
            if (!this.SkyX.MeshManager.IsCreated)
            {
                return;
            }

            GpuProgramParameters parameters = null;

            switch (gpup)
            {
                case GpuProgram.Vertex:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.Fragment:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException("this type is not supported");
                    }
            }
            float[] values = { value.x, value.y };
            parameters.SetNamedConstant(name, values);

            if (!updateGroundPasses)
            {
                return;
            }

            foreach (Pass iter in _groundPasses)
            {
                switch (gpup)
                {
                    case GpuProgram.Vertex:
                        {
                            parameters = iter.VertexProgramParameters;
                        }
                        break;
                    case GpuProgram.Fragment:
                        {
                            parameters = iter.FragmentProgramParameters;
                        }
                        break;
                }

                parameters.SetNamedConstant(name, values);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, Vector3 value)
        {
            SetGpuProgramParameter(gpup, name, value, true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="gpup"></param>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="updateGroundPasses"></param>
        public void SetGpuProgramParameter(GpuProgram gpup, string name, Vector3 value, bool updateGroundPasses)
        {
            if (!this.SkyX.MeshManager.IsCreated)
            {
                return;
            }

            GpuProgramParameters parameters = null;

            switch (gpup)
            {
                case GpuProgram.Vertex:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).VertexProgramParameters;
                    }
                    break;
                case GpuProgram.Fragment:
                    {
                        Material mat = (Material)MaterialManager.Instance.GetByName(this.SkyX.MeshManager.MaterialName);
                        parameters = mat.GetTechnique(0).GetPass(0).FragmentProgramParameters;
                    }
                    break;
                default:
                    {
                        throw new NotSupportedException("this type is not supported");
                    }
            }

            parameters.SetNamedConstant(name, value);

            if (!updateGroundPasses)
            {
                return;
            }

            foreach (Pass iter in _groundPasses)
            {
                switch (gpup)
                {
                    case GpuProgram.Vertex:
                        {
                            parameters = iter.VertexProgramParameters;
                        }
                        break;
                    case GpuProgram.Fragment:
                        {
                            parameters = iter.FragmentProgramParameters;
                        }
                        break;
                }

                parameters.SetNamedConstant(name, value);
            }
        }
        /// <summary>
        /// Add ground pass (Use for atmospheric scattering effect on the terrain)
        /// </summary>
        /// <param name="groundPass">Ground pass</param>
        public void AddGroundPass(Pass groundPass)
        {
            AddGroundPass(groundPass, 0, SceneBlendType.Add);
        }
        /// <summary>
        /// Add ground pass (Use for atmospheric scattering effect on the terrain)
        /// </summary>
        /// <param name="groundPass">Ground pass</param>
        /// <param name="atmosphereRadius">Atmosphere radius (far carmera clip plane, or needed)</param>
        public void AddGroundPass(Pass groundPass, float atmosphereRadius)
        {
            AddGroundPass(groundPass, 0, SceneBlendType.Add);
        }
        /// <summary>
        /// Add ground pass (Use for atmospheric scattering effect on the terrain)
        /// </summary>
        /// <param name="groundPass">Ground pass</param>
        /// <param name="atmosphereRadius">Atmosphere radius (far carmera clip plane, or needed)</param>
        /// <param name="sbt">Scene blend type</param>
        public void AddGroundPass(Pass groundPass, float atmosphereRadius, SceneBlendType sbt)
        {
            groundPass.SetVertexProgram("SkyX_Ground_VP");
            if (this.SkyX.LightingMode == LightingMode.Ldr)
            {
                groundPass.SetFragmentProgram("SkyX_Ground_LDR_FP");
            }
            else
            {
                groundPass.SetFragmentProgram("SkyX_Ground_HDR_FP");
            }
            groundPass.VertexProgramParameters.SetNamedConstant("uSkydomeRadius",
                ((atmosphereRadius == 0) ? this.SkyX.MeshManager.SkydomeRadius : atmosphereRadius) * 10);

            groundPass.LightingEnabled = false;
            groundPass.DepthCheck = true;
            groundPass.DepthWrite = false;

            groundPass.SetSceneBlending(sbt);

            ///TODO
            _groundPasses.Add(groundPass);
            this.SkyX.AtmosphereManager.Update(this.SkyX.AtmosphereManager.Options, true);
        }
        /// <summary>
        ///  Update fragment program materials
        /// </summary>
        /// <remarks>
        /// Only for internal use
        /// </remarks>
        internal void UpdateFP()
        {
            string fp_name = "SkyX_Ground_HDR_FP";

            if (this.SkyX.LightingMode == LightingMode.Ldr)
            {
                fp_name = "SkyX_Ground_LDR_FP";
            }

            for (int k = 0; k < _groundPasses.Count; k++)
            {
                _groundPasses[k].SetFragmentProgram(fp_name);
            }
        }
    }
}
