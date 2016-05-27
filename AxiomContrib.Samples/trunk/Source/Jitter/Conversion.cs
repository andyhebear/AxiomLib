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
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter
{
    public sealed class Conversion
    {
        /// <summary>
        /// Convert a Vector3 to a Jitter Vector
        /// </summary>
        /// <param name="vector">The Vector3 to be converted</param>
        /// <returns>The converted vector</returns>
        public static JVector ToJitterVector( Vector3 vector )
        {
            return new JVector( vector.x, vector.y, vector.z );
        }

        /// <summary>
        /// Convert a Jitter Vector to a Vector3
        /// </summary>
        /// <param name="vector">The JVector to be converted</param>
        /// <returns>The converted Vector3</returns>
        public static Vector3 ToVector3( JVector vector )
        {
            return new Vector3( vector.X, vector.Y, vector.Z );
        }

        public static Matrix3 ToMatrix3( JMatrix matrix )
        {
            Matrix3 result;

            // set it to a transposed matrix since Xna uses row vectors
            result.m00 = matrix.M11;
            result.m01 = matrix.M21;
            result.m02 = matrix.M31;

            result.m10 = matrix.M12;
            result.m11 = matrix.M22;
            result.m12 = matrix.M32;

            result.m20 = matrix.M13;
            result.m21 = matrix.M23;
            result.m22 = matrix.M33;

            return result;
        }

        public static Matrix4 ToMatrix4( JMatrix matrix )
        {
            Matrix4 result;

            // set it to a transposed matrix since Xna uses row vectors
            result.m00 = matrix.M11;
            result.m01 = matrix.M21;
            result.m02 = matrix.M31;
            result.m03 = 0f;

            result.m10 = matrix.M12;
            result.m11 = matrix.M22;
            result.m12 = matrix.M32;
            result.m13 = 0f;

            result.m20 = matrix.M13;
            result.m21 = matrix.M23;
            result.m22 = matrix.M33;
            result.m23 = 0f;

            result.m30 = 0f;
            result.m31 = 0f;
            result.m32 = 0f;
            result.m33 = 1f;

            return result;
        }

        public static Quaternion ToQuat( JMatrix matrix )
        {
            return Quaternion.FromRotationMatrix( ToMatrix3( matrix ) );
        }

        public static JMatrix ToJitterMatrix( Matrix3 matrix )
        {
            JMatrix result;
            result.M11 = matrix.m00;
            result.M12 = matrix.m10;
            result.M13 = matrix.m20;
            result.M21 = matrix.m01;
            result.M22 = matrix.m11;

            result.M23 = matrix.m21;
            result.M31 = matrix.m02;
            result.M32 = matrix.m12;
            result.M33 = matrix.m22;

            return result;
        }

        public static JMatrix ToJitterMatrix( Matrix4 matrix )
        {
            JMatrix result;
            result.M11 = matrix.m00;
            result.M12 = matrix.m10;
            result.M13 = matrix.m20;
            result.M21 = matrix.m01;
            result.M22 = matrix.m11;

            result.M23 = matrix.m21;
            result.M31 = matrix.m02;
            result.M32 = matrix.m12;
            result.M33 = matrix.m22;
            return result;
        }

        public static JMatrix ToJitterMatrix( Quaternion quat )
        {
            return ToJitterMatrix( quat.ToRotationMatrix() );
        }
    }
}
