using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using Axiom.Core;
using Axiom.Math;
using Axiom.Hydrax.Noise;
namespace Axiom.Hydrax.Modules
{
    public class SimpleGrid : Module
    {
        private static Mesh.VertexType SGGetVertexTypeFromNormalMode(MaterialManager.NormalMode NormalMode)
        {
            return Mesh.VertexType.VT_POS;
        }
        private static string SGGetNormalModeString(MaterialManager.NormalMode NormalMode)
        {
            if(NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                return "Vertex";
            return "RTT";
        }
        
        /// <summary>
		/// SimpleGrid Options
		/// </summary>
		public class Options
		{
			/// <summary>
			/// Projected grid complexity (N*N)
			/// </summary>
			public int Complexity;
			/// Size
			public Size MeshSize;// = 100;
			/// <summary>
			/// Strength
			/// </summary>
			public float Strength;
			/// Smooth
			public bool Smooth;
            /// Choppy waves
            public bool ChoppyWaves;
            /// Choppy waves strength
            public float ChoppyStrength;

			/// <summary>
			/// Default constructor
			/// </summary>
			public Options()
			{
				Complexity = 256;
				MeshSize = new Size(100);
				Strength = 32.5f;
				Smooth = false;
                ChoppyWaves = true;
                ChoppyStrength = 0.065f;
			}

		
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="complexity">Simple grid complexity</param>
			/// <param name="meshSize">Water mesh size</param>
			public Options(int  complexity, Size meshSize)
			{
				Complexity = complexity;
				MeshSize = meshSize;
				Strength = 32.5f;
				Smooth = false;
                ChoppyWaves = true;
                ChoppyStrength = 0.065f;
			}

			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="complexity">Simple grid complexity</param>
			/// <param name="meshSize">Water mesh size</param>
			/// <param name="strength">Noise strength</param>
			/// <param name="smooth">Smooth vertex?</param>
			/// <param name="choppyWaves">Choppy waves enabled? Note: Only with Vertex normal mode.</param>
			/// <param name="choppyStrength">Choppy waves strength Note: Only with Vertex normal mode.</param>
			public Options(int   complexity,
				   	 	   Size  meshSize,
				    	   float strength,
						   bool  smooth,
                           bool  choppyWaves,
					       float choppyStrength)
			{
				Complexity = complexity;
				MeshSize = meshSize;
				Strength = strength;
				Smooth = smooth;
                ChoppyWaves = choppyWaves;
                ChoppyStrength = choppyStrength;
			}
		}

        protected Mesh.POS_NORM_VERTEX[] mVerticesPosNormVertex;
        protected Mesh.POS_VERTEX[] mVerticesPosVertex;
        protected Mesh.POS_NORM_VERTEX[] mVerticesChoppyBuffer;
        protected Options mOptions;
        protected Hydrax mHydrax;
        
        
        public SimpleGrid(Hydrax H, BaseNoise Noise, MaterialManager.NormalMode NormalMode, Options Options)
            : base("SimpleGrid" + SimpleGrid.SGGetNormalModeString(NormalMode),
            Noise, new Mesh.Options(256, new Size(100), SimpleGrid.SGGetVertexTypeFromNormalMode(NormalMode), 10),
            NormalMode)
        {
            this.NormalMode = NormalMode;
            mHydrax = H;
            SetOptions(Options);
        }
        
        public void SetOptions(Options options)
        {
            mMeshOptions.MeshSize = options.MeshSize;
            mMeshOptions.MeshStrength = options.Strength;
            mMeshOptions.MeshComplexity = options.Complexity;

            mHydrax.Mesh.SetOptions(mMeshOptions);
            mHydrax.Strength = options.Strength;

            if (IsCreated)
		    {
                if (options.Complexity != mOptions.Complexity || options.ChoppyWaves != mOptions.ChoppyWaves)
			    {
				    Remove();
				    mOptions = options;
				    Create();

				    if (mNormalMode == MaterialManager.NormalMode.NM_RTT)
				    {
					    if (!mNoise.CreateGPUNormalMapResources(mHydrax.GPUNormalMapManager))
					    {
						    LogManager.Instance.Write(mNoise.Name + " doesn't support GPU Normal map generation.");
					    }
				    }

				    String MaterialNameTmp = mHydrax.Mesh.MaterialName;
				    mHydrax.Mesh.Remove();
    				
				    mHydrax.Mesh.SetOptions(MeshOptions);
				    mHydrax.Mesh.MaterialName = MaterialNameTmp;
				    mHydrax.Mesh.Create();

				    return;
			    }

			    mOptions = options;

			    int v, u;
			    if (mNormalMode == MaterialManager.NormalMode.NM_VERTEX)
			    {
				    for(v=0; v<mOptions.Complexity; v++)
				    {
					    for(u=0; u<mOptions.Complexity; u++)
					    {
						    mVerticesPosNormVertex[v*mOptions.Complexity + u].X  = (((float)v)/(mOptions.Complexity-1)) * mOptions.MeshSize.Width;
						    mVerticesPosNormVertex[v*mOptions.Complexity + u].Z  = (((float)u)/(mOptions.Complexity-1)) * mOptions.MeshSize.Height;
					    }
				    }

                    if (mOptions.ChoppyWaves && (mVerticesChoppyBuffer != null))
                    {
                        for (int i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                        {
                            mVerticesChoppyBuffer[i] = mVerticesPosNormVertex[i];
                        }
                    }
			    }
                else if (mNormalMode == MaterialManager.NormalMode.NM_RTT)
			    {
				    for(v=0; v<mOptions.Complexity; v++)
				    {
					    for(u=0; u<mOptions.Complexity; u++)
					    {
                            mVerticesPosVertex[v * mOptions.Complexity + u].X = (((float)v) / (mOptions.Complexity - 1)) * mOptions.MeshSize.Width;
                            mVerticesPosVertex[v * mOptions.Complexity + u].Z = (((float)u) / (mOptions.Complexity - 1)) * mOptions.MeshSize.Height;
					    }
				    }
			    }
    			
			    return;
		    } 

		    mOptions = options;
        }

        public override void Create()
        {
            Hydrax.HydraxLog("Creating " + Name + " module.");
            base.Create();

            int v, u;
            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                mVerticesPosNormVertex = new Mesh.POS_NORM_VERTEX[mOptions.Complexity * mOptions.Complexity];

                for(v=0; v<mOptions.Complexity; v++)
			    {
				    for(u=0; u<mOptions.Complexity; u++)
				    {
					    mVerticesPosNormVertex[v*mOptions.Complexity + u].X   = (((float)v)/(mOptions.Complexity-1)) * mOptions.MeshSize.Width;
                        mVerticesPosNormVertex[v * mOptions.Complexity + u].Z = (((float)u) / (mOptions.Complexity - 1)) * mOptions.MeshSize.Height;

					    mVerticesPosNormVertex[v*mOptions.Complexity + u].NX = 0;
					    mVerticesPosNormVertex[v*mOptions.Complexity + u].NY = -1;
					    mVerticesPosNormVertex[v*mOptions.Complexity + u].NZ = 0;
				    }
			    }

                if (mOptions.ChoppyWaves)
                {
                    mVerticesChoppyBuffer = new Mesh.POS_NORM_VERTEX[mOptions.Complexity * mOptions.Complexity];

                    for (int i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                    {
                        mVerticesChoppyBuffer[i] = mVerticesPosNormVertex[i];
                    }
                }
            }
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                mVerticesPosVertex = new Mesh.POS_VERTEX[mOptions.Complexity * mOptions.Complexity];
                for (v = 0; v < mOptions.Complexity; v++)
                {
                    for (u = 0; u < mOptions.Complexity; u++)
                    {
                        mVerticesPosVertex[v * mOptions.Complexity + u].X = (((float)v) / (mOptions.Complexity - 1)) * mOptions.MeshSize.Width;
                        mVerticesPosVertex[v * mOptions.Complexity + u].Z = (((float)u) / (mOptions.Complexity - 1)) * mOptions.MeshSize.Height;
                    }
                }

            }

            Hydrax.HydraxLog(Name + " created.");
        }

        public override void Update(float TimeSinceLastFrame)
        {
            if (!IsCreated)
                return;

            base.Update(TimeSinceLastFrame);

            // Update heigths
            int i = 0, v, u;

            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                if (mOptions.ChoppyWaves)
                {
                    for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                    {
                        mVerticesPosNormVertex[i] = mVerticesChoppyBuffer[i];
                        mVerticesPosNormVertex[i].Y = mNoise.GetValue(mVerticesPosNormVertex[i].X, mVerticesPosNormVertex[i].Z) * mOptions.Strength;
                    }
                }
                else
                {
                    for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                    {
                        mVerticesPosNormVertex[i].Y = mNoise.GetValue(mVerticesPosNormVertex[i].X, mVerticesPosNormVertex[i].Z) * mOptions.Strength;
                    }
                }
            }
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                // For object-space to world-space conversion
                // RTT normals calculation needs world-space coords
                Vector3 p = new Vector3(0, 0, 0);
                Matrix4[] mWorldMatrix = new Matrix4[1];
                mHydrax.Mesh.Entity.ParentSceneNode.GetWorldTransforms(mWorldMatrix);
                for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                {
                    p.x = mVerticesPosVertex[i].X;
                    p.y = 0;
                    p.z = mVerticesPosVertex[i].Z;

                    // Calculate the world-space position
                    mWorldMatrix[0].TransformAffine(p);
                    mVerticesPosVertex[i].Y = mNoise.GetValue(p.x, p.z) * mOptions.Strength;
                }
            }

