using System;
using Axiom.Hydrax.Noise;
using Axiom.Math;
using Axiom.Core;
using Axiom.Graphics;
namespace Axiom.Hydrax.Modules
{
    public class RadialGrid : Module
    {
        /** Struct wich contains Hydrax simple grid module options
		 */
		public struct Options
		{
			/// Number of steps (Per circle)
			public int Steps;
			/// Number of circles
			public int Circles;
			/// Radius (In world units)
			public float Radius;
			/// Smooth
			public bool Smooth;
			/// Choppy waves
			public bool ChoppyWaves;
			/// Choppy waves strength
			public float ChoppyStrength;
			/// Step cube size
			public float StepSizeCube;
			/// Step size five
			public float StepSizeFive;
			/// Step lin size
			public float StepSizeLin;
			/// Water strength
			public float Strength;

			/** Default constructor
			 
			Options()
				: Steps(250)
				, Circles(250)
				, Radius(100)
				, Smooth(false)
				, ChoppyWaves(true)
				, ChoppyStrength(3.5f)
				, StepSizeCube(0.00001f)
				, StepSizeFive(0.0f)
				, StepSizeLin(0.1f)
				, Strength(32.5f)
			{
			}*/

			/** Constructor
			    @param _Steps Number of steps per circle
				@param _Circles Number of circles
				@param _Radius Mesh radius
			 */
			public Options(int _Steps, 
				    int _Circles,
					float _Radius)
				/*: Steps(_Steps)
				, Circles(_Circles)
				, Radius(_Radius)
				, Smooth(false)
				, ChoppyWaves(true)
				, ChoppyStrength(3.5f)
				, StepSizeCube(0.00001f)
				, StepSizeFive(0.0f)
				, StepSizeLin(0.1f)
				, Strength(32.5f)*/
			{
                this.Steps = _Steps;
                this.Circles = _Circles;
                this.Radius = _Radius;
                Smooth = false;
                ChoppyWaves = true;
                ChoppyStrength = 3.5f;
                StepSizeCube = 0.00001f;
                    StepSizeFive = 0.0f;
                StepSizeLin = 0.1f;
                Strength = 32.5f;
			}

            /** Constructor
                @param _Steps Number of steps per circle
                @param _Circles Number of circles
                @param _Radius Mesh radius
                @param _Smooth Smooth vertex?
                @param _ChoppyWaves Choppy waves enabled? Note: Only with Materialmanager::NM_VERTEX normal mode.
                @param _ChoppyStrength Choppy waves strength Note: Only with Materialmanager::NM_VERTEX normal mode.
                @param _StepSizeCube Step cube size
                @param _StepSizeFive Step five size
                @param _StepSizeLin Step lin size
                @param _Strength Water strength
			 
            Options(const int &_Steps, 
                    const int &_Circles,
                    const float &_Radius,
                    const bool  &_Smooth,
                    const bool  &_ChoppyWaves,
                    const float &_ChoppyStrength,
                    const float &_StepSizeCube,
                    const float &_StepSizeFive,
                    const float &_StepSizeLin,
                    const float &_Strength)
                : Steps(_Steps)
                , Circles(_Circles)
                , Radius(_Radius)
                , Smooth(_Smooth)
                , ChoppyWaves(_ChoppyWaves)
                , ChoppyStrength(_ChoppyStrength)
                , StepSizeCube(_StepSizeCube)
                , StepSizeFive(_StepSizeFive)
                , StepSizeLin(_StepSizeLin)
                , Strength(_Strength)
            {
            }*/
        };

