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
using AxiomContrib.Samples.Jitter.Primitives3D;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;

#endregion

namespace AxiomContrib.Samples.Jitter.PhysicsObjects
{
    public class TerrainObject
    {
        private TerrainPrimitive primitive;
        private RigidBody terrainBody;
        private Matrix4 worldMatrix = Matrix4.Identity;
        private JitterSample sample;

        public Matrix4 World
        {
            get
            {
                return worldMatrix;
            }
            set
            {
                worldMatrix = value;
                terrainBody.Orientation = Conversion.ToJitterMatrix( worldMatrix );
                terrainBody.Position = Conversion.ToJitterVector( worldMatrix.Translation );
            }
        }

        public TerrainObject( JitterSample sample )
        {
            this.sample = sample;
        }

        public void Initialize()
        {
            primitive = new TerrainPrimitive( this.sample, ( int a, int b ) =>
                { return (float)( System.Math.Sin( a * 0.1f ) * System.Math.Cos( b * 0.1f ) ) * 3; } );

            TerrainShape terrainShape = new TerrainShape( primitive.heights, 1.0f, 1.0f );

            terrainBody = new RigidBody( terrainShape );
            terrainBody.IsStatic = true;
            terrainBody.Tag = true;

            sample.PhysicWorld.AddBody( terrainBody );

            Matrix4 wTranslation = Matrix4.Identity;
            wTranslation.Translation = new Vector3( -50, 0, -50 );
            World = wTranslation;
        }

        public void Draw( /*GameTime gameTime*/ )
        {
            //effect.DiffuseColor = ColorEx.Blue;//.ToVector3();
            //primitive.PhysicUpdate( worldMatrix );
            primitive.ResetIndex();
        }
    }
}
