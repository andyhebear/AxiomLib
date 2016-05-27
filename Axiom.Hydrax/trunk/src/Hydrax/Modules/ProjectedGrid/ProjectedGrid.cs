using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Math;
using Axiom.Core;
using Axiom.Hydrax.Noise;
namespace Axiom.Hydrax.Modules
{
    public class ProjectedGrid : Module
    {
        /** Struct wich contains Hydrax projected grid module options
		 */
        public struct GridOptions
        {
            /// Projected grid complexity (N*N)
            public int Complexity;
            /// Strength
            public float Strength;
            /// Elevation 
            public float Elevation;
            /// Smooth
            public bool Smooth;
            /// Force recalculate mesh geometry each frame
            public bool ForceRecalculateGeometry;
            /// Choppy waves
            public bool ChoppyWaves;
            /// Choppy waves strength
            public float ChoppyStrength;

            /** Default constructor
			 
            Options()
                : Complexity(256)
                , Strength(35.0f)
                , Elevation(50.0f)
                , Smooth(false)
                , ForceRecalculateGeometry(false)
                , ChoppyWaves(true)
                , ChoppyStrength(3.75f)
            {

            }*/

            /** Constructor
                @param _Complexity Projected grid complexity
             */
            GridOptions(int Complexity)
            {
                this.Complexity = Complexity;
                this.Strength = 35;
                this.Elevation = 50;
                this.Smooth = false;
                this.ForceRecalculateGeometry = false;
                this.ChoppyWaves = true;
                this.ChoppyStrength = 3.75f;
            }

            /** Constructor
                @param _Complexity Projected grid complexity
                @param _Strength Perlin noise strength
                @param _Elevation Elevation
                @param _Smooth Smooth vertex?
             */
            GridOptions(int Complexity,
                float Strength, float Elevation, bool Smooth)
            {
                this.Complexity = Complexity;
                this.Strength = Strength;
                this.Elevation = Elevation;
                this.Smooth = Smooth;
                this.ForceRecalculateGeometry = false;
                this.ChoppyWaves = true;
                this.ChoppyStrength = 3.75f;
            }

            /** Constructor
                @param _Complexity Projected grid complexity
                @param _Strength Perlin noise strength
                @param _Elevation Elevation
                @param _Smooth Smooth vertex?
                @param _ForceRecalculateGeometry Force to recalculate the projected grid geometry each frame
                @param _ChoppyWaves Choppy waves enabled? Note: Only with Materialmanager::NM_VERTEX normal mode.
                @param _ChoppyStrength Choppy waves strength
             */
            GridOptions(int Complexity,
            float Strength, float Elevation, bool Smooth,
                bool ForceRecalculateGeometry, bool ChoppyWaves,
                float PChoppyStrenght)
            {
                this.Complexity = Complexity;
                this.Strength = Strength;
                this.Elevation = Elevation;
                this.Smooth = Smooth;
                this.ForceRecalculateGeometry = ForceRecalculateGeometry;
                this.ChoppyWaves = ChoppyWaves;
                this.ChoppyStrength = PChoppyStrenght;
            }
        };
        public const float _def_MaxFarClipDistance = 99999;
        #region - Fields -
        Mesh.POS_NORM_VERTEX[] mVerticesPosNormVertex;
        Mesh.POS_VERTEX[] mVerticesPosVertex;
        Mesh.POS_NORM_VERTEX[] mVerticesChoppyBuffer;
        protected Vector4 t_corners0;
        protected Vector4 t_corners1;
        protected Vector4 t_corners2;
        protected Vector4 t_corners3;

        protected Matrix4 mRange;
        protected Plane mBasePlane;
        protected Plane mUpperBoundPlane;
        protected Plane mLowerBoundPlane;

        protected Camera mProjectingCamera;
        protected Camera mRenderingCamera;
        protected Camera mTmpRenderingCamera;

        protected Vector3 mNormal;
        protected Vector3 mPos;

        protected Vector3 mLastPosition;
        protected Quaternion mLastOrientation;
        bool mLastMinMax;
        protected GridOptions mOptions;
        protected Hydrax mHydrax;