            if (mOptions.Smooth)
            {
                if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                {
                    Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

                    for (v = 1; v < (mOptions.Complexity - 1); v++)
                    {
                        for (u = 1; u < (mOptions.Complexity - 1); u++)
                        {
                            mVerticesPosNormVertex[v * mOptions.Complexity + u].Y =
                                 0.2f *
                                (mVerticesPosNormVertex[v * mOptions.Complexity + u].Y +
                                 mVerticesPosNormVertex[v * mOptions.Complexity + (u + 1)].Y +
                                 mVerticesPosNormVertex[v * mOptions.Complexity + (u - 1)].Y +
                                 mVerticesPosNormVertex[(v + 1) * mOptions.Complexity + u].Y +
                                 mVerticesPosNormVertex[(v - 1) * mOptions.Complexity + u].Y);
                        }
                    }
                }
                else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    for (v = 1; v < (mOptions.Complexity - 1); v++)
                    {
                        for (u = 1; u < (mOptions.Complexity - 1); u++)
                        {
                            mVerticesPosVertex[v * mOptions.Complexity + u].Y =
                                 0.2f *
                                (mVerticesPosVertex[v * mOptions.Complexity + u].Y +
                                 mVerticesPosVertex[v * mOptions.Complexity + (u + 1)].Y +
                                 mVerticesPosVertex[v * mOptions.Complexity + (u - 1)].Y +
                                 mVerticesPosVertex[(v + 1) * mOptions.Complexity + u].Y +
                                 mVerticesPosVertex[(v - 1) * mOptions.Complexity + u].Y);
                        }
                    }
                }
            }

