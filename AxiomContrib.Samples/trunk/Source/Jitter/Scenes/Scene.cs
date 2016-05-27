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

namespace AxiomContrib.Samples.Jitter.Scenes
{
    public abstract class Scene
    {
        public JitterSample Sample { get; private set; }

        public Scene( JitterSample demo )
        {
            this.Sample = demo;
        }

        public abstract void Build();
        public abstract void Destroy();

        public void Restore()
        {
            Destroy();
            Build();
        }

        private QuadDrawer quadDrawer = null;
        protected RigidBody ground = null;

        public void AddGround()
        {
            ground = new RigidBody( new BoxShape( 300, 20, 300 ) );
            ground.Position = new JVector( 0, -10, 0 );
            ground.Tag = BodyTag.DontDrawMe;
            ground.IsStatic = true;
            Sample.PhysicWorld.AddBody( ground );
            ground.DynamicFriction = 0.0f;

            quadDrawer = new QuadDrawer( this.Sample, 100 );
        }

        public void RemoveGround()
        {
            Sample.PhysicWorld.RemoveBody( ground );
            if ( quadDrawer != null ) 
				quadDrawer.Dispose();
        }

        public virtual void PhysicUpdate() { }
    }
}