        #endregion
        public ProjectedGrid(Hydrax H, BaseNoise Noise, Plane BasePlane,
            MaterialManager.NormalMode NormalMode,
            GridOptions Options)
            :base("ProjectedGrid" +
            ((NormalMode == MaterialManager.NormalMode.NM_VERTEX) ? "Vertex" : "Rtt"),
            Noise,
            new Mesh.Options(128, new Size(0, 0),
                ((NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                ? Mesh.VertexType.VT_POS_NORM : Mesh.VertexType.VT_POS),
                10),
            NormalMode)
        {
            mHydrax = H;
            mRenderingCamera = mHydrax.Camera;
            mBasePlane = BasePlane;
            mNormal = BasePlane.Normal;
            SetOptions(Options);
        }
        public ProjectedGrid(Hydrax H, BaseNoise Noise, Plane BasePlane,
            MaterialManager.NormalMode NormalMode)
            : base("ProjectedGrid" + (
            (NormalMode == MaterialManager.NormalMode.NM_VERTEX) ?
            "Vertex" : "Rtt"),
            Noise,
            new Mesh.Options(256, new Size(0, 0),
                (
                (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                ? Mesh.VertexType.VT_POS_NORM : Mesh.VertexType.VT_POS),
                10),
                NormalMode)
        {
            mHydrax = H;
            mRenderingCamera = mHydrax.Camera;
            mBasePlane = BasePlane;
            mNormal = BasePlane.Normal;
        }
        public void SetOptions(GridOptions Options)
        {
            // Size(0) -> Infinite mesh
            mMeshOptions.MeshSize = new Size();
            mMeshOptions.MeshStrength = Options.Strength;
            mMeshOptions.MeshComplexity = Options.Complexity;

            mHydrax.Mesh.SetOptions(mMeshOptions);
            mHydrax.Strength = Options.Strength;
            // Re-create geometry if it's needed
            if (mIsCreated && Options.Complexity != mOptions.Complexity)
            {
                Remove();
                mOptions = Options;
                Create();
                if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    if(!mNoise.CreateGPUNormalMapResources(mHydrax.GPUNormalMapManager))
                    {
                        Hydrax.HydraxLog(mNoise.Name + " doesn't support GPU Normal map generation."); 
                    }
                }

                string MaterialNameTmp = mHydrax.Mesh.MaterialName;
                mHydrax.Mesh.Remove();
                mHydrax.Mesh.SetOptions(mMeshOptions);
                mHydrax.Mesh.MaterialName = MaterialNameTmp;
                mHydrax.Mesh.Create();

                // Force to recalculate the geometry on next frame
                mLastPosition = new Vector3();
                mLastOrientation = new Quaternion();
                return;
            }
            mOptions = Options;
        }
        public override void Update(float TimeSinceLastFrame)
        {
            if (!IsCreated)
                return;

            base.Update(TimeSinceLastFrame);

            Vector3 RenderingCameraPos = mRenderingCamera.DerivedPosition;
            if (mLastPosition != RenderingCameraPos ||
                mLastOrientation != mRenderingCamera.DerivedOrientation ||
                mOptions.ForceRecalculateGeometry)
            {
                if (mLastPosition != RenderingCameraPos)
                {
                    Vector3 HydraxPos = new Vector3(
                        RenderingCameraPos.x, mHydrax.Position.y, RenderingCameraPos.z);

                    mHydrax.Mesh.SceneNode.Position = HydraxPos;
                    mHydrax.RttManager.PlaneSceneNode.Position = HydraxPos;

                    // For world-space -> object-space conversion
                    mHydrax.SunPosition = mHydrax.SunPosition;
                }

                float RenderingFarClipDistance = mRenderingCamera.Far;
                if (RenderingFarClipDistance > _def_MaxFarClipDistance)
                {
                    mRenderingCamera.Far = _def_MaxFarClipDistance;
                }

                mLastMinMax = GetMinMax(ref mRange);

                if (mLastMinMax)
                {
                    RenderGeometry(mRange, mProjectingCamera.ViewMatrix, RenderingCameraPos);

                    if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                    {
                        mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosNormVertex);
                    }
                    else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                    {
                        mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosVertex);
                    }
                }
                mRenderingCamera.Far = RenderingFarClipDistance;
            }
            else if (mLastMinMax)
            {
                int i = 0, v, u;
                if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                {
                    Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
                    if (mOptions.ChoppyWaves)
                    {
                        for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                        {
                            Vertices[i] = mVerticesChoppyBuffer[i];
                            Vertices[i].Y = -mBasePlane.D + mNoise.GetValue(RenderingCameraPos.x + Vertices[i].X, RenderingCameraPos.z + Vertices[i].Z) * mOptions.Strength;
                        }
                    }
                    else
                    {
                        for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                        {
                            Vertices[i].Y = -mBasePlane.D + mNoise.GetValue(RenderingCameraPos.x + Vertices[i].X, RenderingCameraPos.z + Vertices[i].Z) * mOptions.Strength;
                        }
                    }
                }
                else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;
                    for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                    {
                        Vertices[i].Y = -mBasePlane.D + mNoise.GetValue(RenderingCameraPos.x + Vertices[i].X, RenderingCameraPos.z + Vertices[i].Z) * mOptions.Strength;
                    }
                }

