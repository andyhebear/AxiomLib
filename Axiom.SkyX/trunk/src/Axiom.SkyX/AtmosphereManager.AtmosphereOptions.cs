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
    partial class AtmosphereManager
    {
        /// <summary>
        /// Atmosphere options
        /// </summary>
        public class AtmosphereOptions : ICloneable
        {
            /// <summary>
            /// Time information: 
            /// x = time in [0, 24]h range, 
            /// y = sunrise hour in [0, 24]h range, 
            /// z = sunset hour in [0, 24] range
            /// </summary>
            public Vector3 Time;
            /// <summary>
            /// East position
            /// </summary>
            public Vector2 EastPosition;
            /// <summary>
            /// Inner atmosphere radius
            /// </summary>
            public float InnerRadius;
            /// <summary>
            /// Outer atmosphere radius
            /// </summary>
            public float OuterRadius;
            /// <summary>
            /// Height position, in [0, 1] range, 0=InnerRadius, 1=OuterRadius
            /// </summary>
            public float HeightPosition;
            /// <summary>
            /// Rayleigh multiplier
            /// </summary>
            public float RayleighMultiplier;
            /// <summary>
            /// Mie multiplier
            /// </summary>
            public float MieMultiplier;
            /// <summary>
            /// Sun intensity
            /// </summary>
            public float SunIntensity;
            /// <summary>
            /// WaveLength for RGB channels
            /// </summary>
            public Vector3 WaveLength;
            /// <summary>
            /// Phase function
            /// </summary>
            public float G;
            /// <summary>
            /// Exposure coeficient
            /// </summary>
            public float Exposure;
            /// <summary>
            /// Number of samples
            /// </summary>
            public int NumberOfSamples;

            /// <summary>
            /// Default constructor
            /// </summary>
            public AtmosphereOptions()
            {
                this.Time = new Vector3(14.0f, 7.50f, 20.50f);
                this.EastPosition = new Vector2(0, 1);
                this.InnerRadius = 9.77501f;
                this.OuterRadius = 10.2963f;
                this.HeightPosition = 0.01f;
                this.RayleighMultiplier = 0.0022f;
                this.MieMultiplier = 0.000675f;
                this.SunIntensity = 30;
                this.WaveLength = new Vector3(0.57f, 0.54f, 0.44f);
                this.G = -0.991f;
                this.Exposure = 2.0f;
                this.NumberOfSamples = 4;
            }

            /// <summary>
            /// Simple constructor
            /// </summary>
            /// <param name="time">
            /// Time information: 
            /// x = time in [0, 24]h range, 
            /// y = sunrise hour in [0, 24]h range, 
            /// z = sunset hour in [0, 24] range
            /// </param>
            public AtmosphereOptions(Vector3 time)
                : this()
            {
                this.Time = time;
            }

            /// <summary>
            ///  Extended constructor
            /// </summary>
            /// <param name="time">x = time in [0, 24]h range, y = sunrise hour in [0, 24]h range, z = sunset hour in [0, 24] range</param>
            /// <param name="eastPosition">East position</param>
            /// <param name="innerRadius">Inner atmosphere radius</param>
            /// <param name="outerRadius">Outer atmosphere radius</param>
            /// <param name="heightPosition">Height position, in [0, 1] range, 0=InnerRadius, 1=OuterRadius</param>
            /// <param name="rayleighMultiplier">Rayleigh multiplier</param>
            /// <param name="mieMultiplier">Mie multiplier</param>
            /// <param name="sunIntensity">Sun intensity</param>
            /// <param name="waveLength">Wave length for RGB channels</param>
            /// <param name="g">Phase function</param>
            /// <param name="exposure">Exposure</param>
            /// <param name="numberOfSamples">Number of samples</param>
            public AtmosphereOptions(Vector3 time, Vector2 eastPosition,
                float innerRadius, float outerRadius, float heightPosition,
                float rayleighMultiplier, float mieMultiplier,
                float sunIntensity, Vector3 waveLength, float g,
                float exposure, int numberOfSamples)
                : this(time)
            {
                this.EastPosition = eastPosition;
                this.InnerRadius = innerRadius;
                this.OuterRadius = outerRadius;
                this.HeightPosition = heightPosition;
                this.RayleighMultiplier = rayleighMultiplier;
                this.MieMultiplier = mieMultiplier;
                this.SunIntensity = sunIntensity;
                this.WaveLength = waveLength;
                this.G = g;
                this.Exposure = exposure;
                this.NumberOfSamples = numberOfSamples;
            }

            /// <summary>
            /// Copy Constructor
            /// </summary>
            /// <param name="source"><see cref="AtmosphereOptions"/> to copy</param>
            public AtmosphereOptions(AtmosphereOptions source)
            {
                this.Time = source.Time;
                this.EastPosition = source.EastPosition;
                this.InnerRadius = source.InnerRadius;
                this.OuterRadius = source.OuterRadius;
                this.HeightPosition = source.HeightPosition;
                this.RayleighMultiplier = source.RayleighMultiplier;
                this.MieMultiplier = source.MieMultiplier;
                this.SunIntensity = source.SunIntensity;
                this.WaveLength = source.WaveLength;
                this.G = source.G;
                this.Exposure = source.Exposure;
                this.NumberOfSamples = source.NumberOfSamples;
            }

            #region IConeable Implementation

            public object Clone()
            {
                return new AtmosphereOptions(this);
            }

            #endregion IConeable Implementation
        }
    }
}
