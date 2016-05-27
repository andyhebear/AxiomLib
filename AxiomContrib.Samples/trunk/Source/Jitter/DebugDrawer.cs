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
using Axiom.Core;
using Axiom.Math;

#endregion

namespace AxiomContrib.Samples.Jitter
{
    /// <summary>
    /// Draw axis aligned bounding boxes, points and lines.
    /// </summary>
    public class DebugDrawer
    {
        public DebugDrawer(  )
        {
        }

        public void Initialize()
        {
        }

        public void DrawLine( Vector3 p0, Vector3 p1, ColorEx color )
        {
            index += 2;

            //if ( index == LineList.Length )
            //{
                //VertexPositionColor[] temp = new VertexPositionColor[ LineList.Length + 50 ];
                //LineList.CopyTo( temp, 0 );
                //LineList = temp;
            //}

            //LineList[ index - 2 ].Color = color;
            //LineList[ index - 2 ].Position = p0;

            //LineList[ index - 1 ].Color = color;
            //LineList[ index - 1 ].Position = p1;
        }

        private void SetElement( ref Vector3 v, int index, float value )
        {
            if ( index == 0 )
                v.x = value;

            else if ( index == 1 )
                v.y = value;

            else if ( index == 2 )
                v.z = value;

            else
                throw new ArgumentOutOfRangeException( "index" );
        }

        private float GetElement( Vector3 v, int index )
        {
            if ( index == 0 )
                return v.x;

            if ( index == 1 )
                return v.y;

            if ( index == 2 )
                return v.z;

            throw new ArgumentOutOfRangeException( "index" );
        }

        public void DrawAabb( Vector3 from, Vector3 to, ColorEx color )
        {
            Vector3 halfExtents = ( to - from ) * 0.5f;
            Vector3 center = ( to + from ) * 0.5f;

            Vector3 edgecoord = new Vector3( 1f, 1f, 1f ), pa, pb;
            for ( int i = 0; i < 4; i++ )
            {
                for ( int j = 0; j < 3; j++ )
                {
                    pa = new Vector3( edgecoord.x * halfExtents.x, edgecoord.y * halfExtents.y,
                        edgecoord.z * halfExtents.z );
                    pa += center;

                    int othercoord = j % 3;
                    SetElement( ref edgecoord, othercoord, GetElement( edgecoord, othercoord ) * -1f );
                    pb = new Vector3( edgecoord.x * halfExtents.x, edgecoord.y * halfExtents.y,
                        edgecoord.z * halfExtents.z );
                    pb += center;

                    DrawLine( pa, pb, color );
                }
                edgecoord = new Vector3( -1f, -1f, -1f );
                if ( i < 3 )
                    SetElement( ref edgecoord, i, GetElement( edgecoord, i ) * -1f );
            }
        }

        //public VertexPositionColor[] LineList = new VertexPositionColor[ 50 ];
        private int index = 0;

        public void Draw()
        {
            //if ( index == 0 ) return;

            //JitterDemo demo = Game as JitterDemo;

            //basicEffect.View = demo.Camera.View;
            //basicEffect.Projection = demo.Camera.Projection;

            //foreach ( EffectPass pass in basicEffect.CurrentTechnique.Passes )
            //{
            //    pass.Apply();

            //    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(
            //        PrimitiveType.LineList, LineList, 0, index / 2 );
            //}

            //index = 0;
        }
    }
}
