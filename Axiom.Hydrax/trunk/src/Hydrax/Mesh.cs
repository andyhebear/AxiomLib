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
using Axiom.Graphics;
using System.Drawing;
#endregion

#region - namespace -
namespace Axiom.Hydrax
{
    #region - class -
    /// <summary>
    /// Class which contains all functions / variables related to
    /// Hydrax water mesh.
    /// </summary>
    public class Mesh
    {
        #region - POS_NORM_UV_VERTEX -
        /// <summary>
        /// Vertex struct for position, normals and uv data.
        /// </summary>
        public struct POS_NORM_UV_VERTEX
        {
            public float X, Y, Z;
            public float NX, NY, NZ;
            public float TU, TV;
            public static int SizeInBytes = (3 + 3 + 2) * sizeof(float);
        }
        #endregion

        #region - POS_NORM_VERTEX -
        /// <summary>
        /// Vertex struct for position and normals data.
        /// </summary>
        public struct POS_NORM_VERTEX
        {
            public float X, Y, Z;
            public float NX, NY, NZ;
            public static int SizeInBytes = (3 + 3) * sizeof(float);
        }
        #endregion

        #region - POS_UV_VERTEX -
        /// <summary>
        /// Vertex struct for position and uv data.
        /// </summary>
        public struct POS_UV_VERTEX
        {
            public float X, Y, Z;
            public float TU, TV;
            public static int SizeInBytes = (3 + 2) * sizeof(float);
        }
        #endregion

        #region - POS_VERTEX -
        /// <summary>
        /// Vertex struct for position data.
        /// </summary>
        public struct POS_VERTEX
        {
            public float X, Y, Z;
            public static int SizeInBytes = (3) * sizeof(float);
        }
        #endregion

        #region - VertexType -
        /// <summary>
        /// Mesh vertex type enum.
        /// </summary>
        public enum VertexType
		{
			VT_POS_NORM_UV = 0,
			VT_POS_NORM    = 1,
			VT_POS_UV      = 2,
			VT_POS         = 3,
		};
        #endregion

        #region - Options -
        /// <summary>
        /// Base Hydrax Mesh options.
        /// </summary>
        public struct Options
        {
            /// <summary>
            /// Mesh Complexity.
            /// </summary>
            public int MeshComplexity;
            /// <summary>
            /// Grid Size (X / Z ) world space.
            /// </summary>
            public Size MeshSize;
            /// <summary>
            /// Water strength.
            /// </summary>
            public float MeshStrength;
            /// <summary>
            /// Vertex type.
            /// </summary>
            public VertexType MeshVertexType;
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="MeshComplexity">Grid complexity</param>
            /// <param name="MeshSize">grid size (X / Z) world space</param>
            /// <param name="MeshStrenght">Water strength (Y axis muliplier)</param>
            /// <param name="MeshVertexType">Mesh.VertexType</param>
            public Options(int MeshComplexity, Size MeshSize, VertexType MeshVertexType,
                int MeshStrenght)
            {
                this.MeshComplexity = MeshComplexity;
                this.MeshSize = MeshSize;
                this.MeshStrength = MeshStrenght;
                this.MeshVertexType = MeshVertexType;
            }
            /// <summary>
            /// Constructor.
            /// </summary>
            /// <param name="MeshComplexity">Grid complexity</param>
            /// <param name="MeshSize">grid size (X / Z) world space</param>
            /// <param name="MeshVertexType">Mesh.VertexType</param>
            public Options(int MeshComplexity, Size MeshSize, VertexType MeshVertexType)
            {
                this.MeshComplexity = MeshComplexity;
                this.MeshSize = MeshSize;
                this.MeshStrength = 10;
                this.MeshVertexType = MeshVertexType;
            }
        }
        #endregion

        #region - Fields -
        protected Mesh.Options mOptions;
        protected bool mIsCreated;
        protected Axiom.Core.Mesh mMesh;
        protected SubMesh mSubMesh;
        protected Entity mEntity;
        protected int mNumFaces;
        protected int mNumVertices;
        protected HardwareVertexBuffer mVertexBuffer;
        protected HardwareIndexBuffer mIndexBuffer;
        protected SceneNode mSceneNode;
        protected string mMaterialName;
        protected Hydrax mHydrax;
        #endregion

