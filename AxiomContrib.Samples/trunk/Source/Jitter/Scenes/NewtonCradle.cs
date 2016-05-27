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

using Jitter;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.Scenes
{
    public class NewtonCradle : Scene
    {
		public NewtonCradle( JitterSample demo )
            : base( demo )
        {
        }

        public override void Build()
        {
            this.Sample.PhysicWorld.Solver = World.SolverType.Sequential;

            AddGround();

            RigidBody boxb = new RigidBody( new BoxShape( 7, 1, 2 ) );
            boxb.Position = new JVector( 3.0f, 12, 0 );
            this.Sample.PhysicWorld.AddBody( boxb );
            boxb.Tag = BodyTag.DontDrawMe;

            boxb.IsStatic = true;

            this.Sample.PhysicWorld.Solver = global::Jitter.World.SolverType.Sequential;

            SphereShape shape = new SphereShape( 0.501f );

            for ( int i = 0; i < 7; i++ )
            {
                RigidBody body = new RigidBody( shape );
                body.Position = new JVector( i, 6, 0 );

                DistanceConstraint dc1 = new DistanceConstraint( boxb, body, body.Position + JVector.Up * 6 + JVector.Backward * 5 + JVector.Down * 0.5f, body.Position );
                dc1.Softness = 1.0f;

                DistanceConstraint dc2 = new DistanceConstraint( boxb, body, body.Position + JVector.Up * 6 + JVector.Forward * 5 + JVector.Down * 0.5f, body.Position );
                dc2.Softness = 1.0f;

                dc1.BiasFactor = dc2.BiasFactor = 0.8f;

                dc1.IsMaxDistance = dc2.IsMaxDistance = false;

                this.Sample.PhysicWorld.AddBody( body );
                this.Sample.PhysicWorld.AddConstraint( dc1 );
                this.Sample.PhysicWorld.AddConstraint( dc2 );

                body.Restitution = 1.0f;
                body.StaticFriction = 1.0f;
            }
        }

        public override void Destroy()
        {
            this.Sample.PhysicWorld.Solver = World.SolverType.Simultaneous;

            RemoveGround();
            this.Sample.PhysicWorld.Clear();
        }
    }
}
