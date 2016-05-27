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
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
namespace Axiom.Forests
{
    /// <summary>
    /// A PageLoader-derived object you can use with PagedGeometry to produce realistic grass.
    /// </summary>
    /// <remarks>
    /// Using a GrassLoader is simple - simply create an instance, attach it to your PagedGeometry object
    /// with PagedGeometry::setPageLoader(), and add your grass. Important: For best performance, it is
    /// recommended that you use GrassPage (included in GrassLoader.h) to display geometry loaded by GrassLoader.
    /// This page type is designed for best performance with this grass system. BatchPage
    /// will work, although performance will be reduced slightly, and ImpostorPage will run extremely slow.
    /// 
    /// To add grass, just call addLayer(). addLayer() returns a GrassLayer object pointer, which you should
    /// use to further configure your newly added grass. Properties like size, density, color, animation, etc.
    /// can be controlled through the GrassLayer class.
    /// </remarks>
    /// <note>
    /// By default, the GrassLoader doesn't know what shape your terrain so all grass will be placed at
    /// 0 height. To inform GrassLoader of the shape of your terrain, you must specify a height function
    /// that returns the height (y coordinate) of your terrain at the given x and z coordinates. See
    /// the TreeLoader2D::setHeightFunction() documentation for more information.
    /// </note>
    /// <warning>
    /// If you attempt to use Ogre's scene queries to get the terrain height,
    /// keep in mind that calculating the height of Ogre's built-in terrain this way can
    /// be VERY slow if not done properly, and may cause stuttering due to long paging delays.
    /// </warning>
    public class GrassLoader : PageLoader
    {

        /// <summary>
        /// 
        /// </summary>
        private List<GrassLayer> mLayerList = new List<GrassLayer>();
        /// <summary>
        /// Height data.
        /// </summary>
        private IHeightFunction mHeightFunction;
        private object mHeightFunctionUserData;
        private PagedGeometry mGeom;
        private RenderQueueGroupID mRenderQueue;
        private float mDensityFactor;

        private ITimer mWindTimer;
        private Vector3 mWindDir;
        private long mLastTime;
        private static long GUID = 0;

