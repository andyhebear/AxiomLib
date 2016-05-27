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
using Axiom.Math;

namespace Axiom.SkyX
{
    public partial class AtmosphereManager : IDisposable
    {
        //public static T Clamp<T>(T value, T min, T max)
        //         where T : System.IComparable<T>
        //{
        //    T result = value;
        //    if (value.CompareTo(max) > 0)
        //        result = max;
        //    if (value.CompareTo(min) < 0)
        //        result = min;
        //    return result;
        //}
        #region Fields and Properties

        /// <summary>
        /// Our options.
        /// </summary>
        private AtmosphereOptions _options;
        /// <summary>
        /// SkyX parent reference
        /// </summary>
        private SkyX _skyX;

        /// <summary>
        /// Our options.
        /// </summary>
        public AtmosphereOptions Options
        {
            get
            {
                return _options.Clone() as AtmosphereOptions;
            }
            set
            {
                Update(value);
            }
        }
        /// <summary>
        /// Get's the sun direction.
        /// </summary>
        public Vector3 SunDirection
        {
            get { return GetSunDirection(); }
        }
        /// <summary>
        /// Get's the world sun position
        /// </summary>
        public Vector3 SunPosition
        {
            get
            {
                return this.SkyX.Camera.DerivedPosition - this.SunDirection * this.SkyX.MeshManager.SkydomeRadius;
            }
        }
        /// <summary>
        /// Get's the SkyX parent reference.
        /// </summary>
        public SkyX SkyX
        {
            get { return _skyX; }
            private set { _skyX = value; }
        }

        #endregion Fields and Properties

        #region Construction and Destruction

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="skyX"> parent skyX reference</param>
        public AtmosphereManager(SkyX skyX)
        {
            this.SkyX = skyX;
            _options = new AtmosphereOptions();
        }
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
        }
        #endregion Construction and Destruction

        #region Methods

        /// <summary>
        ///  Update atmoshpere
        /// </summary>
        /// <param name="options">Update only the differences between actual parameters and new ones.</param>
        internal void Update(AtmosphereOptions options)
        {
            Update(options, false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Vector3 GetSunDirection()
        {
            // 24h day: 
            // 0______A(Sunrise)_______B(Sunset)______24
            //  

            float y,
            X = _options.Time.x,
            A = _options.Time.y,
            B = _options.Time.z,
            AB = A + 24 - B,
            AB_ = B - A,
            XB = X + 24 - B;

            if (X < A || X > B)
            {
                if (X < A)
                {
                    y = -XB / AB;
                }
                else
                {
                    y = -(X - B) / AB;
                }

                if (y > -0.5f)
                {
                    y *= 2;
                }
                else
                {
                    y = -(1 + y) * 2;
                }
            }
            else
            {
                y = (X - A) / (B - A);

                if (y < 0.5f)
                {
                    y *= 2;
                }
                else
                {
                    y = (1 - y) * 2;
                }
            }

            Vector2 East = _options.EastPosition.NormalizedCopy();

            if (X > A && X < B)
            {
                if (X > (A + AB_ / 2))
                {
                    East = -East;
                }
            }
            else
            {
                if (X < A)
                {
                    if (XB < (24 - AB_) / 2)
                    {
                        East = -East;
                    }
                }
                else
                {
                    if ((X - B) < (24 - AB_) / 2)
                    {
                        East = -East;
                    }
                }
            }

            float ydeg = (Utility.PI / 2.0f) * y,
                  sn = Utility.Sin(ydeg),
                  cs = Utility.Cos(ydeg);

            Vector3 SPos = new Vector3(East.x * cs, sn, East.y * cs);

            return -SPos;
        }
        /// <summary>
        ///  Update atmoshpere
        /// </summary>
        /// <param name="options">Update only the differences between actual parameters and new ones.</param>
        /// <param name="forceUpdateToAll">Forces to upload all current parameters to skyx material.</param>
        public void Update(AtmosphereOptions newOptions, bool forceUpdateToAll)
        {
            GpuManager mGpuManager = this.SkyX.GpuManager;

            if (newOptions.Time != _options.Time ||
                newOptions.EastPosition != _options.EastPosition ||
                forceUpdateToAll)
            {
                _options.Time = newOptions.Time;
                _options.EastPosition = newOptions.EastPosition;

                if (SkyX.IsStarFieldEnabled)
                {
                    mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Fragment, "uTime", SkyX.TimeOffset * 0.05f, false);
                }

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uLightDir", -this.SunDirection);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Fragment, "uLightDir", -this.SunDirection, false);

                this.SkyX.MoonManager.Update();
            }

            if (newOptions.InnerRadius != this.Options.InnerRadius ||
                newOptions.OuterRadius != this.Options.OuterRadius ||
                forceUpdateToAll)
            {
                _options.InnerRadius = newOptions.InnerRadius;
                _options.OuterRadius = newOptions.OuterRadius;

                float scale = 1.0f / (_options.OuterRadius - _options.InnerRadius);
                float scaleDepth = (_options.OuterRadius - _options.InnerRadius) / 2.0f;
                float scaleOverScaleDepth = scale / scaleDepth;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uInnerRadius", _options.InnerRadius);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uCameraPos", new Vector3(0, _options.InnerRadius + (_options.OuterRadius - _options.InnerRadius) * _options.HeightPosition, 0));
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uScale", scale);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uScaleDepth", scaleDepth);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uScaleOverScaleDepth", scaleOverScaleDepth);
            }

