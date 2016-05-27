#region License
/*
    Gorilla
    -------
    
    Copyright (c) 2010 Robin Southern
 
    This is a c# (Axiom) port of Gorrilla, developed by Robin Southern, ported by me (bostich)

    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:
                                                                                  
    The above copyright notice and this permission notice shall be included in
    all copies or substantial portions of the Software.
                                                                                  
    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
    THE SOFTWARE. 
 */
#endregion

#region SVN Version Information
// <file>
//     <id value="$Id: 2118 2010-09-26 23:56:56Z bostich $"/>
// </file>
#endregion SVN Version Information

#region Namespace Declarations
using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Core.Collections;
using Axiom.Graphics;
using Axiom.Math;
#endregion
namespace SharpGorilla
{
    /// <summary>
    /// 
    /// </summary>
    public class ScreenRendable : SimpleRenderable, ILayerContainer
    {
        #region - LayerContainer -
        /// <summary>
        /// Master copy of all layers of this Target.
        /// </summary>
        protected List<Layer> _layers = new List<Layer>();
        /// <summary>
        /// Copies pointers to Layers arranged their index.
        /// </summary>
        protected Dictionary<int, IndexData> _indexData = new Dictionary<int, IndexData>();
        /// <summary>
        /// All indexes need to be redrawn regardless of state.
        /// </summary>
        protected bool _indexRedrawNeeded;
        /// <summary>
        /// Compiled layers of all indexes go into here for rendering directly to the screen or scene.
        /// </summary>
        protected HardwareVertexBuffer _vertexBuffer;
        /// <summary>
        /// All indexes need to be redrawn regardless of state.
        /// </summary>
        protected bool _indexRadrawAll;
        /// <summary>
        /// How much the VertexBuffer can hold.
        /// </summary>
        protected int _vertexBufferSize;
        /// <summary>
        /// Pointer to the RenderOperation (Not owned by LayerContainer)
        /// </summary>
        protected RenderOperation _renderOp;
        /// <summary>
        /// Atlas assigned to this LayerContainer
        /// </summary>
        protected TextureAtlas _atlas;
        /// <summary>
        /// Gets atlas assigned to this LayerContainer
        /// </summary>
        public TextureAtlas Atlas
        {
            get { return _atlas; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Real TexelOffsetX
        {
            get { return _renderSystem.HorizontalTexelOffset; }
        }
        /// <summary>
        /// 
        /// </summary>
        public Real TexelOffsetY
        {
            get { return _renderSystem.VerticalTexelOffset; }
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        protected SceneManager _sceneManager;
        protected RenderSystem _renderSystem;
        protected Viewport _viewport;
        protected Vector2 _maxSize;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSize"></param>
        /// <param name="atlas"></param>
        public ScreenRendable(Vector2 maxSize, TextureAtlas atlas)
        {
            _atlas = atlas;
            _maxSize = maxSize;
            box = new AxisAlignedBox();
            _renderSystem = Root.Instance.RenderSystem;
            base.Material = _atlas.Material3D;
            base.material.Load();
            CreateVertexBuffer();
        }
    
        /// <summary>
        /// 
        /// </summary>
        public void FrameStarted()
        {
            RenderOnce();
        }
        public override RenderOperation RenderOperation
        {
            get
            {
                return _renderOp;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void CalculateBoundingBox()
        {
            IndexData indexData;
            box.SetExtents(Vector3.Zero, Vector3.Zero);
            int i = 0;

            foreach (KeyValuePair<int, IndexData> it in _indexData)
            {
                indexData = it.Value;
                for (i = 0; i < indexData.Vertices.Size; i++)
                {
                    box.Merge(indexData.Vertices[i].Position);
                }
            }

            if (!box.IsNull)
            {
                box.Merge(new Vector3(0, 0, -0.25f));
                box.Merge(new Vector3(0, 0, 0.25f));
            }

            SceneNode node = ParentSceneNode;
            if (node != null)
                node.NeedUpdate();
        }

        /// <summary>
        /// 
        /// </summary>
        public override float BoundingRadius
        {
            get { return box.Maximum.LengthSquared; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="camera"></param>
        /// <returns></returns>
        public override  float GetSquaredViewDepth(Camera camera)
        {
            Vector3 min, max, mid, dist = Vector3.Zero;

            min = box.Minimum;
            max = box.Maximum;
            mid = ((max - min) * 0.5f) + min;
            dist = camera.DerivedPosition - mid;
            return dist.LengthSquared;
        }

        #region - LayerContainer -
        /// <summary>
        /// Create a layer for drawing on to.
        /// <para></para>    
        /// Index represents the z-order, 0 being the layer drawn first and 15<para></para>   
        /// the layer drawn last. Layers drawn after another layer will appear<para></para>   
        /// to be top than the other.<para></para>   
        /// Note:<para></para>   
        /// Index must be between or equal to 0 and 15. Any other value will cause
        /// a very nasty crash.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Layer CreateLayer()
        {
            return CreateLayer(0);
        }

        /// <summary>
        /// Create a layer for drawing on to.
        /// <para></para>    
        /// Index represents the z-order, 0 being the layer drawn first and 15<para></para>   
        /// the layer drawn last. Layers drawn after another layer will appear<para></para>   
        /// to be top than the other.<para></para>   
        /// Note:<para></para>   
        /// Index must be between or equal to 0 and 15. Any other value will cause
        /// a very nasty crash.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Layer CreateLayer(int index)
        {
            Layer layer = new Layer(index, this);
            layer.Index = index;
            _layers.Add(layer);

            IndexData data;
            if (!_indexData.TryGetValue(index, out data))
            {
                data = new IndexData();
                data.Layers = new List<Layer>();
                data.Layers.Add(layer);
                data.Vertices = new DynamicBuffer<Vertex>();
                data.RedrawNeeded = true;
            }
            else
            {
                data.Layers.Add(layer);
                data.RedrawNeeded = true;
            }
            _indexData[index] = data;
            _indexRedrawNeeded = true;
            return layer;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="layer"></param>
        public void Destroy(ref Layer layer)
        {
            if (layer == null)
                return;

            if (_indexData.ContainsKey(layer.Index))
                _indexData.Remove(layer.Index);

            layer = null;
        }

        /// <summary>
        /// Create the vertex buffer
        /// </summary>
        public void CreateVertexBuffer()
        {
            CreateVertexBuffer(32);
        }

        /// <summary>
        /// Create the vertex buffer
        /// </summary>
        /// <param name="initialSize">size fo the vertexbuffer</param>
        public void CreateVertexBuffer(int initialSize)
        {
            _vertexBufferSize = initialSize * 6;
            _renderOp = new RenderOperation();
            _renderOp.vertexData = new VertexData();
            _renderOp.vertexData.vertexStart = 0;

            VertexDeclaration vertexDecl = _renderOp.vertexData.vertexDeclaration;
            int offset = 0;

            //position
            vertexDecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);
            offset += VertexElement.GetTypeSize(VertexElementType.Float3);

            //Color
            vertexDecl.AddElement(0, offset, VertexElementType.Float4, VertexElementSemantic.Diffuse);
            offset += VertexElement.GetTypeSize(VertexElementType.Float4);

            //Texture Coordinates
            vertexDecl.AddElement(0, offset, VertexElementType.Float2, VertexElementSemantic.TexCoords);

            _vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(vertexDecl.GetVertexSize(0),
                _vertexBufferSize, BufferUsage.DynamicWriteOnlyDiscardable, false);

            _renderOp.vertexData.vertexBufferBinding.SetBinding(0, _vertexBuffer);
            _renderOp.operationType = OperationType.TriangleList;
            _renderOp.useIndices = false;
        }
        /// <summary>
        /// Destroy the vertex buffer
        /// </summary>
        public void DestroyVertexBuffer()
        {
            _renderOp.vertexData = null;
            _vertexBuffer = null;
            _vertexBufferSize = 0;
        }
        /// <summary>
        ///   Resize the vertex buffer to the greatest nearest power 
        /// of 2 of requestedSize.
        /// </summary>
        public void ResizeVertexBuffer(int requestedSize)
        {
            if (_vertexBufferSize == 0)
                CreateVertexBuffer();

            if (requestedSize > _vertexBufferSize)
            {
                int newVertexBufferSize = 1;

                while (newVertexBufferSize < requestedSize)
                    newVertexBufferSize <<= 1;

                _vertexBuffer = HardwareBufferManager.Instance.CreateVertexBuffer(
                    _renderOp.vertexData.vertexDeclaration.GetVertexSize(0),
                    newVertexBufferSize, BufferUsage.DynamicWriteOnlyDiscardable, false);

                _vertexBufferSize = newVertexBufferSize;
                _renderOp.vertexData.vertexStart = 0;
                _renderOp.vertexData.vertexBufferBinding.SetBinding(0, _vertexBuffer);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public void ReCalculateIndexes()
        {
            // Clear all index data.
            foreach (KeyValuePair<int, IndexData> index_data in _indexData)
            {
				throw new NotImplementedException();
                index_data.Value.Vertices.Clear();
                index_data.Value.Layers.Clear();
                //index_data.Value.SetRedrawNeeded(false);
            }
            // Loop through layers, and add them to IndexData
            foreach (Layer layer in _layers)
            {
                IndexData data;
                if (!_indexData.TryGetValue(layer.Index, out data))
                {
                    data = new IndexData();
                    data.Layers = new List<Layer>();
                    data.Vertices = new DynamicBuffer<Vertex>();
                    _indexData[layer.Index] = data;
                }

                data.Layers.Add(layer);
            }

            // Prune any index data that is not.
            bool deleted = false;
            List<int> toRemove = new List<int>();
            while (true)
            {
                deleted = false;
                foreach (KeyValuePair<int, IndexData> index_data in _indexData)
                {
                    if (index_data.Value.Layers.Count == 0)
                    {
                        toRemove.Add(index_data.Key);
                        deleted = true;
                        break;
                    }
                }

                if (!deleted)
                    break;
            }
            //we need todo it this way, because we can not delete from indexdata while iteration
            if (toRemove.Count > 0)
            {
                for (int i = 0; i < toRemove.Count; i++)
                {
                    _indexData.Remove(toRemove[i]);
                }
            }
            toRemove = null;
            _indexRadrawAll = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="force"></param>
        public void RedrawIndex(int id, bool force)
        {
            IndexData data;
            if (!_indexData.TryGetValue(id, out data))
                return;

            data.Vertices.Clear();
            data.RedrawNeeded = false;
            for (int i = 0; i < data.Layers.Count; i++)
            {
                if (data.Layers[i].IsVisible)
                    data.Layers[i].Render(ref data.Vertices, force);
            }
            _indexData[id] = data;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RedrawAllIndexs()
        {
            RedrawAllIndexs(false);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="force"></param>
		public void RedrawAllIndexs( bool force)
		{
			_indexRedrawNeeded = false;
			List<int> vals = new List<int>();
			foreach ( KeyValuePair<int, IndexData> it in _indexData )
			{
				IndexData indexData = it.Value;
				if ( it.Value.RedrawNeeded || force )
				{
					it.Value.Vertices.Clear();
					vals.Add( it.Key );
					for ( int i = 0; i < indexData.Layers.Count; i++ )
					{
						if ( indexData.Layers[ i ].IsVisible )
							indexData.Layers[ i ].Render( ref indexData.Vertices, force );
					}
				}
			}
			for ( int i = 0; i < vals.Count; i++ )
			{
				IndexData dt = _indexData[ vals[ i ] ];
				dt.RedrawNeeded = false;
				_indexData[ vals[ i ] ] = dt;
			}
		}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        public void RequestIndexRedraw(int index)
        {
            IndexData data;
            if (!_indexData.TryGetValue(index, out data))
                return;

            data.RedrawNeeded = true;
            _indexData[index] = data;
            _indexRedrawNeeded = true;
        }

        /// <summary>
        /// 
        /// </summary>
        public void RenderVertices()
        {
           RenderVertices(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="force"></param>
        public void RenderVertices(bool force)
        {
            if (!_indexRedrawNeeded)
                if (!force)
                    return;

            RedrawAllIndexs(force);

            int knownVertexCount = 0;
            foreach (KeyValuePair<int, IndexData> it in _indexData)
                knownVertexCount += it.Value.Vertices.Size;

            ResizeVertexBuffer(knownVertexCount);

            unsafe
            {
                Vertex* writeIterator = (Vertex*)_vertexBuffer.Lock(BufferLocking.Discard);
                int i = 0;
                IndexData indexData;
                foreach (KeyValuePair<int, IndexData> it in _indexData)
                {
                    indexData = it.Value;
                    for (i = 0; i < indexData.Vertices.Size; i++)
                        *writeIterator++ = indexData.Vertices[i];
                }
                _vertexBuffer.Unlock();
                _renderOp.vertexData.vertexCount = knownVertexCount;
            }
        }
        /// <summary>
        /// Draw the vertices from mVertexBuffer into Axiom.
        /// </summary>
        public void RenderOnce()
        {
            if (_indexRedrawNeeded)
            {
                RenderVertices(false);
                CalculateBoundingBox();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        public virtual void Transform(ref DynamicBuffer<Vertex> vertices, int begin, int end)
        {
            Vector2 halfSize = _maxSize * 0.5f;
            for (int i = begin; i < end; i++)
            {
                float x = (vertices[i].Position.x * 0.01f) - halfSize.x;
                float y = (vertices[i].Position.y * -0.01f) + halfSize.y;
                Vertex v = vertices[i];
                v.Position = new Vector3(x, y, 0);
                vertices[i] = v;
            }
        }
        #endregion
    }
}