        /// <summary>
        /// eturns a list of added grass layers.
        /// </summary>
        public List<GrassLayer> LayerList
        {
            get { return mLayerList; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 WindDirection
        {
            set { mWindDir = value; }
            get { return mWindDir; }
        }

        /// <summary>
        /// 
        /// </summary>
        public float DensityFactor
        {
            set { mDensityFactor = value; }
            get { return mDensityFactor; }
        }
        /// <summary>
        /// 
        /// </summary>
        public RenderQueueGroupID RenderQueue
        {
            set { mRenderQueue = value; }
            get { return mRenderQueue; }
        }
        /// <summary>
        /// 
        /// </summary>
        public IHeightFunction HeightFunction
        {
            set { mHeightFunction = value; }
            get { return mHeightFunction; }
        }
        /// <summary>
        /// 
        /// </summary>
        public object HeightFunctionUserData
        {
            get { return mHeightFunctionUserData; }
            set { mHeightFunctionUserData = value; }
        }
        /// <summary>
        /// Creates a new GrassLoader object. 
        /// </summary>
        /// <param name="geom">The PagedGeometry object that this GrassLoader will be assigned to.</param>
        public GrassLoader(PagedGeometry geom)
        {
            mGeom = geom;
            mHeightFunctionUserData = null;
            mWindDir = Vector3.UnitX;
            mDensityFactor = 1.0f;
            mRenderQueue = RenderQueueGroupID.Six;
            mWindTimer = Root.Instance.Timer;
            mWindTimer.Reset();
            mLastTime = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="material"></param>
        /// <returns></returns>
        public GrassLayer AddLayer(string material)
        {
            GrassLayer layer = new GrassLayer(mGeom, this);
            layer.MaterialName = material;
            mLayerList.Add(layer);
            return layer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void DeleteLayer(ref GrassLayer layer)
        {
            mLayerList.Remove(layer);
            layer = null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void LoadPage(PageInfo page)
        {
             //Seed random number generator based on page indexes
            ushort xSeed = (ushort)(page.XIndex % 0xFFFF);
            ushort zSeed = (ushort)(page.ZIndex % 0xFFFF);
            uint seed = (uint)(xSeed << 16) | zSeed;
            seed = (uint)new Random((int)seed).Next();
            //Keep a list of a generated meshes
            List<Mesh> meshList = new List<Mesh>();
            page.UserData = (object)meshList;

            //Generate meshes
            foreach (GrassLayer it in mLayerList)
            {
                GrassLayer layer = it;
                //Calculate how much grass needs to be added
                float volume = page.Bounds.Width * page.Bounds.Height;
                int grassCount = (int)(layer.Density * DensityFactor * volume);
                //The vertex buffer can't be allocated until the exact number of polygons is known,
                //so the locations of all grasses in this page must be precalculated.

                //Precompute grass locations into an array of floats. A plain array is used for speed;
                //there's no need to use a dynamic sized array since a maximum size is known.
                float[] position = new float[grassCount * 2];
                IntPtr posBuff = Memory.PinObject(position);
                if (layer.DensityMap != null)
                {
                    if (layer.DensityMap.Filter == MapFilter.None)
                    {
                        grassCount = (int)layer.PopulateGrassListUnfilteredDM(page, posBuff, grassCount);
                    }
                    else if (layer.DensityMap.Filter == MapFilter.Bilinear)
                    {
                        grassCount = (int)layer.PopulateGrassListBilenearDM(page, posBuff, grassCount);
                    }
                }
                else
                {
                    grassCount = (int)layer.PopulateGrassListUniform(page, posBuff, grassCount);
                }

                //Don't build a mesh unless it contains something
                if (grassCount != 0)
                {
                    Mesh mesh = null;
                    switch (layer.RenderTechnique)
                    {
                        case GrassTechnique.Quad:
                            mesh = GenerateGrassQuad(page, layer, posBuff, grassCount);
                            break;
                        case GrassTechnique.CrossQuads:
                            mesh = GenerateGrassCrossQuads(page, layer, posBuff, grassCount);
                            break;
                        case GrassTechnique.Sprite:
                            mesh = GenerateGrassSprite(page, layer, posBuff, grassCount);
                            break;
                    }
                    Debug.Assert(mesh != null);
                    //Add the mesh to PagedGeometry
                    Entity entity = mGeom.Camera.SceneManager.CreateEntity(GetUniqueID(), mesh.Name);
                    entity.RenderQueueGroup = mRenderQueue;
                    entity.CastShadows = false;
                    AddEntity(entity, page.CenterPoint, Quaternion.Identity, Vector3.UnitScale);
                    mGeom.SceneManager.RemoveEntity(entity);
                    //Store the mesh pointer
                    meshList.Add(mesh);

                }

                //Delete the position list
                //Memory.UnpinObject(posBuff);
                position = null;
            }

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        public override void UnloadPage(PageInfo page)
        {
            //Unload meshes
            List<Mesh> meshList = (List<Mesh>)page.UserData;
            if (meshList == null)
                return;
            foreach (Mesh i in meshList)
            {
                Mesh mesh = i;
                MeshManager.Instance.Remove(mesh.Name);
            }
            meshList.Clear();
            meshList = null;
        }
        /// <summary>
        /// 
        /// </summary>
        public override void FrameUpdate()
        {
            long currentTime = mWindTimer.Milliseconds;
            long elapsedTime = currentTime - mLastTime;
            mLastTime = currentTime;
            float elapsed = elapsedTime / 1000.0f;

            //Update the vertex shader parameters
            foreach (GrassLayer it in mLayerList)
            {
                GrassLayer layer = it;
                layer.UpdateShaders();

                GpuProgramParameters gparams = layer.Material.GetTechnique(0).GetPass(0).VertexProgramParameters;
                if (layer.IsAnimationEnabled)
                {
                    //Increment animation frame
                    layer.WaveCount += elapsed * (float)(layer.SwaySpeed * System.Math.PI);
                    if (layer.WaveCount > System.Math.PI * 2)
                        layer.WaveCount -= (float)System.Math.PI * 2;

                    //Set vertex shader parameters
                    gparams.SetNamedConstant("time", layer.WaveCount);
                    gparams.SetNamedConstant("frequency", layer.SwayDistribution);

                    Vector3 direction = mWindDir * layer.SwayLength;
                    gparams.SetNamedConstant("direction", new Vector4(direction.x, direction.y, direction.z, 0));
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="layer"></param>
        /// <param name="grassPostions"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        private Mesh GenerateGrassQuad(PageInfo page, GrassLayer layer, IntPtr grassPostions, int grassCount)
        {
            //Calculate the number of quads to be added
            int quadCount = grassCount;

            //Create manual mesh to store grass quads
            Mesh mesh = (Mesh)MeshManager.Instance.CreateManual(GetUniqueID(), ResourceGroupManager.DefaultResourceGroupName, null);
            SubMesh subMesh = mesh.CreateSubMesh();
            subMesh.useSharedVertices = false;

            //Setup vertex format information
            subMesh.vertexData = new VertexData();
            subMesh.vertexData.vertexStart = 0;
            subMesh.vertexData.vertexCount = 4 * quadCount;

            VertexDeclaration dcl = subMesh.vertexData.vertexDeclaration;
            int offset = 0;
            dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            dcl.AddElement(0, offset, VertexElementType.Color, VertexElementSemantic.Diffuse);
            offset += VertexElement.GetTypeSize(VertexElementType.Color);
            dcl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);

            //Populate a new vertex buffer with grass
            HardwareVertexBuffer vbuf = HardwareBufferManager.Instance.CreateVertexBuffer(
                /*offset*/dcl, subMesh.vertexData.vertexCount, BufferUsage.DynamicWriteOnly, false);
            unsafe
            {
                float* pReal = (float*)vbuf.Lock(BufferLocking.Discard);
                //Calculate size variance
                float rndWidth = layer.mMaxWidth - layer.mMinWidth;
                float rndHeight = layer.mMaxHeight - layer.mMinHeight;
                float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
                float* posPtr = (float*)grassPostions; //Position array "iterator"
                for (int i = 0; i < grassCount; i++)
                {
                    //Get the x and z positions from the position array
                    float x = *posPtr++;
                    float z = *posPtr++;

                    //Get the color at the grass position
                    uint color = 0;
                    if (layer.ColorMap != null)
                        color = layer.ColorMap.GetColorAt(x, z);
                    else
                        color = 0xFFFFFFFF;

                    //Calculate size
                    float rnd = Axiom.Math.Utility.UnitRandom();//The same rnd value is used for width and height to maintain aspect ratio
                    float halfScaleX = (layer.mMinWidth + rndWidth * rnd) * 0.5f;
                    float scaleY = (layer.mMinWidth + rndHeight * rnd);

                    //Calculate rotation
                    float angle = Axiom.Math.Utility.RangeRandom(0, Axiom.Math.Utility.TWO_PI);
                    float xTrans = Axiom.Math.Utility.Cos(angle) * halfScaleX;
                    float zTrans = Axiom.Math.Utility.Sin(angle) * halfScaleX;

                    //Calculate heights and edge positions
                    float x1 = x - xTrans, z1 = z - zTrans;
                    float x2 = x + xTrans, z2 = z + zTrans;

                    float y1, y2;
                    if (mHeightFunction != null)
                    {
                        y1 = mHeightFunction.GetHeightAt(x1, z1, mHeightFunctionUserData);
                        y2 = mHeightFunction.GetHeightAt(x2, z2, mHeightFunctionUserData);
                    }
                    else
                    {
                        y1 = 0;
                        y2 = 0;
                    }
                    //Add vertices
                    *pReal++ = (x1 - page.CenterPoint.x); *pReal++ = (y1 + scaleY); *pReal++ = (z1 - page.CenterPoint.z);//pos
                    *((uint*)pReal++) = color;       //color
                    *pReal++ = 0; *pReal++ = 0;     //uv

                    *pReal++ = (x2 - page.CenterPoint.x); *pReal++ = (y2 + scaleY); *pReal++ = (z2 - page.CenterPoint.z);	//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 0;								//uv

                    *pReal++ = (x1 - page.CenterPoint.x); *pReal++ = (y1); *pReal++ = (z1 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 0; *pReal++ = 1;								//uv

                    *pReal++ = (x2 - page.CenterPoint.x); *pReal++ = (y2); *pReal++ = (z2 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 1;								//uv

                    //Update bounds
                    if (y1 < minY) minY = y1;
                    if (y2 < minY) minY = y2;
                    if (y1 + scaleY > maxY) maxY = y1 + scaleY;
                    if (y2 + scaleY > maxY) maxY = y2 + scaleY;
                }

                vbuf.Unlock();
                subMesh.vertexData.vertexBufferBinding.SetBinding(0, vbuf);

                //Populate index buffer
                subMesh.indexData.indexStart = 0;
                subMesh.indexData.indexCount = 6 * quadCount;
                subMesh.indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                    IndexType.Size16, subMesh.indexData.indexCount, BufferUsage.DynamicWriteOnly);
                ushort* pI = (ushort*)subMesh.indexData.indexBuffer.Lock(BufferLocking.Discard);
                for(ushort i = 0; i < quadCount;i++)
                {
                    ushort ofset = (ushort)(i * 4);
                    *pI++ = (ushort)(0 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(1 + ofset);

                    *pI++ = (ushort)(1 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(3 + ofset);
                }

                subMesh.indexData.indexBuffer.Unlock();
                //Finish up mesh
                AxisAlignedBox bounds = new AxisAlignedBox(
                    new Vector3(page.Bounds.Left - page.CenterPoint.x, minY, page.Bounds.Top - page.CenterPoint.z),
                    new Vector3(page.Bounds.Right - page.CenterPoint.x, maxY, page.Bounds.Bottom - page.CenterPoint.z));

                mesh.BoundingBox = bounds;
                Vector3 tmp = bounds.Maximum - bounds.Minimum;
                mesh.BoundingSphereRadius = tmp.Length * 0.5f;
                mesh.Load();
                //Apply grass material to mesh
                subMesh.MaterialName = layer.Material.Name;

                //Return the mesh
                return mesh;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="layer"></param>
        /// <param name="grassPostions"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        private Mesh GenerateGrassCrossQuads(PageInfo page, GrassLayer layer, IntPtr grassPostions, int grassCount)
        {
            //Calculate the number of quads to be added
            int quadCount = grassCount * 2;

            //Create manual mesh to store grass quads
            Mesh mesh = (Mesh)MeshManager.Instance.CreateManual(GetUniqueID(), ResourceGroupManager.DefaultResourceGroupName, null);
            SubMesh subMesh = mesh.CreateSubMesh();
            subMesh.useSharedVertices = false;

            //Setup vertex format information
            subMesh.vertexData = new VertexData();
            subMesh.vertexData.vertexStart = 0;
            subMesh.vertexData.vertexCount = 4 * quadCount;

            VertexDeclaration dcl = subMesh.vertexData.vertexDeclaration;
            int offset = 0;
            dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            dcl.AddElement(0, offset, VertexElementType.Color, VertexElementSemantic.Diffuse);
            offset += VertexElement.GetTypeSize(VertexElementType.Color);
            dcl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);

            //Populate a new vertex buffer with grass
            HardwareVertexBuffer vbuf = HardwareBufferManager.Instance.CreateVertexBuffer(
                /*offset*/dcl, subMesh.vertexData.vertexCount, BufferUsage.DynamicWriteOnly, false);
            unsafe
            {
                float* pReal = (float*)vbuf.Lock(BufferLocking.Discard);
                //Calculate size variance
                float rndWidth = layer.mMaxWidth - layer.mMinWidth;
                float rndHeight = layer.mMaxHeight - layer.mMinHeight;
                float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
                float* posPtr = (float*)grassPostions; //Position array "iterator"
                for (int i = 0; i < grassCount; i++)
                {
                    //Get the x and z positions from the position array
                    float x = *posPtr++;
                    float z = *posPtr++;


                    //Get the color at the grass position
                    uint color = 0;
                    if (layer.ColorMap != null)
                        color = layer.ColorMap.GetColorAt(x, z);
                    else
                        color = 0xFFFFFFFF;

                    //Calculate size
                    float rnd = Axiom.Math.Utility.UnitRandom();//The same rnd value is used for width and height to maintain aspect ratio
                    float halfXScale = (layer.mMinWidth + rndWidth * rnd) * 0.5f;
                    float scaleY = (layer.mMinWidth + rndHeight * rnd);

                    //Calculate rotation
                    float angle = Axiom.Math.Utility.RangeRandom(0, Axiom.Math.Utility.TWO_PI);
                    float xTrans = Axiom.Math.Utility.Cos(angle) * halfXScale;
                    float zTrans = Axiom.Math.Utility.Sin(angle) * halfXScale;


                    //Calculate heights and edge positions
                    float x1 = x - xTrans, z1 = z - zTrans;
                    float x2 = x + xTrans, z2 = z + zTrans;

                    float y1, y2;
                    if (mHeightFunction != null)
                    {
                        y1 = mHeightFunction.GetHeightAt(x1, z1, mHeightFunctionUserData);
                        y2 = mHeightFunction.GetHeightAt(x2, z2, mHeightFunctionUserData);
                    }
                    else
                    {
                        y1 = 0;
                        y2 = 0;
                    }

                    //Add vertices
                    *pReal++ = (x1 - page.CenterPoint.x); *pReal++ = (y1 + scaleY); *pReal++ = (z1 - page.CenterPoint.z);	//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 0; *pReal++ = 0;								//uv

                    *pReal++ = (x2 - page.CenterPoint.x); *pReal++ = (y2 + scaleY); *pReal++ = (z2 - page.CenterPoint.z);	//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 0;								//uv

                    *pReal++ = (x1 - page.CenterPoint.x); *pReal++ = (y1); *pReal++ = (z1 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 0; *pReal++ = 1;								//uv

                    *pReal++ = (x2 - page.CenterPoint.x); *pReal++ = (y2); *pReal++ = (z2 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 1;								//uv

                    //Update bounds
                    if (y1 < minY) minY = y1;
                    if (y2 < minY) minY = y2;
                    if (y1 + scaleY > maxY) maxY = y1 + scaleY;
                    if (y2 + scaleY > maxY) maxY = y2 + scaleY;

                    //Calculate heights and edge positions
                    float x3 = x + zTrans, z3 = z - xTrans;
                    float x4 = x - zTrans, z4 = z + xTrans;

                    float y3, y4;
                    if (mHeightFunction != null)
                    {
                        y3 = mHeightFunction.GetHeightAt(x3, z3, mHeightFunctionUserData);
                        y4 = mHeightFunction.GetHeightAt(x4, z4, mHeightFunctionUserData);
                    }
                    else
                    {
                        y3 = 0;
                        y4 = 0;
                    }

                    //Add vertices
                    *pReal++ = (x3 - page.CenterPoint.x); *pReal++ = (y3 + scaleY); *pReal++ = (z3 - page.CenterPoint.z);	//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 0; *pReal++ = 0;								//uv

                    *pReal++ = (x4 - page.CenterPoint.x); *pReal++ = (y4 + scaleY); *pReal++ = (z4 - page.CenterPoint.z);	//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 0;								//uv

                    *pReal++ = (x3 - page.CenterPoint.x); *pReal++ = (y3); *pReal++ = (z3 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 0; *pReal++ = 1;								//uv

                    *pReal++ = (x4 - page.CenterPoint.x); *pReal++ = (y4); *pReal++ = (z4 - page.CenterPoint.z);			//pos
                    *((uint*)pReal++) = color;							//color
                    *pReal++ = 1; *pReal++ = 1;								//uv

                    //Update bounds
                    if (y3 < minY) minY = y1;
                    if (y4 < minY) minY = y2;
                    if (y3 + scaleY > maxY) maxY = y3 + scaleY;
                    if (y4 + scaleY > maxY) maxY = y4 + scaleY;
                }

                vbuf.Unlock();
                subMesh.vertexData.vertexBufferBinding.SetBinding(0, vbuf);

                //Populate index buffer
                subMesh.indexData.indexStart = 0;
                subMesh.indexData.indexCount = 6 * quadCount;
                subMesh.indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                    IndexType.Size16, subMesh.indexData.indexCount, BufferUsage.DynamicWriteOnly);
                ushort* pI = (ushort*)subMesh.indexData.indexBuffer.Lock(BufferLocking.Discard);
                for (ushort i = 0; i < quadCount; i++)
                {
                    ushort ofset = (ushort)(i * 4);
                    *pI++ = (ushort)(0 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(1 + ofset);

                    *pI++ = (ushort)(1 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(3 + ofset);
                }

                subMesh.indexData.indexBuffer.Unlock();
                //Finish up mesh
                AxisAlignedBox bounds = new AxisAlignedBox(
                    new Vector3(page.Bounds.Left - page.CenterPoint.x, minY, page.Bounds.Top - page.CenterPoint.z),
                    new Vector3(page.Bounds.Right - page.CenterPoint.x, maxY, page.Bounds.Bottom - page.CenterPoint.z));

                mesh.BoundingBox = bounds;
                Vector3 tmp = bounds.Maximum - bounds.Minimum;
                mesh.BoundingSphereRadius = tmp.Length * 0.5f;
                mesh.Load();
                //Apply grass material to mesh
                subMesh.MaterialName = layer.Material.Name;

                //Return the mesh
                return mesh;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        /// <param name="layer"></param>
        /// <param name="grassPostions"></param>
        /// <param name="grassCount"></param>
        /// <returns></returns>
        private Mesh GenerateGrassSprite(PageInfo page, GrassLayer layer, IntPtr grassPostions, int grassCount)
        {
            //Calculate the number of quads to be added
            int quadCount = grassCount;

            //Create manual mesh to store grass quads
            Mesh mesh = (Mesh)MeshManager.Instance.CreateManual(GetUniqueID(), ResourceGroupManager.DefaultResourceGroupName, null);
            SubMesh subMesh = mesh.CreateSubMesh();
            subMesh.useSharedVertices = false;

            //Setup vertex format information
            subMesh.vertexData = new VertexData();
            subMesh.vertexData.vertexStart = 0;
            subMesh.vertexData.vertexCount = 4 * quadCount;

            VertexDeclaration dcl = subMesh.vertexData.vertexDeclaration;
            int offset = 0;
            dcl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            dcl.AddElement(0, offset, VertexElementType.Float4, VertexElementSemantic.Normal);
            offset += VertexElement.GetTypeSize(VertexElementType.Float4);
            dcl.AddElement(0, offset, VertexElementType.Color, VertexElementSemantic.Diffuse);
            offset += VertexElement.GetTypeSize(VertexElementType.Color);
            dcl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);

            //Populate a new vertex buffer with grass
            HardwareVertexBuffer vbuf = HardwareBufferManager.Instance.CreateVertexBuffer(
                /*offset*/dcl, subMesh.vertexData.vertexCount, BufferUsage.DynamicWriteOnly, false);
            unsafe
            {
                float* pReal = (float*)vbuf.Lock(BufferLocking.Discard);
                //Calculate size variance
                float rndWidth = layer.mMaxWidth - layer.mMinWidth;
                float rndHeight = layer.mMaxHeight - layer.mMinHeight;
                float minY = float.PositiveInfinity, maxY = float.NegativeInfinity;
                float* posPtr = (float*)grassPostions; //Position array "iterator"
                for (int i = 0; i < grassCount; i++)
                {
                    //Get the x and z positions from the position array
                    float x = *posPtr++;
                    float z = *posPtr++;


                    //Calculate height
                    float y = 0;
                    if (mHeightFunction != null)
                    {
                        y = mHeightFunction.GetHeightAt(x, z, mHeightFunctionUserData);
                    }
                    else
                    {
                        y = 0;
                    }

                    float x1 = (x - page.CenterPoint.x);
                    float z1 = (z - page.CenterPoint.z);

                    //Get the color at the grass position
                    uint color = 0;
                    if (layer.ColorMap != null)
                        color = layer.ColorMap.GetColorAt(x, z);
                    else
                        color = 0xFFFFFFFF;

                    //Calculate size
                    float rnd = Axiom.Math.Utility.UnitRandom();//The same rnd value is used for width and height to maintain aspect ratio
                    float halfXScale = (layer.mMinWidth + rndWidth * rnd) * 0.5f;
                    float scaleY = (layer.mMinWidth + rndHeight * rnd);

                    //Randomly mirror grass textures
		            float uvLeft, uvRight;
                    if (Axiom.Math.Utility.UnitRandom() > 0.5f)
                    {
                        uvLeft = 0;
                        uvRight = 1;
                    }
                    else
                    {
                        uvLeft = 1;
                        uvRight = 0;
                    }

                    //Add vertices
                    *pReal++ = x1; *pReal++ = y; *pReal++ = z1;					//center position
                    *pReal++ = -halfXScale; *pReal++ = scaleY; *pReal++ = 0; *pReal++ = 0;	//normal (used to store relative corner positions)
                    *((uint*)pReal++) = color;								//color
                    *pReal++ = uvLeft; *pReal++ = 0;							//uv

                    *pReal++ = x1; *pReal++ = y; *pReal++ = z1;					//center position
                    *pReal++ = +halfXScale; *pReal++ = scaleY; *pReal++ = 0; *pReal++ = 0;	//normal (used to store relative corner positions)
                    *((uint*)pReal++) = color;								//color
                    *pReal++ = uvRight; *pReal++ = 0;							//uv

                    *pReal++ = x1; *pReal++ = y; *pReal++ = z1;					//center position
                    *pReal++ = -halfXScale; *pReal++ = 0.0f; *pReal++ = 0; *pReal++ = 0;		//normal (used to store relative corner positions)
                    *((uint*)pReal++) = color;								//color
                    *pReal++ = uvLeft; *pReal++ = 1;							//uv

                    *pReal++ = x1; *pReal++ = y; *pReal++ = z1;					//center position
                    *pReal++ = +halfXScale; *pReal++ = 0.0f; *pReal++ = 0; *pReal++ = 0;		//normal (used to store relative corner positions)
                    *((uint*)pReal++) = color;								//color
                    *pReal++ = uvRight; *pReal++ = 1;							//uv

                    //Update bounds
                    if (y < minY) minY = y;
                    if (y + scaleY > maxY) maxY = y + scaleY;
                }

                vbuf.Unlock();
                subMesh.vertexData.vertexBufferBinding.SetBinding(0, vbuf);

                //Populate index buffer
                subMesh.indexData.indexStart = 0;
                subMesh.indexData.indexCount = 6 * quadCount;
                subMesh.indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                    IndexType.Size16, subMesh.indexData.indexCount, BufferUsage.DynamicWriteOnly);
                ushort* pI = (ushort*)subMesh.indexData.indexBuffer.Lock(BufferLocking.Discard);
                for (ushort i = 0; i < quadCount; i++)
                {
                    ushort ofset = (ushort)(i * 4);
                    *pI++ = (ushort)(0 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(1 + ofset);

                    *pI++ = (ushort)(1 + ofset);
                    *pI++ = (ushort)(2 + ofset);
                    *pI++ = (ushort)(3 + ofset);
                }

                subMesh.indexData.indexBuffer.Unlock();
                //Finish up mesh
                AxisAlignedBox bounds = new AxisAlignedBox(
                    new Vector3(page.Bounds.Left - page.CenterPoint.x, minY, page.Bounds.Top - page.CenterPoint.z),
                    new Vector3(page.Bounds.Right - page.CenterPoint.x, maxY, page.Bounds.Bottom - page.CenterPoint.z));

                mesh.BoundingBox = bounds;
                Vector3 tmp = bounds.Maximum - bounds.Minimum;
                mesh.BoundingSphereRadius = tmp.Length * 0.5f;
                mesh.Load();
                //Apply grass material to mesh
                subMesh.MaterialName = layer.Material.Name;

                //Return the mesh
                return mesh;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static string GetUniqueID()
        {
            return "GrassLDR" + ++GUID;
        }
        
    }
}
