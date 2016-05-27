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
using Jitter.Dynamics.Joints;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.Scenes
{
    class Ragdoll : Scene
    {
		public Ragdoll( JitterSample demo )
            : base( demo )
        {
        }

        public override void Build()
        {
            AddGround();

            for ( int i = 3; i < 8; i++ )
            {
                for ( int e = 3; e < 8; e++ )
                {
                    BuildRagdoll( Sample.PhysicWorld, new JVector( i * 6 - 25, 5, e * 6 - 25 ) );
                }
            }
        }

        public void BuildRagdoll( World world, JVector position )
        {
            // the torso
            RigidBody torso = new RigidBody( new BoxShape( 1.5f, 3, 0.5f ) );
            torso.Position = position;

            // the head
            RigidBody head = new RigidBody( new SphereShape( 0.5f ) );
            head.Position = position + new JVector( 0, 2.1f, 0 );

            // connect head and torso
            DistanceConstraint headTorso = new DistanceConstraint( head, torso,
                position + new JVector( 0, 1.6f, 0 ), position + new JVector( 0, 1.5f, 0 ) );

            RigidBody arm1 = new RigidBody( new CapsuleShape( 0.8f, 0.2f ) );
            arm1.Position = position + new JVector( 1.0f, 0.75f, 0 );

            RigidBody arm2 = new RigidBody( new CapsuleShape( 0.8f, 0.2f ) );
            arm2.Position = position + new JVector( -1.0f, 0.75f, 0 );

            RigidBody lowerarm1 = new RigidBody( new CapsuleShape( 0.6f, 0.2f ) );
            lowerarm1.Position = position + new JVector( 1.0f, -0.45f, 0 );

            RigidBody lowerarm2 = new RigidBody( new CapsuleShape( 0.6f, 0.2f ) );
            lowerarm2.Position = position + new JVector( -1.0f, -0.45f, 0 );

            PointConstraint arm1torso = new PointConstraint( arm1, torso, position + new JVector( 0.9f, 1.4f, 0 ) );
            PointConstraint arm2torso = new PointConstraint( arm2, torso, position + new JVector( -0.9f, 1.4f, 0 ) );

            HingeJoint arm1Hinge = new HingeJoint( world, arm1, lowerarm1, position + new JVector( 1.0f, 0.05f, 0 ), JVector.Right );
            HingeJoint arm2Hinge = new HingeJoint( world, arm2, lowerarm2, position + new JVector( -1.0f, 0.05f, 0 ), JVector.Right );

            RigidBody leg1 = new RigidBody( new CapsuleShape( 1.0f, 0.3f ) );
            leg1.Position = position + new JVector( -0.5f, -2.4f, 0 );

            RigidBody leg2 = new RigidBody( new CapsuleShape( 1.0f, 0.3f ) );
            leg2.Position = position + new JVector( 0.5f, -2.4f, 0 );

            PointConstraint leg1torso = new PointConstraint( leg1, torso, position + new JVector( -0.5f, -1.6f, 0 ) );
            PointConstraint leg2torso = new PointConstraint( leg2, torso, position + new JVector( +0.5f, -1.6f, 0 ) );

            RigidBody lowerleg1 = new RigidBody( new CapsuleShape( 0.8f, 0.3f ) );
            lowerleg1.Position = position + new JVector( -0.5f, -4.0f, 0 );

            RigidBody lowerleg2 = new RigidBody( new CapsuleShape( 0.8f, 0.3f ) );
            lowerleg2.Position = position + new JVector( +0.5f, -4.0f, 0 );

            HingeJoint leg1Hinge = new HingeJoint( world, leg1, lowerleg1, position + new JVector( -0.5f, -3.35f, 0 ), JVector.Right );
            HingeJoint leg2Hinge = new HingeJoint( world, leg2, lowerleg2, position + new JVector( 0.5f, -3.35f, 0 ), JVector.Right );

            lowerleg1.IsActive = false;
            lowerleg2.IsActive = false;
            leg1.IsActive = false;
            leg2.IsActive = false;
            head.IsActive = false;
            torso.IsActive = false;
            arm1.IsActive = false;
            arm2.IsActive = false;
            lowerarm1.IsActive = false;
            lowerarm2.IsActive = false;

            world.AddBody( head ); world.AddBody( torso );
            world.AddBody( arm1 ); world.AddBody( arm2 );
            world.AddBody( lowerarm1 ); world.AddBody( lowerarm2 );
            world.AddBody( leg1 ); world.AddBody( leg2 );
            world.AddBody( lowerleg1 ); world.AddBody( lowerleg2 );

            arm1Hinge.Activate(); arm2Hinge.Activate();
            leg1Hinge.Activate(); leg2Hinge.Activate();

            world.AddConstraint( headTorso );
            world.AddConstraint( arm1torso );
            world.AddConstraint( arm2torso );
            world.AddConstraint( leg1torso );
            world.AddConstraint( leg2torso );
        }

        public override void Destroy()
        {
            RemoveGround();
            Sample.PhysicWorld.Clear();
        }
    }
}