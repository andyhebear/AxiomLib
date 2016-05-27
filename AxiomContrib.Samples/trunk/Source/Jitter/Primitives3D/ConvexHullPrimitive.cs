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

//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Microsoft.Xna.Framework.Graphics;
//using Microsoft.Xna.Framework;
//using Jitter.LinearMath;
//using JitterDemo;

//namespace JitterDemo.Primitives3D
//{
//    public class ConvexHullPrimitive : GeometricPrimitive
//    {

//        //public JConvexHull ConvexHull = new JConvexHull();

//        public ConvexHullPrimitive(GraphicsDevice device, List<JVector> pointCloud)
//        {
//            JConvexHull.Build(pointCloud,JConvexHull.Approximation.Level5);

//            int counter = 0;

//            foreach (JConvexHull.Face face in ConvexHull.HullFaces)
//            {
//                this.AddVertex(Conversion.ToXNAVector(pointCloud[face.VertexC]), Conversion.ToXNAVector(face.Normal));
//                this.AddVertex(Conversion.ToXNAVector(pointCloud[face.VertexB]), Conversion.ToXNAVector(face.Normal));
//                this.AddVertex(Conversion.ToXNAVector(pointCloud[face.VertexA]), Conversion.ToXNAVector(face.Normal));

//                this.AddIndex(counter + 0);
//                this.AddIndex(counter + 1);
//                this.AddIndex(counter + 2);

//                counter+=3;
//            }
            

//            this.InitializePrimitive(device);
//        }

//    }
//}
