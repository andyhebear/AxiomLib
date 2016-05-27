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

using System.Collections.Generic;
using Axiom.Core;
using Axiom.Math;
using Jitter.Collision.Shapes;
using Jitter.LinearMath;
using Jitter.Dynamics;

#endregion

namespace AxiomContrib.Samples.Jitter.Scenes
{
    public class TriangleMesh : Scene
    {
        private SceneNode modelNode;

		public TriangleMesh( JitterSample demo )
            : base( demo )
        {
        }

        /// <summary>
        /// Helper Method to get the vertex and index List from the model.
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="model"></param>
        //public void ExtractData( List<Vector3> vertices, List<JOctree.TriangleVertexIndices> indices, Model model )
        //{
        //    Matrix4[] bones_ = new Matrix4[ model.Bones.Count ];
        //    model.CopyAbsoluteBoneTransformsTo( bones_ );
        //    foreach ( ModelMesh mm in model.Meshes )
        //    {
        //        Matrix4 xform = bones_[ mm.ParentBone.Index ];
        //        foreach ( ModelMeshPart mmp in mm.MeshParts )
        //        {
        //            int offset = vertices.Count;
        //            Vector3[] a = new Vector3[ mmp.NumVertices ];
        //            mmp.VertexBuffer.GetData<Vector3>( mmp.VertexOffset * mmp.VertexBuffer.VertexDeclaration.VertexStride,
        //                a, 0, mmp.NumVertices, mmp.VertexBuffer.VertexDeclaration.VertexStride );
        //            for ( int i = 0; i != a.Length; ++i )
        //                Vector3.Transform( ref a[ i ], ref xform, out a[ i ] );
        //            vertices.AddRange( a );

        //            if ( mmp.IndexBuffer.IndexElementSize != IndexElementSize.SixteenBits )
        //                throw new Exception(
        //                    String.Format( "Model uses 32-bit indices, which are not supported." ) );
        //            short[] s = new short[ mmp.PrimitiveCount * 3 ];
        //            mmp.IndexBuffer.GetData<short>( mmp.StartIndex * 2, s, 0, mmp.PrimitiveCount * 3 );
        //            JOctree.TriangleVertexIndices[] tvi = new JOctree.TriangleVertexIndices[ mmp.PrimitiveCount ];
        //            for ( int i = 0; i != tvi.Length; ++i )
        //            {
        //                tvi[ i ].I0 = s[ i * 3 + 0 ] + offset;
        //                tvi[ i ].I1 = s[ i * 3 + 1 ] + offset;
        //                tvi[ i ].I2 = s[ i * 3 + 2 ] + offset;
        //            }
        //            indices.AddRange( tvi );
        //        }
        //    }
        //}


        public override void PhysicUpdate()
        {
            //foreach ( ModelMesh mesh in modelNode.Meshes )
            //{
            //    foreach ( BasicEffect effect in mesh.Effects )
            //    {
            //        effect.World = boneTransforms[ mesh.ParentBone.Index ];
            //        effect.View = camera.View;
            //        effect.Projection = camera.Projection;

            //        effect.EnableDefaultLighting();
            //        effect.PreferPerPixelLighting = true;
            //    }
            //    mesh.Draw();
            //}
        }

        Matrix4[] boneTransforms;

        public override void Build()
        {
            //modelNode = Demo.Content.Load<Model>( "staticmesh" );
            //boneTransforms = new Matrix4[ modelNode.Bones.Count ];
            //modelNode.CopyAbsoluteBoneTransformsTo( boneTransforms );

            List<JOctree.TriangleVertexIndices> indices = new List<JOctree.TriangleVertexIndices>();
            List<Vector3> vertices = new List<Vector3>();

            //ExtractData( vertices, indices, modelNode );

            List<JVector> jvertices = new List<JVector>( vertices.Count );
            foreach ( Vector3 vertex in vertices )
                jvertices.Add( Conversion.ToJitterVector( vertex ) );

            JOctree octree = new JOctree( jvertices, indices );

            TriangleMeshShape tms = new TriangleMeshShape( octree );
            RigidBody body = new RigidBody( tms );
            body.IsStatic = true;
            body.Tag = BodyTag.DontDrawMe;
            Sample.PhysicWorld.AddBody( body );
        }

        public override void Destroy()
        {
            Sample.PhysicWorld.Clear();
        }
    }
}
