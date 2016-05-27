#region MIT/X11 License
// This file is part of the Axiom.SkyX project
//
// Copyright (c) 2009 Michael Cummings <cummings.michael@gmail.com>
// Copyright (c) 2009 Bostich <bostich83@googlemail.com>

// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//
// Axiom.SkyX is a reimplementation of the SkyX project for .Net/Mono
// SkyX is Copyright (C) 2009 Xavier Verguín González <xavierverguin@hotmail.com> <xavyiy@gmail.com>
#endregion MIT/X11 License
using System;
using System.Collections.Generic;
using Axiom.Math;

namespace Axiom.SkyX
{
    public static class Extensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ret"></param>
        /// <returns></returns>
        public static float Normalize(this Vector2 ret)
        {
            float fLength = Utility.Sqrt(ret.x * ret.x + ret.y * ret.y);

            // Will also work for zero-sized vectors, but will change nothing
            if (fLength > 1e-08)
            {
                float fInvLength = 1.0f / fLength;
                ret.x *= fInvLength;
                ret.y *= fInvLength;
            }

            return fLength;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector2 NormalizedCopy(this Vector2 vector)
        {
            Vector2 ret = new Vector2(vector.x, vector.y);
            ret.Normalize();
            return ret;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static float Length(this Vector2 vector)
        {
            return Utility.Sqrt(vector.x * vector.x + vector.y * vector.y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        public static Vector3 NormalizedCopy(this Vector3 vector)
        {
            Vector3 ret = new Vector3(vector.x, vector.y, vector.z);
            ret.Normalize();
            return ret;
        }
    }
}
