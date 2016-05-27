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
using Axiom.Math;

#endregion

namespace AxiomContrib.Samples.Jitter.Primitives3D
{
    /// <summary>
    /// Geometric primitive class for drawing spheres.
    /// </summary>
    public class SpherePrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new sphere primitive, using default settings.
        /// </summary>
        public SpherePrimitive( JitterSample sample )
            : this( sample, 1, 16 )
        {
        }

        /// <summary>
        /// Constructs a new sphere primitive,
        /// with the specified size and tessellation level.
        /// </summary>
        public SpherePrimitive( JitterSample sample, float radius, int tessellation )
            : base( sample, PrimitiveType.Sphere )
        {
            if ( tessellation < 3 )
                throw new ArgumentOutOfRangeException( "tessellation" );

            int verticalSegments = tessellation;
            int horizontalSegments = tessellation * 2;

            // Start with a single vertex at the bottom of the sphere.
            AddVertex( Vector3.NegativeUnitY * radius, Vector3.NegativeUnitY );

            // Create rings of vertices at progressively higher latitudes.
            for ( int i = 0; i < verticalSegments - 1; i++ )
            {
                float latitude = ( ( i + 1 ) * System.Math.PI /
                                            verticalSegments ) - Utility.HALF_PI;

                float dy = (float)System.Math.Sin( latitude );
                float dxz = (float)System.Math.Cos( latitude );

                // Create a single ring of vertices at this latitude.
                for ( int j = 0; j < horizontalSegments; j++ )
                {
                    float longitude = j * Utility.TWO_PI / horizontalSegments;

                    float dx = (float)System.Math.Cos( longitude ) * dxz;
                    float dz = (float)System.Math.Sin( longitude ) * dxz;

                    Vector3 normal = new Vector3( dx, dy, dz );

                    AddVertex( normal * radius, normal );
                }
            }

            // Finish with a single vertex at the top of the sphere.
            AddVertex( Vector3.UnitY * radius, Vector3.UnitY );

            // Create a fan connecting the bottom vertex to the bottom latitude ring.
            for ( int i = 0; i < horizontalSegments; i++ )
            {
                AddIndex( 1 + i );
                AddIndex( 1 + ( i + 1 ) % horizontalSegments );
                AddIndex( 0 );
            }

            // Fill the sphere body with triangles joining each pair of latitude rings.
            for ( int i = 0; i < verticalSegments - 2; i++ )
            {
                for ( int j = 0; j < horizontalSegments; j++ )
                {
                    int nextI = i + 1;
                    int nextJ = ( j + 1 ) % horizontalSegments;

                    AddIndex( 1 + nextI * horizontalSegments + j );
                    AddIndex( 1 + i * horizontalSegments + nextJ );
                    AddIndex( 1 + i * horizontalSegments + j );

                    AddIndex( 1 + nextI * horizontalSegments + j );
                    AddIndex( 1 + nextI * horizontalSegments + nextJ );
                    AddIndex( 1 + i * horizontalSegments + nextJ );
                }
            }

            // Create a fan connecting the top vertex to the top latitude ring.
            for ( int i = 0; i < horizontalSegments; i++ )
            {
                AddIndex( CurrentVertex - 2 - i );
                AddIndex( CurrentVertex - 2 - ( i + 1 ) % horizontalSegments );
                AddIndex( CurrentVertex - 1 );
            }

            //InitializePrimitive();
        }
    }
}
