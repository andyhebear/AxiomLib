using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Media;
using Axiom.Hydrax;
namespace Axiom.Hydrax.Noise
{
    public class FFT : BaseNoise
    {        
		/// <summary>
		/// Struct wich contains fft noise module options
		/// </summary>
		public struct Options
		{
			/// <summary>
			/// Noise resolution (2^n)
			/// </summary>
			public int Resolution;
			/// <summary>
			/// Physical resolution
			/// </summary>
			public float PhysicalResolution;
			/// <summary>
			/// Noise scale
			/// </summary>
			public float Scale;
			/// <summary>
			/// Wind direction
			/// </summary>
			public Vector2 WindDirection;
			/// <summary>
			/// Animation speed
			/// </summary>
			public float AnimationSpeed;
			/// <summary>
			/// KwPower
			/// </summary>
			public float KwPower;
			/// <summary>
			/// Noise amplitude
			/// </summary>
			public float Amplitude;

			/// <summary>
			/// GPU Normal map generator parameters
			/// Only if GPU normal map generation is active
			/// <remarks>Representes the strength of the normals (i.e. Amplitude)</remarks>
			/// </summary>
			public float GPU_Strength;
			
			/// <summary>
			/// LOD Parameters, in order to obtain a smooth normal map we need to 
            /// decrease the detail level when the pixel is far to the camera.
			/// This parameters are stored in an Vector3:
			/// x -> Initial LOD value (Bigger values -> less detail)
			/// y -> Final LOD value
			/// z -> Final distance
			/// </summary>
			public Vector3 GPU_LODParameters;

			/// <summary>
			/// Default constructor
			/// </summary>
			/// <param name="Resolution"></param>
			public Options(int Resolution)
			{
                this.Resolution = Resolution;
                this.PhysicalResolution = 32.0f;
                this.Scale = 0.25f;
                this.WindDirection = new Vector2(4,5);
                this.AnimationSpeed = 1;
                this.KwPower = 6.0f;
                this.Amplitude = 1.0f;
                this.GPU_Strength = 2.0f;
                this.GPU_LODParameters = new Vector3(0.5f,50,150000);
			}

            
			 
            public Options(int resolution,
                    float physicalResolution,
                    float         scale,
                    Vector2 windDirection,
                    float         animationSpeed,
                    float         kwPower,
                    float         amplitude)
            {
				this.Resolution = resolution;
                this.PhysicalResolution = physicalResolution;
                this.Scale = scale;
                this.WindDirection = windDirection;
                this.AnimationSpeed = animationSpeed;
                this.KwPower = kwPower;
                this.Amplitude = amplitude;
                this.GPU_Strength = 2.0f;
                this.GPU_LODParameters = new Vector3(0.5f,50,150000);
            }

			/// <summary>
			/// User costructor.
			/// </summary>
			/// <param name="resolution"></param>
			/// <param name="physicalResolution"></param>
			/// <param name="scale"></param>
			/// <param name="windDirection"></param>
			/// <param name="animationSpeed"></param>
			/// <param name="kwPower"></param>
			/// <param name="amplitude"></param>
			/// <param name="GPU_Strength"></param>
			/// <param name="GPU_LODParameters"></param>
			public Options( int resolution,
		                    float   physicalResolution,
		                    float   scale,
		                    Vector2 windDirection,
		                    float   animationSpeed,
		                    float   kwPower,
		                    float   amplitude,
		                    float  	GPU_Strength,
		                    Vector3 GPU_LODParameters)
            {
				this.Resolution = resolution;
                this.PhysicalResolution = physicalResolution;
                this.Scale = scale;
                this.WindDirection = windDirection;
                this.AnimationSpeed = animationSpeed;
                this.KwPower = kwPower;
                this.Amplitude = amplitude;
                this.GPU_Strength = GPU_Strength;
                this.GPU_LODParameters = GPU_LODParameters;
                
            }   
        }

        /// FFT resolution
		int resolution;
		/// Pointers to resolution*resolution float size arrays
    	float[] re, img;
	    /// The minimal value of the result data of the fft transformation
    	float maximalValue;
		/// the data which is referred as h0{x,t), that is, the data of the simulation at the time 0.
	    Axiom.Hydrax.MathHelper.Complex[] initialWaves;
	    /// the data of the simulation at time t, which is formed using the data at time 0 and the angular frequencies at time t
        Axiom.Hydrax.MathHelper.Complex[] currentWaves;
	    /// the angular frequencies
        float[] angularFrequencies;
		/// Current time
		float time;
        protected Random random = new Random();
        
