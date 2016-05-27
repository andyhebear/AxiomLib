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
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.PhysicsObjects
{
    public class ConvexHullObject
    {
        private static ConvexHullShape _cvhs = null;
        private static int _uniqueId = 0;
        private JitterSample _sample;
        private SceneNode _sceneNode;

        public RigidBody body;

        public ConvexHullObject( JitterSample sample )
        {
            _sample = sample;

            Entity convexHullEntity = _sample.SceneManager.CreateEntity( "ConvexHull" + _uniqueId++, "ogrehead.mesh" );
            _sceneNode = this._sample.SceneManager.RootSceneNode.CreateChildSceneNode();
            _sceneNode.Scale = Vector3.UnitScale * 0.1f;

            if ( _cvhs == null )
            {
                List<JVector> vertices;
                List<JOctree.TriangleVertexIndices> indices;

                ExtractData( convexHullEntity.Mesh, out vertices, out indices );

                int[] convexHullIndices = JConvexHull.Build( vertices, JConvexHull.Approximation.Level6 );

                List<JVector> hullPoints = new List<JVector>();
                for ( int i = 0; i < convexHullIndices.Length; i++ )
                {
                    hullPoints.Add( vertices[ convexHullIndices[ i ] ] );
                }

                _cvhs = new ConvexHullShape( hullPoints );
            }
           
            _sceneNode.AttachObject( convexHullEntity );

            body = new RigidBody( _cvhs );
            body.Tag = BodyTag.DontDrawMe;
        }

        public void ExtractData( Mesh model, out List<JVector> vertices, out List<JOctree.TriangleVertexIndices> indices )
        {
            //Retrieving Vertices
            TriangleListBuilder builder = new TriangleListBuilder();
            model.AddVertexAndIndexSets( builder, 0 );
            List<TriangleVertices> triangles = new List<TriangleVertices>( builder.Build() );
            vertices = new List<JVector>();

            foreach ( TriangleVertices triangle in triangles )
            {
                foreach ( Vector3 vertex in triangle.Vertices )
                {
                    JVector jvertex = Conversion.ToJitterVector( vertex * _sceneNode.Scale );
                    vertices.Add( jvertex );
                }
            }

            //Retrieving Indices
            indices = new List<JOctree.TriangleVertexIndices>();

            for ( int i = 0; i < model.SubMeshCount; ++i )
            {
                IndexData indexData = model.GetSubMesh( i ).indexData;
                HardwareIndexBuffer indexBuffer = model.GetSubMesh( i ).indexData.indexBuffer;
                short[] subIndices = new short[ indexData.indexCount ];
                IntPtr ptr = Memory.PinObject( subIndices );
                indexBuffer.ReadData( indexData.indexStart, indexData.indexCount * Memory.SizeOf( typeof( short ) ), ptr );
                Memory.UnpinObject( subIndices );

                //Each TriangleVertexIndices holds the three indexes to each vertex that makes up a triangle 
                JOctree.TriangleVertexIndices[] tvi = new JOctree.TriangleVertexIndices[ indexData.indexCount / 3 ];
                for ( int j = 0; j != tvi.Length; ++j )
                {
                    // The offset is because we are storing them all in the one array and the 
                    // vertices were added to the end of the array. 
                    tvi[ i ].I0 = subIndices[ j * 3 ] + indexData.indexStart;
                    tvi[ i ].I1 = subIndices[ j * 3 + 1 ] + indexData.indexStart;
                    tvi[ i ].I2 = subIndices[ j * 3 + 2 ] + indexData.indexStart;
                }
                // Store our triangles 
                indices.AddRange( tvi );
            }
        }

        public void PhysicUpdate()
        {
            ConvexHullShape hullShape = body.Shape as ConvexHullShape;
            Quaternion orientation = Conversion.ToQuat( body.Orientation );

            // RigidBody.Position gives you the position of the center of mass of the shape.
            // But this is not the center of our graphical represantion, use the
            // "shift" property of the more complex shapes to deal with this.
            Vector3 translation = Conversion.ToVector3( body.Position + JVector.Transform( hullShape.Shift(), body.Orientation ) );

            _sceneNode.Orientation = orientation;
            _sceneNode.Position = translation;
        }
    }
}