                // Smooth the heightdata
                if (mOptions.Smooth)
                {
                    if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                    {
                        Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
                        for (v = 1; v < (mOptions.Complexity - 1); v++)
                        {
                            for (u = 1; u < (mOptions.Complexity - 1); u++)
                            {
                                Vertices[v * mOptions.Complexity + u].Y =
                                     0.2f *
                                    (Vertices[v * mOptions.Complexity + u].Y +
                                     Vertices[v * mOptions.Complexity + (u + 1)].Y +
                                     Vertices[v * mOptions.Complexity + (u - 1)].Y +
                                     Vertices[(v + 1) * mOptions.Complexity + u].Y +
                                     Vertices[(v - 1) * mOptions.Complexity + u].Y);
                            }
                        }
                    }
                    else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
                    {
                        Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;
                        for (v = 1; v < (mOptions.Complexity - 1); v++)
                        {
                            for (u = 1; u < (mOptions.Complexity - 1); u++)
                            {
                                Vertices[v * mOptions.Complexity + u].Y =
                                     0.2f *
                                    (Vertices[v * mOptions.Complexity + u].Y +
                                     Vertices[v * mOptions.Complexity + (u + 1)].Y +
                                     Vertices[v * mOptions.Complexity + (u - 1)].Y +
                                     Vertices[(v + 1) * mOptions.Complexity + u].Y +
                                     Vertices[(v - 1) * mOptions.Complexity + u].Y);
                            }
                        }
                    }
                }

