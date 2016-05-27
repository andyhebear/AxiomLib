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

using Axiom.Math;
using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

#endregion

namespace AxiomContrib.Samples.Jitter.Scenes
{
    class Tower : Scene
    {
		public Tower( JitterSample demo )
            : base( demo )
        {
        }

        public override void Build()
        {
            AddGround();

            World world = Sample.PhysicWorld;

            world.SetIterations( 10 );

            Matrix4 halfRotationStep = Matrix4.Compose( Vector3.Zero, Vector3.UnitScale, Quaternion.FromAngleAxis( System.Math.PI * 2.0f / 24.0f, Vector3.UnitY ) );    //Matrix.CreateRotationY( System.Math.PI * 2.0f / 24.0f );
            Matrix4 fullRotationStep = halfRotationStep * halfRotationStep;
            Matrix4 orientation = Matrix4.Identity;

            BoxShape shape = new BoxShape( 2, 1, 1 );

            for ( int e = 0; e < 20; e++ )
            {
                orientation *= halfRotationStep;

                for ( int i = 0; i < 12; i++ )
                {
                    Vector3 position = orientation * new Vector3( 0, 0.5f + e, 6 );// Vector3.Transform( new Vector3( 0, 0.5f + e, 6 ), orientation );

                    RigidBody body = new RigidBody( shape );
                    body.Orientation = Conversion.ToJitterMatrix( orientation );
                    body.Position = Conversion.ToJitterVector( position );

                    world.AddBody( body );

                    orientation *= fullRotationStep;
                }
            }
            ground.StaticFriction = 1.0f;
            ground.DynamicFriction = 1.0f;
        }

        public override void Destroy()
        {
            RemoveGround();
            Sample.PhysicWorld.Clear();
        }
    }
}