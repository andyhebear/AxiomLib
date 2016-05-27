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

using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

#endregion

namespace Axiom.Samples.JitterSample.Scenes
{
    public class Cloth : Scene
    {
        public Cloth( JitterSample demo )
            : base( demo )
        {
        }

        public override void Build()
        {

            AddGround();

            // we need some of them!
            Demo.World.SetIterations( 5 );

            PseudoCloth pc = new PseudoCloth( Demo.World, 20, 20, 0.5f );

            BoxShape boxShape = new BoxShape( JVector.One );

            RigidBody[] boxes = new RigidBody[ 4 ];

            int size = 19;

            for ( int i = 0; i < 4; i++ )
            {
                boxes[ i ] = new RigidBody( boxShape );
                boxes[ i ].Position = new JVector( i % 2 == 0 ? 10.0f : -0.5f, 10.5f, ( i < 2 ) ? 10.0f : -0.5f );
                // Demo.World.AddBody(boxes[i]);

                if ( i == 0 )
                {
                    pc.GetCorner( size, size ).IsStatic = true;
                }
                else if ( i == 1 )
                {
                    pc.GetCorner( size, 0 ).IsStatic = true;
                }
                else if ( i == 2 )
                {
                    pc.GetCorner( 0, size ).IsStatic = true;
                }
                else if ( i == 3 )
                {
                    pc.GetCorner( 0, 0 ).IsStatic = true;
                }

                boxes[ i ].IsStatic = true;
            }

            RigidBody sphereBody = new RigidBody( new SphereShape( 2.0f ) );
            Demo.World.AddBody( sphereBody );
            sphereBody.Mass = 10.0f;
            sphereBody.Position = new JVector( 5, 20, 5 );

            //ConvexHullObject2 obj2 = new ConvexHullObject2(this.Demo);
            //Demo.Components.Add(obj2);

            //obj2.body.Position = new JVector(5, 30, 5);
            //Demo.World.AddBody(obj2.body);
        }

        public override void Destroy()
        {
            RemoveGround();
            this.Demo.World.Clear();
        }

    }
}
