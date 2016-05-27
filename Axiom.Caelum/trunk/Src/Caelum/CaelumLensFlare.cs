/*
This file is part of Caelum for NeoAxis Engine.
Caelum for NeoAxisEngine is a Caelum's modified version.
See http://www.ogre3d.org/wiki/index.php/Caelum for the original version.

Copyright (c) 2008-2009 Association Hat. See Contributors.txt for details.

Caelum for NeoAxis Engine is free software: you can redistribute it and/or modify
it under the terms of the GNU Lesser General Public License as published
by the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

Caelum for NeoAxis Engine is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License
along with Caelum for NeoAxis Engine. If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using Engine;
using Engine.MapSystem;
using Engine.PhysicsSystem;
using Engine.Renderer;
using Engine.MathEx;

namespace Caelum
{
    class CaelumLensFlare : IDisposable
    {
        // Attributes -----------------------------------------------------------------

        protected SceneManager mSceneMgr;
        protected SceneNode mNode;
        protected BillboardSet mHaloSet;
        protected BillboardSet mBurstSet;

        protected bool mDisposed = false;
        private static CaelumLensFlare mInstance;


        // Accessors --------------------------------------------------------------------

        public static CaelumLensFlare Instance
        {
            get { return mInstance; }
        }


        // Methods --------------------------------------------------------------------

        public CaelumLensFlare()
        {
            mSceneMgr = SceneManager.Instance;
            createLensFlare();

            mInstance = this;
            mDisposed = false;
        }

        ~CaelumLensFlare()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (mDisposed)
                return;

            mHaloSet.Dispose();
            mBurstSet.Dispose();
            mNode.Dispose();

            mHaloSet = null;
            mBurstSet = null;
            mNode = null;

            mInstance = null;
            mDisposed = true;
        }

        public void createLensFlare()
        {
            // -----------------------------------------------------
            // We create 2 sets of billboards for the lensflare
            // -----------------------------------------------------
            mHaloSet = mSceneMgr.CreateBillboardSet(3);
            mHaloSet.MaterialName = "lensflare/halo";
            mHaloSet.CullIndividually = true;

            mBurstSet = mSceneMgr.CreateBillboardSet(3);
            mBurstSet.MaterialName = "lensflare/burst";
            mBurstSet.CullIndividually = true;

            // The node is located at the light source.
            mNode = new SceneNode();

            mNode.Attach(mBurstSet);
            mNode.Attach(mHaloSet);

            // -------------------------------
            // Creation of the Halo billboards
            // -------------------------------
            Billboard LF_Halo1 = mHaloSet.CreateBillboard(Vec3.Zero);
            LF_Halo1.SetDimensions(new Vec2(15, 15));
            Billboard LF_Halo2 = mHaloSet.CreateBillboard(Vec3.Zero);
            LF_Halo2.SetDimensions(new Vec2(30, 30));
            Billboard LF_Halo3 = mHaloSet.CreateBillboard(Vec3.Zero);
            LF_Halo3.SetDimensions(new Vec2(7.5f, 7.5f));


            // -------------------------------
            // Creation of the "Burst" billboards
            // -------------------------------
            Billboard LF_Burst1 = mBurstSet.CreateBillboard(Vec3.Zero);
            LF_Burst1.SetDimensions(new Vec2(7.5f, 7.5f));
            Billboard LF_Burst2 = mBurstSet.CreateBillboard(Vec3.Zero);
            LF_Burst2.SetDimensions(new Vec2(15, 15));
            Billboard LF_Burst3 = mBurstSet.CreateBillboard(Vec3.Zero);
            LF_Burst3.SetDimensions(new Vec2(7.5f, 7.5f));

            setVisible(false);
        }

        public void update(float time, Camera camera)
        {
            if (CaelumManager.Instance == null)
                return;

            Vec2 scale = CaelumManager.Instance.LensFlareScale;
            Vec3 sunDir = SolarSystemModel.GetSunDirection();
            Vec3 mLightPosition = (camera.Position - sunDir * CaelumSun.Instance.FarDistance);

            if (!CaelumManager.Instance.LensFlareEffect || isHidden(mLightPosition, camera))
            {
                setVisible(false);
                return;
            }

            mNode.Position = mLightPosition;
            mNode.Scale = new Vec3(scale.Y, scale.Y, scale.Y);
            setColour(SkyColorModel.GetSunLight());

            float LightDistance = mLightPosition.Length();
            Vec3 CameraVect = LightDistance * camera.Direction;

            // The LensFlare effect takes place along this vector.
            Vec3 LFvect = (CameraVect - mLightPosition) * scale.X;

            // The different sprites are placed along this line.
            mHaloSet.Billboards[0].Position = LFvect * 0.450f;
            mHaloSet.Billboards[1].Position = LFvect * 0.125f;
            mHaloSet.Billboards[2].Position = -LFvect * 0.250f;

            mBurstSet.Billboards[0].Position = LFvect * 0.333f;
            mBurstSet.Billboards[1].Position = -LFvect * 0.450f;
            mBurstSet.Billboards[2].Position = -LFvect * 0.180f;

            // We redraw the lensflare (in case it was previouly out of the camera field, and hidden)
            setVisible(true);
        }

        public void setVisible(bool visible)
        {
            mHaloSet.Visible = visible;
            mBurstSet.Visible = visible;
        }

        public void setColour(ColorValue color)
        {
            mHaloSet.Billboards[0].Color = color * new ColorValue(0.7f, 0.6f, 0.5f);
            mHaloSet.Billboards[1].Color = color * new ColorValue(0.9f, 0.2f, 0.1f);
            mHaloSet.Billboards[2].Color = color * new ColorValue(0.4f, 0.1f, 0.9f);

            mBurstSet.Billboards[0].Color = color * new ColorValue(0.9f, 0.9f, 0.2f);
            mBurstSet.Billboards[1].Color = color * new ColorValue(0.93f, 0.82f, 0.73f);
            mBurstSet.Billboards[2].Color = color * new ColorValue(0.7f, 0.6f, 0.5f);
        }

        protected bool isHidden(Vec3 lightPosition, Camera camera)
        {
            Ray ray = new Ray(camera.Position, lightPosition - camera.Position);
            RayCastResult[] piercingResult = PhysicsWorld.Instance.RayCastPiercing(
                ray, (int)ContactGroup.CastAll);

            return (!camera.IsIntersectsFast(lightPosition) || piercingResult.Length != 0);
        }
    }
}
