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
using Axiom.Core;
using Axiom.Graphics;
using Utility = Axiom.Math.Utility;
using Axiom.Math;
namespace Axiom.SkyX
{
    public class MeshManager : IDisposable
    {
        #region - PosUVVertex -
        /// <summary>
        /// Vertex struct for position and tex coords data.
        /// </summary>
        public struct PosUVVertex
        {
            /// <summary>
            /// X position of the vertex.
            /// </summary>
            public float X;
            /// <summary>
            /// Y position of the vertex
            /// </summary>
            public float Y;
            /// <summary>
            /// Z postion of the vertex
            /// </summary>
            public float Z;
            /// <summary>
            /// x normal of the vertex
            /// </summary>
            public float NX;
            /// <summary>
            /// x normal of the vertex
            /// </summary>
            public float NY;
            /// <summary>
            /// z normal of the vertex
            /// </summary>
            public float NZ;
            /// <summary>
            /// Texture coordinate U
            /// </summary>
            public float U;
            /// <summary>
            /// Texture coordinate V
            /// </summary>
            public float V;
            /// <summary>
            /// Opacity
            /// </summary>
            public float Opacity;

            public static int SizeInBytes = (4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4) * 9;
        }
        #endregion

        /// <summary>
        /// was create() allready called?
        /// </summary>
        private bool _created;
        /// <summary>
        /// Mesh
        /// </summary>
        private Mesh _mesh;
        /// <summary>
        /// submesh
        /// </summary>
        private SubMesh _subMesh;
        /// <summary>
        /// entity
        /// </summary>
        private Entity _entity;
        /// <summary>
        /// Vertex buffer.
        /// </summary>
        private HardwareVertexBuffer _vertexBuffer;
        /// <summary>
        /// Index buffer.
        /// </summary>
        private HardwareIndexBuffer _indexBuffer;
        /// <summary>
        /// Vertices.
        /// </summary>
        private PosUVVertex[] _vertices;
        /// <summary>
        /// 
        /// </summary>
        private int _circles;
        /// <summary>
        /// 
        /// </summary>
        private int _steps;
        /// <summary>
        /// 
        /// </summary>
        private bool _smoothSkyDomeFading;
        /// <summary>
        /// 
        /// </summary>
        private float _skydomeFadingPercent;
        /// <summary>
        /// 
        /// </summary>
        private SceneNode _sceneNode;
        /// <summary>
        /// 
        /// </summary>
        private string _materialName;
        /// <summary>
        /// 
        /// </summary>
        private SkyX _skyX;

        /// <summary>
        /// Get's a reference of the main SkyX
        /// </summary>
        public SkyX SkyX
        {
            get
            {
                return _skyX;
            }
            private set
            {
                _skyX = value;
            }
        }
        /// <summary>
        /// Get's a reference of the used mesh
        /// </summary>
        public Mesh Mesh
        {
            get
            {
                return _mesh;
            }
            private set
            {
                _mesh = value;
            }
        }
        /// <summary>
        /// Get's a reference to the submesh
        /// </summary>
        public SubMesh SubMesh
        {
            get
            {
                return _subMesh;
            }
            private set
            {
                _subMesh = value;
            }
        }
        /// <summary>
        /// Get's a reference to the entity
        /// </summary>
        public Entity Entity
        {
            get
            {
                return _entity;
            }
            private set
            {
                _entity = value;
            }
        }
        /// <summary>
        /// Get's a reference to the SmoothSkydomeFading property
        /// </summary>
        public bool SmoothSkydomeFading
        {
            get
            {
                return _smoothSkyDomeFading;
            }
            private set
            {
                _smoothSkyDomeFading = value;
            }
        }