		/// GPUNormalMapManager pointer
		GPUNormalMapManager mGPUNormalMapManager;

		/// Perlin noise options
		Options mOptions;
		
		/// <summary>
		/// Default constructor
		/// </summary>
		public FFT(): base("FFT", true)
		{
			mOptions = new Options();
			resolution = 128;
			maximalValue = 2.0f;
			time = 10;
		}

        public FFT(Options Options) :
            base("FFT", true)
        {
            maximalValue = 2;
            time = 10;
            mOptions = Options;
            resolution = Options.Resolution;

        }
        public override void Create()
        {
            if (IsCreated)
            {
                return;
            }
            InitNoise();

            base.Create();
        }
        public void SetOptions(Options Options)
        {
            if (IsCreated)
            {
                throw new NotImplementedException();
            }
            mOptions = Options;
            resolution = mOptions.Resolution;
        }
        public override bool CreateGPUNormalMapResources(GPUNormalMapManager G)
        {
            if (!base.CreateGPUNormalMapResources(G))
                return false;

            mGPUNormalMapManager = G;

            //create our FFT Texture
            Texture mFFTTexture =
                Axiom.Core.TextureManager.Instance
                .CreateManual("_Hydrax_FFT_Noise",
                         "HYDRAX_RESOURCE_GROUP", Axiom.Graphics.TextureType.TwoD
                         , resolution, resolution, 0, Axiom.Media.PixelFormat.L16
                         , Axiom.Graphics.TextureUsage.DynamicWriteOnly);

            mGPUNormalMapManager.AddTexture(mFFTTexture);

            //Create our normal map generator material
            MaterialManager mMaterialManager = G.Hydrax.MaterialManager;

            string VertexProgramData = "", FragmentProgramData = "";

            GpuProgramParameters VP_Parameters, FP_Parameters;
            string[] EntryPoints = { "main_vp", "main_fp" };
            string[] GpuProgramsData = new string[2]; string[] GpuProgramNames = new string[2];

            #region - VertexProgram -
            //vertex program
            switch (G.Hydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        VertexProgramData +=

                    "void main_vp(\n" +
                            // IN
                        "float4 iPosition       : POSITION,\n" +
                            // OUT
                        "out float4 oPosition   : POSITION,\n" +
                        "out float3 oPosition_  : TEXCOORD0,\n" +
                        "out float4 oWorldUV    : TEXCOORD1,\n" +
                        "out float  oScale      : TEXCOORD2,\n" +
                        "out float3 oCameraPos  : TEXCOORD3,\n" +
                        "out float3 oCameraToPixel : TEXCOORD4,\n" +
                            // UNIFORM
                        "uniform float4x4 uWorldViewProj,\n" +
                        "uniform float4x4 uWorld, \n" +
                        "uniform float3   uCameraPos,\n" +
                        "uniform float    uScale)\n" +
                    "{\n" +
                        "oPosition    = mul(uWorldViewProj, iPosition);\n" +
                        "oPosition_   = iPosition.xyz;\n" +
                        "float2 Scale = uScale*mul(uWorld, iPosition).xz*0.0078125;\n" +
                        "oWorldUV.xy  = Scale;\n" +
                        "oWorldUV.zw  = Scale*16;\n" +
                        "oScale = uScale;\n" +
                        "oCameraPos = uCameraPos,\n" +
                        "oCameraToPixel = iPosition - uCameraPos;\n" +
                    "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;
            }
            #endregion

            #region - FragmentProgram -
            switch (G.Hydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                case MaterialManager.ShaderMode.SM_CG:
                    {
                        FragmentProgramData +=
                        "void main_fp(\n" +
                                                // IN
                            "float3 iPosition     : TEXCOORD0,\n" +
                            "float4 iWorldCoord   : TEXCOORD1,\n" +
                            "float  iScale        : TEXCOORD2,\n" +
                            "float3 iCameraPos    : TEXCOORD3,\n" +
                            "float3 iCameraToPixel : TEXCOORD4,\n" +
                                                // OUT
                            "out float4 oColor    : COLOR,\n" +
                                                // UNIFORM
                            "uniform float     uStrength,\n" +
                            "uniform float3    uLODParameters,\n" +  // x: Initial derivation, y: Final derivation, z: Step
                            "uniform float3    uCameraPos,\n" +
                            "uniform sampler2D uFFT : register(s0))\n" +
                        "{\n" +
                            "float Distance = length(iCameraToPixel);\n" +
                            "float Attenuation = saturate(Distance/uLODParameters.z);\n" +

                            "uLODParameters.x += (uLODParameters.y-uLODParameters.x)*Attenuation;\n" +
                            "uLODParameters.x *= iScale;\n" +

                            "float AngleAttenuation = 1/abs(normalize(iCameraToPixel).y);\n" +
                            "uLODParameters.x *= AngleAttenuation;\n" +

                            "float2 dx = float2(uLODParameters.x*0.0078125, 0);\n" +
                            "float2 dy = float2(0, dx.x);\n" +

                            "float3 p_dx, m_dx, p_dy, m_dy;\n" +

                            "p_dx = float3(\n" +
                                                // x+
                                          "iPosition.x+uLODParameters.x,\n" +
                                                // y+
                                          "tex2D(uFFT, iWorldCoord.xy+dx).x,\n" +
                                                // z
                                          "iPosition.z);\n" +

                            "m_dx = float3(\n" +
                                                // x-
                                          "iPosition.x-uLODParameters.x,\n" +
                                                // y-
                                          "tex2D(uFFT, iWorldCoord.xy-dx).x, \n" +
                                                // z
                                          "iPosition.z);\n" +

                           "p_dy = float3(\n" +
                                                // x
                                          "iPosition.x,\n" +
                                                // y+
                                          "tex2D(uFFT, iWorldCoord.xy+dy).x,\n" +
                                                // z+
                                          "iPosition.z+uLODParameters.x);\n" +

                           "m_dy = float3(\n" +
                                                // x
                                          "iPosition.x,\n" +
                                                // y-
                                          "tex2D(uFFT, iWorldCoord.xy-dy).x,\n" +
                                                // z-
                                          "iPosition.z-uLODParameters.x);\n" +

                           "uStrength *= (1-Attenuation);\n" +
                           "p_dx.y *= uStrength; m_dx.y *= uStrength;\n" +
                           "p_dy.y *= uStrength; m_dy.y *= uStrength;\n" +

                           "float3 normal = normalize(cross(p_dx-m_dx, p_dy-m_dy));\n" +

                           "oColor = float4(saturate(1-(0.5+0.5*normal)),1);\n" +
                        "}\n";
                    }
                    break;
                case MaterialManager.ShaderMode.SM_GLSL:
                    break;
            }
            #endregion

            #region - BuildMaterial -
            mGPUNormalMapManager.NormalMapMaterial =
                (Material)Axiom.Graphics.MaterialManager.Instance.Create
                ("_Hydrax_GPU_Normal_Map_Material", "HYDRAX_RESOURCE_GROUP");

            Pass Technique0_Pass0 = mGPUNormalMapManager.NormalMapMaterial.GetTechnique(0).GetPass(0);

            Technique0_Pass0.LightingEnabled = false;
            Technique0_Pass0.CullingMode = CullingMode.None;
            Technique0_Pass0.DepthWrite = true;
            Technique0_Pass0.DepthCheck = true;

            GpuProgramsData[0] = VertexProgramData; GpuProgramsData[1] = FragmentProgramData;
            GpuProgramNames[0] = "_Hydrax_GPU_Normal_Map_VP"; GpuProgramNames[1] = "_Hydrax_GPU_Normal_Map_FP";
            mMaterialManager.FillGpuProgramsToPass(Technique0_Pass0, GpuProgramNames, G.Hydrax.ShaderMode,
                EntryPoints, GpuProgramsData);

            VP_Parameters = Technique0_Pass0.VertexProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix,0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix,0);
            VP_Parameters.SetNamedAutoConstant("uCameraPos", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);

