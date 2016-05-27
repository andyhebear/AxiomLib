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

using Axiom.Core;
using Axiom.Math;
using AxiomContrib.Samples.Jitter.Primitives3D;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter.Scenes
{
	public class Terrain : Scene
	{
		TerrainPrimitive terrain;

		public Terrain( JitterSample demo )
			: base( demo )
		{
		}

		public override void Build()
		{
			terrain = new TerrainPrimitive( this.Sample, ( ( a, b ) =>
			{
				return (float)( System.Math.Cos( a * 0.2f ) * System.Math.Sin( b * 0.2f ) * 2.0f );
			} ) );

			TerrainShape shape = new TerrainShape( terrain.heights, 1.0f, 1.0f );
			RigidBody body = new RigidBody( shape );
			body.Position -= new JVector( 50, 0, 50 );
			body.IsStatic = true;
			body.Tag = BodyTag.DontDrawMe;
			Sample.PhysicWorld.AddBody( body );


			for ( int i = 0; i < 15; i++ )
			{
				for ( int e = 0; e < 15; e++ )
				{
					RigidBody sphere = new RigidBody( new SphereShape( 0.1f ) );
					this.Sample.PhysicWorld.AddBody( sphere );

					sphere.Position = new JVector( -15 + e * 2f, 10, -15 + i * 2f );
				}
				bool even = ( i % 2 == 0 );

				for ( int e = 0; e < 3; e++ )
				{
					JVector size = ( even ) ? new JVector( 1, 1, 3 ) : new JVector( 3, 1, 1 );
					body = new RigidBody( new BoxShape( size ) );
					body.Position = new JVector( even ? e : 1.0f, i + 2.5f, even ? 1.0f : e );

					Sample.PhysicWorld.AddBody( body );
				}
			}
		}

		public override void Destroy()
		{
			Sample.PhysicWorld.Clear();
		}

		public override void PhysicUpdate()
		{
			Matrix4 trans = Matrix4.Identity;
			trans.Translation = new Vector3( -50, 0, -50 );

			//terrain.PhysicUpdate( trans );
			//Demo.BasicEffect.DiffuseColor = ColorEx.Red;//.ToVector3();
			terrain.ResetIndex();
			base.PhysicUpdate();
		}
	}
}
