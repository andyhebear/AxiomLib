#define _def_PackedNoise_true
using System;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
using System.Collections.Generic;
using Axiom.Media;

namespace Axiom.Hydrax.Noise
{
    public unsafe class Perlin : BaseNoise
    {
        #region - Constants -
        const int n_bits = 5;
        const int n_size = (1 << (n_bits - 1));
        const int n_size_m1 = (n_size - 1);
        const int n_size_sq = (n_size * n_size);
        const int n_size_sq_m1 = (n_size_sq - 1);

        const int n_packsize = 4;

        const int np_bits = (n_bits + n_packsize - 1);
        const int np_size = (1 << (np_bits - 1));
        const int np_size_m1 = (np_size - 1);
        const int np_size_sq = (np_size * np_size);
        const int np_size_sq_m1 = (np_size_sq - 1);

        const int n_dec_bits = 12;
        const int n_dec_magn = 4096;
        const int n_dec_magn_m1 = 4095;

        const int max_octaves = 32;
        const int noise_frames = 256;
        const int noise_frames_m1 = (noise_frames - 1);

        const int noise_decimalbits = 15;
        const int noise_magnitude = (1 << (noise_decimalbits - 1));

        const int scale_decimalbits = 15;
        const int scale_magnitude = (1 << (scale_decimalbits - 1));

        const int nmapsize_x = 512;
        const int nmapsize_y = 1024;
        #endregion

        #region - Options -
        /// <summary>
		/// Class wich contains Perlin noise module options
		/// </summary>
		public class Options
		{
			/// <summary>
			/// Octaves
			/// </summary>
			public int Octaves;
			/// <summary>
			/// Scale
			/// </summary>
			public float Scale;
			/// <summary>
			/// Falloff
			/// </summary>
			public float Falloff;
			/// <summary>
			/// Animspeed
			/// </summary>
			public float Animspeed;
			/// <summary>
			/// Timemulti
			/// </summary>
			public float Timemulti;
			
			/// <summary>
			/// GPU Normal map generator parameters
			/// Only if GPU normal map generation is active
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
			public Options()
			{
				Octaves = 8;
				Scale = 0.085f;
				Falloff = 0.49f;
				Animspeed = 1.4f;
				Timemulti = 1.27f;
				GPU_Strength = 2.0f;
                GPU_LODParameters = new Vector3(0.5f, 50, 150000);
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="octaves"></param>
			/// <param name="scale"></param>
			/// <param name="falloff"></param>
			/// <param name="animspeed"></param>
			/// <param name="timemulti"></param>
			public Options(int   octaves,
							float scale,
							float falloff,
							float animspeed,
							float timemulti)
			{
				Octaves = octaves;
				Scale = scale;
				Falloff = falloff;
				Animspeed = animspeed;
				Timemulti = timemulti;
				GPU_Strength = 2.0f;
                GPU_LODParameters = new Vector3(0.5f, 50, 150000);
			}
			

			/// <summary>
			/// 
			/// </summary>
			/// <param name="octaves"></param>
			/// <param name="scale"></param>
			/// <param name="falloff"></param>
			/// <param name="animspeed"></param>
			/// <param name="timemulti"></param>
			/// <param name="GPU_Strength"></param>
			/// <param name="GPU_LODParameters"></param>
			public Options(int   octaves,
					float scale,
					float falloff,
					float animspeed,
					float timemulti,
					float GPU_Strength,
					Vector3  GPU_LODParameters)
			{
				Octaves = octaves;
				Scale = scale;
				Falloff = falloff;
				Animspeed = animspeed;
				Timemulti = timemulti;
				this.GPU_Strength = GPU_Strength;
				this.GPU_LODParameters = GPU_LODParameters;
			}

		};
        #endregion

        #region - fields -
        /// Perlin noise variables
       
        double[] noise = new double[n_size_sq*noise_frames];
		int[] o_noise = new int[n_size_sq*max_octaves];
        int[] p_noise = new int[262144];//np_size_sq * (max_octaves >> (n_packsize - 1))];	
		int *r_noise;
        float magnitude;
        protected Random mRand = new Random();

        /// Elapsed time
        double time;