            VP_Parameters.SetNamedConstant("uScale", mOptions.Scale);

            FP_Parameters = Technique0_Pass0.FragmentProgramParameters;

            FP_Parameters.SetNamedConstant("uStrength", mOptions.GPU_Strength);
            FP_Parameters.SetNamedConstant("uLODParameters", mOptions.GPU_LODParameters);

            Technique0_Pass0.CreateTextureUnitState(mGPUNormalMapManager.GetTexture(0).Name, 0)
                .SetTextureAddressingMode( TextureAddressing.Wrap );

            mGPUNormalMapManager.NormalMapMaterial.Load();
            mGPUNormalMapManager.Create();
            #endregion
            return true;
        }
        public override void Update(float TimeSinceLastFrame)
        {
            CalculeNoise(TimeSinceLastFrame);

            if (IsGPUNormalMapResourcesCreated)
                UpdateGPUNormalMapResources();
        }
        private void InitNoise()
        {
            initialWaves = new Axiom.Hydrax.MathHelper.Complex[resolution * resolution];
            currentWaves = new Axiom.Hydrax.MathHelper.Complex[resolution * resolution];
            angularFrequencies = new float[resolution * resolution];

            re = new float[resolution * resolution];
            img = new float[resolution * resolution];

            Vector2 wave = new Vector2();
            Axiom.Hydrax.MathHelper.Complex[] pInitialWavesData = initialWaves;
            float[] pAngularFrequenciesData = angularFrequencies;
            float u, v,
              temp;
            int inc = 0, inc2 = 0;
            for (u = 0; u < resolution; u++)
            {
                wave.x = (float)((-0.5f * resolution + u) * (2.0f * System.Math.PI / mOptions.PhysicalResolution));

                for (v = 0; v < resolution; v++)
                {
                    wave.y = (float)((-0.5f * resolution + v) * (2.0f * System.Math.PI / mOptions.PhysicalResolution));

                    temp = (float)(System.Math.Sqrt(0.5f * GetPhillipsSpectrum(wave, mOptions.WindDirection, mOptions.KwPower)));
                    float randomFloat = GetGaussianRandomFloat() * temp;
                    pInitialWavesData[inc++] = new MathHelper.Complex(randomFloat,randomFloat);
                    temp = 9.81f * wave.Length();
                    pAngularFrequenciesData[inc2++] = (float)System.Math.Sqrt(temp);
                }
            }

            CalculeNoise(0);
        }
        private void CalculeNoise(float Delta)
        {
            time += Delta * mOptions.AnimationSpeed;

            Axiom.Hydrax.MathHelper.Complex[] pData = currentWaves;

            int u, v;

            float wt,
              coswt, sinwt,
              realVal, imagVal;
            int inc = 0;
            for (u = 0; u < resolution; u++)
            {
                for (v = 0; v < resolution; v++)
                {
                    Axiom.Hydrax.MathHelper.Complex positive_h0 = initialWaves[u * (resolution) + v];
                    Axiom.Hydrax.MathHelper.Complex negative_h0 = initialWaves[(resolution - 1 - u) * (resolution) + (resolution - 1 - v)];

                    wt = angularFrequencies[u * (resolution) + v] * time;

                    coswt = (float)System.Math.Cos(wt);
                    sinwt = (float)System.Math.Sin(wt);

                    realVal =
                        positive_h0.Real * coswt - positive_h0.Imaginary * sinwt + negative_h0.Real * coswt - (-negative_h0.Imaginary) * (-sinwt);
                    imagVal =
                        positive_h0.Real * sinwt + positive_h0.Imaginary * coswt + negative_h0.Real * (-sinwt) + (-negative_h0.Imaginary) * coswt;

                    pData[inc++] = new MathHelper.Complex(realVal, imagVal);
                }
            }

            ExecuteInverseFFT();
            NormalizeFFTData(0);
        }
        private void ExecuteInverseFFT()
        {
            int l2n = 0, p = 1;
            while (p < resolution)
            {
                p *= 2; l2n++;
            }
            int l2m = 0; p = 1;
            while (p < resolution)
            {
                p *= 2; l2m++;
            }

            resolution = 1 << l2m;
            resolution = 1 << l2n;

            int x, y, i;

            for (x = 0; x < resolution; x++)
            {
                for (y = 0; y < resolution; y++)
                {
                    re[resolution * x + y] = currentWaves[resolution * x + y].Real;
                    img[resolution * x + y] = currentWaves[resolution * x + y].Imaginary;
                }
            }

            //Bit reversal of each row
            int j, k;
            for (y = 0; y < resolution; y++) //for each row
            {
                j = 0;
                for (i = 0; i < resolution - 1; i++)
                {
                    re[resolution * i + y] = currentWaves[resolution * j + y].Real;
                    img[resolution * i + y] = currentWaves[resolution * j + y].Imaginary;

                    k = resolution / 2;
                    while (k <= j)
                    {
                        j -= k;
                        k /= 2;
                    }

                    j += k;
                }
            }

            //Bit reversal of each column
            float tx = 0, ty = 0;
            for (x = 0; x < resolution; x++) //for each column
            {
                j = 0;
                for (i = 0; i < resolution - 1; i++)
                {
                    if (i < j)
                    {
                        tx = re[resolution * x + i];
                        ty = img[resolution * x + i];
                        re[resolution * x + i] = re[resolution * x + j];
                        img[resolution * x + i] = img[resolution * x + j];
                        re[resolution * x + j] = tx;
                        img[resolution * x + j] = ty;
                    }
                    k = resolution / 2;
                    while (k <= j)
                    {
                        j -= k;
                        k /= 2;
                    }
                    j += k;
                }
            }

            //Calculate the FFT of the columns
            float ca, sa,
                  u1, u2,
                  t1, t2,
                  z;

            int l1, l2,
                l, i1;

            for (x = 0; x < resolution; x++) //for each column
            {
                //This is the 1D FFT:
                ca = -1.0f;
                sa = 0.0f;
                l1 = 1;
                l2 = 1;

                for (l = 0; l < l2n; l++)
                {
                    l1 = l2;
                    l2 *= 2;
                    u1 = 1.0f;
                    u2 = 0.0f;
                    for (j = 0; j < l1; j++)
                    {
                        for (i = j; i < resolution; i += l2)
                        {
                            i1 = i + l1;
                            t1 = u1 * re[resolution * x + i1] - u2 * img[resolution * x + i1];
                            t2 = u1 * img[resolution * x + i1] + u2 * re[resolution * x + i1];
                            re[resolution * x + i1] = re[resolution * x + i] - t1;
                            img[resolution * x + i1] = img[resolution * x + i] - t2;
                            re[resolution * x + i] += t1;
                            img[resolution * x + i] += t2;
                        }
                        z = u1 * ca - u2 * sa;
                        u2 = u1 * sa + u2 * ca;
                        u1 = z;
                    }
                    sa = (float)System.Math.Sqrt((1.0f - ca) / 2.0f);
                    ca = (float)System.Math.Sqrt((1.0f + ca) / 2.0f);
                }
            }
            //Calculate the FFT of the rows
            for (y = 0; y < resolution; y++) //for each row
            {
                //This is the 1D FFT:
                ca = -1.0f;
                sa = 0.0f;
                l1 = 1;
                l2 = 1;

                for (l = 0; l < l2m; l++)
                {
                    l1 = l2;
                    l2 *= 2;
                    u1 = 1.0f;
                    u2 = 0.0f;
                    for (j = 0; j < l1; j++)
                    {
                        for (i = j; i < resolution; i += l2)
                        {
                            i1 = i + l1;
                            t1 = u1 * re[resolution * i1 + y] - u2 * img[resolution * i1 + y];
                            t2 = u1 * img[resolution * i1 + y] + u2 * re[resolution * i1 + y];
                            re[resolution * i1 + y] = re[resolution * i + y] - t1;
                            img[resolution * i1 + y] = img[resolution * i + y] - t2;
                            re[resolution * i + y] += t1;
                            img[resolution * i + y] += t2;
                        }
                        z = u1 * ca - u2 * sa;
                        u2 = u1 * sa + u2 * ca;
                        u1 = z;
                    }
                    sa = (float)System.Math.Sqrt((1.0f - ca) / 2.0f);
                    ca = (float)System.Math.Sqrt((1.0f + ca) / 2.0f);
                }
            }

            for (x = 0; x < resolution; x++)
            {
                for (y = 0; y < resolution; y++)
                {
                    if (((x + y) & 0x1) == 1)
                    {
                        re[x * resolution + y] *= 1;
                    }
                    else
                    {
                        re[x * resolution + y] *= -1;
                    }
                }
            }
        }
        private void NormalizeFFTData(float Scale)
        {
            float scaleCoef = 0.000001f;
            int i;

            // Perform automatic detection of maximum value
            if (Scale == 0.0f)
            {
                float min = re[0], max = re[0],
                      currentMax = maximalValue; ;

                for (i = 1; i < resolution * resolution; i++)
                {
                    if (min > re[i]) min = re[i];
                    if (max < re[i]) max = re[i];
                }

                min = System.Math.Abs(min);
                max = System.Math.Abs(max);

                currentMax = (min > max) ? min : max;

                if (currentMax > maximalValue) maximalValue = currentMax;

                scaleCoef += maximalValue;
            }
            else
            {	// User defined scale		
                scaleCoef = Scale;
            }

            // Scale all the value, and clamp to [0,1] range
            int x, y;
            for (x = 0; x < resolution; x++)
            {
                for (y = 0; y < resolution; y++)
                {
                    i = x * resolution + y;
                    re[i] = (re[i] + scaleCoef) / (scaleCoef * 2);
                }
            }
        }
        private float GetPhillipsSpectrum(Vector2 WaveVector, Vector2 Wind, float kwPower)
        {
                        // Compute the length of the vector
            float k = WaveVector.Length();

            // To avoid division by 0
            if (k < 0.0000001f)
            {
                return 0;
            }
            else
            {
                float windVelocity = Wind.Length();
                float l = (float)System.Math.Pow(windVelocity, 2.0f) / 9.81f;
                float dot = WaveVector.Dot(Wind);

                return (float)(mOptions.Amplitude *
                    (System.Math.Exp(-1 / System.Math.Pow(k * 1, 2)) / (System.Math.Pow(k, 2) *
                    System.Math.Pow(k, 2))) * System.Math.Pow(-dot / (k * windVelocity), kwPower));
            }
        }
        private float GetPhillipsSpectrum(Vector2 WaveVector, Vector2 Wind)
        {
            return GetPhillipsSpectrum(WaveVector, Wind, 2);
        }
        public override float GetValue(float x, float y)
        {
            // Scale world coords
            float xScale = x * mOptions.Scale,
                  yScale = y * mOptions.Scale;

            // Convert coords from world-space to data-space
            int xs = (int)(xScale) % resolution,
                ys = (int)(yScale) % resolution;

            // If data-space coords are negative, transform it to positive
            if (x < 0) xs += resolution - 1;
            if (y < 0) ys += resolution - 1;

            // Determine x and y diff for linear interpolation
            int xINT = (x > 0) ? (int)(xScale) : (int)(xScale - 1),
                yINT = (y > 0) ? (int)(yScale) : (int)(yScale - 1);

            // Calculate interpolation coeficients
            float xDIFF = xScale - xINT,
                  yDIFF = yScale - yINT,
                  _xDIFF = 1 - xDIFF,
                  _yDIFF = 1 - yDIFF;

            // To adjust the index if coords are out of range
            int xxs = (xs == resolution - 1) ? -1 : xs,
                yys = (ys == resolution - 1) ? -1 : ys;

            //   A      B
            //     
            //
            //   C      D
            float A = re[(ys * resolution + xs)],
                  B = re[(ys * resolution + xxs + 1)],
                  C = re[((yys + 1) * resolution + xs)],
                  D = re[((yys + 1) * resolution + xxs + 1)];

            // Return the result of the linear interpolation
            return (A * _xDIFF * _yDIFF +
                    B * xDIFF * _yDIFF +
                    C * _xDIFF * yDIFF +
                    D * xDIFF * yDIFF) // Range [-0.3, 0.3]
                                     * 0.6f - 0.3f;
        }
        private float GetGaussianRandomFloat()
        {
            float x1, x2, w, y1;

            do
            {
                x1 = 2.0f * UniformDeviate() - 1.0f;
                x2 = 2.0f * UniformDeviate() - 1.0f;

                w = x1 * x1 + x2 * x2;

            } while (w >= 1.0f);

            w = (float)System.Math.Sqrt((-2.0f * System.Math.Log(w)) / w);
            y1 = x1 * w;

            return y1;
        }
        private float UniformDeviate()
        {
            return random.Next(32767) * (1.0f / (32767 + .0f));
        }
        
        private unsafe void UpdateGPUNormalMapResources()
        {
            ushort* Data;
            HardwarePixelBuffer PixelBuffer;
            PixelBuffer = mGPUNormalMapManager.GetTexture(0).GetBuffer();

            PixelBuffer.Lock(BufferLocking.Discard);
            PixelBox PixelBox = PixelBuffer.CurrentLock;

            Data = (ushort*)PixelBox.Data.ToPointer();
            for (int u = 0; u < resolution * resolution; u++)
            {
                Data[u] = (ushort)(re[u] * 65535);
            }

            PixelBuffer.Unlock();
        }
    }
}
