#region MIT/X11 License
// This file is part of the Axiom.PagedGeometry project
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>
//
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
// Axiom.PagedGeometry is a reimplementation of the PagedGeometry project for .Net/Mono
// PagedGeometry is Copyright (C) 2006 John Judnich
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
namespace Axiom.Forests
{
    public class BatchPage : GeometryPage
    {
        private bool mFadeEnabled;
        private bool mShadersSupported;
        private float mVisibleDist;
        private float mInvisibleDist;
        private List<Material> mUnfadedMaterials = new List<Material>();
        private SceneManager mSceneMgr;
        private BatchedGeometry mBatch;
        private static long mRefCount = 0;
        private static long GUID = 0;
        /// <summary>
        /// 
        /// </summary>
        public bool IsVisble
        {
            get { return mBatch.IsVisible; }
            set { mBatch.IsVisible = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override AxisAlignedBox BoundingBox
        {
            get
            {
                return mBatch.BoundingBox;
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        private static string GetUniqueID(string prefix)
        {
            return prefix + ++GUID;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        public override void Init(PagedGeometry geom)
        {
            mSceneMgr = geom.SceneManager;
            mBatch = new BatchedGeometry(mSceneMgr, geom.SceneNode);
            
            mFadeEnabled = false;
			RenderSystemCapabilities caps = Root.Instance.RenderSystem.Capabilities;
            if (caps.HasCapability(Capabilities.VertexPrograms))
                mShadersSupported = true;
            else
                mShadersSupported = false;

            ++mRefCount;
        }
        public override void Build()
        {
            mBatch.Build();

            foreach (BatchedGeometry.SubBatch it in mBatch.SubBatches.Values)
            {
                BatchedGeometry.SubBatch subBatch = it;
                Material mat = subBatch.Material;

                //Disable specular unless a custom shader is being used.
                //This is done because the default shader applied by BatchPage
                //doesn't support specular, and fixed-function needs to look
                //the same as the shader (for computers with no shader support)
                for (int t = 0; t < mat.TechniqueCount; t++)
                {
                    Technique tech = mat.GetTechnique(t);
                    for (int p = 0; p < tech.PassCount; p++)
                    {
                        Pass pass = tech.GetPass(p);
                        if (pass.VertexProgramName == "")
                            pass.Specular = new ColorEx(1, 0, 0, 0);
                    }
                }

                //store the original materials
                mUnfadedMaterials.Add(mat);
            }

            UpdateShaders();
        }
        public void AddEntityToBoundingBox()
        {
            throw new NotImplementedException();
        }
        public void ClearBoundingBox()
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public override void RemoveEntites()
        {
            mBatch.Clear();
            mUnfadedMaterials.Clear();
            mFadeEnabled = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        public override void SetFade(bool enabled, float visibleDist, float invisibleDist)
        {
            if (!mShadersSupported)
                return;

            //if fade status has changed...
            if (mFadeEnabled != enabled)
                mFadeEnabled = enabled;

            mBatch.RenderQueueGroup = RenderQueueGroupID.Six;
            mVisibleDist = visibleDist;
            mInvisibleDist = invisibleDist;
            UpdateShaders();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public override void AddEntity(Entity ent, Vector3 position, Quaternion rotation, Vector3 scale, ColorEx color)
        {
            mBatch.AddEntity(ent, position, rotation, scale, color);
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateShaders()
        {
            if (!mShadersSupported)
                return;

            int i = 0;
            foreach (BatchedGeometry.SubBatch it in mBatch.SubBatches.Values)
            {
                BatchedGeometry.SubBatch subBatch = it;
                Material mat = mUnfadedMaterials[i++];

                //check ig lighting should be enabled
                bool lightningEnabled = false;
                for (int t = 0; t < mat.TechniqueCount; t++)
                {
                    Technique tech = mat.GetTechnique(t);
                    for (int p = 0; p < tech.PassCount; p++)
                    {
                        Pass pass = tech.GetPass(p);
                        if (pass.LightingEnabled)
                        {
                            lightningEnabled = true;
                            break;
                        }
                        if (lightningEnabled)
                            break;
                    }
                }

                //Compile the CG shader script based on various material / fade options
                string tmpName = string.Empty;

                tmpName += "BatchPage_";
                if (mFadeEnabled)
                    tmpName += "fade_";
                if (lightningEnabled)
                    tmpName += "lit_";

                tmpName += "vp";

                string vertexProgName = tmpName;

                //If the shader hasn't been created yet, create it
                if (HighLevelGpuProgramManager.Instance.GetByName(tmpName) == null)
                {
                    string vertexProgSource =
                        "void main( \n" +
				        "	float4 iPosition : POSITION, \n"  +
				        "	float3 normal    : NORMAL,	\n" +
				        "	float2 iUV       : TEXCOORD0,	\n" +
				        "	float4 iColor    : COLOR, \n" +

				        "	out float4 oPosition : POSITION, \n" +
				        "	out float2 oUV       : TEXCOORD0,	\n" +
                        "	out float4 oColor : COLOR, \n" +
				        "	out float4 oFog : FOG,	\n";

                    if (lightningEnabled)
                    {
                        vertexProgSource +=
                            "	uniform float4 objSpaceLight,	\n" +
				            "	uniform float4 lightDiffuse,	\n" +
				            "	uniform float4 lightAmbient,	\n";
                    }

                    if (mFadeEnabled)
                    {
                        vertexProgSource +=
                            "	uniform float3 camPos, \n";
                    }

                    vertexProgSource +=
				        "	uniform float4x4 worldViewProj,	\n" +
				        "	uniform float fadeGap, \n" +
				        "   uniform float invisibleDist )\n" +
				        "{	\n";

                    if (lightningEnabled) vertexProgSource +=
				        //Perform lighting calculations (no specular)
				        "	float3 light = normalize(objSpaceLight.xyz - (iPosition.xyz * objSpaceLight.w)); \n" +
				        "	float diffuseFactor = max(dot(normal, light), 0); \n" +
				        "	oColor = (lightAmbient + diffuseFactor * lightDiffuse) * iColor; \n";
			        else vertexProgSource +=
				        "	oColor = iColor; \n";

                    if (mFadeEnabled) vertexProgSource +=
				        //Fade out in the distance
				        "	float dist = distance(camPos.xz, iPosition.xz);	\n" +
				        "	oColor.a *= (invisibleDist - dist) / fadeGap;   \n";

			        vertexProgSource +=
				        "	oUV = iUV;	\n" +
				        "	oPosition = mul(worldViewProj, iPosition);  \n" +
				        "	oFog.x = oPosition.z; \n" +
				        "}";

                    HighLevelGpuProgram vertexShader = HighLevelGpuProgramManager.Instance.CreateProgram(
                        vertexProgName,
                        ResourceGroupManager.DefaultResourceGroupName,
                        "cg", GpuProgramType.Vertex);

                    vertexShader.Source = vertexProgSource;
                    vertexShader.SetParam("profiles", "vs_1_1 arbvp1");
                    vertexShader.SetParam("entry_point", "main");
                    vertexShader.Load();
                        
                }

                //Now that the shader is ready to be applied, apply it
                string materialSignature = string.Empty;
                materialSignature += "BatchMat|";
                materialSignature += mat.Name + "|";
                if (mFadeEnabled)
                {
                    materialSignature += mVisibleDist + "|";
                    materialSignature += mInvisibleDist + "|";
                }
                //Search for the desired material
                Material generatedMaterial = (Material)MaterialManager.Instance.GetByName(materialSignature);
                if (generatedMaterial == null)
                {
                    //Clone the material
                    generatedMaterial = mat.Clone(materialSignature);

                    //And apply the fade shader
                    for (int t = 0; t < generatedMaterial.TechniqueCount; t++)
                    {
                        Technique tech = generatedMaterial.GetTechnique(t);
                        for (int p = 0; p < tech.PassCount; p++)
                        {
                            Pass pass = tech.GetPass(p);
                            //Setup vertex program
                            if (pass.VertexProgramName == "")
                                pass.VertexProgramName = vertexProgName;
                            try
                            {
                                GpuProgramParameters gparams = pass.VertexProgramParameters;
                                if (lightningEnabled)
                                {
                                    gparams.SetNamedAutoConstant("objSpaceLight", GpuProgramParameters.AutoConstantType.LightPositionObjectSpace,0);
                                    gparams.SetNamedAutoConstant("lightDiffuse", GpuProgramParameters.AutoConstantType.LightDiffuseColor, 0);
                                    gparams.SetNamedAutoConstant("lightAmbient", GpuProgramParameters.AutoConstantType.AmbientLightColor, 0);
                                }

                                gparams.SetNamedAutoConstant("worldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);

                                if (mFadeEnabled)
                                {
                                    gparams.SetNamedAutoConstant("camPos", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);

                                    //set fade ranges
                                    gparams.SetNamedAutoConstant("invisibleDist", GpuProgramParameters.AutoConstantType.Custom, 0);
                                    gparams.SetNamedConstant("invisibleDist", mInvisibleDist);
                                    gparams.SetNamedAutoConstant("fadeGap", GpuProgramParameters.AutoConstantType.Custom, 0);
                                    gparams.SetNamedConstant("fadeGap", mInvisibleDist - mVisibleDist);

                                    if (pass.AlphaRejectFunction == CompareFunction.AlwaysPass)
                                        pass.SetSceneBlending(SceneBlendType.TransparentAlpha);

                                }
                            }
                            catch
                            {
                                throw new Exception("Error configuring batched geometry transitions." +
                                "If you're using materials with custom vertex shaders, they will need to implement fade transitions to be compatible with BatchPage.");
                            }
                        }
                    }
                }

                //Apply the material
                subBatch.Material = generatedMaterial;
            }
        }
    }
}
