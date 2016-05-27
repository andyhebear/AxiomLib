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
    /// Geometric primitive class for drawing cylinders.
    /// </summary>
    public class CylinderPrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cylinder primitive, using default settings.
        /// </summary>
        public CylinderPrimitive( JitterSample sample )
            : this( sample, 1, 1, 32 )
        {
        }

        /// <summary>
        /// Constructs a new cylinder primitive,
        /// with the specified size and tessellation level.
        /// </summary>
        public CylinderPrimitive( JitterSample sample, float height, float radius, int tessellation )
            : base( sample, PrimitiveType.Cylinder )
        {
            if ( tessellation < 3 )
                throw new ArgumentOutOfRangeException( "tessellation" );

            height /= 2;

            // Create a ring of triangles around the outside of the cylinder.
            for ( int i = 0; i < tessellation; i++ )
            {
                Vector3 normal = GetCircleVector( i, tessellation );

                AddVertex( normal * radius + Vector3.UnitY * height, normal );
                AddVertex( normal * radius + Vector3.NegativeUnitY * height, normal );

                AddIndex( ( i * 2 + 2 ) % ( tessellation * 2 ) );
                AddIndex( i * 2 + 1 );
                AddIndex( i * 2 );

                AddIndex( ( i * 2 + 2 ) % ( tessellation * 2 ) );
                AddIndex( ( i * 2 + 3 ) % ( tessellation * 2 ) );
                AddIndex( i * 2 + 1 );
            }

            // Create flat triangle fan caps to seal the top and bottom.
            CreateCap( tessellation, height, radius, Vector3.UnitY );
            CreateCap( tessellation, height, radius, Vector3.NegativeUnitY );

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
                Vector3 position = GetCircleVector( i, tessellation ) * radius + normal * height;
                AddVertex( position, normal );
            }
        }


        /// <summary>
        /// Helper method computes a point on a circle.
        /// </summary>
        static Vector3 GetCircleVector( int i, int tessellation )
        {
            float angle = i * 2 * (float)System.Math.PI / tessellation;

            float dx = (float)System.Math.Cos( angle );
            float dz = (float)System.Math.Sin( angle );

            return new Vector3( dx, 0, dz );
        }
    }
}
