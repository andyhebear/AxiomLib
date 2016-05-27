#region - License -

#region LGPL License
/*
 This file is part of the Axiom 3D Hydrax port.

 Originial project developer:
 Xavier Verguín González <xavierverguin@hotmail.com>
                         <xavyiy@gmail.com>
 (see http://www.ogre3d.org/addonforums/viewforum.php?f=20&sid=e58afbbf10919de484bf7c4f590f7d17 )
 
 Axiom Hydrax developer:
 Bostich

The overall design, and a majority of the core engine and rendering code 
contained within this library is a derivative of the open source Object Oriented 
Graphics Engine OGRE, which can be found at http://ogre.sourceforge.net.  
Many thanks to the OGRE team for maintaining such a high quality project.

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion

#endregion

#region - using -
using System;
using System.Collections.Generic;
using Axiom.Math;
using Axiom.Core;
using Axiom.Core.Collections;
using Axiom.Graphics;
using Axiom.Hydrax.Noise;
using Axiom.Collections;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
#warning GodRaysManager: Some properties are missing for now!
    #region - class -
    /// <summary>
    /// Underwater god rays manager class
    /// God Rays.
    /// </summary>
    public class GodRaysManager
    {
        #region - Constants -
        public const string _def_GodRays_Projector_Camera_Name = "_Hydrax_GodRays_Projector_Camera";
        public const string _def_GodRays_ManualObject_Name    = "_Hydrax_GodRays_ManualObject";
        public const string  _def_GodRays_Depth_Map           =  "_Hydrax_GodRays_Depth_Map";

        public const string _def_GodRays_Material_Name  ="_Hydrax_GodRays_Material";
        public const string _def_GodRays_Shader_VP_Name ="_Hydrax_GodRays_VP";
        public const string  _def_GodRays_Shader_FP_Name ="_Hydrax_GodRays_FP";

        public const string  _def_GodRaysDepth_Material_Name = "_Hydrax_GodRaysDepth_Material";
        public const string _def_GodRaysDepth_Shader_VP_Name ="_Hydrax_GodRaysDepth_VP";
        public const string _def_GodRaysDepth_Shader_FP_Name = "_Hydrax_GodRaysDepth_FP";
        #endregion

        #region - fields -
        protected Matrix4 
        PROJECTIONCLIPSPACE2DTOIMAGESPACE_PERSPECTIVE = new Matrix4(
               0.5f,    0,    0,  0.5f,
               0,   -0.5f,    0,  0.5f,
               0,      0,    1,    0,
               0,      0,    0,    1);

        /// <summary>
        /// If Manager is created, true, otherwise false.
        /// </summary>
        protected bool mIsCreated = false;
        /// <summary>
        /// Manual object to create god rays
        /// </summary>
        protected ManualObject mManualGodRays;
		/// <summary>
        /// Camera used to project rays
		/// </summary>
		protected Camera mProjectorCamera;
		/// <summary>
        /// Projector scene node
		/// </summary>
		protected SceneNode mProjectorSN;
		/// <summary>
        /// Our Perlin noise module
		/// </summary>
		protected Perlin mPerlin;

        #region - Noise parameters (Used in _calculateRayPosition(...)) -
		/// <summary>
        /// Normal derivation value
		/// </summary>
		protected float mNoiseDerivation;
		/// <summary>
        /// PositionMultiplier value
		/// </summary>
		protected float mNoisePositionMultiplier;
		/// <summary>
        /// Y normal component multiplier
		/// </summary>
		protected float mNoiseYNormalMultiplier;
		/// <summary>
        /// Normal multiplier
		/// </summary>
		protected float mNoiseNormalMultiplier;
        #endregion

        /// <summary>
        /// God rays materials
        /// 0-God rays, 1-Depth
		/// </summary>
		Material[] mMaterials = new Material[2];
        /// <summary>
        /// 
        /// </summary>
        protected Queue<string> mDepthMaterials = new Queue<string>();
		/// <summary>
        /// Technique list for AddDepthTechnique(...)
		/// </summary>
		protected List<Technique> mDepthTechniques;
		/// <summary>
        /// God rays simulation speed
		/// </summary>
		protected float mSimulationSpeed;
		/// <summary>
        /// Number of rays
		/// </summary>
		protected int mNumberOfRays;
		/// <summary>
        /// God rays size
		/// </summary>
		protected float mRaysSize;
		/// <summary>
        /// Are god rays objects intersections active?
		/// </summary>
		protected bool mObjectsIntersections;
		/// <summary>
        /// For rays intersection with objects we use a depth map based technique
        /// Depth RTT texture
		/// </summary>
		protected Texture mProjectorRTT;
		/// Depth RTT listener
		//DepthMapListener mDepthMapListener;
		/// <summary>
		/// Hydrax object
		/// </summary>
		Hydrax mHydrax;
        /// <summary>
        /// Godrays visible or not?
        /// </summary>
        protected bool mIsVisible;
        #endregion

        #region - MaterialType -
        /// <summary>
        /// God rays material enumeration
		/// </summary>
		public enum MaterialType
		{
			/// <summary>
            /// Material used for god rays
			/// </summary>
			MAT_GODRAYS = 0,
			/// <summary>
            /// Used for depth mat(for objects intersections)
			/// </summary>
			MAT_DEPTH   = 1
		};
        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Underwater god rays manager class
        /// God rays
        /// </summary>
        /// <param name="Hydrax">Current Hydrax object.</param>
        public GodRaysManager(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            mSimulationSpeed = 5.0f;
            mNumberOfRays = 100;
            mRaysSize = 0.03f;
            mObjectsIntersections = false;
            mNoiseDerivation = 3;
            mNoisePositionMultiplier = 50;
            mNoiseYNormalMultiplier = 10;
            mNoiseNormalMultiplier = 0.175f;
            mDepthTechniques = new List<Technique>();
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~GodRaysManager()
        {
            Remove();
        }
        #endregion

        #region - properties -
        /// <summary>
        /// Godrays visible or not?
        /// </summary>
        public bool IsVisible
        {
            set { mIsVisible = value; }
            get { return mIsVisible; }
        }
        /// <summary>
        /// Simulationspeed of the GodRays.
        /// </summary>
        public float SimulationSpeed
        {
            set { mSimulationSpeed = value; }
            get { return mSimulationSpeed; }
        }
        /// <summary>
        /// Size of each ray.
        /// </summary>
        public float RaySize
        {
            set { mRaysSize = value; }
            get { return mRaysSize; }
        }
        #endregion

        #region - methods -

        #region - Remove -
        /// <summary>
        /// Remove GodRaysManager.
        /// </summary>
        public void Remove()
        {
            if (!mIsCreated)
                return;

            mPerlin = null;
#warning throws sometimes exception!
           // mHydrax.SceneManager.DestroyMovableObject(mManualGodRays);
            if (Axiom.Graphics.MaterialManager.Instance.ResourceExists(_def_GodRays_Material_Name))
            {
                Axiom.Graphics.MaterialManager.Instance.Remove(_def_GodRays_Material_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_GodRays_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_GodRays_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_GodRays_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_GodRays_Shader_VP_Name);
            }

            if (Axiom.Graphics.MaterialManager.Instance.ResourceExists(_def_GodRaysDepth_Material_Name))
            {
                Axiom.Graphics.MaterialManager.Instance.Remove(_def_GodRaysDepth_Material_Name);

                HighLevelGpuProgramManager.Instance.Unload(_def_GodRaysDepth_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Unload(_def_GodRaysDepth_Shader_VP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_GodRaysDepth_Shader_FP_Name);
                HighLevelGpuProgramManager.Instance.Remove(_def_GodRaysDepth_Shader_VP_Name);
            }
            for (int k = 0; k < 2; k++)
            {
                mMaterials[k] = null;
            }
            if (mProjectorRTT != null)
            {
                try
                {
                    RenderTarget RT = mProjectorRTT.GetBuffer().GetRenderTarget();
                    RT.RemoveAllViewports();

                    Axiom.Core.TextureManager.Instance.Remove(mProjectorRTT.Name);
                    mProjectorRTT = null;
                }
                catch { };
            }

            mHydrax.SceneManager.DestroyCamera(mProjectorCamera);
            mProjectorCamera = null;
            mProjectorSN.DetachAllObjects();
            mProjectorSN.Parent.RemoveChild(mProjectorSN.Name);
            mProjectorSN = null;
            mIsCreated = false;
        }
        #endregion

        #region - Create -
        /// <summary>
        /// Create GodRaysManager.
        /// </summary>
        /// <param name="Components">Current Hydrax Components.</param>
        public void Create(HydraxComponent Components)
        {
            if (mIsCreated)
                Remove();
            // Create our perlin noise module
            mPerlin = new Perlin(new Perlin.Options(8, 0.085f, 0.49f, 2, 0.672f));
            mPerlin.Create();

            // Initial values, some of them need to be updated each frame
            mProjectorCamera = mHydrax.SceneManager.CreateCamera(_def_GodRays_Projector_Camera_Name);
            mProjectorCamera.ProjectionType = Projection.Perspective;

            // Not forget to set near+far distance in materials
            mProjectorCamera.Near = 8;
            mProjectorCamera.Far = 40;
            mProjectorCamera.AspectRatio = 1;
            mProjectorCamera.FieldOfView = 45;
            mProjectorCamera.IsVisible = false;
            mProjectorSN = mHydrax.SceneManager.RootSceneNode.CreateChildSceneNode();
            mProjectorSN.Position = new Vector3(0, 0, 0);
            mProjectorSN.AttachObject(mProjectorCamera);
            mProjectorSN.SetDirection(new Vector3(0, -1, 0));

            if (mObjectsIntersections)
            {
                CreateDepthRTT();
            }

            CreateMaterials(Components);

            foreach (Technique TechIt in mDepthTechniques)
            {
                if (TechIt == null)
                {
                    mDepthTechniques.Remove(TechIt);
                    continue;
                }

                AddDepthTechnique(TechIt, false);
            }

            CreateGodRays();

            mIsCreated = true;
        }
        #endregion

        #region - CreateGodRays -
        /// <summary>
        /// Create god rays manual object.
        /// </summary>
        private void CreateGodRays()
        {
            mManualGodRays = mHydrax.SceneManager.CreateManualObject(_def_GodRays_ManualObject_Name);
            mManualGodRays.Dynamic = true;
            mManualGodRays.IsVisible = mHydrax.IsCurrentFrameUnderwater;

            mManualGodRays.Begin(_def_GodRays_Material_Name, OperationType.TriangleList);
            mManualGodRays.RenderQueueGroup = RenderQueueGroupID.Nine + 1;

            for (int k = 0; k < mNumberOfRays; k++)
            {
                // Rays are modeled as piramids, 12 vertex each ray
                for (int p = 0; p < 12; p++)
                {
                    mManualGodRays.Position(0, 0, 0);
                    mManualGodRays.Index((ushort)p);
                }
            }

            mManualGodRays.End();
            mProjectorSN.AttachObject(mManualGodRays);
        }
        #endregion

        #region - SetNumberOfRays -
        /// <summary>
        /// Set the number of god rays
        /// </summary>
        /// <param name="NumberOfRays">NumberOfRays Number of god rays</param>
        public void SetNumbersOfRays(int NumberOfRays)
        {
            mNumberOfRays = NumberOfRays;
            if (!mIsCreated)
                return;

            mProjectorSN.DetachObject(mManualGodRays);

#warning in original DestroyManualObject!
            mHydrax.SceneManager.DestroyMovableObject(mManualGodRays);

            mManualGodRays = null;

            CreateGodRays();

        }
        #endregion

        #region - SetObjectIntersectionsEnabled -
        /// <summary>
        /// Set objects intersections enabled
        /// </summary>
        /// <param name="Enable">Enable true for yes, false for not</param>
        public void SetObjectIntersectionsEnabled(bool Enable)
        {
            mObjectsIntersections = Enable;
            HydraxComponent Components = mHydrax.Components;
#warning fix remove here
            //Create(Components);
        }
        #endregion

        #region - AddDepthTechnique -
        /// <summary>
        /// Add god rays depth technique to an especified material.
        /// </summary>
        /// <param name="Technique">Technique Technique where depth technique will be added.</param>
        /// <param name="AutoUpdate">AutoUpdate The technique will be automatically updated when god rays parameters change.</param>
        /// <remarks>Call it after Hydrax.Create()/Hydrax.SetComponents(...)
        /// The technique will be automatically updated when god rays parameters change if parameter AutoUpdate == true
        /// Add depth technique when a material is not an Entity, such terrains, PLSM2 materials, etc.
        /// This depth technique will be added with "HydraxGodRaysDepth" scheme in ordeto can use it in the G.R. depth RTT. 
        /// </remarks>
        private void AddDepthTechnique(Technique Technique, bool AutoUpdate)
        {
            if (!Axiom.Graphics.MaterialManager.Instance.ResourceExists(_def_GodRaysDepth_Material_Name))
            {
                Hydrax.HydraxLog("GodRaysManager::addDepthTechnique(...) Objects intersection must be enabled and Hydrax::create() already called, skipping...");
                return;
            }

            Technique.RemoveAllPasses();
            Technique.CreatePass();
            Technique.SchemeName = "HydraxGodRaysDepth";

            Pass DM_Technique_Pass0 = Technique.GetPass(0);
            DM_Technique_Pass0.VertexProgramName = _def_GodRays_Shader_VP_Name;
            DM_Technique_Pass0.FragmentProgramName = _def_GodRays_Shader_FP_Name;

            GpuProgramParameters VP_Parameters = DM_Technique_Pass0.VertexProgramParameters;
            GpuProgramParameters FP_Parameters = DM_Technique_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);

            FP_Parameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
            FP_Parameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);

            if (AutoUpdate)
                mDepthTechniques.Add(Technique);
        }
        #endregion

        #region - CreateDepthRTT -
        /// <summary>
        /// Create depth RTT
        /// </summary>
        private void CreateDepthRTT()
        {
            mProjectorRTT = Axiom.Core.TextureManager.Instance
                .CreateManual(_def_GodRays_Depth_Map, "HYDRAX_RESOURCE_GROUP"
                , TextureType.TwoD, 256, 256
                , 0
                , Axiom.Media.PixelFormat.L8
                , TextureUsage.RenderTarget);

            Axiom.Graphics.RenderTarget RT_Texture = mProjectorRTT.GetBuffer().GetRenderTarget();
            RT_Texture.IsAutoUpdated = false;

			RT_Texture.BeforeUpdate += new RenderTargetEventHandler( DepthMap_BeforeUpdate );
			RT_Texture.AfterUpdate += new RenderTargetEventHandler( DepthMap_AfterUpdate );

            Viewport RT_Texture_Viewport = RT_Texture.AddViewport(mProjectorCamera);

            RT_Texture_Viewport.ClearEveryFrame = true;
            RT_Texture_Viewport.MaterialScheme = "HydraxGodRaysDepth";
            RT_Texture_Viewport.BackgroundColor = ColorEx.White;
            RT_Texture_Viewport.ShowOverlays = false;
            RT_Texture_Viewport.ShowSkies = false;
            RT_Texture_Viewport.ShowShadows = false;

        }
        #endregion

        #region - CreateMaterials -
        /// <summary>
        /// Create materials that we need(God rays depth too if it's needed)
        /// </summary>
        /// <param name="Components">Current Hydrax components</param>
        private void CreateMaterials(HydraxComponent Components)
        {
            string VertexProgramData, FragmentProgramData;
		    GpuProgramParameters VP_Parameters, FP_Parameters;
		    string[] EntryPoints     = {"main_vp", "main_fp"};
		    string[] GpuProgramsData= new string[2]; string[] GpuProgramNames = new string[2];
            MaterialManager mMaterialManager = mHydrax.MaterialManager;

            int NumberOfDepthChannels = 0;
            string[] GB = { "0, 1, 0", "0, 0, 1" };

            if (IsComponent(Components, HydraxComponent.Caustics))
            {
                NumberOfDepthChannels++;
            }

            //God Rays Material

            VertexProgramData = "";
            FragmentProgramData = "";

            #region - VertexProgram -
            switch (mHydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        VertexProgramData +=
                            
                    "void main_vp(\n" +
                            // IN
                        "float4 iPosition      : POSITION,\n" +
                            // OUT
                        "out float4 oPosition  : POSITION,\n";
                        if (mObjectsIntersections)
                        {
                            VertexProgramData +=
                                "out float3 oPosition_ : TEXCOORD0,\n" +
                                "out float4 oProjUV    : TEXCOORD1,\n" +
                                // UNIFORM
                                "uniform float4x4 uWorld,\n" +
                                "uniform float4x4 uTexViewProj,\n";
                        }
                        VertexProgramData +=
                            "uniform float4x4 uWorldViewProj)\n" +
                        "{\n" +
                            "oPosition   = mul(uWorldViewProj, iPosition);\n";
                        if (mObjectsIntersections)
                        {
                            VertexProgramData +=
                                 "float4 wPos = mul(uWorld, iPosition);\n" +
                                 "oPosition_  = wPos.xyz;\n" +
                                 "oProjUV     = mul(uTexViewProj, wPos);\n";
                        }
                        VertexProgramData +=
                         "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;

            }
            #endregion

            #region - FragmentProgram -
            switch (mHydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        if (mObjectsIntersections)
                            FragmentProgramData +=
                                "void main_fp(\n" +
                                // IN
                                    "float3 iPosition     : TEXCOORD0,\n" +
                                    "float4 iProjUV       : TEXCOORD1,\n" +
                                // OUT
                                    "out float4 oColor    : COLOR,\n" +
                                // UNIFORM
                                    "uniform float3    uLightPosition,\n" +
                                    "uniform float     uLightFarClipDistance,\n" +
                                    "uniform sampler2D uDepthMap : register(s0))\n" +
                                "{\n" +
                                    "iProjUV = iProjUV / iProjUV.w;\n" +
                                    "float Depth  = tex2D(uDepthMap,  iProjUV.xy).r;\n" +
                                    "if (Depth < saturate( length(iPosition-uLightPosition) / uLightFarClipDistance ))\n" +
                                    "{\n" +
                                        "oColor = float4(0,0,0,1);\n" +
                                    "}\n" +
                                    "else\n" +
                                    "{\n" +
                                        "oColor = float4(float3(" + GB[NumberOfDepthChannels] + ") * 0.1, 1);\n" +
                                    "}\n" +
                                "}\n";
                        else
                            FragmentProgramData +=
                                "void main_fp(\n" +
                                // OUT
                                    "out float4 oColor    : COLOR)\n" +
                                "{\n" +
                                    "oColor = float4(float3(" + GB[NumberOfDepthChannels] + ") * 0.1, 1);\n" +
                                "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;
            }
            #endregion

            #region - BuildMaterial -
            mMaterials[0] = (Material)Axiom.Graphics.MaterialManager.Instance
                .Create(_def_GodRays_Material_Name, "HYDRAX_RESOURCE_GROUP");

            Pass GR_Technique0_Pass0 = mMaterials[0].GetTechnique(0).GetPass(0);

            GR_Technique0_Pass0.LightingEnabled = false;
            GR_Technique0_Pass0.CullingMode = CullingMode.None;
            GR_Technique0_Pass0.DepthWrite = false;
            GR_Technique0_Pass0.DepthCheck = mObjectsIntersections;
            GR_Technique0_Pass0.SetSceneBlending(SceneBlendType.Add);

            GpuProgramsData[0] = VertexProgramData; GpuProgramsData[1] = FragmentProgramData;
            GpuProgramNames[0] = _def_GodRays_Shader_VP_Name; GpuProgramNames[1] = _def_GodRays_Shader_FP_Name;

            mMaterialManager.FillGpuProgramsToPass(GR_Technique0_Pass0, GpuProgramNames, mHydrax.ShaderMode, EntryPoints, GpuProgramsData);

            VP_Parameters = GR_Technique0_Pass0.VertexProgramParameters;
            FP_Parameters = GR_Technique0_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);

            if (!mObjectsIntersections)
            {
                return;
            }
            


            Matrix4 TexViewProj =
                PROJECTIONCLIPSPACE2DTOIMAGESPACE_PERSPECTIVE *
                mProjectorCamera.ProjectionMatrixRSDepth *
                mProjectorCamera.ViewMatrix;

            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            VP_Parameters.SetNamedConstant("uTexViewProj", TexViewProj);

            FP_Parameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
            FP_Parameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);

			GR_Technique0_Pass0.CreateTextureUnitState( _def_GodRays_Depth_Map ).SetTextureAddressingMode( TextureAddressing.Clamp );
            GR_Technique0_Pass0.GetTextureUnitState(0).SetTextureName(_def_GodRays_Depth_Map);
#endregion

            //Depth material

            VertexProgramData = "";
            FragmentProgramData = "";

            #region - VertexProgram -
            switch (mHydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        VertexProgramData +=
                    "void main_vp(\n" +
                            // IN
                        "float4 iPosition      : POSITION,\n" +
                        "float2 iUV            : TEXCOORD0,\n" +
                            // OUT
                        "out float4 oPosition  : POSITION,\n" +
                        "out float3 oPosition_ : TEXCOORD0,\n" +
                            // UNIFORM
                        "uniform float4x4 uWorld,\n" +
                        "uniform float4x4 uWorldViewProj)\n" +
                    "{\n" +
                        "oPosition   = mul(uWorldViewProj, iPosition);\n" +
                        "float4 wPos = mul(uWorld, iPosition);\n" +
                        "oPosition_  = wPos.xyz;\n" +
                    "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;
            }
            #endregion

            #region - FragmentProgram -
            switch (mHydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        FragmentProgramData +=
                    "void main_fp(\n" +
                            // IN
                        "float3 iPosition     : TEXCOORD0,\n" +
                            // OUT
                        "out float4 oColor    : COLOR,\n" +
                            // UNIFORM
                        "uniform float3    uLightPosition,\n" +
                        "uniform float     uLightFarClipDistance)\n" +
                    "{\n" +
                        "float depth = saturate( length(iPosition-uLightPosition) / uLightFarClipDistance );\n" +
                        "oColor = float4(depth, 0, 0, 0);\n" +
                    "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;
            }
            #endregion

            #region - BuildMaterial -
            mMaterials[1] = (Material)Axiom.Graphics.MaterialManager.Instance
                .Create(_def_GodRaysDepth_Material_Name, "HYDRAX_RESOURCE_GROUP");

            Pass GRD_Technique0_Pass0 = mMaterials[1].GetTechnique(0).GetPass(0);

            mMaterials[1].GetTechnique(0).SchemeName = "HydraxGodRaysDepth";

            GRD_Technique0_Pass0.LightingEnabled = false;
            GRD_Technique0_Pass0.CullingMode = CullingMode.None;

            GpuProgramsData[0] = VertexProgramData; GpuProgramsData[1] = FragmentProgramData;
            GpuProgramNames[0] = _def_GodRaysDepth_Shader_VP_Name; GpuProgramNames[1] = _def_GodRaysDepth_Shader_FP_Name;

            mMaterialManager.FillGpuProgramsToPass(GRD_Technique0_Pass0, GpuProgramNames, mHydrax.ShaderMode, EntryPoints, GpuProgramsData);

            VP_Parameters = GRD_Technique0_Pass0.VertexProgramParameters;
            FP_Parameters = GRD_Technique0_Pass0.FragmentProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);

            FP_Parameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
            FP_Parameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);
            #endregion

        }
        #endregion

        #region - IsComponent -
        /// <summary>
        /// Is component in the given list?
        /// </summary>
        /// <param name="List">Components list</param>
        /// <param name="ToCheck">Component to check</param>
        /// <returns>true if the component is in the given list.</returns>
        private bool IsComponent(HydraxComponent List, HydraxComponent ToCheck)
        {
            if (List == ToCheck)
                return true;

            if (ToCheck == HydraxComponent.None && List == HydraxComponent.None)
            {
                return true;
            }

            if (ToCheck == HydraxComponent.All && List == HydraxComponent.All)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region - CalculateRayPosition -
        /// <summary>
        /// Calculate the current position of a ray
        /// </summary>
        /// <param name="RayNumber">Number of the ray range[0,NumerOfRays]</param>
        /// <returns>position range[-1,1]x[-1,1]</returns>
        private Vector2 CalculateRayPosition(int RayNumber)
        {
            float sqrt_NumberOfRays = (float)System.Math.Sqrt(mNumberOfRays);
            float XCoord = RayNumber;

            while (XCoord >= sqrt_NumberOfRays)
            {
                XCoord -= sqrt_NumberOfRays;
            }

            Vector2 RayPos = new Vector2
                (XCoord,
                ((RayNumber + sqrt_NumberOfRays) / sqrt_NumberOfRays) -1);

            RayPos.x /= sqrt_NumberOfRays;
            RayPos.y /= sqrt_NumberOfRays;
            RayPos -= new Vector2(0.5f, 0.5f);
            RayPos *= 2;

            Vector2 Position = RayPos * mNoisePositionMultiplier + new Vector2(mProjectorSN.Position.x, mProjectorSN.Position.z);

            Vector3
                m_x = new Vector3(Position.x - mNoiseDerivation, mPerlin.GetValue(Position.x - mNoiseDerivation, 0), 0),
                p_x = new Vector3(Position.x + mNoiseDerivation, mPerlin.GetValue(Position.x + mNoiseDerivation, 0), 0),
                m_y = new Vector3(0, mPerlin.GetValue(0, Position.y - mNoiseDerivation), Position.y - mNoiseDerivation),
                p_y = new Vector3(0, mPerlin.GetValue(0, Position.y + mNoiseDerivation), Position.y + mNoiseDerivation);

            m_x.y *= mNoiseYNormalMultiplier; p_x.y *= mNoiseYNormalMultiplier;
            m_y.y *= mNoiseYNormalMultiplier; p_y.y *= mNoiseYNormalMultiplier;

            Vector3 Normal = (p_x - m_x).Cross((p_y - m_y));
            Normal *= mNoiseNormalMultiplier;

            return RayPos + new Vector2(Normal.x, Normal.z);
        }
        #endregion

        #region - UpdateRays -
        /// <summary>
        /// Update god rays
        /// </summary>
        private void UpdateRays()
        {
            // Get frustum corners to calculate far plane dimensions
            Vector3[] FrustumCorners = mProjectorCamera.WorldSpaceCorners;
            // Calcule far plane dimensions
            float FarWidth = (FrustumCorners[4] - FrustumCorners[5]).Length;

            float RaysLength = mProjectorCamera.Far;

            mManualGodRays.BeginUpdate(0);

            Vector2 Pos;
            float Dis, RayLength;
            // Rays are modeled as piramids, 12 vertex each ray
            //        
            //       // 0\\
            //      /|    | |
            //      ||    | |
            //      ||    | |     (0,0)   (1,0)
            //      ||    | |     A       B
            //      ||    | |
            //      |A----|-|B    (0,1)   (1,1)
            //      |/    |/      C       D
            //     C------D

            for (int k = 0; k < mNumberOfRays; k++)
            {
                Pos = CalculateRayPosition(k);
                Dis = mRaysSize * RaysLength;
                RayLength = (float)(RaysLength * (0.95 + Pos.Length()));

                Pos *= FarWidth / 2;

                // 4 Planes, 3 vertices each plane, 12 vertices per ray
                // ----> 1/4
                // 0
                mManualGodRays.Position(0, 0, 0);
                // A
                mManualGodRays.Position(Pos.x, Pos.y, -RayLength);
                // B
                mManualGodRays.Position(Pos.x + Dis, Pos.y, -RayLength);
                // ----> 2/4
                // 0
                mManualGodRays.Position(0, 0, 0);
                // D
                mManualGodRays.Position(Pos.x + Dis, Pos.y + Dis, -RayLength);
                // B
                mManualGodRays.Position(Pos.x + Dis, Pos.y, -RayLength);
                // ----> 3/4
                // 0
                mManualGodRays.Position(0, 0, 0);
                // C
                mManualGodRays.Position(Pos.x, Pos.y + Dis, -RayLength);
                // D
                mManualGodRays.Position(Pos.x + Dis, Pos.y + Dis, -RayLength);
                // ----> 4/4
                // 0
                mManualGodRays.Position(0, 0, 0);
                // C
                mManualGodRays.Position(Pos.x, Pos.y + Dis, -RayLength);
                // A
                mManualGodRays.Position(Pos.x, Pos.y, -RayLength);
            }

            mManualGodRays.End();
        }
        #endregion

        #region - UpdateProjector -
        /// <summary>
        /// Update projector.
        /// </summary>
        private void UpdateProjector()
        {
            Vector3 SunPosition = mHydrax.SunPosition;
            Vector3 CameraPosition = mHydrax.Camera.DerivedPosition;

            Plane WaterPlane = new Plane(new Vector3(0, 1, 0), mHydrax.Position);
            Ray SunToCameraRay = new Ray(SunPosition, CameraPosition - SunPosition);

            Vector3 WaterProjectionPoint = SunToCameraRay.GetPoint(SunToCameraRay.Intersects(WaterPlane).Distance);
            Vector3 WaterPostion = new Vector3(WaterProjectionPoint.x, mHydrax.GetHeight(WaterProjectionPoint), WaterProjectionPoint.z);

            mProjectorSN.Position = WaterProjectionPoint;
            mProjectorCamera.Far = (WaterProjectionPoint - CameraPosition).Length;
            mProjectorSN.SetDirection(-(WaterProjectionPoint - CameraPosition).ToNormalized(), TransformSpace.World);
        }
        #endregion

        #region - UpdateMaterialsParameters -
        /// <summary>
        /// Update materials parameters
        /// </summary>
        private void UpdateMaterialsParameters()
        {
            if (!mObjectsIntersections)
                return;
            GpuProgramParameters VP_Parameters, FP_Parameters;

            // God rays material
            VP_Parameters = mMaterials[0].GetTechnique(0).GetPass(0).VertexProgramParameters;
            FP_Parameters = mMaterials[0].GetTechnique(0).GetPass(0).FragmentProgramParameters;

            Matrix4 TexViewProj =
                PROJECTIONCLIPSPACE2DTOIMAGESPACE_PERSPECTIVE *
                mProjectorCamera.ProjectionMatrixRSDepth *
                mProjectorCamera.ViewMatrix;

            VP_Parameters.SetNamedConstant("uTexViewProj", TexViewProj);

            FP_Parameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
            FP_Parameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);

            //Depth material
            FP_Parameters = mMaterials[1].GetTechnique(0).GetPass(0).FragmentProgramParameters;

            FP_Parameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
            FP_Parameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);

            foreach (Technique TechIt in mDepthTechniques)
            {
                if (TechIt == null)
                {
                    mDepthTechniques.Remove(TechIt);
                    continue;
                }

                TechIt.GetPass(0).FragmentProgramParameters.SetNamedConstant("uLightPosition", mProjectorSN.Position);
                TechIt.GetPass(0).FragmentProgramParameters.SetNamedConstant("uLightFarClipDistance", mProjectorCamera.Far);

            }
        }
        #endregion

        #region - Update -
        /// <summary>
        /// Call each frame.
        /// </summary>
        /// <param name="TimeSinceLastFrame">Time since last frame</param>
        public void Update(float TimeSinceLastFrame)
        {
            if (!mIsCreated || !mHydrax.IsCurrentFrameUnderwater)
                return;

            mPerlin.Update(TimeSinceLastFrame);

            UpdateRays();
            UpdateProjector();

            if (mObjectsIntersections)
            {
                UpdateMaterialsParameters();
                mProjectorRTT.GetBuffer().GetRenderTarget().Update();
            }
        }
        #endregion

        #endregion

        #region - Events -

        #region - DepthMap_AfterUpdate -
        /// <summary>
        /// Fucntion that is called after the Rtt will render
        /// </summary>
        /// <param name="e">RenderTargetEvent</param>
        void DepthMap_AfterUpdate(RenderTargetEventArgs e)
        {
        	MovableObjectCollection Entitys;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);
            
            mHydrax.Mesh.Entity.IsVisible = true;
            Entity CurrentEntity = null;
            int k = 0;
            foreach (Entity Ent in Entitys)
            {
                CurrentEntity = Ent;

                for (k = 0; k < CurrentEntity.SubEntityCount; k++)
                {
                    CurrentEntity.GetSubEntity(k).MaterialName = mDepthMaterials.Dequeue();
                }
            }
        }
        #endregion

        #region - Depth_BeforeUpdate -
        /// <summary>
        /// Funtion that is called before the Rtt will render
        /// </summary>
        /// <param name="e">RenderTargetEvent</param>
        void DepthMap_BeforeUpdate(RenderTargetEventArgs e)
        {
        	MovableObjectCollection Entitys;
            mHydrax.SceneManager.MovableObjectCollectionMap.TryGetValue("Entity", out Entitys);
            
            mDepthMaterials.Clear();
            mHydrax.Mesh.Entity.IsVisible = false;
            Entity CurrentEntity = null;
            int k = 0;
            foreach (Entity Ent in Entitys)
            {
                CurrentEntity = Ent;

                for (k = 0; k < CurrentEntity.SubEntityCount; k++)
                {
                    mDepthMaterials.Enqueue(CurrentEntity.GetSubEntity(k).MaterialName);
                    CurrentEntity.GetSubEntity(k).MaterialName = _def_GodRaysDepth_Material_Name;
                }
            }
        }
        #endregion

        #endregion
    }//end class
    #endregion
}//end namespace
#endregion