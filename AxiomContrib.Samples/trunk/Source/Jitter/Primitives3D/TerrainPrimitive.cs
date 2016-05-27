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
    public class TerrainPrimitive : GeometricPrimitive
    {
        public delegate float TerrainFunction( int coordX, int coordZ );

        public float[ , ] heights;

        public TerrainPrimitive( JitterSample sample, TerrainFunction function )
            : base( sample, (PrimitiveType)(-1) )
        {
            heights = new float[ 100, 100 ];

            for ( int i = 0; i < 100; i++ )
            {
                for ( int e = 0; e < 100; e++ )
                {
                    heights[ i, e ] = function( i, e );
                }
            }

            Vector3[] neighbour = new Vector3[ 4 ];

            for ( int i = 0; i < 100; i++ )
            {
                for ( int e = 0; e < 100; e++ )
                {
                    Vector3 pos = new Vector3( i, heights[ i, e ], e );

                    if ( i > 0 ) neighbour[ 0 ] = new Vector3( i - 1, heights[ i - 1, e ], e );
                    else neighbour[ 0 ] = pos;

                    if ( e > 0 ) neighbour[ 1 ] = new Vector3( i, heights[ i, e - 1 ], e - 1 );
                    else neighbour[ 1 ] = pos;

                    if ( i < 99 ) neighbour[ 2 ] = new Vector3( i + 1, heights[ i + 1, e ], e );
                    else neighbour[ 2 ] = pos;

                    if ( e < 99 ) neighbour[ 3 ] = new Vector3( i, heights[ i, e + 1 ], e + 1 );
                    else neighbour[ 3 ] = pos;

                    Vector3 normal = Vector3.Zero;

                    normal += ( neighbour[ 1 ] - pos ).Cross( neighbour[ 0 ] - pos );
                    normal += ( neighbour[ 2 ] - pos ).Cross( neighbour[ 1 ] - pos );
                    normal += ( neighbour[ 3 ] - pos ).Cross( neighbour[ 2 ] - pos );
                    normal += ( neighbour[ 0 ] - pos ).Cross( neighbour[ 3 ] - pos );
                    normal.Normalize();

                    this.AddVertex( new Vector3( i, heights[ i, e ], e ), normal );
                }
            }

            for ( int i = 1; i < 100; i++ )
            {
                for ( int e = 1; e < 100; e++ )
                {
                    this.AddIndex( i * 100 + e );
                    this.AddIndex( i * 100 + ( e - 1 ) );
                    this.AddIndex( ( i - 1 ) * 100 + e );

                    this.AddIndex( ( i - 1 ) * 100 + ( e - 1 ) );
                    this.AddIndex( ( i - 1 ) * 100 + e );
                    this.AddIndex( i * 100 + ( e - 1 ) );
                }
            }

            //this.InitializePrimitive();
        }
    }
}
