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
    /// Geometric primitive class for drawing cubes.
    /// </summary>
    public class BoxPrimitive : GeometricPrimitive
    {
        /// <summary>
        /// Constructs a new cube primitive, using default settings.
        /// </summary>
        public BoxPrimitive( JitterSample sample )
            : this( sample, 1 )
        {
        }

        /// <summary>
        /// Constructs a new cube primitive, with the specified size.
        /// </summary>
        public BoxPrimitive( JitterSample sample, float size )
            : base( sample, PrimitiveType.Box )
        {
            // A cube has six faces, each one pointing in a different direction.
            Vector3[] normals =
            {
                Vector3.UnitZ,
                Vector3.NegativeUnitZ,
                Vector3.UnitX,
                Vector3.NegativeUnitX,
                Vector3.UnitY,
                Vector3.NegativeUnitY
            };

            // Create each face in turn.
            foreach ( Vector3 normal in normals )
            {
                // Get two vectors perpendicular to the face normal and to each other.
                Vector3 side1 = new Vector3( normal.y, normal.z, normal.x );
                Vector3 side2 = normal.Cross( side1 );

                // Six indices (two triangles) per face.
                AddIndex( CurrentVertex + 2 );
                AddIndex( CurrentVertex + 1 );
                AddIndex( CurrentVertex + 0 );

                AddIndex( CurrentVertex + 3 );
                AddIndex( CurrentVertex + 2 );
                AddIndex( CurrentVertex + 0 );

                // Four vertices per face.
                AddVertex( ( normal - side1 - side2 ) * size / 2, normal );
                AddVertex( ( normal - side1 + side2 ) * size / 2, normal );
                AddVertex( ( normal + side1 + side2 ) * size / 2, normal );
                AddVertex( ( normal + side1 - side2 ) * size / 2, normal );
            }

            //InitializePrimitive();
        }
    }
}
