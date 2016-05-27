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
    public class BatchedGeometry : MovableObject
    {
		static int name = 0;
        #region - fields -
        private Vector3 mCenter;
        private AxisAlignedBox mBounds;
        private bool mBoundsUndefinded;
        private float mRadius;
        private SceneManager mSceneMgr;
        private SceneNode mSceneNode;
        private SceneNode mParentSceneNode;
        private float mMinDistanceSquared;
        private bool mWithinFarDistance;
        private bool mBuild;
        private Dictionary<string, SubBatch> mSubBatches = new Dictionary<string, SubBatch>();
        #endregion

        #region - properties -
        /// <summary>
        /// 
        /// </summary>
        public float MinDistanceSquared
        {
            get { return mMinDistanceSquared; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector3 Center
        {
            get { return mCenter; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, SubBatch> SubBatches
        {
            get { return mSubBatches; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SceneNode SceneNode
        {
            get { return mSceneNode; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override bool IsVisible
        {
            get
            {
                return this.isVisible && mWithinFarDistance;
            }
            set
            {
                base.IsVisible = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public override AxisAlignedBox BoundingBox
        {
            get { return mBounds; }
        }
        /// <summary>
        /// 
        /// </summary>
        public override float BoundingRadius
        {
            get { return mRadius; }
        }
		///// <summary>
		///// 
		///// </summary>
		//public override string MovableType
		//{
		//    get
		//    {
		//        return "BatchedGeometry";
		//    }
		//    //set
		//    //{
		//    //    base.MovableType = value;
		//    //}
		//}
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mgr"></param>
        /// <param name="rootNode"></param>
        public BatchedGeometry(SceneManager mgr, SceneNode rootNode)
			:base("BatchedGeom" + name++.ToString())
        {
            mWithinFarDistance = false;
            mMinDistanceSquared = 0;
            mSceneNode = null;
            mSceneMgr = mgr;
            mBuild = false;
            mBounds = new AxisAlignedBox(new Vector3(0, 0, 0), new Vector3(0, 0, 0));
            mBoundsUndefinded = true;
            mParentSceneNode = rootNode;

            Clear();

        }

        #region - AddEntity Overloads -
        public void AddEntity(Entity ent, Vector3 position)
        {
            AddEntity(ent, position, Quaternion.Identity, Vector3.UnitScale, ColorEx.White);
        }
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation)
        {
            AddEntity(ent, position, orientation, Vector3.UnitScale, ColorEx.White);
        }
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation, Vector3 scale)
        {
            AddEntity(ent, position, orientation, scale, ColorEx.White);
        }
       
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <param name="position"></param>
        /// <param name="orientation"></param>
        /// <param name="scale"></param>
        /// <param name="color"></param>
        public void AddEntity(Entity ent, Vector3 position, Quaternion orientation, Vector3 scale, ColorEx color)
        {
            Mesh mesh = ent.Mesh;
            if(mesh.SharedVertexData != null)
                throw new Exception("Shared vertex data not allowed");

            //For each subentity
            for (int i = 0; i < ent.SubEntityCount; i++)
            {
                //get the subentity
                SubEntity subEntity = ent.GetSubEntity(i);
                SubMesh subMesh = subEntity.SubMesh;

                //Generate a format string that uniquely identifies this material & vertex/index format
                if (subMesh.vertexData == null)
                    throw new Exception("Submesh vertex data not found!");

                string formatStr = GetFormatString(subEntity);
                //If a batch using an identical format exists...
                SubBatch batch = null;
                if (!mSubBatches.TryGetValue(formatStr, out batch))
                {
                    batch = new SubBatch(this, subEntity);
                    mSubBatches.Add(formatStr, batch);
                }
                //Now add the submesh to the compatible batch
                batch.AddSubEntity(subEntity, position, orientation, scale, color);
            }//end for

            //Update bounding box
            Matrix4 mat = Matrix4.FromMatrix3(orientation.ToRotationMatrix());
            mat.Scale = scale;
            AxisAlignedBox entBounds = ent.BoundingBox;
            entBounds.Transform(mat);
            if (mBoundsUndefinded)
            {
                mBounds.Minimum = entBounds.Minimum + position;
                mBounds.Maximum = entBounds.Maximum + position;
                mBoundsUndefinded = false;
            }
            else
            {
                Vector3 min = mBounds.Minimum;
                Vector3 max = mBounds.Maximum;

                min.Floor(entBounds.Minimum + position);
                max.Ceil(entBounds.Maximum + position);
                mBounds.Minimum = min;
                mBounds.Maximum = max;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void Build()
        {
            ///Make sure the batch hasn't already been build
            if (mBuild)
                throw new Exception("Invalid call to build() - geometry is allready batched (call clear() first)");

            if (mSubBatches.Count != 0)
            {
                //Finish bounds information
                mCenter = mBounds.Center;
                mBounds.Minimum = mBounds.Minimum - mCenter;
                mBounds.Maximum = mBounds.Maximum - mCenter;

                mRadius = mBounds.Maximum.Length;

                //create scene node
                mSceneNode = mParentSceneNode.CreateChildSceneNode(mCenter);

                //build each batch
                foreach (SubBatch batch in mSubBatches.Values)
                    batch.Build();

                //Attach the batch to the scene node
                mSceneNode.AttachObject(this);

                //Debug
                //mSceneNode.ShowBoundingBox(true);

                mBuild = true;

            }

        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            if(mSceneNode != null)
            {
                mSceneNode.RemoveAllChildren();
                mSceneMgr.DestroySceneNode(mSceneNode);
                mSceneNode = null;
            }

            mBoundsUndefinded = true;
            mCenter = Vector3.Zero;
            mRadius = 0;

            mSubBatches.Clear();
            mBuild = false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="globalVec"></param>
        /// <returns></returns>
        public Vector3 ConvertToLocal(Vector3 globalVec)
        {
            Debug.Assert(mParentSceneNode != null, "Parent scenenode can not be null!");

            //Convert from the given global position to the local coordinate system of the parent scene node.
            return (mParentSceneNode.Orientation.Inverse() * globalVec);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        public override void NotifyCurrentCamera(Camera camera)
        {
#warning add MovableObject.RenderingDistance
            if (false)
            {
                mWithinFarDistance = true;
            }
            else
            {
                //Calculate camera distance
                Vector3 camVec = ConvertToLocal(camera.DerivedPosition) - mCenter;
                float centerDistanceSquared = camVec.LengthSquared;
                mMinDistanceSquared = System.Math.Max(0.0f, centerDistanceSquared - (mRadius * mRadius));
                //Note: centerDistanceSquared measures the distance between the camera and the center of the GeomBatch,
                //while minDistanceSquared measures the closest distance between the camera and the closest edge of the
                //geometry's bounding sphere.

                //Determine whether the BatchedGeometry is within the far rendering distance
#warning add MovableObject.RenderingDistance
                mWithinFarDistance = mMinDistanceSquared <= Axiom.Math.Utility.Sqr(0);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        public override void UpdateRenderQueue(RenderQueue queue)
        {
            //If visible...
            if (IsVisible)
            {
                //Ask each batch to add itself to the render queue if appropriate
                foreach (SubBatch batch in mSubBatches.Values)
                    batch.AddSelfToRenderQueue(queue, base.renderQueueID);//base.RenderQueueGroup
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ent"></param>
        /// <returns></returns>
        private string GetFormatString(SubEntity ent)
        {
            string str = string.Empty;

            str += ent.MaterialName + "|";
            str += ent.SubMesh.indexData.indexBuffer.Type  + "|";

            List<VertexElement> elemList = ent.SubMesh.vertexData.vertexDeclaration.Elements;
            foreach (VertexElement element in elemList)
            {
                str += element.Source + "|";
                str += element.Semantic + "|";
                str += element.Type + "|";
            }

            return str;
        }
        #region - subBatch -
        /// <summary>
        /// 
        /// </summary>
        public class SubBatch : SimpleRenderable
        {
            /// <summary>
            /// A structure defining the desired position/orientation/scale of a batched mesh.
            /// The SubMesh is not specified since that can be determined by which MeshQueue this belongs to.
            /// </summary>
            public struct QueuedMesh
            {
                public SubMesh Mesh;
                public Vector3 Postion;
                public Quaternion Orientation;
                public Vector3 Scale;
                public ColorEx Color;
            }

            #region - fields -
            private bool mBuild;
            private bool mRequireVertexColors;
            private SubMesh mMeshType;
            private BatchedGeometry mParent;
            private Material mMaterial;
            private Technique mBestTechnique;
            private List<QueuedMesh> mMeshQueue = new List<QueuedMesh>();
            #endregion
            /// <summary>
            /// 
            /// </summary>
            public override bool CastShadows
            {
                get
                {
                    return mParent.CastShadows;
                }
                set
                {
                    base.CastShadows = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override float BoundingRadius
            {
                get { return mParent.BoundingRadius; }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Material Material
            {
                get
                {
                    return base.Material;
                }
                set
                {
                    base.Material = value;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Quaternion WorldOrientation
            {
                get
                {
                    return mParent.SceneNode.DerivedOrientation;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public override Vector3 WorldPosition
            {
                get
                {
                    return mParent.SceneNode.DerivedPosition;
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public string MaterialName
            {
                get { return Material.Name; }
            }
            public override void GetWorldTransforms(Matrix4[] matrices)
            {
                matrices[0] = mParent.ParentNodeFullTransform;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="ent"></param>
            public SubBatch(BatchedGeometry parent, SubEntity ent)
            {
                mMeshType = ent.SubMesh;
                mParent = parent;
                mBuild = false;
                mRequireVertexColors = false;
                // Material must always exist
                Material origMat = (Material)MaterialManager.Instance.GetByName(ent.MaterialName);
                if (origMat != null)
                {
                    material = (Material)MaterialManager.Instance.GetByName(GetMaterialClone(origMat).Name);
                }
                else
                {
                    Tuple<Resource, bool> result = MaterialManager.Instance.CreateOrRetrieve("PagedGeometry_Batched_Material", "General");
                    if (result.First == null)
                        throw new Exception("BatchedGeometry failed to create a material for entity with invalid material.");

                    material = (Material)result.First;
                }

                //Setup vertex/index data structure
                vertexData = mMeshType.vertexData.Clone(false);
                indexData = mMeshType.indexData.Clone(false);

                //Remove blend weights from vertex format
                VertexElement blendIndices = vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.BlendIndices);
                VertexElement blendWeights = vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.BlendWeights);

                if (blendIndices != null && blendWeights != null)
                {
                    Debug.Assert(blendIndices.Source == blendWeights.Source, "Blend indices and weights should be in the same buffer");
                    Debug.Assert(blendIndices.Size + blendWeights.Size == vertexData.vertexBufferBinding.GetBuffer(blendIndices.Source).VertexSize,
                        "Blend indices and blend buffers should have buffer to themselves!");

                    //Remove the blend weights
                    vertexData.vertexBufferBinding.UnsetBinding(blendIndices.Source);
                    vertexData.vertexDeclaration.RemoveElement(VertexElementSemantic.BlendIndices);
                    vertexData.vertexDeclaration.RemoveElement(VertexElementSemantic.BlendWeights);
                }

                //Reset vertex/index count
                vertexData.vertexStart = 0;
                vertexData.vertexCount = 0;
                indexData.indexStart = 0;
                indexData.indexCount = 0;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="queue"></param>
            /// <param name="group"></param>
            public void AddSelfToRenderQueue(RenderQueue queue, RenderQueueGroupID group)
            {
                if (mBuild)
                {
                    //Update material technique based on camera distance
                    Debug.Assert(material != null);
#warning missing function getLodIndexSquaredDepth
                    mBestTechnique = material.GetBestTechnique(material.GetLodIndex(mParent.MinDistanceSquared));

                    //Add to render queue
                    queue.AddRenderable(this, group);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ent"></param>
            /// <param name="position"></param>
            /// <param name="orientation"></param>
            /// <param name="scale"></param>
            public void AddSubEntity(SubEntity ent, Vector3 position, Quaternion orientation, Vector3 scale)
            {
                AddSubEntity(ent, position, orientation, scale, ColorEx.White);
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="ent"></param>
            /// <param name="position"></param>
            /// <param name="orientation"></param>
            /// <param name="scale"></param>
            /// <param name="color"></param>
            public void AddSubEntity(SubEntity ent, Vector3 position, Quaternion orientation, Vector3 scale, ColorEx color)
            {
                Debug.Assert(!mBuild);
                //Add this submesh to the queue
                QueuedMesh newMesh = new QueuedMesh();
                newMesh.Mesh = ent.SubMesh;
                newMesh.Postion = position;
                newMesh.Orientation = orientation;
                newMesh.Scale = scale;
                newMesh.Color = color;

                if (newMesh.Color != ColorEx.White)
                {
                    mRequireVertexColors = true;
                    //VertexElementType format = Root.Instance.RenderSystem.Colo
                    throw new NotSupportedException("Please use ColorEx.White for now!");
                }

                mMeshQueue.Add(newMesh);
                //Increment the vertex/index count so the buffers will have room for this mesh
                vertexData.vertexCount += ent.SubMesh.vertexData.vertexCount;
                indexData.indexCount += ent.SubMesh.indexData.indexCount;
            }
            /// <summary>
            /// 
            /// </summary>
            public void Build()
            {
                Debug.Assert(!mBuild);

                //Misc. setup
                Vector3 batchCenter = mParent.Center;

                IndexType srcIndexType = mMeshType.indexData.indexBuffer.Type;
                IndexType destIndexType;

                if (vertexData.vertexCount > 0xFFFF || srcIndexType == IndexType.Size32)
                    destIndexType = IndexType.Size32;
                else
                    destIndexType = IndexType.Size16;

                //Allocate the index buffer
                indexData.indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                    destIndexType, indexData.indexCount, BufferUsage.StaticWriteOnly);

                unsafe
                {
                    uint* indexBuffer32 = (uint*)IntPtr.Zero;
                    ushort* indexBuffer16 = (ushort*)IntPtr.Zero;
                    if (destIndexType == IndexType.Size32)
                        indexBuffer32 = (uint*)indexData.indexBuffer.Lock(BufferLocking.Discard);
                    else
                        indexBuffer16 = (ushort*)indexData.indexBuffer.Lock(BufferLocking.Discard);

                    //Allocate & lock the vertex buffers
                    List<IntPtr> vertexBuffers = new List<IntPtr>();
                    List<List<VertexElement>> vertexBufferElements = new List<List<VertexElement>>();

                    VertexBufferBinding vertBinding = vertexData.vertexBufferBinding;
                    VertexDeclaration vertDecl = vertexData.vertexDeclaration;

                    for (short i = 0; i < vertBinding.BindingCount; i++)
                    {
                        HardwareVertexBuffer buffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                            vertDecl/*.GetVertexSize(i)*/, vertexData.vertexCount, BufferUsage.StaticWriteOnly);
                        vertBinding.SetBinding(i, buffer);

                        vertexBuffers.Add(buffer.Lock(BufferLocking.Discard));
                        vertexBufferElements.Add(vertDecl.FindElementBySource(i));
                    }

                    //If no vertex colors are used, make sure the final batch includes them (so the shade values work)
                    if (mRequireVertexColors)
                    {
                        if (vertexData.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.Diffuse) == null)
                        {
                            short i = (short)vertBinding.BindingCount;
                            vertDecl.AddElement(i, 0, VertexElementType.Color, VertexElementSemantic.Diffuse);

                            HardwareVertexBuffer buffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                                vertDecl/*.GetVertexSize(i)*/, vertexData.vertexCount, BufferUsage.StaticWriteOnly);

                            vertexBuffers.Add(buffer.Lock(BufferLocking.Discard));
                            vertexBufferElements.Add(vertDecl.FindElementBySource(i));
                        }
                        Pass p = material.GetTechnique(0).GetPass(0);
						p.VertexColorTracking = TrackVertexColor.Ambient;
#warning missing function p->setVertexColourTracking(TVC_AMBIENT);
                    }
                    //For each queued mesh...
                    int indexOffset = 0;
                    foreach (QueuedMesh it in mMeshQueue)
                    {
                        QueuedMesh queuedMesh = it;
                        IndexData sourceIndexData = queuedMesh.Mesh.indexData;
                        VertexData sourceVerterxData = queuedMesh.Mesh.vertexData;

                        //Copy mesh vertex data into the vertex buffer
                        VertexBufferBinding sourceBinds = sourceVerterxData.vertexBufferBinding;
                        VertexBufferBinding destBinds = vertexData.vertexBufferBinding;
                        for (short i = 0; i < destBinds.BindingCount; i++)
                        {
                            if (i < sourceBinds.BindingCount)
                            {
                                //Lock the input buffer
                                HardwareVertexBuffer sourceBuffer = sourceBinds.GetBuffer(i);
                                byte* sourceBase = (byte*)(sourceBuffer.Lock(BufferLocking.ReadOnly));

                                //Get the locked output buffer
                                byte* destBase = (byte*)vertexBuffers[i];
                                /* pReal = (float*)(vertex + posElem.Offset);
                        Vector3 pt = new Vector3(pReal[0], pReal[1], pReal[2]);
                        vertices[current_offset + j] = (orientation * (pt * scale)) + position;
                                 */
                                //Copy vertices
                                float* sourcePtr;
                                float* destPtr;
                                for (int v = 0; v < sourceVerterxData.vertexCount; v++)
                                {
                                    // Iterate over vertex elements
                                    List<VertexElement> elems = vertexBufferElements[i];
                                    foreach (VertexElement ei in elems)
                                    {
                                        VertexElement elem = ei;
                                        IntPtr st = IntPtr.Zero;
                                        IntPtr dt = IntPtr.Zero;
                                        BaseVertexToPointerElement((IntPtr)sourceBase, out st, elem.Offset);
                                        BaseVertexToPointerElement((IntPtr)destBase, out dt, elem.Offset);
                                        sourcePtr = (float*)st;
                                        destPtr = (float*)dt;
                                        Vector3 tmp = Vector3.Zero;
                                        uint tmpColor = 0;
                                        byte tmpR = 0, tmpG = 0, tmpB = 0, tmpA = 0;
                                        switch (elem.Semantic)
                                        {
                                            case VertexElementSemantic.Position:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //transform
                                                tmp = (queuedMesh.Orientation * (tmp * queuedMesh.Scale)) + queuedMesh.Postion;
                                                tmp -= batchCenter; //Adjust for batch center

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            case VertexElementSemantic.Normal:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //rotate
                                                tmp = queuedMesh.Orientation * tmp;

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            case VertexElementSemantic.Diffuse:
                                                tmpColor = *((uint*)sourcePtr++);
                                                tmpR = (byte)(((tmpColor) & 0xFF) * queuedMesh.Color.r);
                                                tmpG = (byte)(((tmpColor >> 8) & 0xFF) * queuedMesh.Color.g);
                                                tmpB = (byte)(((tmpColor >> 16) & 0xFF) * queuedMesh.Color.b);
                                                tmpA = (byte)((tmpColor >> 24) & 0xFF);

                                                tmpColor = (uint)(tmpR | (tmpG << 8) | (tmpB << 16) | (tmpA << 24));
                                                *((uint*)destPtr++) = tmpColor;
                                                break;
                                            case VertexElementSemantic.Tangent:
                                            case VertexElementSemantic.Binormal:
                                                tmp.x = *sourcePtr++;
                                                tmp.y = *sourcePtr++;
                                                tmp.z = *sourcePtr++;

                                                //rotate
                                                tmp = queuedMesh.Orientation * tmp;

                                                *destPtr++ = tmp.x;
                                                *destPtr++ = tmp.y;
                                                *destPtr++ = tmp.z;
                                                break;
                                            default:
                                                //raw copy
                                                Memory.Copy((IntPtr)sourcePtr, (IntPtr)destPtr, VertexElement.GetTypeSize(elem.Type));
                                                break;
                                        }
                                    }
                                    // Increment both pointers
                                    destBase += sourceBuffer.VertexSize;
                                    sourceBase += sourceBuffer.VertexSize;
                                }
                                //Unlock the input buffer
                                vertexBuffers[i] = (IntPtr)destBase;
                                sourceBuffer.Unlock();
                            }//end if
                            else
                            {
                                Debug.Assert(mRequireVertexColors);

                                //get the locket outout buffer
                                uint* startPtr = (uint*)vertexBuffers[vertBinding.BindingCount - 1];
                                uint* endPtr = startPtr + sourceVerterxData.vertexCount;

                                //Generate color
                                byte tmpR = (byte)(queuedMesh.Color.r * 255);
                                byte tmpG = (byte)(queuedMesh.Color.g * 255);
                                byte tmpB = (byte)(queuedMesh.Color.b * 255);
                                uint tmpColor = (uint)(tmpR | (tmpG << 8) | (tmpB << 16) | (0xFF << 24));
                                //Copy colors
                                while (startPtr < endPtr)
                                {
                                    *startPtr++ = tmpColor;
                                }

                                int idx = vertBinding.BindingCount-1;
                                ushort* bf = (ushort*)vertexBuffers[idx];
                                bf += (sizeof(uint) * sourceVerterxData.vertexCount);
                                vertexBuffers[idx] = (IntPtr)bf;
                            }
                        }//end for

                        //Copy mesh index data into the index buffer
                        if (srcIndexType == IndexType.Size32)
                        {
                            //Lock the input buffer
                            uint* source = (uint*)sourceIndexData.indexBuffer.Lock(
                                sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.ReadOnly);

                            uint* sourceEnd = source + sourceIndexData.indexCount;

                            //And copy it to the output buffer
                            while (source != sourceEnd)
                            {
                                *indexBuffer32++ = (uint)(*source++ + indexOffset);
                            }

                            //Unlock the input buffer
                            sourceIndexData.indexBuffer.Unlock();
                            //Increment the index offset
                            indexOffset += sourceVerterxData.vertexCount;
                        }
                        else
                        {
                            if (destIndexType == IndexType.Size32)
                            {
                                //-- Convert 16 bit to 32 bit indices --
                                //Lock the input buffer
                                ushort* source = (ushort*)sourceIndexData.indexBuffer.Lock(
                                    sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.ReadOnly);
                                ushort* sourceEnd = source + sourceIndexData.indexCount;

                                //And copy it to the output buffer
                                while (source != sourceEnd)
                                {
                                    uint indx = *source++;
                                    *indexBuffer32++ = (uint)(indx + indexOffset);
                                }

                                //Unlock the input buffer
                                sourceIndexData.indexBuffer.Unlock();

                                //Increment the index offset
                                indexOffset += sourceVerterxData.vertexCount;
                            }
                            else
                            {
                                //Lock the input buffer
                                ushort* source = (ushort*)sourceIndexData.indexBuffer.Lock(
                                    sourceIndexData.indexStart, sourceIndexData.indexCount, BufferLocking.Normal);
                                ushort* sourceEnd = source + sourceIndexData.indexCount;

                                //And copy it to the output buffer
                                while (source != sourceEnd)
                                {
                                    *indexBuffer16++ = (ushort)(*source++ + indexOffset);
                                }

                                //Unlock the input buffer
                                sourceIndexData.indexBuffer.Unlock();

                                //Increment the index offset
                                indexOffset += sourceVerterxData.vertexCount;

                            }
                        }
                    }
                    //Unlock buffers
                    indexData.indexBuffer.Unlock();
                    for (short i = 0; i < vertBinding.BindingCount; i++)
                        vertBinding.GetBuffer(i).Unlock();

                    //Clear mesh queue
                    mMeshQueue.Clear();
                    mBuild = true;
                }
            }
		
            private void BaseVertexToPointerElement(IntPtr pBase, out IntPtr pElem, int offset)
            {
                unsafe
                {
                    pElem = (IntPtr)(byte*)((byte*)(pBase) + offset);
                }
            }
            /// <summary>
            /// 
            /// </summary>
            public void Clear()
            {
                //If built, delete the batch
                if (mBuild)
                {
                    //Delete buffers
                    indexData.indexBuffer = null;
                    vertexData.vertexBufferBinding.UnsetAllBindings();

                    //Reset vertex/index count
                    vertexData.vertexStart = 0;
                    vertexData.vertexCount = 0;
                    indexData.indexStart = 0;
                    indexData.indexCount = 0;
                }

                //Clear mesh queue
                mMeshQueue.Clear();

                mBuild = false;
            }
            
            public override RenderOperation RenderOperation
            {
                get
                {
                    base.renderOperation.operationType = OperationType.TriangleList;
                    base.renderOperation.useIndices = true;
                    base.renderOperation.vertexData = vertexData;
                    base.renderOperation.indexData = indexData;
                    return base.renderOperation;
                }
            }
//            /// <summary>
//            /// 
//            /// </summary>
//            /// <param name="op"></param>
//            public override void GetRenderOperation(RenderOperation op)
//            {
//                op.operationType = OperationType.TriangleList;
//#warning: missing op.srcRenderable
//                op.useIndices = true;
//                op.vertexData = vertexData;
//                op.indexData = indexData;
//            }
#warning missing LightListProperty
            /// <summary>
            /// 
            /// </summary>
            /// <param name="camera"></param>
            /// <returns></returns>
            public override float GetSquaredViewDepth(Camera camera)
            {
                Vector3 camVec = mParent.ConvertToLocal(camera.DerivedPosition) - mParent.Center;
				return camVec.LengthSquared;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="mat"></param>
            /// <returns></returns>
            private Material GetMaterialClone(Material mat)
            {
                string clonedName = mat.Name + "_Batched";
                Material clonedMat = (Material)MaterialManager.Instance.GetByName(clonedName);
                if (clonedMat == null)
                    clonedMat = mat.Clone(clonedName);

                return clonedMat;
            }
           
        }
        #endregion

    }
}
