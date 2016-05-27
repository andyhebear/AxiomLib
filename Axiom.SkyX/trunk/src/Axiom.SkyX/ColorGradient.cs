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
using Axiom.Core;

namespace Axiom.SkyX
{
    /// <summary>
    /// 
    /// </summary>
    public class ColorGradient
    {
        /// <summary>
        /// Color frame list
        /// </summary>
        private List<KeyValuePair<Vector3, float>> _cFrameList = new List<KeyValuePair<Vector3, float>>();
        /// <summary>
        /// Mal formed color gradient?
        /// </summary>
        private bool _malFormed;

        /// <summary>
        /// 
        /// </summary>
        public ColorGradient()
        {
            _malFormed = true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="colorFrame"></param>
        public void AddFrame(KeyValuePair<Vector3, float> colorFrame)
        {
            _cFrameList.Add(colorFrame);

            _malFormed = !CheckBounds();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Clear()
        {
            _cFrameList.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public Vector3 GetColor(float p)
        {
            if (_malFormed)
            {
                LogManager.Instance.Write("SkyX: Mal-formed ColorGradient", null);
                return Vector3.Zero;
            }

            if (_cFrameList.Count == 0)
            {
                return Vector3.Zero;
            }
            else if (_cFrameList.Count == 1)
            {
                return _cFrameList[0].Key;
            }

            KeyValuePair<int, float> minBound = new KeyValuePair<int, float>(0,-1);
            KeyValuePair<int, float> maxBound = new KeyValuePair<int, float>(0, 2);

            // Min value

            for (int k = 0; k < _cFrameList.Count; k++)
            {
                if (_cFrameList[k].Value < p && _cFrameList[k].Value > minBound.Value)
                {
                    minBound = new KeyValuePair<int, float>(k, _cFrameList[k].Value);
                }
            }

            // Max value
            for (int k = 0; k < _cFrameList.Count; k++)
            {
                if (_cFrameList[k].Value > p && _cFrameList[k].Value < maxBound.Value)
                {
                    maxBound = new KeyValuePair<int, float>(k, _cFrameList[k].Value);
                }
            }

            float range = maxBound.Value - minBound.Value;
            float rangePoint = (p - minBound.Value) / range;

            return _cFrameList[minBound.Key].Key * (1 - rangePoint) + _cFrameList[maxBound.Key].Key * rangePoint;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool CheckBounds()
        {
            bool existBoundsFirst = false;
            bool existBoundsSecond = false;

            for (int k = 0; k < _cFrameList.Count; k++)
            {
                if (_cFrameList[k].Value == 0)
                {
                    // More than one min bound
                    if (existBoundsFirst)
                    {
                        return false;
                    }

                    existBoundsFirst = true;
                }

                if (_cFrameList[k].Value < 0 || _cFrameList[k].Value > 1)
                {
                    return false;
                }
            }

            for (int k = 0; k < _cFrameList.Count; k++)
            {
                if (_cFrameList[k].Value == 1)
                {
                    // More than one min bound
                    if (existBoundsSecond)
                    {
                        return false;
                    }

                    existBoundsSecond = true;
                }
            }

            if (!existBoundsFirst || !existBoundsSecond)
            {
                return false;
            }

            return true;
        }
    }
}