        #region - Properties -
        public Axiom.Core.Mesh HMesh
        {
            set { mMesh = value; }
            get { return mMesh; }
        }
        public HardwareIndexBuffer HardwareIndexBuffer
        {
            set { mIndexBuffer = value; }
            get { return mIndexBuffer; }
        }
        public HardwareVertexBuffer HardwareVertexBuffer
        {
            set { mVertexBuffer = value; }
            get { return mVertexBuffer; }
        }
        public SubMesh SubMesh
        {
            set { mSubMesh = value; }
            get { return mSubMesh; }
        }
        public SceneNode SceneNode
        {
            set { mSceneNode = value; }
            get { return mSceneNode; }
        }
        public string MaterialName
        {
            set { mMaterialName = value; }
            get { return mMaterialName; }
        }
        public Entity Entity
        {
            set { mEntity = value; }
            get { return mEntity; }
        }
        public Options MeshOptions
        {
            set { mOptions = value; }
            get { return mOptions; }
        }
        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="Hydrax">Hydrax</param>
        public Mesh(Hydrax Hydrax)
        {
            mHydrax = Hydrax;
            mMaterialName = "_NULL_";
        }
        /// <summary>
        /// Destructor.
        /// </summary>
        ~Mesh()
        {
            Remove();
        }
        #endregion

        #region - Methods -
        #region - SetOptions -
        public void SetOptions(Options Options)
        {
            if (mIsCreated)
            {
                AxisAlignedBox meshBounds;

                if (Options.MeshSize.Width == 0 && Options.MeshSize.Height == 0)
                {
                    meshBounds = new AxisAlignedBox(
                                 new Vector3(-1000000, -mOptions.MeshStrength / 2, -1000000),
                                 new Vector3( 1000000,  mOptions.MeshStrength / 2,  1000000));
                }
                else
                {
                    meshBounds = new AxisAlignedBox(
                                 new Vector3(-1000000, -mOptions.MeshStrength / 2, -1000000),
                                 new Vector3( 1000000,  mOptions.MeshStrength / 2,  1000000));
                }

                mMesh.BoundingBox = meshBounds;
                mSceneNode.NeedUpdate();
            }

            mOptions = Options;
        }
        #endregion

        #region - Create -
        /// <summary>
        /// Create our water mesh, geometry, entity, etc...
        /// <remarks>Call it after SetMeshOptions and SetMaterialName</remarks>
        /// </summary>
        public void Create()
        {
            if (mIsCreated)
            {
                return;
            }
            //create mesh and submesh
            mMesh = (Axiom.Core.Mesh)MeshManager.Instance.CreateManual("HydraxMesh", "HYDRAX_RESOURCE_GROUP", null);
            mSubMesh = mMesh.CreateSubMesh();
            mSubMesh.useSharedVertices = false;

            if (mHydrax.Module != null)
            {
                if (!mHydrax.Module.CreateGeometry(this))
                {
                    CreateGeometry();
                }
            }

            AxisAlignedBox meshBounds;

            if (mOptions.MeshSize.Width == 0 && mOptions.MeshSize.Height == 0)
            {
                meshBounds = new AxisAlignedBox(
                    new Vector3(-1000000, -mOptions.MeshStrength / 2, -1000000),
                    new Vector3(1000000, mOptions.MeshStrength / 2, 1000000));
            }
            else
            {
                meshBounds = new AxisAlignedBox(
                    new Vector3(-1000000, -mOptions.MeshStrength / 2, -1000000),
                    new Vector3(1000000, mOptions.MeshStrength / 2, 1000000));
            }

            mMesh.BoundingBox = meshBounds;
            mMesh.Load();
            mMesh.Touch();

            mEntity = mHydrax.SceneManager.CreateEntity("HydraxMeshEnt", "HydraxMesh");
            mEntity.MaterialName = mMaterialName;
            mEntity.CastShadows = false;
            mEntity.RenderQueueGroup = RenderQueueGroupID.One;

            mSceneNode = mHydrax.SceneManager.RootSceneNode.CreateChildSceneNode();
            mSceneNode.ShowBoundingBox = false;
            mSceneNode.AttachObject(mEntity);
            mSceneNode.Position = new Vector3(
                mHydrax.Position.x - mOptions.MeshSize.Width / 2,
                mHydrax.Position.y,
                mHydrax.Position.z - mOptions.MeshSize.Height / 2);
            mEntity.RenderQueueGroup = RenderQueueGroupID.Eight;

            mIsCreated = true;
        }
        #endregion

