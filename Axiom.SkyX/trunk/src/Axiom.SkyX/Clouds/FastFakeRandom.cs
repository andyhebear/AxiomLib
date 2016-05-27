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
    public class FastFakeRandom
    {
        float[] _data;
        int _capacity;
        int _index;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="n"></param>
        /// <param name="min"></param>
        /// <param name="max"></param>
        public FastFakeRandom(int n, float min, float max)
        {
            _capacity = n;
            _index = -1;
            _data = new float[n];

            for (int k = 0; k < n; k++)
            {
                _data[k] = Utility.RangeRandom(min, max);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public float Get()
        {
            _index++;
            if (_index >= _capacity)
                _index = 0;

            return _data[_index];
        }
    }
}
