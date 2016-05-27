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

namespace Axiom.SkyX.Clouds
{
    public class Ellipsoid
    {
        private int _a;
        private int _b;
        private int _c;
        private int _a2;
        private int _b2;
        private int _c2;
        private int _nx;
        private int _ny;
        private int _nz;
        private int _x;
        private int _y;
        private int _z;
        private float _density;

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Dimensions
        {
            get { return new Vector3(_a, _b, _c); }
            set
            {
                _a = (int)value.x;
                _b = (int)value.y;
                _c = (int)value.z;

                _a2 = (int)Utility.Pow((float)_a, 2);
                _b2 = (int)Utility.Pow((float)_b, 2);
                _c2 = (int)Utility.Pow((float)_c, 2);
            }
        }
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public Ellipsoid(int a, int b, int c,
            int nx, int ny, int nz,
            int x, int y, int z)
            : this(a, b, c, nx, ny, nz, x, y, z, 1.0f)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="dynlibManager"></param>
        public Ellipsoid(int a, int b, int c,
            int nx, int ny, int nz,
            int x, int y, int z,
            float density)
        {
            _a = a;
            _b = b;
            _c = c;
            _a2 = (int)Utility.Pow((float)a, 2.0f);
            _b2 = (int)Utility.Pow((float)b, 2.0f);
            _c2 = (int)Utility.Pow((float)c, 2.0f);
            _nx = nx;
            _ny = ny;
            _nz = nz;
            _x = x;
            _y = y;
            _z = z;
            _density = density;
            
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ax"></param>
        /// <param name="ay"></param>
        /// <param name="az"></param>
        public void Move(int ax, int ay, int az)
        {
            _x += ax;
            _y += ay;
            _z += az;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public Vector3 GetProbabilities(int x, int y, int z)
        {
            float density = Utility.Pow(1 - GetLength(x, y, z), 1.0f / _density);

            return new Vector3(density, 1 - density, density);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="nx"></param>
        /// <param name="ny"></param>
        /// <param name="nz"></param>
        public void UpdatePropabilities(DataManager.Cell[,,] c, int nx, int ny, int nz)
        {
            int u = 0, v = 0, w = 0, uu = 0, vv = 0;

            float length;

            for (u = _x - _a; u < _x + _a; u++)
            {
                for (v = _y - _b; v < _y + _b; v++)
                {
                    for (w = _z - _c; w < _z + _c; w++)
                    {
                        //	prob = getProbabilities(u, v, w);

                        length = GetLength(uu, vv, w);

                        // x/y Seamless!
                        uu = (u < 0) ? (u + nx) : u; if (u >= nx) { uu -= nx; }
                        vv = (v < 0) ? (v + ny) : v; if (v >= ny) { vv -= ny; }

                        //	c[uu][vv][w].phum = Ogre::Math::Clamp<Ogre::Real>(c[uu][vv][w].phum+prob.x, 0, 1);
                        //	c[uu][vv][w].pext = std::min<float>(prob.y, c[uu][vv][w].pext);
                        //	c[uu][vv][w].pact = Ogre::Math::Clamp<Ogre::Real>(c[uu][vv][w].pact+prob.z, 0, 1);

                        if (length < 1)
                        {
                            c[uu,vv,w].Phum = 0.1f;
                            c[uu,vv,w].Pext = 0.5f;
                            c[uu,vv,w].Pact = 0.2f;
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsOutOfCells()
        {
            if ((_x + _a) >= _nx || (_x - _a) < 0 ||
                (_y + _b) >= _ny || (_y - _b) < 0 ||
                (_z + _z) >= _nz || (_z - _z) < 0)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        private float GetLength(int x, int y, int z)
        {
            //  x^2   y^2   z^2
            //  /   + /   + /    = 1  (Ellipsoid ecuation)
            //  a^2   b^2   c^2
            // 
            //  maxradatdir = lambda (Xo, Yo, Zo) = lambda; where Xo, Yo and Zo are the components of the normaliced direction vector
            //
            //  => lambda^2 = 1 / ( EllipsoidEcuation...)
            //
            //  => lambda = sqrt (...) => maxradatdir = lambda

            Vector3 direction = new Vector3(x - _x, y - _y, z - _z);
            Vector3 directionNormalized = direction.NormalizedCopy();

            float a = Utility.Pow(directionNormalized.x, 2.0f) / (float)_a2 +
                      Utility.Pow(directionNormalized.y, 2.0f) / (float)_b2 +
                      Utility.Pow(directionNormalized.z, 2.0f) / (float)_c2;
            float lambda = 1.0f / Utility.Sqrt(a);

            return Utility.Clamp<float>(direction.Length / lambda, 1, 0);
        }

    }
}