                CalcueNormals();
                PerformChoppyWaves();
                if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                    mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosNormVertex);
                else if(NormalMode == MaterialManager.NormalMode.NM_RTT)
                    mHydrax.Mesh.UpdateGeometry(mOptions.Complexity * mOptions.Complexity, mVerticesPosVertex);
            }

            mLastPosition = RenderingCameraPos;
            mLastOrientation = mRenderingCamera.DerivedOrientation;
        }
        public override void Create()
        {
            Hydrax.HydraxLog("Creating " + Name + " module.");
            base.Create();
            if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                mVerticesPosNormVertex = new Mesh.POS_NORM_VERTEX[mOptions.Complexity * mOptions.Complexity];

                Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
                for (int i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                {
                    Vertices[i].NX = 0;
                    Vertices[i].NY = -1;
                    Vertices[i].NZ = 0;
                }

                mVerticesChoppyBuffer = new Mesh.POS_NORM_VERTEX[mOptions.Complexity * mOptions.Complexity];
            }
            else if (NormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                mVerticesPosVertex = new Mesh.POS_VERTEX[mOptions.Complexity * mOptions.Complexity];
            }

            SetDisplacementAmplitude(0);
            mTmpRenderingCamera = new Camera("PG_TmpRndrngCamera", null);
            mProjectingCamera = new Camera("PG_ProjectingCamera", null);

            Hydrax.HydraxLog(Name + " created.");
        }
        private bool RenderGeometry(Matrix4 m, Matrix4 viewMat, Vector3 WorldPos)
        {
            t_corners0 = CalculateWorldPosition(new Vector2(0.0f, 0.0f), m, viewMat);
            t_corners1 = CalculateWorldPosition(new Vector2(1.0f, 0.0f), m, viewMat);
            t_corners2 = CalculateWorldPosition(new Vector2(0.0f, 1.0f), m, viewMat);
            t_corners3 = CalculateWorldPosition(new Vector2(1.0f, 1.0f), m, viewMat);


            float du = 1.0f / (mOptions.Complexity - 1),
              dv = 1.0f / (mOptions.Complexity - 1),
              u, v = 0.0f,
                // _1_u = (1.0f-u)
              _1_u, _1_v = 1.0f,
              divide;

            Vector4 result = Vector4.Zero;

            int i = 0, iv, iu;

            if (mNormalMode == MaterialManager.NormalMode.NM_VERTEX)
            {
                Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

                for (iv = 0; iv < mOptions.Complexity; iv++)
                {
                    u = 0.0f;
                    _1_u = 1.0f;
                    for (iu = 0; iu < mOptions.Complexity; iu++)
                    {
                        result.x = _1_v * (_1_u * t_corners0.x + u * t_corners1.x) + v * (_1_u * t_corners2.x + u * t_corners3.x);
                        result.z = _1_v * (_1_u * t_corners0.z + u * t_corners1.z) + v * (_1_u * t_corners2.z + u * t_corners3.z);
                        result.w = _1_v * (_1_u * t_corners0.w + u * t_corners1.w) + v * (_1_u * t_corners2.w + u * t_corners3.w);

                        divide = 1.0f / result.w;
                        result.x *= divide;
                        result.z *= divide;

                        Vertices[i].X = result.x;
                        Vertices[i].Z = result.z;
                        Vertices[i].Y = -mBasePlane.D + mNoise.GetValue(WorldPos.x + result.x, WorldPos.z + result.z) * mOptions.Strength;

                        i++;
                        u += du;
                        _1_u = 1.0f - u;
                    }
                    v += dv;
                    _1_v = 1.0f - v;
                }
                if (mOptions.ChoppyWaves)
                {
                    for (i = 0; i < mOptions.Complexity * mOptions.Complexity; i++)
                    {
                        mVerticesChoppyBuffer[i] = Vertices[i];
                    }
                }
            }
            else if (mNormalMode == MaterialManager.NormalMode.NM_RTT)
            {
                Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;

                for (iv = 0; iv < mOptions.Complexity; iv++)
                {
                    u = 0.0f;
                    _1_u = 1.0f;
                    for (iu = 0; iu < mOptions.Complexity; iu++)
                    {
                        result.x = _1_v * (_1_u * t_corners0.x + u * t_corners1.x) + v * (_1_u * t_corners2.x + u * t_corners3.x);
                        result.z = _1_v * (_1_u * t_corners0.z + u * t_corners1.z) + v * (_1_u * t_corners2.z + u * t_corners3.z);
                        result.w = _1_v * (_1_u * t_corners0.w + u * t_corners1.w) + v * (_1_u * t_corners2.w + u * t_corners3.w);

                        divide = 1.0f / result.w;
                        result.x *= divide;
                        result.z *= divide;

                        Vertices[i].X = result.x;
                        Vertices[i].Y = result.z;
                        Vertices[i].Z = -mBasePlane.D + mNoise.GetValue(WorldPos.x + result.x, WorldPos.z + result.z) * mOptions.Strength;

                        i++;
                        u += du;
                        _1_u = 1.0f - u;
                    }
                    v += dv;
                    _1_v = 1.0f - v;
                }
            }

            // Smooth the heightdata
            if (mOptions.Smooth)
            {
                if (NormalMode == MaterialManager.NormalMode.NM_VERTEX)
                {
                    Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

                    for (iv = 1; iv < (mOptions.Complexity - 1); iv++)
                    {
                        for (iu = 1; iu < (mOptions.Complexity - 1); iu++)
                        {
                            Vertices[iv * mOptions.Complexity + iu].Y =
                                 0.2f *
                                (Vertices[iv * mOptions.Complexity + iu].Y +
                                 Vertices[iv * mOptions.Complexity + (iu + 1)].Y +
                                 Vertices[iv * mOptions.Complexity + (iu - 1)].Y +
                                 Vertices[(iv + 1) * mOptions.Complexity + iu].Y +
                                 Vertices[(iv - 1) * mOptions.Complexity + iu].Y);
                        }
                    }
                }
                else if (mNormalMode == MaterialManager.NormalMode.NM_RTT)
                {
                    Mesh.POS_VERTEX[] Vertices = mVerticesPosVertex;

                    for (iv = 1; iv < (mOptions.Complexity - 1); iv++)
                    {
                        for (iu = 1; iu < (mOptions.Complexity - 1); iu++)
                        {
                            Vertices[iv * mOptions.Complexity + iu].Y =
                                 0.2f *
                                (Vertices[iv * mOptions.Complexity + iu].Y +
                                 Vertices[iv * mOptions.Complexity + (iu + 1)].Y +
                                 Vertices[iv * mOptions.Complexity + (iu - 1)].Y +
                                 Vertices[(iv + 1) * mOptions.Complexity + iu].Y +
                                 Vertices[(iv - 1) * mOptions.Complexity + iu].Y);
                        }
                    }
                }
            }

            CalcueNormals();

            PerformChoppyWaves();

            return true;
        }

        private void PerformChoppyWaves()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX || !mOptions.ChoppyWaves)
                return;

            int v, u,
            Underwater = 1;

            if (mHydrax.IsCurrentFrameUnderwater)
                Underwater = -1;

            float Dis1, Dis2;

            Vector3 CameraDir, Norm;
            Vector2 Dir, Perp, Norm2;

            CameraDir = mRenderingCamera.DerivedDirection;
            Dir = new Vector2(CameraDir.x, CameraDir.z).ToNormalized();
            Perp = Dir.Perpendicular();

            if (Dir.x < 0) Dir.x = -Dir.x;
            if (Dir.y < 0) Dir.y = -Dir.y;

            if (Perp.x < 0) Perp.x = -Perp.x;
            if (Perp.y < 0) Perp.y = -Perp.y;


            Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;
            for (v = 1; v < (mOptions.Complexity - 1); v++)
            {
                Dis1 = (new Vector2(mVerticesChoppyBuffer[v * mOptions.Complexity + 1].X,
                                       mVerticesChoppyBuffer[v * mOptions.Complexity + 1].Z) -
                         new Vector2(mVerticesChoppyBuffer[(v + 1) * mOptions.Complexity + 1].X,
                                       mVerticesChoppyBuffer[(v + 1) * mOptions.Complexity + 1].Z)).Length();

                /*Dis1_ = (Ogre::Vector2(mVerticesChoppyBuffer[v*mOptions.Complexity + 1].x,
                                       mVerticesChoppyBuffer[v*mOptions.Complexity + 1].z) -
                         Ogre::Vector2(mVerticesChoppyBuffer[(v-1)*mOptions.Complexity + 1].x,
                                       mVerticesChoppyBuffer[(v-1)*mOptions.Complexity + 1].z)).length();

                Dis1 = (Dis1+Dis1_)/2;*/

                for (u = 1; u < (mOptions.Complexity - 1); u++)
                {
                    Dis2 = (new Vector2(mVerticesChoppyBuffer[v * mOptions.Complexity + u].X,
                                          mVerticesChoppyBuffer[v * mOptions.Complexity + u].Z) -
                            new Vector2(mVerticesChoppyBuffer[v * mOptions.Complexity + u + 1].X,
                                          mVerticesChoppyBuffer[v * mOptions.Complexity + u + 1].Z)).Length();
                    /*
                                    Dis2_ = (Ogre::Vector2(mVerticesChoppyBuffer[v*mOptions.Complexity + u].x,
                                                           mVerticesChoppyBuffer[v*mOptions.Complexity + u].z) -
                                             Ogre::Vector2(mVerticesChoppyBuffer[v*mOptions.Complexity + u-1].x,
                                                           mVerticesChoppyBuffer[v*mOptions.Complexity + u-1].z)).length();

                                    Dis2 = (Dis2+Dis2_)/2;*/

                    Norm = new Vector3(Vertices[v * mOptions.Complexity + u].NX,
                                         Vertices[v * mOptions.Complexity + u].NY,
                                         Vertices[v * mOptions.Complexity + u].NZ).ToNormalized();

                    Norm2 = new Vector2(Norm.x, Norm.z) *
                            (
                            (Dir  * Dis1) +
                            (Perp * Dis2)
                            )                           *
                             mOptions.ChoppyStrength;

                    Vertices[v * mOptions.Complexity + u].X = mVerticesChoppyBuffer[v * mOptions.Complexity + u].X + Norm2.x * Underwater;
                    Vertices[v * mOptions.Complexity + u].Z = mVerticesChoppyBuffer[v * mOptions.Complexity + u].Z + Norm2.y * Underwater;
                }
            }

        }
        private void SetDisplacementAmplitude(float Amplitude)
        {
            mUpperBoundPlane = new Plane(mNormal, mPos + Amplitude * mNormal);
            mLowerBoundPlane = new Plane(mNormal, mPos - Amplitude * mNormal);
        }
        public override float GetHeight(Vector2 Position)
        {
            return mHydrax.Position.y + mNoise.GetValue(Position.x, Position.y) * mOptions.Strength;
        }
        private Vector4 CalculateWorldPosition(Vector2 uv, Matrix4 m, Matrix4 _viewMat)
        {
            Vector4 origin = new Vector4(uv.x, uv.y, -1, 1);
            Vector4 direction = new Vector4(uv.x, uv.y, 1, 1);

            origin = m * origin;
            direction = m * direction;

            Vector3 _org = new Vector3(origin.x / origin.w, origin.y / origin.w, origin.z / origin.w);
            Vector3 _dir = new Vector3(direction.x / direction.w, direction.y / direction.w, direction.z / direction.w);

            _dir -= _org;
            _dir.Normalize();

            Ray _ray = new Ray(_org, _dir);
            IntersectResult _result = _ray.Intersects(mBasePlane);
            float l = _result.Distance;
            Vector3 worldPos = _org + _dir * l;
            Vector4 _tempVec = (Vector4)(_viewMat * new Vector4(worldPos.x, worldPos.y, worldPos.z, 1));
            float _temp = -_tempVec.z / _tempVec.w;
            Vector4 retPos = new Vector4(worldPos.x, worldPos.y, worldPos.z, 1);
            retPos.x /= _temp;
            retPos.y /= _temp;
            retPos.z /= _temp;
            retPos.w /= _temp;
            return retPos;
        }
        private bool GetMinMax(ref Matrix4 Range)
        {
            SetDisplacementAmplitude(mOptions.Strength);

            float x_min, y_min, x_max, y_max;
            Vector3[] frustum = new Vector3[8];
            Vector3[] proj_points = new Vector3[24];

            int i = 0, n_points = 0, src = 0, dst = 0;
            int[] cube = 
		   {0,1,	0,2,	2,3,	1,3,
		    0,4,	2,6,	3,7,	1,5,
		    4,6,	4,5,	5,7,	6,7};

            Vector3 _testLine = new Vector3();
            float _dist = 0;
            Ray _ray = new Ray();
            IntersectResult _result;

            //Set temporal rendering camera parameters
            mTmpRenderingCamera.FrustumOffset = mRenderingCamera.FrustumOffset;
            mTmpRenderingCamera.AspectRatio = mRenderingCamera.AspectRatio;
            mTmpRenderingCamera.Direction = mRenderingCamera.DerivedDirection;
            mTmpRenderingCamera.Far = mRenderingCamera.Far;
			mTmpRenderingCamera.FieldOfView = (float)Utility.RadiansToDegrees( (Real)mRenderingCamera.FieldOfView );
            mTmpRenderingCamera.Near = mRenderingCamera.Near;
            mTmpRenderingCamera.Orientation = mRenderingCamera.DerivedOrientation;
            mTmpRenderingCamera.Position = new Vector3(0, mRenderingCamera.DerivedPosition.y - mHydrax.Position.y, 0);
            
            Matrix4 invviewproj = (mTmpRenderingCamera.ProjectionMatrixRSDepth * mTmpRenderingCamera.ViewMatrix).Inverse();
          //  Matrix4 invviewproj = (mRenderingCamera.ProjectionMatrixRSDepth * mRenderingCamera.ViewMatrix).Inverse();
            frustum[0] = invviewproj * new Vector3(-1, -1, 0);
            frustum[1] = invviewproj * new Vector3(+1, -1, 0);
            frustum[2] = invviewproj * new Vector3(-1, +1, 0);
            frustum[3] = invviewproj * new Vector3(+1, +1, 0);
            frustum[4] = invviewproj * new Vector3(-1, -1, +1);
            frustum[5] = invviewproj * new Vector3(+1, -1, +1);
            frustum[6] = invviewproj * new Vector3(-1, +1, +1);
            frustum[7] = invviewproj * new Vector3(+1, +1, +1);

            //check intersections with upper_bound and lower_bound
            for (i = 0; i < 12; i++)
            {
                src = cube[i * 2]; dst = cube[i * 2 + 1];
                _testLine = frustum[dst] - frustum[src];
                _dist = _testLine.Normalize();
                _ray = new Ray(frustum[src], _testLine);
                _result = _ray.Intersects(mUpperBoundPlane);
                if ((_result.Hit) && (_result.Distance < _dist + 0.00001f))
                {
                    proj_points[n_points++] = frustum[src] + _result.Distance * _testLine;
                }
                _result = _ray.Intersects(mLowerBoundPlane);
                if ((_result.Hit) && (_result.Distance < _dist + 0.00001f))
                {
                    proj_points[n_points++] = frustum[src] + _result.Distance * _testLine;
                }
            }

            //Check if any of the frustums vertices lie between the upper_bound and lower_bound planes
            for (i = 0; i < 8; i++)
            {
                if (mUpperBoundPlane.GetDistance(frustum[i]) / mLowerBoundPlane.GetDistance(frustum[i]) < 0)
                {
                    proj_points[n_points++] = frustum[i];
                }

            }

            // Set projecting camera parameters
            mProjectingCamera.FrustumOffset = mTmpRenderingCamera.FrustumOffset;
            mProjectingCamera.AspectRatio = mTmpRenderingCamera.AspectRatio;
            mProjectingCamera.Direction = mTmpRenderingCamera.DerivedDirection;
            mProjectingCamera.Far = mTmpRenderingCamera.Far;
			mProjectingCamera.FieldOfView = (float)Utility.RadiansToDegrees( (Real)mTmpRenderingCamera.FieldOfView );
            mProjectingCamera.Near = mTmpRenderingCamera.Near;
            mProjectingCamera.Orientation = mTmpRenderingCamera.DerivedOrientation;
            mProjectingCamera.Position = mTmpRenderingCamera.DerivedPosition;
            
            float height_in_plane = mBasePlane.GetDistance(mProjectingCamera.RealPosition);

            bool keep_it_simple = true;
            bool underwater = false;

            if (height_in_plane < 0.0f)
                underwater = true;
            if (keep_it_simple)
                mProjectingCamera.Direction = mTmpRenderingCamera.DerivedDirection;
            else
            {
                Vector3 aimpoint = Vector3.Zero, aimpoint2 = Vector3.Zero;

                if (height_in_plane < (mOptions.Strength + mOptions.Elevation))
                {
                    if (underwater)
                    {
                        mProjectingCamera.Position = mProjectingCamera.RealPosition +
                            mLowerBoundPlane.Normal * (mOptions.Strength + mOptions.Elevation -
                            2 * height_in_plane);
                    }
                    else
                    {
                        mProjectingCamera.Position = mProjectingCamera.RealPosition +
                            mLowerBoundPlane.Normal * (mOptions.Strength + mOptions.Elevation -
                            height_in_plane);
                    }
                }
                // Aim the projector at the point where the camera view-vector intersects the plane
                // if the camera is aimed away from the plane, mirror it's view-vector against the plane
                if (((mBasePlane.Normal).Dot(mTmpRenderingCamera.DerivedDirection) < 0.0f) ||
                    ((mBasePlane.Normal).Dot(mTmpRenderingCamera.DerivedPosition) < 0.0f))
                {
                    _ray = new Ray(mTmpRenderingCamera.DerivedPosition, mTmpRenderingCamera.DerivedDirection);
                    _result = _ray.Intersects(mBasePlane);

                    if (!_result.Hit)
                        _result.Distance = -_result.Distance;

                    aimpoint = mTmpRenderingCamera.DerivedPosition + _result.Distance *
                        mTmpRenderingCamera.DerivedDirection;
                }
                else
                {
                    Vector3 flipped = mTmpRenderingCamera.DerivedDirection - 2 * mNormal *
                                       (mTmpRenderingCamera.DerivedDirection).Dot(mNormal);
                    flipped.Normalize();
                    _ray = new Ray(mTmpRenderingCamera.DerivedPosition, flipped);
                    _result = _ray.Intersects(mBasePlane);
                    aimpoint = mTmpRenderingCamera.DerivedPosition + _result.Distance * flipped;
                }

                // Force the point the camera is looking at in a plane, and have the projector look at it
                // works well against horizon, even when camera is looking upwards
                // doesn't work straight down/up
                float af = System.Math.Abs((mBasePlane.Normal).Dot(mTmpRenderingCamera.DerivedDirection));

                aimpoint2 = mTmpRenderingCamera.DerivedPosition + 10.0f * mTmpRenderingCamera.DerivedDirection;
                aimpoint2 = aimpoint2 - mNormal * (aimpoint2.Dot(mNormal));

                // Fade between aimpoint & aimpoint2 depending on view angle
                aimpoint = aimpoint * af + aimpoint2 * (1.0f - af);

                mProjectingCamera.Direction = (aimpoint - mProjectingCamera.RealPosition);
            }
            for (i = 0; i < n_points; i++)
            {
                // Project the point onto the surface plane
                proj_points[i] = proj_points[i] - mBasePlane.Normal * mBasePlane.GetDistance(proj_points[i]);
                proj_points[i] = mProjectingCamera.ViewMatrix * proj_points[i];
                proj_points[i] = mProjectingCamera.ProjectionMatrixRSDepth * proj_points[i];
            }

            // Get max/min x & y-values to determine how big the "projection window" must be
            if (n_points > 0)
            {
                x_min = proj_points[0].x;
                x_max = proj_points[0].x;
                y_min = proj_points[0].y;
                y_max = proj_points[0].y;

                for (i = 1; i < n_points; i++)
                {
                    if (proj_points[i].x > x_max) x_max = proj_points[i].x;
                    if (proj_points[i].x < x_min) x_min = proj_points[i].x;
                    if (proj_points[i].y > y_max) y_max = proj_points[i].y;
                    if (proj_points[i].y < y_min) y_min = proj_points[i].y;
                }
                // Build the packing matrix that spreads the grid across the "projection window"
                Matrix4 pack = new Matrix4(
                    x_max - x_min, 0            , 0, x_min,
                    0            , y_max - y_min, 0, y_min,
                    0            , 0            , 1, 0,
                    0            , 0            , 0, 1);

                invviewproj =
                     (mProjectingCamera.ProjectionMatrixRSDepth *
                     mProjectingCamera.ViewMatrix).Inverse();
                Range = invviewproj * pack;
                //mRange = invviewproj * pack;
                return true;
            }
            return false;
        }
        private void CalcueNormals()
        {
            if (NormalMode != MaterialManager.NormalMode.NM_VERTEX)
                return;

            int v = 0;
            int u = 0;

            Vector3 vec1 = new Vector3();
            Vector3 vec2 = new Vector3();
            Vector3 normal = new Vector3();

            Mesh.POS_NORM_VERTEX[] Vertices = mVerticesPosNormVertex;

            for (v = 1; v < (mOptions.Complexity - 1); v++)
            {
                for(u = 1; u < (mOptions.Complexity -1);u++)
                {
                    vec1 = new Vector3(
                        Vertices[v * mOptions.Complexity + u + 1].X - Vertices[v * mOptions.Complexity + u - 1].X,
                        Vertices[v * mOptions.Complexity + u + 1].Y - Vertices[v * mOptions.Complexity + u - 1].Y,
                        Vertices[v * mOptions.Complexity + u + 1].Z - Vertices[v * mOptions.Complexity + u - 1].Z);
                    vec2 = new Vector3(
                        Vertices[(v + 1) * mOptions.Complexity + u].X - Vertices[(v - 1) * mOptions.Complexity + u].X,
                        Vertices[(v + 1) * mOptions.Complexity + u].Y - Vertices[(v - 1) * mOptions.Complexity + u].Y,
                        Vertices[(v + 1) * mOptions.Complexity + u].Z - Vertices[(v - 1) * mOptions.Complexity + u].Z);

                    normal = vec2.Cross(vec1);

                    Vertices[v * mOptions.Complexity + u].NX = normal.x;
                    Vertices[v * mOptions.Complexity + u].NY = normal.y;
                    Vertices[v * mOptions.Complexity + u].NZ = normal.z;

                }
            }
        }
    }
}
