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
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;

#endregion

namespace AxiomContrib.Samples.Jitter
{
    public class QuadDrawer : IDisposable
    {
        private JitterSample _sample;

        private struct VertexPositionNormalTexture
        {
            public Vector3 Position;
            public Vector3 Normal;
            public Vector2 TextureCoordinate;
        };

        private float size = 100.0f;

        private VertexPositionNormalTexture[] vertices;
        private int[] indices;

        public QuadDrawer( JitterSample sample, float size )
        {
            _sample = sample;
            this.size = size;
            BuildVertices();
            InitializePrimitive();
        }

        private void BuildVertices()
        {
            vertices = new VertexPositionNormalTexture[ 4 ];
            indices = new int[ 6 ];

            vertices[ 0 ].Position = Vector3.NegativeUnitZ + Vector3.NegativeUnitX;
            vertices[ 0 ].TextureCoordinate = new Vector2( 0.0f, 1.0f );
            vertices[ 1 ].Position = Vector3.UnitZ + Vector3.NegativeUnitX;
            vertices[ 1 ].TextureCoordinate = new Vector2( 0.0f, 0.0f );
            vertices[ 2 ].Position = Vector3.NegativeUnitZ + Vector3.UnitX;
            vertices[ 2 ].TextureCoordinate = new Vector2( 1.0f, 1.0f );
            vertices[ 3 ].Position = Vector3.UnitZ + Vector3.UnitX;
            vertices[ 3 ].TextureCoordinate = new Vector2( 1.0f, 0.0f );

            for ( int i = 0; i < vertices.Length; i++ )
            {
                vertices[ i ].Normal = Vector3.UnitY;
                vertices[ i ].Position *= size;
                vertices[ i ].TextureCoordinate *= size;
            }

            indices[ 5 ] = 0; indices[ 4 ] = 1; indices[ 3 ] = 2;
            indices[ 2 ] = 2; indices[ 1 ] = 1; indices[ 0 ] = 3;
        }

        private void InitializePrimitive()
        {
            ManualObject primitive = _sample.SceneManager.CreateManualObject( this.GetType().Name );
            primitive.Begin( @"JitterSample/QuadDrawer", OperationType.TriangleList );

            //Vertices and normals
            foreach ( VertexPositionNormalTexture current in vertices )
            {
                primitive.Position( current.Position );
                primitive.Normal( current.Normal );
                primitive.TextureCoord( current.TextureCoordinate );
            }

            //Indices
            foreach ( ushort idx in indices )
            {
                primitive.Index( idx );
            }

            primitive.End();

            SceneNode n = _sample.SceneManager.RootSceneNode.CreateChildSceneNode();
            n.AttachObject( primitive );
        }

        public void Dispose()
        {
            _sample = null;
        }
    }
}
