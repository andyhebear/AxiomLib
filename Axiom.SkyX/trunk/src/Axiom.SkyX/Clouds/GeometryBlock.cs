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
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.SkyX.Clouds
{
    public class GeometryBlock : IDisposable
    {

        /// <summary>
        /// Vertex struct.
        /// </summary>
        public struct Vertex
        {
            /// <summary>
            /// 
            /// </summary>
            public float X;
            /// <summary>
            /// 
            /// </summary>
            public float Y;
            /// <summary>
            /// 
            /// </summary>
            public float Z;
            /// <summary>
            /// 
            /// </summary>
            public float XC;
            /// <summary>
            /// 
            /// </summary>
            public float YC;
            /// <summary>
            /// 
            /// </summary>
            public float ZC;
            /// <summary>
            /// 
            /// </summary>
            public float U;
            /// <summary>
            /// 
            /// </summary>
            public float V;
            /// <summary>
            /// 
            /// </summary>
            public float O;

            public static int SizeInBytes = (4 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 4) * 9;
        }
        private bool _isCreated;
        private Mesh _mesh;
        private SubMesh _subMesh;
        private Entity _entity;
        private HardwareVertexBuffer _vertexBuffer;
        private HardwareIndexBuffer _indexBuffer;
        private Vertex[] _vertices;
        private int _numberOfTriangles;
        private int _vertexCount;
        private float _height;
        private Radian _alpha;
        private Radian _beta;
        private float _radius;
        private Radian _phie;
        private int _na;
        private int _nb;
        private int _nc;
        private float _a;
        private float _b;
        private float _c;
        private int _position;
        private Vector2 _v2Cos;
        private Vector2 _v2Sin;
        private float _betaSin;
        private float _alphaSin;
        private Vector3 _displacement;
        private Vector2 _worldOffset;
        private VClouds _VClouds;

        /// <summary>
        /// 
        /// </summary>
        public bool IsCreated
        {
            get { return _isCreated; }
            private set { _isCreated = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Mesh Mesh
        {
            get { return _mesh; }
            private set { _mesh = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public SubMesh SubMesh
        {
            get { return _subMesh; }
            private set { _subMesh = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Entity Entity
        {
            private set { _entity = value; }
            get { return _entity; }
        }
        /// <summary>
        /// 
        /// </summary>
        public HardwareVertexBuffer HardwareVertexBuffer
        {
            get { return _vertexBuffer; }
            private set { _vertexBuffer = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public HardwareIndexBuffer HardwareIndexBuffer
        {
            get { return _indexBuffer; }
            private set { _indexBuffer = value; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Vector2 WorldOffset
        {
            set { _worldOffset = value; }
            get { return _worldOffset; }
        }
        public GeometryBlock(VClouds vc,
            float height,
            Radian alpha, Radian beta, float radius,
            Radian phi, int na, int nb, int nc, int position)
        {
            _VClouds = vc;
            _isCreated = false;
            _subMesh = null;
            _entity = null;
            _vertices = null;
            _numberOfTriangles = 0;
            _vertexCount = 0;
            _height = height;
            _alpha = alpha;
            _beta = beta;
            _radius = radius;
            _phie = phi;
            _na = na;
            _nb = nb;
            _nc = nc;
            _position = position;
            _displacement = Vector3.Zero;
            _worldOffset = Vector2.Zero;

            CalculateSizeData();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Remove();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Create()
        {
            Remove();

            // Create mesh and submesh
            _mesh = Axiom.Core.MeshManager.Instance.CreateManual("_SkyX_VClouds_Block" + _position, SkyX.SkyXResourceGroup, null);
            _subMesh = _mesh.CreateSubMesh();
            _subMesh.useSharedVertices = false;

            // Create mesh geometry
            CreateGeometry();

            // End mesh creation
            _mesh.Load();
            _mesh.Touch();

            // Create entity
            _entity = _VClouds.SceneManager.CreateEntity("_SkyX_VClouds_BlockEnt" + _position, "_SkyX_VClouds_Block" + _position);
            _entity.MaterialName = "SkyX_VolClouds";
            _entity.CastShadows = false;
            _entity.RenderQueueGroup = RenderQueueGroupID.SkiesLate;

            //set bounds
            _mesh.SetBounds(BuildAABox());

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
            Axiom.Core.MeshManager.Instance.Remove(_mesh.Name);
            _VClouds.SceneManager.RemoveEntity(_entity);

            _mesh = null;
            _subMesh = null;
            _vertexBuffer = null;
            _indexBuffer = null;

            _vertices = null;

            this.IsCreated = false;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private AxisAlignedBox BuildAABox()
        {
            Vector2 center = new Vector2(0, 0);
            Vector2 V1 = _radius * new Vector2(Utility.Cos((float)_phie * _position), Utility.Sin((float)_phie * _position));
            Vector2 V2 = _radius * new Vector2(Utility.Cos((float)_phie * (_position + 1)), Utility.Sin((float)_phie * (_position + 1)));

            Vector2 Max = new Vector2(Utility.Max(Utility.Max(V1.x, V2.x), center.x), Utility.Max(Utility.Max(V1.y, V2.y), center.y));
            Vector2 Min = new Vector2(Utility.Min(Utility.Min(V1.x, V2.x), center.x), Utility.Min(Utility.Min(V1.y, V2.y), center.y));

            return new AxisAlignedBox(
                new Vector3(Min.x, 0, Min.y),
                new Vector3(Max.x, _height, Max.y));
        }

        /// <summary>
        /// 
        /// </summary>
        private void CalculateSizeData()
        {
            _vertexCount = 7 * _na + 6 * _nb + 4 * _nc;
            _numberOfTriangles = 5 * _na + 4 * _nb + 2 * _nc;

            _a = _height / (Utility.Cos(Utility.PI / 2.0f - (float)(Radian)(_beta)));
            _b = _height / (Utility.Cos(Utility.PI / 2.0f - (float)(Radian)(_alpha)));
            _c = _radius;
            _v2Cos = new Vector2(Utility.Cos(_position * (float)_phie), Utility.Cos((_position + 1) * (float)_phie));
            _v2Sin = new Vector2(Utility.Sin(_position * (float)_phie), Utility.Sin((_position + 1) * (float)_phie));
            _betaSin = Utility.Sin(Utility.PI - (float)(Radian)_beta);
            _alphaSin = Utility.Sin(Utility.PI - (float)(Radian)_alpha);
       }

        /// <summary>
        /// 
        /// </summary>
        private void CreateGeometry()
        {
            // Vertex buffers
            _subMesh.vertexData = new VertexData();
            _subMesh.vertexData.vertexStart = 0;
            _subMesh.vertexData.vertexCount = _vertexCount;

            VertexDeclaration vdecl = _subMesh.vertexData.vertexDeclaration;
            VertexBufferBinding vbind = _subMesh.vertexData.vertexBufferBinding;

            int offset = 0;
            // Position
            vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            // 3D coords
            vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.TexCoords, 0);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);
            // Noise coords
            vdecl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords, 1);
            offset += VertexElement.GetTypeSize(VertexElementType.Float2);
            // Opacity
            vdecl.AddElement(0, offset, VertexElementType.Float1, VertexElementSemantic.TexCoords, 2);
            offset += VertexElement.GetTypeSize(VertexElementType.Float1);

            _vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                offset, _vertexCount,
                 BufferUsage.DynamicWriteOnly);

            vbind.SetBinding(0, _vertexBuffer);

            int[] indexbuffer = new int[_numberOfTriangles * 3];

            int IndexOffset = 0;
            int VertexOffset = 0;
            // C
            for (int k = 0; k < _nc; k++)
            {
                // First triangle
                indexbuffer[IndexOffset] = VertexOffset;
                indexbuffer[IndexOffset + 1] = VertexOffset + 1;
                indexbuffer[IndexOffset + 2] = VertexOffset + 3;

                // Second triangle
                indexbuffer[IndexOffset + 3] = VertexOffset;
                indexbuffer[IndexOffset + 4] = VertexOffset + 3;
                indexbuffer[IndexOffset + 5] = VertexOffset + 2;

                IndexOffset += 6;
                VertexOffset += 4;
            }
            // B
            for (int k = 0; k < _nb; k++)
            {
                // First triangle
                indexbuffer[IndexOffset] = VertexOffset;
                indexbuffer[IndexOffset + 1] = VertexOffset + 1;
                indexbuffer[IndexOffset + 2] = VertexOffset + 3;

                // Second triangle
                indexbuffer[IndexOffset + 3] = VertexOffset;
                indexbuffer[IndexOffset + 4] = VertexOffset + 3;
                indexbuffer[IndexOffset + 5] = VertexOffset + 2;

                // Third triangle
                indexbuffer[IndexOffset + 6] = VertexOffset + 2;
                indexbuffer[IndexOffset + 7] = VertexOffset + 3;
                indexbuffer[IndexOffset + 8] = VertexOffset + 5;

                // Fourth triangle
                indexbuffer[IndexOffset + 9] = VertexOffset + 2;
                indexbuffer[IndexOffset + 10] = VertexOffset + 5;
                indexbuffer[IndexOffset + 11] = VertexOffset + 4;

                IndexOffset += 12;
                VertexOffset += 6;
            }

            // A
            for (int k = 0; k < _na; k++)
            {
                // First triangle
                indexbuffer[IndexOffset] = VertexOffset;
                indexbuffer[IndexOffset + 1] = VertexOffset + 1;
                indexbuffer[IndexOffset + 2] = VertexOffset + 3;

                // Second triangle
                indexbuffer[IndexOffset + 3] = VertexOffset;
                indexbuffer[IndexOffset + 4] = VertexOffset + 3;
                indexbuffer[IndexOffset + 5] = VertexOffset + 2;

                // Third triangle
                indexbuffer[IndexOffset + 6] = VertexOffset + 2;
                indexbuffer[IndexOffset + 7] = VertexOffset + 3;
                indexbuffer[IndexOffset + 8] = VertexOffset + 5;

                // Fourth triangle
                indexbuffer[IndexOffset + 9] = VertexOffset + 2;
                indexbuffer[IndexOffset + 10] = VertexOffset + 5;
                indexbuffer[IndexOffset + 11] = VertexOffset + 4;

                // Fifth triangle
                indexbuffer[IndexOffset + 12] = VertexOffset + 4;
                indexbuffer[IndexOffset + 13] = VertexOffset + 5;
                indexbuffer[IndexOffset + 14] = VertexOffset + 6;

                IndexOffset += 15;
                VertexOffset += 7;

            }
            // Prepare buffer for indices
            _indexBuffer =
                HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size32,
                _numberOfTriangles * 3, BufferUsage.Static, true);

            _indexBuffer.WriteData(
                0, _indexBuffer.Size, indexbuffer, true);

            //indexbufferArr = null;

            // Set index buffer for this submesh
            _subMesh.indexData.indexBuffer = _indexBuffer;
            _subMesh.indexData.indexStart = 0;
            _subMesh.indexData.indexCount = _numberOfTriangles * 3;

            // Create our internal buffer for manipulations
            _vertices = new Vertex[_vertexCount];
            // Update geometry
            UpdateGeometry();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="offset"></param>
        public void Update(float offset)
        {
            if (!this.IsCreated)
            {
                return;
            }

            _displacement += new Vector3(offset);

            if (_displacement.z < 0 || _displacement.z > (_c - _b) / _nc)
            {
                _displacement.z -= ((_c - _b) / _nc) * (int)System.Math.Floor((_displacement.z) / ((_c - _b) / _nc));
            }
            if (_displacement.y < 0 || _displacement.y > (_b - _a) / _nb)
            {
                _displacement.y -= ((_b - _a) / _nb) * (int)System.Math.Floor((_displacement.y) / ((_b - _a) / _nb));
            }
            if (_displacement.x < 0 || _displacement.x > _a / _na)
            {
                _displacement.x -= (_a / _na) * (int)System.Math.Floor((_displacement.x) / (_a / _na));
            }
            if (IsInFrustum(_VClouds.Camera))
            {
                UpdateGeometry();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public bool IsInFrustum(Camera camera)
        {
            if (!this.IsCreated)
            {
                return false;
            }

            if (_entity.ParentSceneNode == null)
            {
                return false;
            }

            // TODO: See Ogre::PlaneBoundedVolume (A volume bounded by planes, Ogre::Ray intersection ;) )
            // Se contruye el planebvol y se lanza un rayo con cada esquina del frustum, si intersecta, está dentro y tiene que ser visible
            // Tambien puede ocurrir que no intersecte porque todo el objeto está dentro, entonces para ver si está dentro
            // Frustum::isVisibile(Ogre::Vector3 vertice) con un vertice cualkiera, por ejemplo mVertices[0].xyz ;)

            return camera.IsObjectVisible(_entity.ParentSceneNode.WorldAABB);
        }
        /// <summary>
        /// 
        /// </summary>
        private void UpdateGeometry()
        {
            // Update zone C
            for (int k = 0; k < _nc; k++)
            {
                UpdateZoneCSlice(k);
            }

             //Update zone B
            for (int k = 0; k < _nb; k++)
            {
                UpdateZoneBSlice(k);
            }

            // Update zone A
            for (int k = 0; k < _na; k++)
            {
                UpdateZoneASlice(k);
            }
             //Upload changes
            _vertexBuffer.
                    WriteData(0,
                             _vertexBuffer.Size,
                             _vertices,
                             true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        private void UpdateZoneCSlice(int n)
        {
            int VertexOffset = n * 4;

            // TODO, calculate constants by zone, not by slice
            float Radius = _b + ((_c - _b) / _nc) * (_nc - n);
            Radius += _displacement.z;
            float opacity = 1;

            if (n == 0)
            {
                opacity = 1 - _displacement.z / ((_c - _b) / _nc);
            }
            else if (n == _nc - 1)
            {
                opacity = _displacement.z / ((_c - _b) / _nc);
            }

            Vector2 x1 = Radius * _v2Cos,
                          x2 = Radius * _betaSin * _v2Cos,
                          z1 = Radius * _v2Sin,
                          z2 = Radius * _betaSin * _v2Sin;

            Vector3 or0 = new Vector3(x1.x, 0, z1.x),
                          or1 = new Vector3(x1.y, 0, z1.y);

            float y0 = Radius * Utility.Sin(_alpha),
                   d = new Vector2(x1.x - x2.x, z1.x - z2.x).Length(),
                 ang = (float)(Radian)Utility.ATan(y0 / d),
                 hip = _height / Utility.Sin(ang);

            // Vertex 0
            SetVertexData(VertexOffset, new Vector3(x1.x, 0, z1.x), opacity);
            // Vertex 1
            SetVertexData(VertexOffset + 1, new Vector3(x1.y, 0, z1.y), opacity);
            // Vertex 2
            SetVertexData(VertexOffset + 2, or0 + (new Vector3(x2.x, y0, z2.x) - or0).NormalizedCopy() * hip, opacity);
            // Vertex 3
            SetVertexData(VertexOffset + 3, or1 + (new Vector3(x2.y, y0, z2.y) - or1).NormalizedCopy() * hip, opacity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        private void UpdateZoneBSlice(int n)
        {
            int VertexOffset = _nc * 4 + n * 6;

            // TODO
            float Radius = _a + ((_b - _a) / _nb) * (_nb - n);

            Radius += _displacement.y;

            float opacity = 1;

            if (n == 0)
            {
                opacity = 1 - _displacement.y / ((_b - _a) / _nb);
            }
            else if (n == _nb - 1)
            {
                opacity = _displacement.y / ((_b - _a) / _nb);
            }

            Vector2 x1 = Radius * _v2Cos,
                          x2 = Radius * _betaSin * _v2Cos,
                          z1 = Radius * _v2Sin,
                          z2 = Radius * _betaSin * _v2Sin;

            float y0 = Radius * Utility.Sin(_alpha);

            // Vertex 0
            SetVertexData(VertexOffset, new Vector3(x1.x, 0, z1.x), opacity);
            // Vertex 1
            SetVertexData(VertexOffset + 1, new Vector3(x1.y, 0, z1.y), opacity);
            // Vertex 2
            SetVertexData(VertexOffset + 2, new Vector3(x2.x, y0, z2.x), opacity);
            // Vertex 3
            SetVertexData(VertexOffset + 3, new Vector3(x2.y, y0, z2.y), opacity);

            Vector2 x3 = Radius * _alphaSin * _v2Cos,
                          z3 = Radius * _alphaSin * _v2Sin;

            Vector3 or0 = new Vector3(x2.x, y0, z2.x),
                          or1 = new Vector3(x2.y, y0, z2.y);

            float y1 = Radius * Utility.Sin(_beta),
                  y3 = y1 - y0,
                   d = new Vector2(x3.x - x2.x, z3.x - z2.x).Length(),
                 ang = (float)(Radian)Utility.ATan(y3 / d),
                 hip = (_height - y0) / Utility.Sin(ang);

            // Vertex 4
            SetVertexData(VertexOffset + 4, or0 + (new Vector3(x3.x, y1, z3.x) - or0).NormalizedCopy() * hip, opacity);
            // Vertex 5
            SetVertexData(VertexOffset + 5, or1 + (new Vector3(x3.y, y1, z3.y) - or1).NormalizedCopy() * hip, opacity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        private void UpdateZoneASlice(int n)
        {
            int VertexOffset = _nc*4 + _nb*6 +n*7;

		// TODO
		float Radius = (_a/_na)*(_na-n);

		Radius += _displacement.x;

		float opacity = (n == 0) ? (1-_displacement.x/(_a/_na)) : 1.0f;

		Vector2 x1 = Radius*_v2Cos,
				      x2 = Radius*_betaSin*_v2Cos,
				      z1 = Radius*_v2Sin,
					  z2 = Radius*_betaSin*_v2Sin;
	    
		float y0 = Radius*Utility.Sin(_alpha);

		// Vertex 0
		SetVertexData(VertexOffset, new Vector3(x1.x, 0, z1.x), opacity);
		// Vertex 1
		SetVertexData(VertexOffset+1,new  Vector3(x1.y, 0, z1.y), opacity);
		// Vertex 2
		SetVertexData(VertexOffset+2, new Vector3(x2.x, y0, z2.x), opacity);
		// Vertex 3
		SetVertexData(VertexOffset+3, new Vector3(x2.y, y0, z2.y), opacity);

		Vector2 x3 = Radius*_alphaSin*_v2Cos,
					  z3 = Radius*_alphaSin*_v2Sin;

		float y1 = Radius*Utility.Sin(_beta);

		// Vertex 4
		SetVertexData(VertexOffset+4, new Vector3(x3.x, y1, z3.x), opacity);
		// Vertex 5
		SetVertexData(VertexOffset+5, new Vector3(x3.y, y1, z3.y), opacity);

		// Vertex 6
		SetVertexData(VertexOffset+6, new Vector3(0, Radius, 0), opacity);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="p"></param>
        /// <param name="o"></param>
        private void SetVertexData(int index, Vector3 p, float o)
        {
            // Position
            _vertices[index].X = p.x;
            _vertices[index].Y = p.y;
            _vertices[index].Z = p.z;

            // 3D coords (Z-UP)
            float scale = _VClouds.CloudFieldScale / _radius;
            _vertices[index].XC = (p.x + _worldOffset.x) * scale;
            _vertices[index].YC = (p.z + _worldOffset.y) * scale;
            _vertices[index].ZC = Utility.Clamp<float>((p.y / _height) * 0.5f, 1.0f, 0f);

            // Noise coords
            float noise_scale = _VClouds.NoiseScale / _radius; //0.000175f;
            float xz_length_radius = new Vector2(p.x, p.z).Length() / _radius;
            Vector3 origin = new Vector3(0, (_entity != null && _entity.ParentSceneNode != null) ? 
                -(_entity.ParentSceneNode.DerivedPosition.y - _VClouds.Camera.DerivedPosition.y) 
                - _radius * (0.5f + 0.5f * new Vector2(p.x, p.z).Length() / _radius) : -100, 0);
            Vector3 dir = (p - origin).NormalizedCopy();
            float hip = Utility.Sqrt(Utility.Pow(xz_length_radius * _radius, 2) + Utility.Pow(origin.y, 2));
            Vector3 uv = dir * hip; // Only x/z, += origin don't needed
            float far_scalemultiplier = 1 - 0.5f * xz_length_radius;
            if (xz_length_radius < 0.01f) far_scalemultiplier -= 0.25f * 100 * (0.01f - xz_length_radius);
            _vertices[index].U = (uv.x * far_scalemultiplier + _worldOffset.x) * noise_scale;
            _vertices[index].V = (uv.z * far_scalemultiplier + _worldOffset.y) * noise_scale;

            // Opacity
            _vertices[index].O = o * _VClouds.GlobalOpacity;
        }
    }
}