        #region - Remove -
        /// <summary>
        /// Removes all resources.
        /// </summary>
        public void Remove()
        {
            try
            {
                if (!mIsCreated)
                {
                    return;
                }

                mSceneNode.DetachAllObjects();
                mSceneNode.Parent.RemoveChild(mSceneNode.Name);
                mSceneNode = null;

                mMesh = null;
                mSubMesh = null;
                mEntity = null;
                mVertexBuffer = null;
                mIndexBuffer = null;
                mMaterialName = "_NULL_";
                mIsCreated = false;
            }
            catch { };
        }
        #endregion

        #region - UpdateGeometry -
        /// <summary>
        /// Update Geometry
        /// </summary>
        /// <param name="NumVer">Number of vertices</param>
        /// <param name="VerArray">Vertices array</param>
        /// <returns>false if number of vertices do not correspond</returns>
        public bool UpdateGeometry(int NumVer, Array VerArray)
        {
            
            if (NumVer != mVertexBuffer.VertexCount || !mIsCreated)
            {
                return false;
            }

            if (VerArray != null)
            {
                mVertexBuffer.WriteData(
                    0,
                    mVertexBuffer.Size,
                    VerArray,
                    true);
            }

            return true;
        }
        #endregion

        #region - IsPointInGrid -
        /// <summary>
        /// Get if a Position point is inside of the grid
        /// </summary>
        /// <param name="Position">World-space point</param>
        /// <returns>true if Position point is inside of the grid, else false.</returns>
        public bool IsPointInGrid(Vector2 Position)
        {

            AxisAlignedBox WorldMeshBounds = mEntity.BoundingBox;
            // Get our mesh grid rectangle:
            // c-----------d
            // |           |
            // |           |
            // |           |
            // a-----------b

            Vector3 a = WorldMeshBounds.Corners[0];//FAR_LEFT_BOTTOM
            Vector3 b = WorldMeshBounds.Corners[3];//FAR_RIGHT_BOTTOM
            Vector3 c = WorldMeshBounds.Corners[7];//NEAR_RIGHT_BOTTOM
            Vector3 d = WorldMeshBounds.Corners[6];//NEAR_LEFT_BOTTOM

            //Transform all corners to Vector2 array
            Vector2[] Corners2D = new Vector2[4];
            Corners2D[0] = new Vector2(a.x, a.z);
            Corners2D[1] = new Vector2(b.x, b.z);
            Corners2D[2] = new Vector2(c.x, c.z);
            Corners2D[3] = new Vector2(d.x, d.z);

            // Determinate if Position is into our rectangle, we use a line intersection detection
            // because our mesh rectangle can be rotated, if the number of collisions with the four
            // segments AB, BC, CD, DA is one, the Position point is into the rectangle, else(if number 
            // of collisions are 0 or 2, the Position point is outside the rectangle.
            int NumberOfCollisions = 0;
            // Find a point wich isn't be inside the rectangle
            Vector2 mZero = new Vector2(0, 0);
            Vector2 DestPoint = Corners2D[0] + (Corners2D[1] - Corners2D[0]) * 2;
            for (int k = 0; k < 3; k++)
            {
                //MathHelper.IntersectionOfTwoLines(Corners2D[k], Corners2D[k + 1], Position, DestPoint, ref result);
                Vector2 mIntersection = MathHelper.IntersectionOfTwoLines(Corners2D[k], Corners2D[k + 1], Position, DestPoint);
                if (mIntersection.x != mZero.x || mIntersection.y != mZero.y)
                {
                    NumberOfCollisions++;
                }
                if (k == 2)
                {
                    mIntersection = MathHelper.IntersectionOfTwoLines(Corners2D[k], Corners2D[k + 1], Position, DestPoint);
                    //MathHelper.IntersectionOfTwoLines(Corners2D[k], Corners2D[k + 1], Position, DestPoint, ref result);
                    if (mIntersection.x != mZero.x || mIntersection.y != mZero.y)
                    {
                        NumberOfCollisions++;
                    }
                }
            }
            if (NumberOfCollisions == 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region - GridPosition -
        /// <summary>
        /// Get the [0,1] range x/y grid position from a 2D world space x/z point
        /// </summary>
        /// <param name="Position"> World-space point</param>
        /// <returns>(-1,-1) if the point isn't in the grid</returns>
        public Vector2 GridPosition(Vector2 Position)
        {
            if (mOptions.MeshSize.Width == 0 && mOptions.MeshSize.Height == 0)
            {
                return Position;
            }
            if (!IsPointInGrid(Position))
            {
                new Vector2(-1, -1);
            }

            AxisAlignedBox WorldMeshBounds = mEntity.BoundingBox;
            // Get our mesh grid rectangle: (Only a,b,c corners)
            // c
            // |           
            // |           
            // |           
            // a-----------b
            Vector3 a = WorldMeshBounds.Corners[0];//FAR_LEFT_BOTTOM
            Vector3 b = WorldMeshBounds.Corners[3];//FAR_RIGHT_BOTTOM
            Vector3 c = WorldMeshBounds.Corners[6];//NEAR_LEFT_BOTTOM

            //Transform all corners to Vector2 array
            Vector2[] Corners2D = new Vector2[4];
            Corners2D[0] = new Vector2(a.x, a.z);
            Corners2D[1] = new Vector2(b.x, b.z);
            Corners2D[2] = new Vector2(c.x, c.z);

            // Get segments AB and AC
            Vector2 AB = Corners2D[1] - Corners2D[0];
            Vector2 AC = Corners2D[2] - Corners2D[0];

            // Find the X/Y position projecting the Position point to AB and AC segments.
            Vector2 XProjectedPoint = Position - AC;
            Vector2 YProjectedPoint = Position - AB;

            // Fint the intersections points
            Vector2 XPoint = MathHelper.IntersectionOfTwoLines(Corners2D[0], Corners2D[1], Position, XProjectedPoint);
            Vector2 YPoint = MathHelper.IntersectionOfTwoLines(Corners2D[0], Corners2D[2], Position, YProjectedPoint);
          //  MathHelper.IntersectionOfTwoLines(Corners2D[0], Corners2D[1], Position, XProjectedPoint, ref XPoint);
          //  MathHelper.IntersectionOfTwoLines(Corners2D[0], Corners2D[2], Position, YProjectedPoint, ref YPoint);

            //find lengths
            float ABLength = AB.Length();
            float ACLength = AC.Length();
            float XLength = (XPoint - Corners2D[0]).Length();
            float YLength = (YPoint - Corners2D[0]).Length();

            // Find final x/y grid positions in [0,1] range

            float XFinal = XLength / ABLength;
            float YFinal = YLength / ACLength;

            return new Vector2(XFinal, YFinal);
        }
        #endregion

        public Vector3 GetObjectSpacePosition(Vector3 WorldSpacePosition)
        {
#warning checkme
            Matrix4[] mWorldMatrix = new Matrix4[1];

            if (mIsCreated)
            {
                mEntity.ParentSceneNode.GetWorldTransforms(mWorldMatrix);
            }
            else
            {
                SceneNode mTmpSN = new SceneNode(mHydrax.SceneManager, "0");
                mTmpSN.Position = mHydrax.Position;

                mTmpSN.GetWorldTransforms(mWorldMatrix);

                mTmpSN = null;
            }

            return mWorldMatrix[0].InverseAffine().TransformAffine(WorldSpacePosition);
        }

        public Vector3 GetWorldSpacePosition(Vector3 ObjectSpacePosition)
        {
            Matrix4[] mWorldMatrix = new Matrix4[1];

            if (mIsCreated)
            {
                mEntity.ParentSceneNode.GetWorldTransforms(mWorldMatrix);
            }
            else
            {
                SceneNode mTmpSN = new SceneNode(mHydrax.SceneManager, "0");
                mTmpSN.Position = mHydrax.Position;

                mTmpSN.GetWorldTransforms(mWorldMatrix);

                mTmpSN = null;
            }

            return mWorldMatrix[0].TransformAffine(ObjectSpacePosition);
        }
        #endregion

        #region - CreateGeometry -
        /// <summary>
        /// Create's the mesh geometry
        /// </summary>
        private void CreateGeometry()
        {
            int Complexity = mOptions.MeshComplexity;
            mNumVertices = Complexity * Complexity;
            int numEule = 6 * (Complexity - 1) * (Complexity - 1);

            //Vertex buffers
            mSubMesh.vertexData = new VertexData();
            mSubMesh.vertexData.vertexStart = 0;
            mSubMesh.vertexData.vertexCount = mNumVertices;

            VertexDeclaration vdecl = mSubMesh.vertexData.vertexDeclaration;
            VertexBufferBinding vbind = mSubMesh.vertexData.vertexBufferBinding;

            int offset = 0;

            switch (mOptions.MeshVertexType)
            {
                case VertexType.VT_POS_NORM_UV:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);
                    offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                    vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Normal);
                    offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                    vdecl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);

                    mVertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                        /*POS_NORM_UV_VERTEX.SizeInBytes*/vdecl,
                        mNumVertices,
                         BufferUsage.DynamicWriteOnly);
                    break;
                case VertexType.VT_POS_NORM:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);
                    offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                    vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Normal);

                    mVertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                        /*POS_NORM_VERTEX.SizeInBytes*/vdecl,
                        mNumVertices,
                         BufferUsage.DynamicWriteOnly);
                    break;
                case VertexType.VT_POS_UV:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);
                    offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                    vdecl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);

                    mVertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                        /*POS_UV_VERTEX.SizeInBytes*/vdecl,
                        mNumVertices,
                         BufferUsage.DynamicWriteOnly);
                    break;
                case VertexType.VT_POS:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);

                    mVertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                        /*POS_VERTEX.SizeInBytes*/vdecl,
                        mNumVertices,
                         BufferUsage.DynamicWriteOnly);
                    break;
            }

            vbind.SetBinding(0, mVertexBuffer);

            int[] indexbuffer = new int[numEule];

            int i = 0;
            for (int v = 0; v < Complexity - 1; v++)
            {
                for (int u = 0; u < Complexity - 1; u++)
                {
                    //face 1 |/
                    indexbuffer[i++] = v * Complexity + u;
                    indexbuffer[i++] = v * Complexity + u + 1;
                    indexbuffer[i++] = (v + 1) * Complexity + u;

                    // face 2 /|
                    indexbuffer[i++] = (v + 1) * Complexity + u;
                    indexbuffer[i++] = v * Complexity + u + 1;
                    indexbuffer[i++] = (v + 1) * Complexity + u + 1;
                }
            }

            //prepare buffer for indices
            mIndexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(
                 IndexType.Size32,
                 numEule,
                 BufferUsage.Static,
                 true);

            mIndexBuffer.WriteData(0,
                mIndexBuffer.Size,
                indexbuffer,
                true);

            indexbuffer = null;

            ///set index buffer for this submesh
            mSubMesh.indexData.indexBuffer = mIndexBuffer;
            mSubMesh.indexData.indexStart = 0;
            mSubMesh.indexData.indexCount = numEule;
        }
        #endregion
    }//end class
    #endregion
}//end namespace
#endregion