            CalculeNormals();
            PerformChoppyWaves();
            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosNormVertex);
            //if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            //    mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosVertex);
        }

        private void CalculeNormals()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX)
                return;

            int v, u;
            Vector3 vec1, vec2, normal;

            for(v=1; v<(mOptions.Complexity-1); v++)
			{
				for(u=1; u<(mOptions.Complexity-1); u++)
				{
					vec1 = new Vector3(
						mVerticesPosNormVertex[v*mOptions.Complexity + u + 1].X-mVerticesPosNormVertex[v*mOptions.Complexity + u - 1].X,
						mVerticesPosNormVertex[v*mOptions.Complexity + u + 1].Y-mVerticesPosNormVertex[v*mOptions.Complexity + u - 1].Y, 
						mVerticesPosNormVertex[v*mOptions.Complexity + u + 1].Z-mVerticesPosNormVertex[v*mOptions.Complexity + u - 1].Z);
	
					vec2 = new Vector3(
						mVerticesPosNormVertex[(v+1)*mOptions.Complexity + u].X - mVerticesPosNormVertex[(v-1)*mOptions.Complexity + u].X,
						mVerticesPosNormVertex[(v+1)*mOptions.Complexity + u].Y - mVerticesPosNormVertex[(v-1)*mOptions.Complexity + u].Y,
						mVerticesPosNormVertex[(v+1)*mOptions.Complexity + u].Z - mVerticesPosNormVertex[(v-1)*mOptions.Complexity + u].Z);
	
					normal = vec2.Cross(vec1);
					mVerticesPosNormVertex[v*mOptions.Complexity + u].NX = normal.x;
					mVerticesPosNormVertex[v*mOptions.Complexity + u].NY = normal.y;
					mVerticesPosNormVertex[v*mOptions.Complexity + u].NZ = normal.z;
				}
			}
        }
        private void PerformChoppyWaves()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX || !mOptions.ChoppyWaves)
                return;

            int v, u,
            Underwater = 1;
            if (mHydrax.IsCurrentFrameUnderwater)
                Underwater = -1;

            for (v = 1; v < (mOptions.Complexity - 1); v++)
            {
                for (u = 1; u < (mOptions.Complexity - 1); u++)
                {
                    mVerticesPosNormVertex[v * mOptions.Complexity + u].X += mVerticesPosNormVertex[v * mOptions.Complexity + u].NX * mOptions.ChoppyStrength * Underwater;
                    mVerticesPosNormVertex[v * mOptions.Complexity + u].Z += mVerticesPosNormVertex[v * mOptions.Complexity + u].NZ * mOptions.ChoppyStrength * Underwater;
                }
            }
        }

        public override float GetHeight(Vector2 Position)
        {
            if (NormalMode != MaterialManager.NormalMode.NM_RTT)
            {
                Vector2 RelativePos = mHydrax.Mesh.GridPosition(Position);

                RelativePos.x *= mOptions.MeshSize.Width;
                RelativePos.y *= mOptions.MeshSize.Height;

                return mHydrax.Position.y + mNoise.GetValue(RelativePos.x, RelativePos.y) * mOptions.Strength;
            }
            else// RTT Normals calculations works with world-space coords
            {
                return mHydrax.Position.y + mNoise.GetValue(Position.x, Position.y) * mOptions.Strength;
            }
        }
    }
}