        /// <summary>
        /// Get's a reference to the SkydomeFadingPercent property
        /// </summary>
        public float SkydomeFadingPercent
        {
            get
            {
                return _skydomeFadingPercent;
            }
            private set
            {
                _skydomeFadingPercent = value;
            }
        }
        /// <summary>
        /// Get's or set's the material name.
        /// </summary>
        public string MaterialName
        {
            get
            {
                return _materialName;
            }
            set
            {
                SetMaterialName(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HardwareVertexBuffer HardwareVertexBuffer
        {
            get
            {
                return _vertexBuffer;
            }
            private set
            {
                _vertexBuffer = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public HardwareIndexBuffer HardwareIndexBuffer
        {
            get
            {
                return _indexBuffer;
            }
            private set
            {
                _indexBuffer = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public SceneNode SceneNode
        {
            get
            {
                return _sceneNode;
            }
            private set
            {
                _sceneNode = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get
            {
                return _created;
            }
            private set
            {
                _created = value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public int Steps
        {
            get
            {
                return _steps;
            }
            private set
            {
                _steps = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Circles
        {
            get
            {
                return _circles;
            }
            private set
            {
                _circles = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public float SkydomeRadius
        {
            get
            {
                return this.SkyX.Camera.Far * 0.95f;
            }
        }
        #region Construction and Destruction

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skyX"></param>
        public MeshManager(SkyX skyX)
        {
            this.SkyX = skyX;
            this.SmoothSkydomeFading = true;
            this.SkydomeFadingPercent = 0.05f;
            this.MaterialName = "_NULL_";
            this.Steps = 70;
            this.Circles = 80;
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }
        #endregion Construction and Destruction

        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            if (this.IsCreated)
            {
                return;
            }

            // Create mesh and submesh
            this.Mesh = Axiom.Core.MeshManager.Instance.CreateManual("SkyXMesh", SkyX.SkyXResourceGroup, null);
            this.SubMesh = this.Mesh.CreateSubMesh();
            this.SubMesh.useSharedVertices = false;

            // Create mesh geometry
            CreateGeometry();

            // End mesh creation
            this.Mesh.Load();
            this.Mesh.Touch();

            this.Entity = this.SkyX.SceneManager.CreateEntity("SkyXMeshEnt", "SkyXMesh");
            SetMaterialName(this.MaterialName);
            this.Entity.CastShadows = false;
            this.Entity.RenderQueueGroup = RenderQueueGroupID.SkiesEarly + 1;

            this.SceneNode = this.SkyX.SceneManager.RootSceneNode.CreateChildSceneNode();
            this.SceneNode.ShowBoundingBox = false;
            this.SceneNode.AttachObject(this.Entity);
            this.SceneNode.Position = this.SkyX.Camera.DerivedPosition;

            this.IsCreated = true;

            UpdateGeometry();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Remove()
        {
            if (!this.IsCreated)
            {
                return;
            }

            this.SceneNode.DetachAllObjects();
            this.SceneNode.Parent.RemoveAndDestroyChild(this.SceneNode.Name);
            this.SceneNode = null;

            Axiom.Core.MeshManager.Instance.Remove("SkyMesh");
            this.SkyX.SceneManager.RemoveEntity(this.Entity);

            this.Mesh = null;
            this.SubMesh = null;
            this.Entity = null;
            this.HardwareVertexBuffer = null;
            this.HardwareIndexBuffer = null;
            this.MaterialName = "_NULL_";
            this._vertices = null;
            this.IsCreated = false;
        }


        internal void UpdateGeometry()
        {
            
            if (!this.IsCreated)
            {
                return;
            }
            // 95% of camera far clip distance
            float TODO_Radius = this.SkyX.Camera.Far * 0.95f;

            _vertices[0].X = 0;
            _vertices[0].Z = 0;
            _vertices[0].Y = TODO_Radius;
            _vertices[0].NX = 0;
            _vertices[0].NZ = 0;
            _vertices[0].NY = 1;
            _vertices[0].U = 4;
            _vertices[0].V = 4;
            _vertices[0].Opacity = 1;

            float angleStep = (Utility.PI / 2.0f) / (this.Circles - 1.0f);

            float r, uvr, c, s, sc;
            int x, y;

            for (y = 0; y < this.Circles - 1; y++)
            {
                r = Utility.Cos(Utility.PI / 2.0f - angleStep * (y + 1.0f));
                uvr = (y + 1.0f) / (this.Circles - 1.0f);

                for (x = 0; x < this.Steps; x++)
                {
                    c = Utility.Cos(Utility.TWO_PI * x / (float)this.Steps) * r;
                    s = Utility.Sin(Utility.TWO_PI * x / (float)this.Steps) * r;
                    sc = Utility.Sin(Utility.ACos(r));

                    _vertices[1 + y * this.Steps + x].X = c * TODO_Radius;
                    _vertices[1 + y * this.Steps + x].Z = s * TODO_Radius;
                    _vertices[1 + y * this.Steps + x].Y = sc * TODO_Radius;

                    _vertices[1 + y * this.Steps + x].NX = c;
                    _vertices[1 + y * this.Steps + x].NZ = s;
                    _vertices[1 + y * this.Steps + x].NY = sc;

                    _vertices[1 + y * this.Steps + x].U = (1.0f + c * uvr / r) * 4.0f;
                    _vertices[1 + y * this.Steps + x].V = (1.0f + s * uvr / r) * 4.0f;

                    _vertices[1 + y * this.Steps + x].Opacity = 1;
                }
            }

            r = Utility.Cos(angleStep);
            uvr = (this.Circles + 1.0f) / (this.Circles - 1.0f);

            for (x = 0; x < this.Steps; x++)
            {
                c = Utility.Cos(Utility.TWO_PI * x / (float)this.Steps) * r;
                s = Utility.Sin(Utility.TWO_PI * x / (float)this.Steps) * r;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].X = _vertices[1 + (this.Circles - 2) * this.Steps + x].X;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].Z = _vertices[1 + (this.Circles - 2) * this.Steps + x].Z;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].Y = _vertices[1 + (this.Circles - 2) * this.Steps + x].Y - TODO_Radius * this.SkydomeFadingPercent;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].NX = _vertices[1 + (this.Circles - 2) * this.Steps + x].NX;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].NZ = _vertices[1 + (this.Circles - 2) * this.Steps + x].NZ;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].NY = _vertices[1 + (this.Circles - 2) * this.Steps + x].NY;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].U = (1.0f + c * uvr / 4) * 4.0f;
                _vertices[1 + (this.Circles - 1) * this.Steps + x].V = (1.0f + s * uvr / 4) * 4.0f;

                _vertices[1 + (this.Circles - 1) * this.Steps + x].Opacity = this.SmoothSkydomeFading ? 0 : 1;
            }
            // Update data
            _vertexBuffer.WriteData(0, _vertexBuffer.Size, _vertices, true);

            // Update bounds
            AxisAlignedBox meshBounds = new AxisAlignedBox(new Vector3(-TODO_Radius, 0, -TODO_Radius),
                                                            new Vector3(TODO_Radius, TODO_Radius, TODO_Radius));

            //_mesh.BoundingBox = meshBounds;
            _mesh.SetBounds(meshBounds);
            _sceneNode.NeedUpdate();
            //for (int i = 0; i < _vertices.Length; i++)
            //    LogVertices(_vertices[i]);
        }
        void LogVertices(PosUVVertex vertices)
        {
            LogManager.Instance.Write(
                "NX " + vertices.NX + " " +
                "NY " + vertices.NY + " " +
                "NZ " + vertices.NZ + " " +
                "O " + vertices.Opacity + " " +
                "U " + vertices.U + " " +
                "V " + vertices.V + " " +
                "X " + vertices.X + " " +
                "Y " + vertices.Y + " " +
                "Z " + vertices.Z);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="smoothSkyDomeFading"></param>
        public void SetSkydomeFadingParameters(bool smoothSkyDomeFading)
        {
            SetSkydomeFadingParameters(smoothSkyDomeFading, 0.05f);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="smoothSkydomeFading"></param>
        /// <param name="skydomeFadingPercent"></param>
        public void SetSkydomeFadingParameters(bool smoothSkydomeFading, float skydomeFadingPercent)
        {
            this.SmoothSkydomeFading = smoothSkydomeFading;
            this.SkydomeFadingPercent = skydomeFadingPercent;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        private void SetMaterialName(string name)
        {
            _materialName = name;

            if (this.IsCreated)
            {
                this.Entity.MaterialName = this.MaterialName;
            }
        }

        internal void SetGeometryParameters(int steps, int circles)
        {
            this.Steps = steps;
            this.Circles = circles;

            if (this.IsCreated)
            {
                Remove();
                Create();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void CreateGeometry()
        {
            int numVertices = this.Steps * this.Circles + 1;
            int numEule = 6 * this.Steps * (this.Circles - 1) + 3 * this.Steps;

            // Vertex buffers
            this.SubMesh.vertexData = new VertexData();
            this.SubMesh.vertexData.vertexStart = 0;
            this.SubMesh.vertexData.vertexCount = numVertices;

            VertexDeclaration vdecl = this.SubMesh.vertexData.vertexDeclaration;
            VertexBufferBinding vbind = this.SubMesh.vertexData.vertexBufferBinding;

            int offset = 0;
            vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.TexCoords, 0);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            vdecl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords, 1);
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);
            vdecl.AddElement(0, offset, VertexElementType.Float1, VertexElementSemantic.TexCoords, 2);
            offset += VertexElement.GetTypeSize(VertexElementType.Float1);

            _vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(offset, numVertices, BufferUsage.DynamicWriteOnly);

            vbind.SetBinding(0, _vertexBuffer);

            int[] indexBuffer = new int[numEule];
            for (int k = 0; k < this.Steps; k++)
            {
                indexBuffer[k * 3] = 0;
                indexBuffer[k * 3 + 1] = k + 1;

                if (k != this.Steps - 1)
                {
                    indexBuffer[k * 3 + 2] = k + 2;
                }
                else
                {
                    indexBuffer[k * 3 + 2] = 1;
                }
            }

            for (int y = 0; y < this.Circles - 1; y++)
            {
                for (int x = 0; x < this.Steps; x++)
                {

                    int twoface = (y * this.Steps + x) * 6 + 3 * this.Steps;

                    int p0 = 1 + y * this.Steps + x;
                    int p1 = 1 + y * this.Steps + x + 1;
                    int p2 = 1 + (y + 1) * this.Steps + x;
                    int p3 = 1 + (y + 1) * this.Steps + x + 1;

                    if (x == this.Steps - 1)
                    {
                        p1 -= x + 1;
                        p3 -= x + 1;
                    }

                    // First triangle
                    indexBuffer[twoface + 2] = p0;
                    indexBuffer[twoface + 1] = p1;
                    indexBuffer[twoface + 0] = p2;

                    // Second triangle
                    indexBuffer[twoface + 5] = p1;
                    indexBuffer[twoface + 4] = p3;
                    indexBuffer[twoface + 3] = p2;
                }

            }
            // Prepare buffer for indices
            _indexBuffer = HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size32, numEule, BufferUsage.Static, true);
            //for(int z = 0; z < indexBuffer.Length;z++)
            //    LogManager.Instance.Write("Index " + indexBuffer[z]);
            _indexBuffer.WriteData(0, _indexBuffer.Size, indexBuffer, true);

            // Set index buffer for this submesh
            this.SubMesh.indexData.indexBuffer = _indexBuffer;
            this.SubMesh.indexData.indexStart = 0;
            this.SubMesh.indexData.indexCount = numEule;

            // Create our internal buffer for manipulations
            _vertices = new PosUVVertex[1 + this.Steps * this.Circles];
        }
    }
}