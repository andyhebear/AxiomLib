﻿#region MIT/X11 License
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
using Axiom.Math;
namespace Axiom.SkyX
{
	partial class CloudLayer
	{
        /// <summary>
        /// Layer cloud options 
        /// </summary>
        public class Options
        {
            /// <summary>
            /// Cloud layer height
            /// </summary>
            public float Height;
            /// <summary>
            /// Cloud layer scale
            /// </summary>
            public float Scale;
            /// <summary>
            /// Wind direction
            /// </summary>
            public Vector2 WindDirection;
            /// <summary>
            /// Time multiplier
            /// </summary>
            public float TimeMultiplier;
            /// <summary>
            /// Distance attenuation
            /// </summary>
            public float DistanceAttenuation;
            /// <summary>
            /// Detail attenuation
            /// </summary>
            public float DetailAttenuation;
            /// <summary>
            /// Normal multiplier
            /// </summary>
            public float NormalMultiplier;
            /// <summary>
            /// Cloud layer height volume(For volumetric effects on the gpu)
            /// </summary>
            public float HeightVolume;
            /// <summary>
            /// Volumetric displacement(For volumetric effects on the gpu)
            /// </summary>
            public float VolumetricDisplacement;

            /// <summary>
            /// Default constructor.
            /// </summary>
            public Options()
            {
                Height = 100;
                Scale = 0.001f;
                WindDirection = new Vector2(1, 1);
                TimeMultiplier = 0.125f;
                DistanceAttenuation = 0.05f;
                DetailAttenuation = 1;
                NormalMultiplier = 2;
                HeightVolume = 0.25f;
                VolumetricDisplacement = 0.01f;
            }
        }
	}
}