        protected Hydrax mHydrax;
        protected Options mOptions;
        protected Mesh.POS_NORM_VERTEX[] mVerticesChoppyBuffer;
        Mesh.POS_NORM_VERTEX[] mVerticesPosNormVertex;
        Mesh.POS_VERTEX[] mVerticesPosVertex;
        public RadialGrid(Hydrax H, BaseNoise Noise, Plane BasePlane,
            MaterialManager.NormalMode NormalMode,
            Options Options)
            :base("ProjectedGrid" +
            ((NormalMode == MaterialManager.NormalMode.NM_VERTEX) ? "Vertex" : "Rtt"),
            Noise,
            new Mesh.Options(256, new Size(200, 200),
                ((NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                ? Mesh.VertexType.VT_POS_NORM : Mesh.VertexType.VT_POS),
                10),
            NormalMode)
        {
            mHydrax = H;
            mOptions = Options;
            SetOptions(Options);
        }
        public override void Update(float TimeSinceLastFrame)
        {
            if (!IsCreated)
                return;

            base.Update(TimeSinceLastFrame);

            //update heights
            int i, x, y;
            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
                if (mOptions.ChoppyWaves)
                {
                    for (i = 0; i < 1 + mOptions.Circles * mOptions.Steps; i++)
                    {
                        Vertices[i] = mVerticesChoppyBuffer[i];
                    }
                }

                Vector3 HydraxPos = mHydrax.Position;
                for (i = 0; i < 1 + mOptions.Circles * mOptions.Steps; i++)
                {
                    
                    Vertices[i].Y = 
                        mNoise.GetValue(HydraxPos.x + Vertices[i].X, 
                                        HydraxPos.z + Vertices[i].Z) * mOptions.Strength;
                    if (Vertices[i].Y != 0)
                    {
                    }
                }
            }
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;
                Vector3 HydraxPos = mHydrax.Position;

                for (i = 0; i < 1 + mOptions.Circles * mOptions.Steps; i++)
                {
                    Vertices[i].Y = mNoise.GetValue
                        (HydraxPos.x + Vertices[i].X, HydraxPos.z + Vertices[i].Z) * mOptions.Strength;
                    if (Vertices[i].Y != 0)
                    {
                    }
                }
            }

            if (mOptions.Smooth)
            {
                if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                {
                    Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
                    for (y = 1; y < mOptions.Circles - 1; y++)
                    {
                        for (x = 1; x < mOptions.Steps - 1; x++)
                        {
                            Vertices[y * mOptions.Steps + x].Y =
                                0.2f *
                               (Vertices[y * mOptions.Steps + x].Y +
                                Vertices[y * mOptions.Steps + x + 1].Y +
                                Vertices[y * mOptions.Steps + x - 1].Y +
                                Vertices[(y + 1) * mOptions.Steps + x].Y +
                                Vertices[(y - 1) * mOptions.Steps + x].Y);
                        }
                    }
                }
                else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;
                    for (y = 1; y < mOptions.Circles - 1; y++)
                    {
                        for (x = 1; x < mOptions.Steps - 1; x++)
                        {
                            Vertices[y * mOptions.Steps + x].Y =
                                0.2f *
                               (Vertices[y * mOptions.Steps + x].Y +
                                Vertices[y * mOptions.Steps + x + 1].Y +
                                Vertices[y * mOptions.Steps + x - 1].Y +
                                Vertices[(y + 1) * mOptions.Steps + x].Y +
                                Vertices[(y - 1) * mOptions.Steps + x].Y);
                        }
                    }
                }
            }

            CalculeNormals();

