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
    /// <summary>
    /// 
    /// </summary>
    public class GrassLayer
    {
        private GrassLoader mParent;
        private Material mMaterial;
        private float mDensity;
        internal float mMinWidth;
        internal float mMaxWidth;
        internal float mMinHeight;
        internal float mMaxHeight;
        internal float mMinY;
        internal float mMaxY;
        private FadeTechnique mFadeTechnique;
        private GrassTechnique mRenderTechnique;
        private TBounds mMapBounds;
        private DensityMap mDensityMap;
        private MapFilter mDensityMapFilter;
        private ColorMap mColorMap;
        private MapFilter mColorMapFilter;
        private bool mAnimate;
        private bool mBlend;
        private bool mShaderNeedsUpdate;
        private float mAnimMag;
        private float mAnimSpeed;
        private float mAnimFreq;
        private float mWaveCount;
        private PagedGeometry mGeom;

        /// <summary>
        /// 
        /// </summary>
        public GrassTechnique RenderTechnique
        {
            get { return mRenderTechnique; }
        }
        /// <summary>
        /// Sets the material that is applied to all grass billboards/quads
        /// </summary>
        public string MaterialName
        {
            set 
            {
                if (mMaterial == null || mMaterial.Name != value)
                {
                    mMaterial = (Material)MaterialManager.Instance.GetByName(value);
                    if (mMaterial == null)
                        throw new Exception("The specified grass material does not exist");

                    mShaderNeedsUpdate = true;
                }
            }
            get
            {
                return mMaterial != null ? mMaterial.Name : "Unknown";
            }
        }
        /// <summary>
        ///  Get's or Set's the maximum density (measured in grass quads/billboards per square unit) of grass.
        /// </summary>
        public float Density
        {
            set { mDensity = value; }
            get { return mDensity; }
        }
        /// <summary>
        /// 
        /// </summary>
        public MapFilter DensityMapFilter
        {
            set 
            { 
                mDensityMapFilter = value;
                if (mDensityMap != null)
                    mDensityMap.Filter = value;
            }
            get { return mDensityMapFilter; }
        }
        /// <summary>
        /// 
        /// </summary>
        public MapFilter ColorMapFilter
        {
            set 
            { 
                mColorMapFilter = value;
                if (mColorMap != null)
                    mColorMap.Filter = value;
            }
            get { return mColorMapFilter; }
        }
        /// <summary>
        /// 
        /// </summary>
        public TBounds MapBounds
        {
            get { return mMapBounds; }
            set
            {
                mMapBounds = value;
                if (mDensityMap != null)
                    mDensityMap.MapBounds = mMapBounds;
                if (mColorMap != null)
                    mColorMap.MapBounds = mMapBounds;
            }
        }
        /// <summary>
        /// Get's the density map. To set one, see GrassLayer.SetDensityMap(..);
        /// </summary>
        public DensityMap DensityMap
        {
            get { return mDensityMap; }
        }

        /// <summary>
        /// Get's the color map. To set one, see GrassLayer.SetColorMap(..);
        /// </summary>
        public ColorMap ColorMap
        {
            get { return mColorMap; }
        }
        /// <summary>
        /// 
        /// </summary>
        public FadeTechnique FadeTechnique
        {
            set 
            {
                if (mFadeTechnique != value)
                {
                    mFadeTechnique = value;
                    mShaderNeedsUpdate = true;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsAnimationEnabled
        {
            set 
            {
                if (mAnimate != value)
                {
                    mAnimate = value;
                    mShaderNeedsUpdate = true;
                }
            }
            get { return mAnimate; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float SwayLength
        {
            set { mAnimMag = value; }
            get { return mAnimMag; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float SwaySpeed
        {
            set { mAnimSpeed = value; }
            get { return mAnimSpeed; }
        }
        /// <summary>
        /// 
        /// </summary>
        public float SwayDistribution
        {
            set { mAnimFreq = value; }
            get { return mAnimFreq; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal Material Material
        {
            get { return mMaterial; }
        }
        /// <summary>
        /// 
        /// </summary>
        internal float WaveCount
        {
            get { return mWaveCount; }
            set { mWaveCount = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="geom"></param>
        /// <param name="ldr"></param>
        internal GrassLayer(PagedGeometry geom, GrassLoader ldr)
        {
            mGeom = geom;
            mParent = ldr;
            mDensity = 1.0f;
            mMinWidth = 1.0f;
            mMinHeight = 1.0f;
            mMaxWidth = 1.0f;
            mMaxHeight = 1.0f;
            mRenderTechnique = GrassTechnique.Quad;
            mFadeTechnique = FadeTechnique.Alpha;
            mAnimMag = 1.0f;
            mAnimSpeed = 1.0f;
            mAnimFreq = 1.0f;
            mWaveCount = 0.0f;
            mAnimate = false;
            mShaderNeedsUpdate = true;
            mDensityMap = null;
            mDensityMapFilter = MapFilter.Bilinear;
            mColorMap = null;
            mColorMapFilter = MapFilter.Bilinear;
        }
        /// <summary>
        /// Sets the minimum size that grass quads/billboards will be
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMinimumSize(float width, float height)
        {
            mMinWidth = width;
            mMinHeight = height;
        }
        /// <summary>
        /// Sets the maximum size that grass quads/billboards will be
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void SetMaximumSize(float width, float height)
        {
            mMaxWidth = width;
            if (mMaxHeight != height)
            {
                mMaxHeight = height;
                mShaderNeedsUpdate = true;
            }
        }
        /// <summary>
        /// see -> SetHeightRange(float minHeight, float maxHeight), for detailed description.
        /// </summary>
        /// <param name="minHeight"></param>
        public void SetHeightRange(float minHeight)
        {
            SetHeightRange(minHeight, 0);
        }
        /// <summary>
        /// Sets a minimum / maximum height where grass may appear
        /// </summary>
        /// <param name="minHeight">Sets the minimum height grass may have. 0 = no minimum</param>
        /// <param name="maxHeight">Sets the maximum height grass may have. 0 = no maximum</param>
        /// <remarks>
        /// By default grass appears at all altitudes. You can use this function to restrict grass to a
	    /// certain height range. For example, if sea level is at 100 units Y, you might restrict this
	    /// layer to display only above 100 units (so your grass doesn't grow under water).
        /// 
	    /// It's possible to use density maps (see setDensityMap()) to achieve similar results, but if
	    /// your density map is extremely low resolution, this function may be the only practical option
	    /// to prevent grass from growing under water (when used in combination with your density map).
        /// 
	    /// Setting minHeight to 0 means grass has no minimum height - it can grow as low as necessary.
	    /// Similarly, setting maxHeight to 0 means grass has no maximum height - it can grow as high
        /// as necessary.
        /// 
        /// </remarks>
        public void SetHeightRange(float minHeight, float maxHeight)
        {
            mMinY = minHeight;
            mMaxY = maxHeight;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        public void SetDensityMap(string mapFile)
        {
            SetDensityMap(mapFile, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        /// <param name="channel"></param>
        public void SetDensityMap(string mapFile, MapChannel channel)
        {
            if (mDensityMap != null)
            {
                mDensityMap.Unload();
                mDensityMap = null;
            }
            if (mapFile != "")
            {
                mDensityMap = DensityMap.Load(mapFile, channel);
                mDensityMap.MapBounds = mMapBounds;
                mDensityMap.Filter = mDensityMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void SetDensityMap(Texture map)
        {
            SetDensityMap(map, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="channel"></param>
        public void SetDensityMap(Texture map, MapChannel channel)
        {
            if (mDensityMap != null)
            {
                mDensityMap.Unload();
                mDensityMap = null;
            }
            if (map != null)
            {
                mDensityMap = DensityMap.Load(map, channel);
                mDensityMap.MapBounds = mMapBounds;
                mDensityMap.Filter = mDensityMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        public void SetColorMap(string mapFile)
        {
            SetColorMap(mapFile, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mapFile"></param>
        /// <param name="channel"></param>
        public void SetColorMap(string mapFile, MapChannel channel)
        {
            if (mColorMap != null)
            {
                mColorMap.Unload();
                mColorMap = null;
            }
            if (mapFile != "")
            {
                mColorMap = ColorMap.Load(mapFile, channel);
                mColorMap.MapBounds = mMapBounds;
                mColorMap.Filter = mColorMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        public void SetColorMap(Texture map)
        {
            SetColorMap(map, MapChannel.Color);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="map"></param>
        /// <param name="channel"></param>
        public void SetColorMap(Texture map, MapChannel channel)
        {
            if (mColorMap != null)
            {
                mColorMap.Unload();
                mColorMap = null;
            }
            if (map != null)
            {
                mColorMap = ColorMap.Load(map, channel);
                mColorMap.MapBounds = mMapBounds;
                mColorMap.Filter = mColorMapFilter;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        public void SetRenderTechnique(GrassTechnique style)
        {
            SetRenderTechnique(style, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        /// <param name="blendBase"></param>
        public void SetRenderTechnique(GrassTechnique style, bool blendBase)
        {
            if (blendBase != mBlend || mRenderTechnique != style)
            {
                mBlend = blendBase;
                mRenderTechnique = style;
                mShaderNeedsUpdate = true;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        internal void UpdateShaders()
        {
            if (mShaderNeedsUpdate)
            {
                mShaderNeedsUpdate = false;
                //Proceed only if there is no custom vertex shader and the user's computer supports vertex shaders
                RenderSystemCapabilities caps = Root.Instance.RenderSystem.Capabilities;
                if (caps.HasCapability(Capabilities.VertexPrograms))
                {
                    //Generate a string ID that identifies the current set of vertex shader options
                    string tmpName = string.Empty;
                    tmpName += "GrassVS_";
                    if (mAnimate)
                        tmpName += "anim_";
                    if (mBlend)
                        tmpName += "blend_";
                    tmpName += mRenderTechnique.ToString() + "_";
                    tmpName += mFadeTechnique.ToString() + "_";
                    if (mFadeTechnique == FadeTechnique.Grow || mFadeTechnique == FadeTechnique.AlphaGrow)
                        tmpName += mMaxHeight + "_";
                    tmpName += "vp";

                    string vsName = tmpName;
                    //Generate a string ID that identifies the material combined with the vertex shader
                    string matName = mMaterial.Name + "_" + vsName;

                    //Check if the desired material already exists (if not, create it)
                    Material tmpMat = (Material)MaterialManager.Instance.GetByName(matName);
                    if (tmpMat == null)
                    {
                        //Clone the original material
                        tmpMat = mMaterial.Clone(matName);

                        //Disable lighting
                        tmpMat.Lighting = false;
                        //Check if the desired shader already exists (if not, compile it)
                        HighLevelGpuProgram vertexShader = (HighLevelGpuProgram)HighLevelGpuProgramManager.Instance.GetByName(vsName);
                        if (vertexShader == null)
                        {
                            //Generate the grass shader
                            string vertexProgSource = string.Empty;
                            vertexProgSource +=
                                "void main( \n" +
                                "	float4 iPosition : POSITION, \n" +
                                "	float4 iColor : COLOR, \n" +
                                "	float2 iUV       : TEXCOORD0,	\n" +
                                "	out float4 oPosition : POSITION, \n" +
                                "	out float4 oColor : COLOR, \n" +
                                "	out float2 oUV       : TEXCOORD0,	\n";

                            if (mAnimate)
                            {
                                vertexProgSource +=
                                    "	uniform float time,	\n" +
                                    "	uniform float frequency,	\n" +
                                    "	uniform float4 direction,	\n";
                            }
                            if (mFadeTechnique == FadeTechnique.Grow || mFadeTechnique == FadeTechnique.AlphaGrow)
                            {
                                vertexProgSource +=
                                    "	uniform float grassHeight,	\n";
                            }
                            if (mRenderTechnique == GrassTechnique.Sprite)
                            {
                                vertexProgSource +=
                                       "	float4 iNormal : NORMAL, \n";
                            }

                            vertexProgSource +=
                                "	uniform float4x4 worldViewProj,	\n" +
                                "	uniform float3 camPos, \n" +
                                "	uniform float fadeRange ) \n" +
                                "{	\n" +
                                "	oColor.rgb = iColor.rgb;   \n" +
                                "	float4 position = iPosition;	\n" +
                                "	float dist = distance(camPos.xz, position.xz);	\n";

                            if (mFadeTechnique == FadeTechnique.Alpha || mFadeTechnique == FadeTechnique.AlphaGrow)
                            {
                                vertexProgSource +=
                                    //Fade out in the distance
                                "	oColor.a = 2.0f - (2.0f * dist / fadeRange);   \n";
                            }
                            else
                            {
                                vertexProgSource +=
                                    "	oColor.a = 1.0f;   \n";
                            }

                            vertexProgSource +=
                                "	float oldposx = position.x;	\n";

                            if (mRenderTechnique == GrassTechnique.Sprite)
                            {
                                vertexProgSource +=
                                    //Face the camera
                                "	float3 dirVec = (float3)position - (float3)camPos;		\n" +
                                "	float3 p = normalize(cross(float4(0,1,0,0), dirVec));	\n" +
                                "	position += float4(p.x * iNormal.x, iNormal.y, p.z * iNormal.x, 0);	\n";
                            }

                            if (mAnimate)
                            {
                                vertexProgSource +=
                                "	if (iUV.y == 0.0f){	\n" +
                                    //Wave grass in breeze
                                "		float offset = sin(time + oldposx * frequency);	\n" +
                                "		position += direction * offset;	\n" +
                                "	}	\n";
                            }
                            if (mBlend && mAnimate)
                            {
                                vertexProgSource +=
                                "	else {	\n";
                            }
                            else if (mBlend)
                            {
                                vertexProgSource +=
                                "	if (iUV.y != 0.0f){	\n";
                            }
                            if (mBlend)
                            {
                                vertexProgSource +=
                                    //Blend the base of nearby grass into the terrain
                                "		if (oColor.a >= 1.0f) \n" +
                                "			oColor.a = 4.0f * ((dist / fadeRange) - 0.1f);	\n" +
                                "	}	\n";
                            }
                            if (mFadeTechnique == FadeTechnique.Grow || mFadeTechnique == FadeTechnique.AlphaGrow)
                            {
                                vertexProgSource +=
                                    "	float offset = (2.0f * dist / fadeRange) - 1.0f; \n" +
                                    "	position.y -= grassHeight * clamp(offset, 0, 1); ";
                            }
                            vertexProgSource +=
                                "	oPosition = mul(worldViewProj, position);  \n";

                            vertexProgSource +=
                                "	oUV = iUV;\n" +
                                "}";
                            vertexShader = HighLevelGpuProgramManager.Instance.CreateProgram(
                                vsName,
                                ResourceGroupManager.DefaultResourceGroupName,
                                "cg", GpuProgramType.Vertex);
                            vertexShader.Source = vertexProgSource;
                            vertexShader.SetParam("profiles", "vs_1_1 arbvp1");
                            vertexShader.SetParam("entry_point", "main");
                            vertexShader.Load();
                        }
                        //Now the vertex shader (vertexShader) has either been found or just generated
                        //(depending on whether or not it was already generated). 

                        //Apply the shader to the material
                        Pass pass = tmpMat.GetTechnique(0).GetPass(0);
                        pass.VertexProgramName = vsName;
                        GpuProgramParameters gparams = pass.VertexProgramParameters;

                        gparams.SetNamedAutoConstant("worldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
                        gparams.SetNamedAutoConstant("camPos", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);
                        gparams.SetNamedAutoConstant("fadeRange", GpuProgramParameters.AutoConstantType.Custom, 1);

                        if (mAnimate)
                        {
                            gparams.SetNamedAutoConstant("time", GpuProgramParameters.AutoConstantType.Custom, 1);
                            gparams.SetNamedAutoConstant("frequency", GpuProgramParameters.AutoConstantType.Custom, 1);
                            gparams.SetNamedAutoConstant("direction", GpuProgramParameters.AutoConstantType.Custom, 4);
                        }

                        if (mFadeTechnique == FadeTechnique.Grow || mFadeTechnique == FadeTechnique.AlphaGrow)
                        {
                            gparams.SetNamedAutoConstant("grassHeight", GpuProgramParameters.AutoConstantType.Custom, 1);
                            gparams.SetNamedConstant("grassHeight", mMaxHeight * 1.05f);
                        }

                        float farViewDist = mGeom.DetailLevels[0].FarRange;
                        pass.VertexProgramParameters.SetNamedConstant("fadeRange", farViewDist / 1.225f);
                        //Note: 1.225 ~= sqrt(1.5), which is necessary since the far view distance is measured from the centers
                        //of pages, while the vertex shader needs to fade grass completely out (including the closest corner)
                        //before the page center is out of range.

                    }
                    //Now the material (tmpMat) has either been found or just created (depending on whether or not it was already
                    //created). The appropriate vertex shader should be applied and the material is ready for use.

                    //Apply the new material
                    mMaterial = tmpMat;
                }

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="posBuff"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        internal uint PopulateGrassListUniform(PageInfo page, IntPtr posBuff, int grassCount)
        {
            unsafe
            {
                
                float* posPtr = (float*)posBuff;
                float* posBufBase = (float*)new IntPtr(posPtr);
                //No density map
                if (mMinY == 0 && mMaxY == 0)
                {
                    //No height range
                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);
                        //Add to list in within bounds
                        if (mColorMap == null)
                        {
                            *posPtr++ = x;
                            *posPtr++ = z;
                        }
                        else if (x >= mMapBounds.Left && x <= mMapBounds.Right && z >= mMapBounds.Top && z <= mMapBounds.Bottom)
                        {
                            *posPtr++ = x;
                            *posPtr++ = z;
                        }
                    }
                }
                else
                {
                    //Height range
                    float min = 0, max = 0;
                    if (mMinY != 0) min = mMinY; else min = float.NegativeInfinity;
                    if (mMaxY != 0) max = mMaxY; else max = float.PositiveInfinity;

                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);

                        //Calculate height
                        float y = mParent.HeightFunction.GetHeightAt(x, z, mParent.HeightFunctionUserData);

                        //Add to list if in range
                        if (y >= min && y <= max)
                        {
                            if (mColorMap == null)
                            {
                                *posPtr++ = x;
                                *posPtr++ = z;
                            }
                            else if (x >= mMapBounds.Left && x <= mMapBounds.Right && z >= mMapBounds.Top && z <= mMapBounds.Bottom)
                            {
                                *posPtr++ = x;
                                *posPtr++ = z;
                            }
                        }
                    }
                }
                uint count = (uint)(posPtr - posBufBase) / 2;
                return count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="posBuff"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        internal uint PopulateGrassListUnfilteredDM(PageInfo page, IntPtr posBuff, int grassCount)
        {
            unsafe
            {

                float* posPtr = (float*)posBuff;
                float* posBufBase = (float*)new IntPtr(posPtr);
                //No density map
                if (mMinY == 0 && mMaxY == 0)
                {
                    //No height range
                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);
                        //Add to list in within bounds
                        if (Axiom.Math.Utility.UnitRandom() < mDensityMap.GetDensityAtUnfiltered(x, z))
                        {
                            //Add to list
                            *posPtr++ = x;
                            *posPtr++ = z;
                        }
                    }
                }
                else
                {
                    //Height range
                    float min = 0, max = 0;
                    if (mMinY != 0) min = mMinY; else min = float.NegativeInfinity;
                    if (mMaxY != 0) max = mMaxY; else max = float.PositiveInfinity;

                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);

                        if (Axiom.Math.Utility.UnitRandom() < mDensityMap.GetDensityAtUnfiltered(x, z))
                        {
                            //Calculate height
                            float y = mParent.HeightFunction.GetHeightAt(x, z, mParent.HeightFunctionUserData);

                            //Add to list if in range
                            if (y >= min && y <= max)
                            {
                                *posPtr++ = x;
                                *posPtr++ = z;
                            }
                        }
                    }
                }
                uint count = (uint)(posPtr - posBufBase) / 2;
                return count;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="posBuff"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        internal uint PopulateGrassListBilenearDM(PageInfo page, IntPtr posBuff, int grassCount)
        {
            unsafe
            {

                float* posPtr = (float*)posBuff;
                float* posBufBase = (float*)new IntPtr(posPtr);
                //No density map
                if (mMinY == 0 && mMaxY == 0)
                {
                    //No height range
                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);
                        //Add to list in within bounds
                        if (Axiom.Math.Utility.UnitRandom() < mDensityMap.GetDensityAtBilenear(x, z))
                        {
                            //Add to list
                            *posPtr++ = x;
                            *posPtr++ = z;
                        }
                    }
                }
                else
                {
                    //Height range
                    float min = 0, max = 0;
                    if (mMinY != 0) min = mMinY; else min = float.NegativeInfinity;
                    if (mMaxY != 0) max = mMaxY; else max = float.PositiveInfinity;

                    for (int i = 0; i < grassCount; i++)
                    {
                        //Pick a random position
                        float x = Axiom.Math.Utility.RangeRandom(page.Bounds.Left, page.Bounds.Right);
                        float z = Axiom.Math.Utility.RangeRandom(page.Bounds.Top, page.Bounds.Bottom);

                        if (Axiom.Math.Utility.UnitRandom() < mDensityMap.GetDensityAtBilenear(x, z))
                        {
                            //Calculate height
                            float y = mParent.HeightFunction.GetHeightAt(x, z, mParent.HeightFunctionUserData);

                            //Add to list if in range
                            if (y >= min && y <= max)
                            {
                                *posPtr++ = x;
                                *posPtr++ = z;
                            }
                        }
                    }
                }
                uint count = (uint)(posPtr - posBufBase) / 2;
                return count;
            }
        }
    }
}