        /// GPUNormalMapManager pointer
        GPUNormalMapManager mGPUNormalMapManager;
        /// Perlin noise options
        Options mOptions;
        #endregion

        #region - Constructor, Destructor -
        /// <summary>
        /// 
        /// </summary>
        public Perlin()
            : base("Perlin", true)
        {
            magnitude = n_dec_magn * 0.085f;
            mOptions = new Options();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Options"></param>
        public Perlin(Options Options)
            : base("Perlin", true)
        {
            magnitude = n_dec_magn * 0.085f;
            mOptions = Options;
        }
        #endregion

        /// <summary>
        /// Return some noise for a specific seed.
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public static float Noise(int i)
        {
            i = (i << 13) ^ i;
            return (1.0f - ((i * (i * i * 15731 + 789221) + 1376312589) & 0x7FFFFFFF) / 1073741824.0f);
        }
        /// <summary>
        /// 
        /// </summary>
        public override void Create()
        {
            if (IsCreated)
                return;

            base.Create();
            InitNoise();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Options"></param>
        public void SetOptions(Options Options)
        {
            if (IsCreated)
            {
                throw new NotImplementedException();
            }
            else
            {
                mOptions = Options;
                mOptions.Octaves = (mOptions.Octaves < max_octaves) ? mOptions.Octaves : max_octaves;
            }

            magnitude = n_dec_magn * mOptions.Scale;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="G"></param>
        /// <returns></returns>
        public override bool CreateGPUNormalMapResources(GPUNormalMapManager G)
        {
            if (!base.CreateGPUNormalMapResources(G))
                return false;

            mGPUNormalMapManager = G;

            //create our perlin textures

            Texture mPerlinTexture0
                = Axiom.Core.TextureManager.Instance.
                CreateManual("_Hydrax_Perlin_Noise0",
                             "HYDRAX_RESOURCE_GROUP",
                             Axiom.Graphics.TextureType.TwoD,
                             np_size, np_size, 0,
                             Axiom.Media.PixelFormat.L16,
                             Axiom.Graphics.TextureUsage.DynamicWriteOnly);

            Texture mPerlinTexture1
                = Axiom.Core.TextureManager.Instance.
                CreateManual("_Hydrax_Perlin_Noise1",
                             "HYDRAX_RESOURCE_GROUP",
                             Axiom.Graphics.TextureType.TwoD,
                             np_size, np_size, 0,
                             Axiom.Media.PixelFormat.L16,
                             Axiom.Graphics.TextureUsage.DynamicWriteOnly);

            mGPUNormalMapManager.AddTexture(mPerlinTexture0);
            mGPUNormalMapManager.AddTexture(mPerlinTexture1);

            //create our normal map generator material.

            MaterialManager mMaterialManager = G.Hydrax.MaterialManager;

            string VertexProgramData = "", FragmentProgramData = "";

            GpuProgramParameters VP_Parameters, FP_Parameters;
            string[] EntryPoints = { "main_vp", "main_fp" };
            string[] GpuProgramsData = new string[2];
            string[] GpuProgramNames = new string[2];

            #region - VertexProgram -
            switch (G.Hydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                //fall through
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
                    //not yet supported
                    break;
            }
            #endregion

            #region - FragmentProgram -
            switch (G.Hydrax.ShaderMode)
            {
                case MaterialManager.ShaderMode.SM_HLSL:
                //fall through
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
                        "uniform sampler2D uNoise0 : register(s0),\n" +
                        "uniform sampler2D uNoise1 : register(s1))\n" +
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
                                      "tex2D(uNoise0, iWorldCoord.xy+dx).x + tex2D(uNoise1, iWorldCoord.zw+dx*16).x,\n" +
                            // z
                                      "iPosition.z);\n" +

                        "m_dx = float3(\n" +
                            // x-
                                      "iPosition.x-uLODParameters.x,\n" +
                            // y-
                                      "tex2D(uNoise0, iWorldCoord.xy-dx).x + tex2D(uNoise1, iWorldCoord.zw-dx*16).x,\n" +
                            // z
                                      "iPosition.z);\n" +

                       "p_dy = float3(\n" +
                            // x
                                      "iPosition.x,\n" +
                            // y+
                                      "tex2D(uNoise0, iWorldCoord.xy+dy).x + tex2D(uNoise1, iWorldCoord.zw+dy*16).x,\n" +
                            // z+
                                      "iPosition.z+uLODParameters.x);\n" +

                       "m_dy = float3(\n" +
                            // x
                                      "iPosition.x,\n" +
                            // y-
                                      "tex2D(uNoise0, iWorldCoord.xy-dy).x + tex2D(uNoise1, iWorldCoord.zw-dy*16).x,\n" +
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
                    //not yet supported
                    break;
            }
            #endregion

            #region - BuildMaterial -
            Material mNormalMapMaterial = mGPUNormalMapManager.NormalMapMaterial;
            mNormalMapMaterial = (Material)
                Axiom.Graphics.MaterialManager.Instance.Create(
                "_Hydrax_GPU_Normal_Map_Material", "HYDRAX_RESOURCE_GROUP");
            Pass Technique0_Pass0 = mNormalMapMaterial.GetTechnique(0).GetPass(0);
            mGPUNormalMapManager.NormalMapMaterial = mNormalMapMaterial;
            Technique0_Pass0.LightingEnabled = false;
            Technique0_Pass0.CullingMode = CullingMode.None;
            Technique0_Pass0.DepthWrite = true;
            Technique0_Pass0.DepthCheck = true;

            GpuProgramsData[0] = VertexProgramData;
            GpuProgramsData[1] = FragmentProgramData;

            GpuProgramNames[0] = "_Hydrax_GPU_Normal_Map_VP";
            GpuProgramNames[1] = "_Hydrax_GPU_Normal_Map_FP";

            mMaterialManager.FillGpuProgramsToPass(Technique0_Pass0, GpuProgramNames,
                G.Hydrax.ShaderMode, EntryPoints, GpuProgramsData);

            VP_Parameters = Technique0_Pass0.VertexProgramParameters;

            VP_Parameters.SetNamedAutoConstant("uWorldViewProj", GpuProgramParameters.AutoConstantType.WorldViewProjMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uWorld", GpuProgramParameters.AutoConstantType.WorldMatrix, 0);
            VP_Parameters.SetNamedAutoConstant("uCameraPos", GpuProgramParameters.AutoConstantType.CameraPositionObjectSpace, 0);

            VP_Parameters.SetNamedConstant("uScale", mOptions.Scale);

            FP_Parameters = Technique0_Pass0.FragmentProgramParameters;

            FP_Parameters.SetNamedConstant("uStrength", mOptions.GPU_Strength);
            FP_Parameters.SetNamedConstant("uLODParameters", mOptions.GPU_LODParameters);

			Technique0_Pass0.CreateTextureUnitState( mGPUNormalMapManager.GetTexture( 0 ).Name, 0 ).
				SetTextureAddressingMode( TextureAddressing.Mirror );

            Technique0_Pass0.CreateTextureUnitState(mGPUNormalMapManager.GetTexture(1).Name, 1).
				SetTextureAddressingMode( TextureAddressing.Mirror );

            mNormalMapMaterial.Load();
            mGPUNormalMapManager.Create();

            #endregion

            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="TimeSinceLastFrame"></param>
        public override void Update(float TimeSinceLastFrame)
        {
            time += TimeSinceLastFrame * mOptions.Animspeed;
            CalculeNoise();
            if (base.IsGPUNormalMapResourcesCreated)
            {
                UpdateNormalMapResources();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        private unsafe void UpdateNormalMapResources()
        {
            ushort* Data;

            int Offset;
            HardwarePixelBuffer PixelBuffer;

            for (int k = 0; k < 2; k++)
            {
                Offset = np_size_sq * k;
                PixelBuffer = mGPUNormalMapManager.GetTexture(k).GetBuffer();

                PixelBuffer.Lock(BufferLocking.Discard);
                PixelBox PixelBox = PixelBuffer.CurrentLock;

                Data = (ushort*)PixelBox.Data.ToPointer();
                for (int u = 0; u < np_size_sq; u++)
                {
                    Data[u] = (ushort)(32768 + p_noise[u + Offset]);
                }

                PixelBuffer.Unlock();

            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="X"></param>
        /// <param name="Y"></param>
        /// <returns></returns>
        public override float GetValue(float X, float Y)
        {
            return GetHeightValue(X, Y);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        private float GetHeightValue(float u, float v)
        {
            int value = 0;
            // Pointer to the current noise source octave	
            fixed (int* rPtr = p_noise)
            {
                r_noise = rPtr;

                int ui = (int)(u * magnitude),
                    vi = (int)(v * magnitude),
                    i,
                    hoct = mOptions.Octaves / n_packsize;

                for (i = 0; i < hoct; i++)
                {
                    value += ReadTexelLinearDual(ui, vi, 0);
                    ui = ui << n_packsize;
                    vi = vi << n_packsize;
                    r_noise += np_size_sq;
                }
            }
            double a = (double)noise_magnitude;
            double c = (double)value;
            double ret = (c / a);
            float retf = (float)ret;
            return retf;
        }
        /// <summary>
        /// 
        /// </summary>
        private void InitNoise()
        {

            //create noise (uniform)
            float[] tempnoise = new float[n_size_sq * noise_frames];
            double temp;

            int i, frame, v, u,
            v0, v1, v2, u0, u1, u2, f;

            for (i = 0; i < (n_size_sq * noise_frames); i++)
            {
               double ran = (double)mRand.Next(32767);
                temp = (ran / 32767);
                tempnoise[i] = (float)(4 * (temp - 0.5f));
            }

            for (frame = 0; frame < noise_frames; frame++)
            {
                for (v = 0; v < n_size; v++)
                {
                    for (u = 0; u < n_size; u++)
                    {
                        v0 = ((v - 1) & n_size_m1) * n_size;
                        v1 = v * n_size;
                        v2 = ((v + 1) & n_size_m1) * n_size;
                        u0 = ((u - 1) & n_size_m1);
                        u1 = u;
                        u2 = ((u + 1) & n_size_m1);
                        f = frame * n_size_sq;

                        temp = (1.0f / 14.0f) *
                       (tempnoise[f + v0 + u0] + tempnoise[f + v0 + u1] + tempnoise[f + v0 + u2] +
                        tempnoise[f + v1 + u0] + 6.0f * tempnoise[f + v1 + u1] + tempnoise[f + v1 + u2] +
                        tempnoise[f + v2 + u0] + tempnoise[f + v2 + u1] + tempnoise[f + v2 + u2]);

                        noise[frame * n_size_sq + v * n_size + u] = (noise_magnitude * temp);
                    }
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="o"></param>
        /// <returns></returns>
        private int ReadTexelLinearDual(int u, int v, int o)
        {
            int iu, iup, iv, ivp, fu, fv;
            iu = (u >> n_dec_bits) & np_size_m1;
            iv = ((v >> n_dec_bits) & np_size_m1) * np_size;

            iup = ((u >> n_dec_bits) + 1) & np_size_m1;
            ivp = (((v >> n_dec_bits) + 1) & np_size_m1) * np_size;

            fu = u & n_dec_magn_m1;
            fv = v & n_dec_magn_m1;

            int ut01 = ((n_dec_magn - fu) * r_noise[iv + iu] + fu * r_noise[iv + iup]) >> n_dec_bits;
            int ut23 = ((n_dec_magn - fu) * r_noise[ivp + iu] + fu * r_noise[ivp + iup]) >> n_dec_bits;
            int ut = ((n_dec_magn - fv) * ut01 + fv * ut23) >> n_dec_bits;
            return ut;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="upsamplepower"></param>
        /// <param name="octave"></param>
        /// <returns></returns>
        private int MapSample(int u, int v, int upsamplepower, int octave)
        {
            int magnitude = 1 << upsamplepower;
            int pu = u >> upsamplepower;
            int pv = v >> upsamplepower;
            int fu = u & (magnitude - 1);
            int fv = v & (magnitude - 1);
            int fu_m = magnitude - fu;
            int fv_m = magnitude - fv;

            int o = fu_m * fv_m * o_noise[octave * n_size_sq + ((pv) & n_size_m1) * n_size + ((pu) & n_size_m1)] +
                    fu * fv_m * o_noise[octave * n_size_sq + ((pv) & n_size_m1) * n_size + ((pu + 1) & n_size_m1)] +
                    fu_m * fv * o_noise[octave * n_size_sq + ((pv + 1) & n_size_m1) * n_size + ((pu) & n_size_m1)] +
                    fu * fv * o_noise[octave * n_size_sq + ((pv + 1) & n_size_m1) * n_size + ((pu + 1) & n_size_m1)];
            return o >> (upsamplepower + upsamplepower);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="ipart"></param>
        /// <returns></returns>
        double modf(double x, double ipart)
        {
            int i = 0;
            if (x >= 10000)
            {
                if (x >= 50000) i = 50000;
                else i = 10000;
            }
            while (x >= i) i++;
            ipart = 3;// (i - 1);
            return (x - (ipart));
        }
        /// <summary>
        /// 
        /// </summary>
        private void CalculeNoise()
        {
            int i, o, v, u, iImage;
            int[] multitable = new int[max_octaves];
            int[] amount = new int[3];



            int[] image = new int[3];

            float sum = 0.0f;
            float[] f_multitable = new float[max_octaves];

            double dImage = 0, fraction = 0;

            // calculate the strength of each octave
            for (i = 0; i < mOptions.Octaves; i++)
            {
                f_multitable[i] = (float)System.Math.Pow(mOptions.Falloff, 1.0f * i);
                sum += f_multitable[i];
            }

            for (i = 0; i < mOptions.Octaves; i++)
            {
                f_multitable[i] /= sum;
            }

            for (i = 0; i < mOptions.Octaves; i++)
            {
                multitable[i] = (int)(scale_magnitude * f_multitable[i]);
            }

            double r_timemulti = 1.0;
            const float PI_3 = (float)System.Math.PI / 3;

            for (o = 0; o < mOptions.Octaves; o++)
            {

                fraction = modf((time * r_timemulti), dImage);
                iImage = (int)dImage;

                amount[0] = (int)(scale_magnitude * f_multitable[o] * (System.Math.Pow(System.Math.Sin((fraction + 2) * PI_3), 2) / 1.5));
                amount[1] = (int)(scale_magnitude * f_multitable[o] * (System.Math.Pow(System.Math.Sin((fraction + 1) * PI_3), 2) / 1.5));
                amount[2] = (int)(scale_magnitude * f_multitable[o] * (System.Math.Pow(System.Math.Sin((fraction) * PI_3), 2) / 1.5));

                image[0] = (iImage) & noise_frames_m1;
                image[1] = (iImage + 1) & noise_frames_m1;
                image[2] = (iImage + 2) & noise_frames_m1;

                for (i = 0; i < n_size_sq; i++)
                {
                    o_noise[i + n_size_sq * o] = (
                       ((int)(amount[0] * noise[i + n_size_sq * image[0]]) >> scale_decimalbits) +
                       ((int)(amount[1] * noise[i + n_size_sq * image[1]]) >> scale_decimalbits) +
                       ((int)(amount[2] * noise[i + n_size_sq * image[2]]) >> scale_decimalbits));
                }

                r_timemulti *= mOptions.Timemulti;
            }
#if _def_PackedNoise_true
            {

                try
                {
                    int octavepack = 0;
                    for (o = 0; o < mOptions.Octaves; o += n_packsize)
                    {
                        for (v = 0; v < np_size; v++)
                        {
                            for (u = 0; u < np_size; u++)
                            {
                                p_noise[v * np_size + u + octavepack * np_size_sq] = o_noise[(o + 3) * n_size_sq + (v & n_size_m1) * n_size + (u & n_size_m1)];
                                p_noise[v * np_size + u + octavepack * np_size_sq] += MapSample(u, v, 3, o);
                                p_noise[v * np_size + u + octavepack * np_size_sq] += MapSample(u, v, 2, o + 1);
                                p_noise[v * np_size + u + octavepack * np_size_sq] += MapSample(u, v, 1, o + 2);
                            }
                        }

                        octavepack++;
                    }
                }
                catch { };
            }
#endif
        }
       
    }
}


