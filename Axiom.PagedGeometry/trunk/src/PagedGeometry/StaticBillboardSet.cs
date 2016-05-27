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
using System.Diagnostics;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.Forests
{
    
    public class StaticBillboardSet
    {
        private SceneManager mSceneMgr;
        private bool mVisible;
        private SceneNode mNode;
        private Entity mEntity;
        private Mesh mMesh;
        private SubMesh mSubMesh;
        private string mEntityName;
        private Material mMaterial;
        private Material mFadeMaterial;
        private List<StaticBillboard> mBillboardBuffer = new List<StaticBillboard>();
        private BillboardMethod mRenderMethod;
        private BillboardSet mFallbackSet;
        private BillboardOrigin mBBOrigin;
        private bool mFadeEnabled;
        private float mFadeVisibleDist;
        private float mFadeInvisibleDist;
        private static Dictionary<string, Material> mFadedMaterialMap = new Dictionary<string, Material>();
        private float mUFactor;
        private float mVFactor;
        private static uint mSelfInstances = 0;
        private static long GUID = 0;

        /// <summary>
        /// 
        /// </summary>
        public string MaterialName
        {
            set
            {
                if (mRenderMethod == BillboardMethod.Accelerated)
                {
                    if (mMaterial == null || mMaterial.Name != value)
                    {
                        //Update material reference list
                        if (mFadeEnabled)
                        {
                            Debug.Assert(mFadeMaterial != null);
                            SBMaterialRef.RemoveMaterialRef(mFadeMaterial);
                        }
                        else
                        {
                            if (mMaterial != null)
                                SBMaterialRef.RemoveMaterialRef(mMaterial);
                        }

                        mMaterial = (Material)MaterialManager.Instance.GetByName(value);
                        if (mFadeEnabled)
                        {
                            mFadeMaterial = GetFadeMaterial(mFadeVisibleDist, mFadeInvisibleDist);
                            SBMaterialRef.AddMaterialRef(mFadeMaterial, mBBOrigin);
                        }
                        else
                        {
                            SBMaterialRef.AddMaterialRef(mMaterial, mBBOrigin);
                        }

                        //Apply material to entity
                        if (mEntity != null)
                        {
                            if (mFadeEnabled)
                            {
                                mEntity.MaterialName = mFadeMaterial.Name;
                            }
                            else
                            {
                                mEntity.MaterialName = mMaterial.Name;
                            }
                        }
                    }
                }
                else
                {
                    if (mMaterial == null || mMaterial.Name != value)
                    {
                        mMaterial = (Material)MaterialManager.Instance.GetByName(value);
                        mFallbackSet.MaterialName = mMaterial.Name;
                    }
                }
            }

            get
            {
                return mMaterial.Name;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="rootSceneNode"></param>
        public StaticBillboardSet(SceneManager mgr, SceneNode rootSceneNode)
            : this(mgr, rootSceneNode, BillboardMethod.Accelerated)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="rootSceneNode"></param>
        /// <param name="method"></param>
        public StaticBillboardSet(SceneManager mgr, SceneNode rootSceneNode, BillboardMethod method)
        {
            mSceneMgr = mgr;
            mRenderMethod = method;
            mVisible = true;
            mFadeEnabled = false;
            mBBOrigin = BillboardOrigin.Center;

            //Fall back to Compatible if vertex shaders are not available
            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                RenderSystemCapabilities caps = Root.Instance.RenderSystem.Capabilities;
                if (!caps.HasCapability(Capabilities.VertexPrograms))
                    mRenderMethod = BillboardMethod.Compatible;
            }

            mNode = rootSceneNode.CreateChildSceneNode();
            mEntityName = GetUniqueID("SBSEntity");

            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                //Accelerated billboard method
                mEntity = null;
                mUFactor = 1.0f;
                mVFactor = 1.0f;

                //Load vertex shader to align billboards to face the camera (if not loaded already)
                if (++mSelfInstances == 1)
                {
                    //First shader, simple camera-alignment
                    HighLevelGpuProgram vertexShader =
                        (HighLevelGpuProgram)HighLevelGpuProgramManager.Instance.GetByName("Sprite_vp");
                    if (vertexShader == null)
                    {
                        string vertexProg = string.Empty;

                        vertexProg =
                            "void Sprite_vp(	\n" +
                            "	float4 position : POSITION,	\n" +
                            "	float3 normal   : NORMAL,	\n" +
                            "	float4 color	: COLOR,	\n" +
                            "	float2 uv       : TEXCOORD0,	\n" +
                            "	out float4 oPosition : POSITION,	\n" +
                            "	out float2 oUv       : TEXCOORD0,	\n" +
                            "	out float4 oColor    : COLOR, \n" +
                            "	out float4 oFog      : FOG,	\n" +
                            "	uniform float4x4 worldViewProj,	\n" +
                            "	uniform float    uScroll, \n" +
                            "	uniform float    vScroll, \n" +
                            "	uniform float4   preRotatedQuad[4] )	\n" +
                            "{	\n" +
                            //Face the camera
                            "	float4 vCenter = float4( position.x, position.y, position.z, 1.0f );	\n" +
                            "	float4 vScale = float4( normal.x, normal.y, normal.x, 1.0f );	\n" +
                            "	oPosition = mul( worldViewProj, vCenter + (preRotatedQuad[normal.z] * vScale) );  \n" +

                            //Color
                            "	oColor = color;   \n" +

                            //UV Scroll
                            "	oUv = uv;	\n" +
                            "	oUv.x += uScroll; \n" +
                            "	oUv.y += vScroll; \n" +

                            //Fog
                            "	oFog.x = oPosition.z; \n" +
                            "}";

                        vertexShader = HighLevelGpuProgramManager.Instance.CreateProgram(
                            "Sprite_vp",
                            ResourceGroupManager.DefaultResourceGroupName,
                            "cg", GpuProgramType.Vertex);

                        vertexShader.Source = vertexProg;
                        vertexShader.SetParam("profiles", "vs_1_1 arbvp1");
                        vertexShader.SetParam("entry_point", "Sprite_vp");
                        vertexShader.Load();
                    }

                    //Second shader, camera alignment and distance based fading
                    HighLevelGpuProgram vertexShader2 =
                        (HighLevelGpuProgram)HighLevelGpuProgramManager.Instance.GetByName("SpriteFade_vp");
                    if (vertexShader2 == null)
                    {
                        string vertexProg2 = string.Empty;

                        vertexProg2 =
                            "void SpriteFade_vp(	\n" +
                            "	float4 position : POSITION,	\n" +
                            "	float3 normal   : NORMAL,	\n" +
                            "	float4 color	: COLOR,	\n" +
                            "	float2 uv       : TEXCOORD0,	\n" +
                            "	out float4 oPosition : POSITION,	\n" +
                            "	out float2 oUv       : TEXCOORD0,	\n" +
                            "	out float4 oColor    : COLOR, \n" +
                            "	out float4 oFog      : FOG,	\n" +
                            "	uniform float4x4 worldViewProj,	\n" +

                            "	uniform float3 camPos, \n" +
                            "	uniform float fadeGap, \n" +
                            "   uniform float invisibleDist, \n" +

                            "	uniform float    uScroll, \n" +
                            "	uniform float    vScroll, \n" +
                            "	uniform float4   preRotatedQuad[4] )	\n" +
                            "{	\n" +
                            //Face the camera
                            "	float4 vCenter = float4( position.x, position.y, position.z, 1.0f );	\n" +
                            "	float4 vScale = float4( normal.x, normal.y, normal.x, 1.0f );	\n" +
                            "	oPosition = mul( worldViewProj, vCenter + (preRotatedQuad[normal.z] * vScale) );  \n" +

                            "	oColor.rgb = color.rgb;   \n" +

                            //Fade out in the distance
                            "	float dist = distance(camPos.xz, position.xz);	\n" +
                            "	oColor.a = (invisibleDist - dist) / fadeGap;   \n" +

                            //UV scroll
                            "	oUv = uv;	\n" +
                            "	oUv.x += uScroll; \n" +
                            "	oUv.y += vScroll; \n" +

                            //Fog
                            "	oFog.x = oPosition.z; \n" +
                            "}";

                        vertexShader2 = HighLevelGpuProgramManager.Instance.CreateProgram(
                            "SpriteFade_vp",
                            ResourceGroupManager.DefaultResourceGroupName,
                            "cg", GpuProgramType.Vertex);
                        vertexShader2.Source = vertexProg2;
                        vertexShader2.SetParam("profiles", "vs_1_1 arbvp1");
                        vertexShader2.SetParam("entry_point", "SpriteFade_vp");
                        vertexShader2.Load();
                    }
                }
            }
            else
            {
                //Compatible billboard method
                mFallbackSet = mSceneMgr.CreateBillboardSet(GetUniqueID("SBS"), 100);
                mNode.AttachObject(mFallbackSet);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public BillboardMethod BillboardMethod
        {
            get { return mRenderMethod; }
        }
        /// <summary>
        /// 
        /// </summary>
        public BillboardOrigin BillboardOrigin
        {
            get { return mBBOrigin; }
            set
            {
                if (value != BillboardOrigin.Center && value != BillboardOrigin.BottomCenter)
                    throw new Exception("Invalid origin - only CENTER and BOTTOM_CENTER is supported");
                if (mRenderMethod == BillboardMethod.Accelerated)
                {
                    mBBOrigin = value;
                }
                else
                {
                    mBBOrigin = value;
                    mFallbackSet.BillboardOrigin = value;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Build()
        {
            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                Clear();

                //If there are no billboards to create, exit
                if (mBillboardBuffer.Count == 0)
                    return;

                //Create manual mesh to store billboard quads
                mMesh = MeshManager.Instance.CreateManual(GetUniqueID("SBSMesh"), ResourceGroupManager.DefaultResourceGroupName, null);
                mSubMesh = mMesh.CreateSubMesh();
                mSubMesh.useSharedVertices = false;

                //Setup vertex format information
                mSubMesh.vertexData = new VertexData();
                mSubMesh.vertexData.vertexStart = 0;
                mSubMesh.vertexData.vertexCount = 4 * mBillboardBuffer.Count;

                VertexDeclaration dcl = mSubMesh.vertexData.vertexDeclaration;

                int offset = 0;
                dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
                offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Normal);
                offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Diffuse);
                offset += VertexElement.GetTypeSize(VertexElementType.Color);
                dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.TexCoords);
                offset += VertexElement.GetTypeSize(VertexElementType.Float2);


                //Populate a new vertex buffer
                HardwareVertexBuffer vbuf = HardwareBufferManager.Instance.CreateVertexBuffer(
                    /*offset*/dcl, mSubMesh.vertexData.vertexCount, BufferUsage.StaticWriteOnly, false);
                unsafe
                {
                    float* pReal = (float*)vbuf.Lock(BufferLocking.Discard);

                    float minX = float.PositiveInfinity;
                    float maxX = float.NegativeInfinity;
                    float minY = float.PositiveInfinity;
                    float maxY = float.NegativeInfinity;
                    float minZ = float.PositiveInfinity;
                    float maxZ = float.NegativeInfinity;

                    foreach (StaticBillboard it in mBillboardBuffer)
                    {
                        StaticBillboard bb = it;
                        float halfXScale = bb.XScale * 0.5f;
                        float halfYScale = bb.YScale * 0.5f;

                        // position
                        *pReal++ = bb.Position.x;
                        *pReal++ = bb.Position.y;
                        *pReal++ = bb.Position.z;

                        // normals (actually used as scale / translate info for vertex shader)
                        *pReal++ = halfXScale;
                        *pReal++ = halfYScale;
                        *pReal++ = 0.0f;
                        // color
                        *((uint*)pReal++) = bb.Color;
                        // uv
                        *pReal++ = (float)(bb.TextCoordIndexU * mUFactor);
                        *pReal++ = (float)(bb.TextCoordIndexV * mVFactor);


                        // position
                        *pReal++ = bb.Position.x;
                        *pReal++ = bb.Position.y;
                        *pReal++ = bb.Position.z;

                        // normals (actually used as scale / translate info for vertex shader)
                        *pReal++ = halfXScale;
                        *pReal++ = halfYScale;
                        *pReal++ = 1.0f;
                        // color
                        *((uint*)pReal++) = bb.Color;
                        // uv
                        *pReal++ = (float)((bb.TextCoordIndexU + 1)* mUFactor);
                        *pReal++ = (float)(bb.TextCoordIndexV * mVFactor);

                        // position
                        *pReal++ = bb.Position.x;
                        *pReal++ = bb.Position.y;
                        *pReal++ = bb.Position.z;

                        // normals (actually used as scale / translate info for vertex shader)
                        *pReal++ = halfXScale;
                        *pReal++ = halfYScale;
                        *pReal++ = 2.0f;
                        // color
                        *((uint*)pReal++) = bb.Color;
                        // uv
                        *pReal++ = (float)(bb.TextCoordIndexU * mUFactor);
                        *pReal++ = (float)((bb.TextCoordIndexV + 1) * mVFactor);

                        // position
                        *pReal++ = bb.Position.x;
                        *pReal++ = bb.Position.y;
                        *pReal++ = bb.Position.z;

                        // normals (actually used as scale / translate info for vertex shader)
                        *pReal++ = halfXScale;
                        *pReal++ = halfYScale;
                        *pReal++ = 3.0f;
                        // color
                        *((uint*)pReal++) = bb.Color;
                        // uv
                        *pReal++ = (float)((bb.TextCoordIndexU + 1) * mUFactor);
                        *pReal++ = (float)((bb.TextCoordIndexV + 1) * mVFactor);


                        //Update bounding box
                        if (bb.Position.x - halfXScale < minX) minX = bb.Position.x - halfXScale;
                        if (bb.Position.x + halfXScale > maxX) maxX = bb.Position.x + halfXScale;
                        if (bb.Position.y - halfYScale < minY) minY = bb.Position.y - halfYScale;
                        if (bb.Position.y + halfYScale > maxY) maxY = bb.Position.y + halfYScale;
                        if (bb.Position.z - halfXScale < minZ) minZ = bb.Position.z - halfXScale;
                        if (bb.Position.z + halfXScale > maxZ) maxZ = bb.Position.z + halfXScale;

                    }

                    AxisAlignedBox bounds = new AxisAlignedBox(
                        new Vector3(minX, minY, minZ),
                        new Vector3(maxX, maxY, maxZ));
                    vbuf.Unlock();
                    mSubMesh.vertexData.vertexBufferBinding.SetBinding(0, vbuf);

                    //Populate index buffer
                    mSubMesh.indexData.indexStart = 0;
                    mSubMesh.indexData.indexCount = 6 * mBillboardBuffer.Count;
                    mSubMesh.indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                        IndexType.Size16, mSubMesh.indexData.indexCount, BufferUsage.StaticWriteOnly);

                    ushort* pI = (ushort*)mSubMesh.indexData.indexBuffer.Lock(BufferLocking.Discard);
                    for (ushort i = 0; i < mBillboardBuffer.Count; i++)
                    {
                        ushort ofset = (ushort)(i * 4);

                        *pI++ = (ushort)(0 + ofset);
                        *pI++ = (ushort)(2 + ofset);
                        *pI++ = (ushort)(1 + ofset);

                        *pI++ = (ushort)(1 + ofset);
                        *pI++ = (ushort)(2 + ofset);
                        *pI++ = (ushort)(3 + ofset);
                    }

                    mSubMesh.indexData.indexBuffer.Unlock();

                    //Finish up mesh
                    mMesh.BoundingBox = bounds;
                    Vector3 tmp = bounds.Maximum - bounds.Minimum;
                    mMesh.BoundingSphereRadius = tmp.Length * 0.5f;

                    LoggingLevel lvl = LogManager.Instance.LogDetail;
                    LogManager.Instance.LogDetail = LoggingLevel.Low;
                    mMesh.Load();
                    LogManager.Instance.LogDetail = lvl;

                    //Empty the billboardBuffer now, because all billboards have been built
                    mBillboardBuffer.Clear();

                    //Create an entity for the mesh
                    mEntity = mSceneMgr.CreateEntity(mEntityName, mMesh.Name);
                    mEntity.CastShadows = false;

                    //Apply texture
                    if (mFadeEnabled)
                    {
                        Debug.Assert(mFadeMaterial != null);
                        mEntity.MaterialName = mFadeMaterial.Name;
                    }
                    else
                    {
                        Debug.Assert(mMaterial != null);
                        mEntity.MaterialName = mMaterial.Name;
                    }

                    //Add to scene
                    mNode.AttachObject(mEntity);
                    mEntity.IsVisible = mVisible;
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                //Delete the entity and mesh data
                if (mEntity != null)
                {
                    //Delete entity
                    mNode.DetachAllObjects();
                    mSceneMgr.RemoveEntity(mEntity);
                    mEntity = null;

                    //Delete mesh
                    Debug.Assert(mMesh != null);
                    string meshName = mMesh.Name;
                    mMesh = null;
                    if (MeshManager.Instance != null)
                        MeshManager.Instance.Remove(meshName);
                }

                //Remove any billboard data which might be left over if the user forgot to call build()
                mBillboardBuffer.Clear();

            }
            else
            {
                mFallbackSet.Clear();
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="enabled"></param>
        /// <param name="visibleDist"></param>
        /// <param name="invisbleDist"></param>
        public void SetFade(bool enabled, float visibleDist, float invisbleDist)
        {
            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                if (enabled)
                {
                    if (mMaterial == null)
                        throw new Exception("Billboard fading cannot be enabled without a material applied first");

                    //Update material reference list
                    if (mFadeEnabled)
                    {
                        Debug.Assert(mFadeMaterial != null);
                        SBMaterialRef.RemoveMaterialRef(mFadeMaterial);
                    }
                    else
                    {
                        Debug.Assert(mMaterial != null);
                        SBMaterialRef.RemoveMaterialRef(mMaterial);
                    }

                    mFadeMaterial = GetFadeMaterial(visibleDist, invisbleDist);
                    SBMaterialRef.AddMaterialRef(mFadeMaterial, mBBOrigin);

                    //Apply material to entity
                    if (mEntity != null)
                        mEntity.MaterialName = mFadeMaterial.Name;

                    mFadeEnabled = enabled;
                    mFadeVisibleDist = visibleDist;
                    mFadeInvisibleDist = invisbleDist;
                }
                else
                {
                    if (mFadeEnabled)
                    {
                        //Update material reference list
                        Debug.Assert(mFadeMaterial != null);
                        Debug.Assert(mMaterial != null);
                        SBMaterialRef.RemoveMaterialRef(mFadeMaterial);
                        SBMaterialRef.AddMaterialRef(mMaterial, mBBOrigin);
                        //Apply material to entity
                        if (mEntity != null)
                            mEntity.MaterialName = mFadeMaterial.Name;

                        mFadeEnabled = enabled;
                        mFadeVisibleDist = visibleDist;
                        mFadeInvisibleDist = invisbleDist;
                    }
                }
            }
        }
        public void SetVisible(bool visible)
        {
            if (mVisible != visible)
            {
                mVisible = visible;
                mNode.IsVisible = visible;
            }
        }
        public static void UpdateAll(Vector3 cameraDirection)
        {
            if (mSelfInstances > 0)//selfInstances will only be greater than 0 if one or more StaticBillboardSet's are using BB_METHOD_ACCELERATED
            //Set shader parameter so material will face camera
            {
                Vector3 forward = cameraDirection;
                Vector3 vRight = forward.Cross(Vector3.UnitY);
                Vector3 vUp = forward.Cross(vRight);
                vRight.Normalize();
                vUp.Normalize();

                //Even if camera is upside down, the billboards should remain upright
                if (vUp.y < 0) vUp *= -1;

                //For each material in use by the billboard system..
                foreach (SBMaterialRef it in SBMaterialRef.SelfList.Values)
                {
                    Material mat = it.Material;
                    BillboardOrigin bbOrigin = it.Origin;

                    Vector3 vPoint0 = Vector3.Zero;
                    Vector3 vPoint1 = Vector3.Zero;
                    Vector3 vPoint2 = Vector3.Zero;
                    Vector3 vPoint3 = Vector3.Zero;

                    if (bbOrigin == BillboardOrigin.Center)
                    {
                        vPoint0 = (-vRight + vUp);
                        vPoint1 = (vRight + vUp);
                        vPoint2 = (-vRight - vUp);
                        vPoint3 = (vRight - vUp);
                    }
                    else if (bbOrigin == BillboardOrigin.BottomCenter)
                    {
                        vPoint0 = (-vRight + vUp + vUp);
                        vPoint1 = (vRight + vUp + vUp);
                        vPoint2 = (-vRight);
                        vPoint3 = (vRight);
                    }
                    //single prerotated quad oriented towards the camera
                    float[] preRotatedQuad = {
				        vPoint0.x, vPoint0.y, vPoint0.z, 0.0f,
				        vPoint1.x, vPoint1.y, vPoint1.z, 0.0f,
				        vPoint2.x, vPoint2.y, vPoint2.z, 0.0f,
				        vPoint3.x, vPoint3.y, vPoint3.z, 0.0f
			        };

                    Pass p = mat.GetTechnique(0).GetPass(0);
                    if (!p.HasVertexProgram)
                    {
                        p.SetVertexProgram("Sprite_vp");
                        GpuProgramParameters gparams = p.VertexProgramParameters;
                        gparams.SetNamedAutoConstant("worldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
                        gparams.SetNamedAutoConstant("uScroll", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("vScroll", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[0]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[1]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[2]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[3]", GpuProgramParameters.AutoConstantType.Custom, 0);
                    }
                    //Update the vertex shader parameters
                    GpuProgramParameters gparams2 = p.VertexProgramParameters;
					for ( int i = 0; i < preRotatedQuad.Length; i++ )
						gparams2.SetNamedConstant( string.Format( "preRotatedQuad[{0}]", i ), (Real)preRotatedQuad[ i ] );
                    //gparams2.SetNamedConstant("preRotatedQuad[0]", preRotatedQuad);
                    gparams2.SetNamedConstant("uScroll", p.GetTextureUnitState(0).TextureScrollU);
                    gparams2.SetNamedConstant("vScroll", p.GetTextureUnitState(0).TextureScrollV);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stacks"></param>
        /// <param name="slices"></param>
        public void SetTextureStacksAndSlices(short stacks, short slices)
        {
            mUFactor = 1.0f / slices;
            mVFactor = 1.0f / stacks;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        public void CreateBillboard(Vector3 position)
        {
            CreateBillboard(position, 1.0f, 1.0f, ColorEx.White, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        public void CreateBillboard(Vector3 position, float xScale)
        {
            CreateBillboard(position, xScale, 1.0f, ColorEx.White, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        public void CreateBillboard(Vector3 position, float xScale, float yScale)
        {
            CreateBillboard(position, xScale, yScale, ColorEx.White, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="color"></param>
        public void CreateBillboard(Vector3 position, float xScale, float yScale, ColorEx color)
        {
            CreateBillboard(position, xScale, yScale, color, 0, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="color"></param>
        /// <param name="texcoordIndexU"></param>
        public void CreateBillboard(Vector3 position, float xScale, float yScale, ColorEx color, short texcoordIndexU)
        {
            CreateBillboard(position, xScale, yScale, color, texcoordIndexU, 0);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="xScale"></param>
        /// <param name="yScale"></param>
        /// <param name="color"></param>
        /// <param name="texcoordIndexU"></param>
        /// <param name="texcoordsIndexV"></param>
        public void CreateBillboard(Vector3 position, float xScale, float yScale, ColorEx color, short texcoordIndexU, short texcoordsIndexV)
        {
            if (mRenderMethod == BillboardMethod.Accelerated)
            {
                StaticBillboard bb = new StaticBillboard();
                bb.Position = position;
                bb.XScale = xScale;
                bb.YScale = yScale;
                bb.TextCoordIndexU = texcoordIndexU;
                bb.TextCoordIndexV = texcoordsIndexV;

                uint packedColor = (uint)Root.Instance.RenderSystem.ConvertColor(color);
                bb.Color = packedColor;
                mBillboardBuffer.Add(bb);
            }
            else
            {
                Billboard bb = mFallbackSet.CreateBillboard(position);
                bb.SetDimensions(xScale, yScale);
                bb.TexcoordRect = new RectangleF(
                    texcoordIndexU * mUFactor,
                    texcoordsIndexV * mVFactor,
                    (texcoordIndexU + 1) * mUFactor,
                    (texcoordsIndexV + 1) * mVFactor);

                bb.Color = color;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="visibleDist"></param>
        /// <param name="invisibleDist"></param>
        /// <returns></returns>
        private Material GetFadeMaterial(float visibleDist, float invisibleDist)
        {
            string materialSignature = string.Empty;

            materialSignature += mEntityName + "|";
            materialSignature += visibleDist + "|";
            materialSignature += invisibleDist + "|";
            materialSignature += mMaterial.GetTechnique(0).GetPass(0).GetTextureUnitState(0).TextureScrollU + "|";
            materialSignature += mMaterial.GetTechnique(0).GetPass(0).GetTextureUnitState(0).TextureScrollV + "|";

            Material fadeMaterial = null;
            if (!mFadedMaterialMap.TryGetValue(materialSignature, out fadeMaterial))
            {
                //clone the material
                fadeMaterial = mMaterial.Clone(GetUniqueID("ImpostorFade"));

                //And apply the fade shader
                for (int t = 0; t < fadeMaterial.TechniqueCount; t++)
                {
                    Technique tech = fadeMaterial.GetTechnique(t);
                    for (int p = 0; p < tech.PassCount; p++)
                    {
                        Pass pass = tech.GetPass(p);
                        //Setup vertex program
                        pass.SetVertexProgram("SpriteFade_vp");
                        GpuProgramParameters gparams = pass.VertexProgramParameters;
                        gparams.SetNamedAutoConstant("worldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
                        gparams.SetNamedAutoConstant("uScroll", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("vScroll", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[0]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[1]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[2]", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("preRotatedQuad[3]", GpuProgramParameters.AutoConstantType.Custom, 0);

                        gparams.SetNamedAutoConstant("camPos", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);
                        gparams.SetNamedAutoConstant("fadeGap", GpuProgramParameters.AutoConstantType.Custom, 0);
                        gparams.SetNamedAutoConstant("invisibleDist", GpuProgramParameters.AutoConstantType.Custom, 0);

                        //Set fade ranges
                        gparams.SetNamedConstant("invisibleDist", invisibleDist);
                        gparams.SetNamedConstant("fadeGap", invisibleDist - visibleDist);

                        pass.SetSceneBlending(SceneBlendType.TransparentAlpha);

                    }
                }

                //Add it to the list so it can be reused later
                mFadedMaterialMap.Add(materialSignature, fadeMaterial);

            }

            return fadeMaterial;
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

    }
}