            if (newOptions.HeightPosition != this.Options.HeightPosition ||
                forceUpdateToAll)
            {
                _options.HeightPosition = newOptions.HeightPosition;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uCameraPos", new Vector3(0, _options.InnerRadius + (_options.OuterRadius - _options.InnerRadius) * _options.HeightPosition, 0));
            }

            if (newOptions.RayleighMultiplier != this.Options.RayleighMultiplier ||
                newOptions.SunIntensity != this.Options.SunIntensity ||
                forceUpdateToAll)
            {
                _options.RayleighMultiplier = newOptions.RayleighMultiplier;

                float Kr4PI = _options.RayleighMultiplier * 4.0f * Math.Utility.PI;
                float KrESun = _options.RayleighMultiplier * _options.SunIntensity;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uKr4PI", Kr4PI);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uKrESun", KrESun);
            }

            if (newOptions.MieMultiplier != this.Options.MieMultiplier ||
                newOptions.SunIntensity != this.Options.SunIntensity ||
                forceUpdateToAll)
            {
                _options.MieMultiplier = newOptions.MieMultiplier;

                float Km4PI = _options.MieMultiplier * 4.0f * Math.Utility.PI;
                float KmESun = _options.MieMultiplier * _options.SunIntensity;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uKm4PI", Km4PI);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uKmESun", KmESun, false);
            }

            if (newOptions.NumberOfSamples != this.Options.NumberOfSamples ||
                forceUpdateToAll)
            {
                _options.NumberOfSamples = newOptions.NumberOfSamples;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uNumberOfSamples", _options.NumberOfSamples);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uSamples", (float)_options.NumberOfSamples);
            }

            if (newOptions.WaveLength != this.Options.WaveLength ||
                forceUpdateToAll)
            {
                _options.WaveLength = newOptions.WaveLength;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Vertex, "uInvWaveLength",
                    new Vector3(1.0f / Math.Utility.Pow(_options.WaveLength.x, 4.0f),
                                1.0f / Math.Utility.Pow(_options.WaveLength.y, 4.0f),
                                1.0f / Math.Utility.Pow(_options.WaveLength.z, 4.0f)));
            }

            if (newOptions.G != this.Options.G ||
                forceUpdateToAll)
            {
                _options.G = newOptions.G;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Fragment, "uG", _options.G, false);
                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Fragment, "uG2", _options.G * _options.G, false);
            }

            if ((newOptions.Exposure != this.Options.Exposure ||
                forceUpdateToAll) &&
                (this.SkyX.LightingMode == LightingMode.Ldr))
            {
                _options.Exposure = newOptions.Exposure;

                mGpuManager.SetGpuProgramParameter(GpuManager.GpuProgram.Fragment, "uExposure", _options.Exposure);
            }

            this.SkyX.CloudsManager.UpdateInternal();
        }
        /// <summary>
        /// Get current atmosphere color at the given direction
        /// </summary>
        /// <param name="direction">*Normalized* direction</param>
        /// <returns>Atmosphere color at the especified direction</returns>
        public Vector3 GetColorAt(Vector3 direction)
        {
            if (direction.y < 0)
            {
                return new Vector3(0, 0, 0);
            }

            //parameters
            double scale = 1.0f / (_options.OuterRadius - _options.InnerRadius);
            double scaleDepth = (_options.OuterRadius - _options.InnerRadius) / 2.0f;
            double scaleOverScaleDepth = scale / scaleDepth;
            double Kr4PI = _options.RayleighMultiplier * 4.0f * Math.Utility.PI;
            double KrESun = _options.RayleighMultiplier * _options.SunIntensity;
            double Km4PI = _options.MieMultiplier * 4.0f * Math.Utility.PI;
            double KmESun = _options.MieMultiplier * _options.SunIntensity;

            // --- Start vertex program simulation ---
            Vector3 uLightDir = -this.SunDirection;
            Vector3 v3Pos = direction;
            Vector3 uCameraPos = new Vector3(0, _options.InnerRadius + (_options.OuterRadius - _options.InnerRadius) * _options.HeightPosition, 0);
            Vector3 uInvWaveLength = new Vector3(
                1.0f / Utility.Pow(_options.WaveLength.x, 4.0f),
                1.0f / Utility.Pow(_options.WaveLength.y, 4.0f),
                1.0f / Utility.Pow(_options.WaveLength.z, 4.0f));

            // Get the ray from the camera to the vertex, and it's length (far point)
            v3Pos.y += _options.InnerRadius;
            Vector3 v3Ray = v3Pos - uCameraPos;
            double fFar = v3Ray.Length;
            v3Ray /= (float)fFar;

            // Calculate the ray's starting position, then calculate its scattering offset
            Vector3 v3Start = uCameraPos;
            double fHeight = uCameraPos.y;
            double fStartAngle = v3Ray.Dot(v3Start) / fHeight;
            double fDepth = System.Math.Exp(scaleOverScaleDepth * (_options.InnerRadius - uCameraPos.y));
            double fStartOffset = fDepth * Scale(fStartAngle, scaleDepth);

            // Init loop variables
            double fSampleLength = fFar / (double)(_options.NumberOfSamples);
            double fScaledLength = fSampleLength * scale;
            double fHeight_ = 0, fDepth_ = 0, fLightAngle = 0, fCameraAngle = 0, fScatter = 0;

            Vector3 v3SampleRay = v3Ray * (float)fSampleLength;
            Vector3 v3SamplePoint = v3Start + v3SampleRay * 0.5f;
            Vector3 color = Vector3.Zero, v3Attenuate = Vector3.Zero;

            // Loop the ray
            for (int i = 0; i < _options.NumberOfSamples; i++)
            {
                fHeight_ = v3SamplePoint.Length;
                fDepth_ = System.Math.Exp(scaleOverScaleDepth * (_options.InnerRadius - fHeight_));

                fLightAngle = uLightDir.Dot(v3SamplePoint) / fHeight_;
                fCameraAngle = v3Ray.Dot(v3SamplePoint) / fHeight_;

                fScatter = (fStartOffset + fDepth * (Scale(fLightAngle, scaleDepth) - Scale(fCameraAngle, scaleDepth)));
                v3Attenuate = new Vector3(
                    (float)System.Math.Exp(-fScatter * (uInvWaveLength.x * Kr4PI + Km4PI)),
                    (float)System.Math.Exp(-fScatter * (uInvWaveLength.y * Kr4PI + Km4PI)),
                    (float)System.Math.Exp(-fScatter * (uInvWaveLength.z * Kr4PI + Km4PI)));

                // Accumulate color
                v3Attenuate *= (float)(fDepth_ * fScaledLength);
                color += v3Attenuate;

                // Next sample point
                v3SamplePoint += v3SampleRay;
            }

            // Outputs
            Vector3 oRaylieghtColor = color * (uInvWaveLength * (float)KrESun);
            Vector3 oMieColor = color * (float)KmESun;
            Vector3 oDirection = uCameraPos - v3Pos;

            // --- End vertex program simulation ---
            // --- Start fragment program simulation ---
            double cos = uLightDir.Dot(oDirection / oDirection.Length);
            double cos2 = cos * cos;
            double rayleighPhase = 0.75 * (1.0 + 0.5 * cos2);
            double g2 = _options.G * _options.G;
            double miePhase = 1.5f * ((1.0f - g2) / (2.0f + g2)) * (1.0f + cos2) / Utility.Pow(1.0f + g2 - 2.0f * _options.G * cos, 1.5f);

            Vector3 oColor = Vector3.Zero;

            if (this.SkyX.LightingMode == LightingMode.Ldr)
            {
                oColor = new Vector3(
                    1 - (float)System.Math.Exp(_options.Exposure * (rayleighPhase * oRaylieghtColor.x + miePhase * oMieColor.x)),
                    1 - (float)System.Math.Exp(_options.Exposure * (rayleighPhase * oRaylieghtColor.y + miePhase * oMieColor.y)),
                    1 - (float)System.Math.Exp(_options.Exposure * (rayleighPhase * oRaylieghtColor.z + miePhase * oMieColor.z)));
            }
            else
            {
                oColor = (float)rayleighPhase * oRaylieghtColor + (float)miePhase * oMieColor;
            }

            // For night rendering
            oColor += Utility.Clamp<float>(
                ((1 -
                Utility.Max(oColor.x, Utility.Max(oColor.y, oColor.z)) * 10))
                , 1, 0)
                * (new Vector3(0.05f, 0.05f, 0.1f)
                * (2 - 0.75f * Utility.Clamp<float>(-uLightDir.y, 1, 0)) * Utility.Pow(1 - direction.y, 3));

            // --- End fragment program simulation ---

            // Output color
            return oColor;
        }
        /// <summary>
        /// Shader scale funtion 
        /// </summary>
        /// <param name="cos">Cos</param>
        /// <param name="uScaleDepth">Scale depth</param>
        /// <returns>scale</returns>
        internal float Scale(double cos, double uScaleDepth)
        {
            float x = (float)(1.0f - cos);
            return (float)(uScaleDepth * System.Math.Exp(-0.00287 + x * (0.459 + x * (3.83 + x * (-6.80 + x * 5.25)))));
        }
        #endregion Methods
    }


}