            PerformChoppyWaves();

            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                mHydrax.Mesh.UpdateGeometry(1 + mOptions.Steps * mOptions.Circles, mVerticesPosNormVertex);
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                mHydrax.Mesh.UpdateGeometry(1 + mOptions.Steps * mOptions.Circles, mVerticesPosVertex);

        }
        private void CalculeNormals()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX)
                return;

            int x, y;
            Vector3 vec1, vec2, normal;

            Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

            int Steps_4 = (int)(mOptions.Steps / 4);

            // Calculate the normal of the center grid point
            vec2 = new Vector3(
                        Vertices[1].X - Vertices[1 + Steps_4 * 2].X,
                        Vertices[1].Y - Vertices[1 + Steps_4 * 2].Y,
                        Vertices[1].Z - Vertices[1 + Steps_4 * 2].Z);

            vec1 = new Vector3(
                        Vertices[1 + Steps_4].X - Vertices[1 + Steps_4 * 3].X,
                        Vertices[1 + Steps_4].Y - Vertices[1 + Steps_4 * 3].Y,
                        Vertices[1 + Steps_4].Z - Vertices[1 + Steps_4 * 3].Z);

            normal = vec2.Cross(vec1);

            Vertices[0].NX = normal.x;
            Vertices[0].NY = normal.y;
            Vertices[0].NZ = normal.z;

            // Calculate first circle normals
            for (x = 0; x < mOptions.Steps; x++)
            {
                vec2 = new Vector3(
                    Vertices[x + 2].X - Vertices[x].X,
                    Vertices[x + 2].Y - Vertices[x].Y,
                    Vertices[x + 2].Z - Vertices[x].Z);

                vec1 = new Vector3(
                    Vertices[0].X - Vertices[mOptions.Steps + x].X,
                    Vertices[0].Y - Vertices[mOptions.Steps + x].Y,
                    Vertices[0].Z - Vertices[mOptions.Steps + x].Z);

                normal = vec2.Cross(vec1);

                Vertices[1 + x].NX = normal.x;
                Vertices[1 + x].NY = normal.y;
                Vertices[1 + x].NZ = normal.z;
            }

            // Calculate all the other vertex normals
            for (y = 1; y < mOptions.Circles - 1; y++)
            {
                for (x = 0; x < mOptions.Steps; x++)
                {
                    vec2 = new Vector3(
                        Vertices[y * mOptions.Steps + x + 2].X - Vertices[y * mOptions.Steps + x].X,
                        Vertices[y * mOptions.Steps + x + 2].Y - Vertices[y * mOptions.Steps + x].Y,
                        Vertices[y * mOptions.Steps + x + 2].Z - Vertices[y * mOptions.Steps + x].Z);

                    vec1 = new Vector3(
                        Vertices[(y - 1) * mOptions.Steps + x + 1].X - Vertices[(y + 1) * mOptions.Steps + x].X,
                        Vertices[(y - 1) * mOptions.Steps + x + 1].Y - Vertices[(y + 1) * mOptions.Steps + x].Y,
                        Vertices[(y - 1) * mOptions.Steps + x + 1].Z - Vertices[(y + 1) * mOptions.Steps + x].Z);

                    normal = vec2.Cross(vec1);

                    Vertices[1 + y * mOptions.Steps + x].NX = normal.x;
                    Vertices[1 + y * mOptions.Steps + x].NY = normal.y;
                    Vertices[1 + y * mOptions.Steps + x].NZ = normal.z;
                }
            }
        }
        private void PerformChoppyWaves()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX || !mOptions.ChoppyWaves)
                return;

            int x, y,
            Underwater = 1;

            if (mHydrax.IsCurrentFrameUnderwater)
            {
                Underwater = -1;
            }

            Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

            Vector2 Current, NearStep, CircleStep,
                      Proportion,
                      Dir, Perp,
                      Norm2;
            Vector3 Norm;
            for (y = 0; y < mOptions.Circles - 1; y++)
            {
                Current = new Vector2(Vertices[y * mOptions.Steps + 1].X, Vertices[y * mOptions.Steps + 1].Z);
                NearStep = new Vector2(Vertices[y * mOptions.Steps + 2].X, Vertices[y * mOptions.Steps + 2].Z);
                CircleStep = new Vector2(Vertices[(y + 1) * mOptions.Steps + 1].X, Vertices[(y + 1) * mOptions.Steps + 1].Z);

                Proportion = new Vector2(
                    (Current - NearStep).Length(),
                    (Current - CircleStep).Length());

                for (x = 0; x < mOptions.Steps; x++)
                {
                    Dir = new Vector2(Vertices[1 + y * mOptions.Steps + x].NX, Vertices[1 + y * mOptions.Steps + x].NZ).ToNormalized();
                    Perp = Dir.Perpendicular();

                    if (Dir.x < 0) Dir.x = -Dir.x;
                    if (Dir.y < 0) Dir.y = -Dir.y;

                    if (Perp.x < 0) Perp.x = -Perp.x;
                    if (Perp.y < 0) Perp.y = -Perp.y;

                    Norm = new Vector3(
                       Vertices[1 + y * mOptions.Steps + x].NX,
                       Vertices[1 + y * mOptions.Steps + x].NY,
                       Vertices[1 + y * mOptions.Steps + x].NZ).ToNormalized();

                    Norm2 = new Vector2(Norm.x, Norm.z).Multiply(

                                  mOptions.ChoppyStrength) + (Dir.Multiply(Proportion.x) + Perp.Multiply(Proportion.y));

                    Vertices[1 + y * mOptions.Steps + x].X += Norm2.x * Underwater;
                    Vertices[1 + y * mOptions.Steps + x].Z += Norm2.y * Underwater;
                }
            }
        }
        public override float GetHeight(Vector2 Position)
        {
            return mHydrax.Position.y +mNoise.GetValue(Position.x, Position.y) * mOptions.Strength;
        }
        public override void Create()
        {
            Hydrax.HydraxLog("Creaing " + Name + " module.");
            base.Create();

            int x, y;
            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                mVerticesPosNormVertex = new Mesh.POS_NORM_VERTEX[1 + mOptions.Steps * mOptions.Steps];
                Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

                Vertices[0].X = mOptions.Radius;
                Vertices[0].Y = 0;
                Vertices[0].Z = mOptions.Radius;

                Vertices[0].NX = 0;
                Vertices[0].NY = -1;
                Vertices[0].NZ = 0;

                float r_scale = (float)(mOptions.Radius /
                               (mOptions.StepSizeLin * mOptions.Circles +
                                mOptions.StepSizeCube * System.Math.Pow(mOptions.Circles, 3) +
                                mOptions.StepSizeFive * System.Math.Pow(mOptions.Circles, 5)));

                for (y = 0; y < mOptions.Circles; y++)
                {
                    float r = r_scale * (float)((mOptions.StepSizeLin * (y + 1) + mOptions.StepSizeCube * System.Math.Pow(y + 1, 3) + mOptions.StepSizeFive * System.Math.Pow(y + 1, 5)));

                    for (x = 0; x < mOptions.Steps; x++)
                    {
                        Vertices[1 + y * mOptions.Steps + x].X = mOptions.Radius + r * (float)System.Math.Cos((System.Math.PI * 2) * x / mOptions.Steps);
                        Vertices[1 + y * mOptions.Steps + x].Y = 0;
                        Vertices[1 + y * mOptions.Steps + x].Z = mOptions.Radius + r * (float)System.Math.Sin((System.Math.PI * 2) * x / mOptions.Steps);

                        Vertices[1 + y * mOptions.Steps + x].NX = 0;
                        Vertices[1 + y * mOptions.Steps + x].NY = -1;
                        Vertices[1 + y * mOptions.Steps + x].NZ = 0;
                    }
                }

                if (mOptions.ChoppyWaves)
                {
                    mVerticesChoppyBuffer = new Mesh.POS_NORM_VERTEX[1 + mOptions.Circles * mOptions.Steps];

                    for (int i = 0; i < 1 + mOptions.Circles * mOptions.Steps; i++)
                    {
                        mVerticesChoppyBuffer[i] = Vertices[i];
                    }
                }

            }
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                mVerticesPosVertex = new Mesh.POS_VERTEX[1 + mOptions.Steps * mOptions.Circles];
                Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;
                Vertices[0].X = mOptions.Radius;
                Vertices[0].Y = 0;
                Vertices[0].Z = (int)mOptions.Radius;

                float r_scale = (float)(mOptions.Radius /
                               (mOptions.StepSizeLin * mOptions.Circles +
                                mOptions.StepSizeCube * System.Math.Pow(mOptions.Circles, 3) +
                                mOptions.StepSizeFive * System.Math.Pow(mOptions.Circles, 5)));

                for (y = 0; y < mOptions.Circles; y++)
                {
                    float r = r_scale * (float)((mOptions.StepSizeLin * (y + 1) + mOptions.StepSizeCube * System.Math.Pow(y + 1, 3) + mOptions.StepSizeFive * System.Math.Pow(y + 1, 5)));

                    for (x = 0; x < mOptions.Steps; x++)
                    {
                        Vertices[1 + y * mOptions.Steps + x].X = mOptions.Radius + r * (float)System.Math.Cos((System.Math.PI * 2) * x / mOptions.Steps);
                        Vertices[1 + y * mOptions.Steps + x].Y = 0;
                        Vertices[1 + y * mOptions.Steps + x].Z = mOptions.Radius + r * (float)System.Math.Sin((System.Math.PI * 2) * x / mOptions.Steps);
                    }
                }
            }

            Hydrax.HydraxLog("Module " + Name + " created.");
        }

        public override bool CreateGeometry(Mesh mMesh)
        {
            int numVertices = mOptions.Steps * mOptions.Circles + 1;
            int numEle = 6 * mOptions.Steps * (mOptions.Circles - 1) + 3 * mOptions.Steps;

            mMesh.SubMesh.vertexData = new VertexData();
            mMesh.SubMesh.VertexData.vertexStart = 0;
            mMesh.SubMesh.VertexData.vertexCount = numVertices;

            VertexDeclaration vdecl = mMesh.SubMesh.VertexData.vertexDeclaration;
            VertexBufferBinding vbind = mMesh.SubMesh.VertexData.vertexBufferBinding;

            int offset = 0;

            switch (mMeshOptions.MeshVertexType)
            {
                case Mesh.VertexType.VT_POS_NORM:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);
                    offset += VertexElement.GetTypeSize(VertexElementType.Float3);
                    vdecl.AddElement(0, offset, VertexElementType.Float3, VertexElementSemantic.Normal);

                    mMesh.HardwareVertexBuffer = HardwareBufferManager.Instance.
                        CreateVertexBuffer(/*Mesh.POS_NORM_VERTEX.SizeInBytes*/ vdecl,
                        numVertices,
                        BufferUsage.DynamicWriteOnly);
                    break;
                case Mesh.VertexType.VT_POS:
                    vdecl.AddElement(0, 0, VertexElementType.Float3, VertexElementSemantic.Position);

                    mMesh.HardwareVertexBuffer = HardwareBufferManager.Instance.
                        CreateVertexBuffer(/*Mesh.POS_VERTEX.SizeInBytes*/ vdecl,
                        numVertices,
                        BufferUsage.DynamicWriteOnly);
                    break;
            }

            vbind.SetBinding(0, mMesh.HardwareVertexBuffer);
            
            int[] indexbuffer = new int[numEle];
          //  int* indexbuffer;
            for (int k = 0; k < mOptions.Steps; k++)
            {
                indexbuffer[k * 3 + 2] = 0;
                indexbuffer[k * 3 + 1] = k + 1;

                if (k != mOptions.Steps - 1)
                {
                    indexbuffer[k * 3] = k + 2;
                }
                else
                {
                    indexbuffer[k * 3] = 1;
                }
            }
            unsafe
            {
                int i = 750;
                for (int y = 0; y < mOptions.Circles - 1; y++)
                {
                    for (int x = 0; x < mOptions.Steps; x++)
                    {
                        fixed (int* intPtr = indexbuffer)
                        {
                            int* twoface = intPtr;
                            // twoface += (y * mOptions.Steps + x) * 6 + 3 * mOptions.Steps;
                            twoface = twoface + (y * mOptions.Steps + x) * 6 + 3 * mOptions.Steps;
                            int p0 = 1 + y * mOptions.Steps + x;
                            int p1 = 1 + y * mOptions.Steps + x + 1;
                            int p2 = 1 + (y + 1) * mOptions.Steps + x;
                            int p3 = 1 + (y + 1) * mOptions.Steps + x + 1;

                            if (x == mOptions.Steps - 1)
                            {
                                p1 -= x + 1;
                                p3 -= x + 1;
                            }

                            // First triangle
                            twoface[0] = p0;
                            twoface[1] = p1;
                            twoface[2] = p2;
                            
                            // Second triangle
                            twoface[3] = p1;
                            twoface[4] = p3;
                            twoface[5] = p2;
                        }

                    }
                }
            }
            //prepare buffer fpr indices

            mMesh.HardwareIndexBuffer =
                HardwareBufferManager.Instance.CreateIndexBuffer(IndexType.Size32
                , numEle, BufferUsage.Static, true);

            mMesh.HardwareIndexBuffer.WriteData(0, mMesh.HardwareIndexBuffer.Size,
                indexbuffer, true);
            indexbuffer = null;

            mMesh.SubMesh.indexData.indexBuffer = mMesh.HardwareIndexBuffer;
            mMesh.SubMesh.indexData.indexStart = 0;
            mMesh.SubMesh.indexData.indexCount = numEle;
            return true;
        }

        public void SetOptions(Options Options)
        {
            Mesh.Options MeshOptions = new Mesh.Options();
            MeshOptions.MeshSize = new Size((int)Options.Radius * 2, (int)Options.Radius * 2);
            MeshOptions.MeshStrength = Options.Strength;
            mHydrax.Mesh.SetOptions(MeshOptions);
            mHydrax.Strength = mOptions.Strength;

            if (IsCreated)
            {
                throw new NotImplementedException();
            }

            mOptions = Options;
        }
    }
}
