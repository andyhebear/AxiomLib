#region MIT License
/*
-----------------------------------------------------------------------------
This source file is part of the Jitter Sample
Jitter Physics Engine Copyright (c) 2010 Thorben Linneweber (admin@jitter-physics.com)

This a port for Axiom of samples using Jitter Physics Engine,
developed by Thorben Linneweber and ported by Francesco Guastella (aka romeoxbm).
 
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
-----------------------------------------------------------------------------
*/
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;

#endregion

namespace AxiomContrib.Samples.Jitter.Primitives3D
{
	public enum PrimitiveType : int
	{
		Box = 0,
		Sphere = 1,
		Cylinder = 2,
		Cone = 3,
		Capsule = 4,
		Count = 5
	}

	/// <summary>
	/// Base class for simple geometric primitive models. This provides a vertex
	/// buffer, an index buffer, plus methods for drawing the model. Classes for
	/// specific types of primitive (CubePrimitive, SpherePrimitive, etc.) are
	/// derived from this common base, and use the AddVertex and AddIndex methods
	/// to specify their geometry.
	/// </summary>
	public abstract class GeometricPrimitive : IDisposable
	{
		#region Fields

		private List<SceneNode> _nodes = new List<SceneNode>();
		private int _currentIndex = 0;

		protected JitterSample _sample { get; private set; }

		// During the process of constructing a primitive model, vertex
		// and index data is stored on the CPU in these managed lists.
		private List<Vector3> _vertexNormalSet = new List<Vector3>();
		private List<ushort> _indexSet = new List<ushort>();

		#endregion

		public PrimitiveType Type { get; private set; }

		protected GeometricPrimitive( JitterSample sample, PrimitiveType type )
		{
			this._sample = sample;
			this.IsDisposed = false;
			this.Type = type;
		}

		/// <summary>
		/// Adds a new vertex to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddVertex( Vector3 position, Vector3 normal )
		{
			_vertexNormalSet.Add( position );
			_vertexNormalSet.Add( normal );
		}


		/// <summary>
		/// Adds a new index to the primitive model. This should only be called
		/// during the initialization process, before InitializePrimitive.
		/// </summary>
		protected void AddIndex( int index )
		{
			if ( index > ushort.MaxValue )
				throw new ArgumentOutOfRangeException( "index" );

			_indexSet.Add( (ushort)index );
		}


		/// <summary>
		/// Queries the index of the current vertex. This starts at
		/// zero, and increments every time AddVertex is called.
		/// </summary>
		protected int CurrentVertex
		{
			get { return _vertexNormalSet.Count / 2; }
		}

		/// <summary>
		/// Once all the geometry has been specified by calling AddVertex and AddIndex,
		/// this method copies the vertex and index data into GPU format buffers, ready
		/// for efficient rendering.
		private ManualObject InitializePrimitive()
		{
			ManualObject primitive = _sample.SceneManager.CreateManualObject( this.GetType().Name + this._nodes.Count );
			primitive.Begin( "JitterSample/Geometries", OperationType.TriangleList );

			//Vertices and normals
			for ( int i = 0; i < _vertexNormalSet.Count; i += 2 )
			{
				primitive.Position( _vertexNormalSet[ i ] );
				primitive.Normal( _vertexNormalSet[ i + 1 ] );
			}

			//Indices
			foreach ( ushort idx in _indexSet )
			{
				primitive.Index( idx );
			}

			primitive.End();
			return primitive;
		}

		public void PhysicUpdate( Vector3 pos, Quaternion orientation, Vector3 scale )
		{
			if ( _currentIndex == _nodes.Count )
			{
				SceneNode n = this._sample.SceneManager.RootSceneNode.CreateChildSceneNode();
				n.AttachObject( this.InitializePrimitive() );
				_nodes.Add( n );
			}

			_nodes[ _currentIndex ].Scale = scale;
			_nodes[ _currentIndex ].Orientation = orientation;
			_nodes[ _currentIndex ].Position = pos;
			_currentIndex++;
		}

		public void ResetIndex()
		{
			_currentIndex = 0;
		}

		#region IDisposable Members

		public bool IsDisposed { get; private set; }

		public virtual void Dispose()
		{
			if ( !this.IsDisposed )
			{
				//Do cleaning up
				_nodes.Clear();
				_nodes = null;

				_vertexNormalSet.Clear();
				_vertexNormalSet = null;

				_indexSet.Clear();
				_indexSet = null;

				_sample = null;
			}

			this.IsDisposed = true;
		}

		#endregion
	}
}
