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

#endregion

namespace AxiomContrib.Samples.Jitter.Primitives3D
{
    /// <summary>
    /// Geometric primitive class for drawing cones.
    /// </summary>
    public class ConePrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, using default settings.
        /// </summary>
        public ConePrimitive( JitterSample sample )
            : this( sample, 1.0f, 1.0f, 32 )
        {
        }

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public ConePrimitive( JitterSample sample, float height, float radius, int tessellation )
            : base( sample, PrimitiveType.Cone )
        {
            // Create a ring of triangles around the outside of the cylinder.
            AddVertex( Vector3.UnitY * ( 2.0f / 3.0f ) * height, Vector3.UnitY );

            for ( int i = 0; i < tessellation; i++ )
            {
                Vector3 normal = GetCircleVector( i, tessellation );
                AddVertex( normal * radius + ( 1.0f / 3.0f ) * height * Vector3.NegativeUnitY, normal );

                AddIndex( i + 1 );
                AddIndex( i );
                AddIndex( 0 );
            }

            AddIndex( 1 );
            AddIndex( tessellation );
            AddIndex( 0 );

            CreateCap( tessellation, ( 1.0f / 3.0f ) * height, radius, Vector3.NegativeUnitY );

            //InitializePrimitive();
        }

        /// <summary>
        /// Helper method creates a triangle fan to close the ends of the cylinder.
        /// </summary>
        void CreateCap( int tessellation, float height, float radius, Vector3 normal )
        {
            // Create cap indices.
            for ( int i = 0; i < tessellation - 2; i++ )
            {
                if ( normal.y > 0 )
                {
                    AddIndex( CurrentVertex + ( i + 2 ) % tessellation );
                    AddIndex( CurrentVertex + ( i + 1 ) % tessellation );
                    AddIndex( CurrentVertex );
                }
                else
                {
                    AddIndex( CurrentVertex + ( i + 1 ) % tessellation );
                    AddIndex( CurrentVertex + ( i + 2 ) % tessellation );
                    AddIndex( CurrentVertex );
                }
            }

            // Create cap vertices.
            for ( int i = 0; i < tessellation; i++ )
            {
                Vector3 position = GetCircleVector( i, tessellation ) * radius +
                                   normal * height;

                AddVertex( position, normal );
            }
        }


        /// <summary>
        /// Helper method computes a point on a circle.
        /// </summary>
        static Vector3 GetCircleVector( int i, int tessellation )
        {
            float angle = i * Utility.TWO_PI / tessellation;

            float dx = (float)System.Math.Cos( angle );
            float dz = (float)System.Math.Sin( angle );

            return new Vector3( dx, 0, dz );
        }
    }
}
