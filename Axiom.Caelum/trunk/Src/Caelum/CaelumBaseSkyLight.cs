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
using Engine.MathEx;
using Engine.Renderer;

namespace Caelum
{
    /// <summary>
    /// Describes a base class for all elements of caelum wich are rendered with a 
    /// billboard and wich are a source of light.</summary>
    public class CaelumBaseSkyLight : CaelumBase
    {
        // Attributes -----------------------------------------------------------------

        protected BillboardSet mBillboardSet;

        protected RenderLight mMainLight;

        protected float mFarDistance = 990.0f;

        // Accessors --------------------------------------------------------------------

        /// <summary>
        /// Gets the main node's material.</summary>
        /// <remarks>Returns null if there isn't material</remarks>
        public Material MainMaterial
        {
            get
            {
                if (mBillboardSet == null)
                    return null;

                string material = mBillboardSet.MaterialName;
                return MaterialManager.Instance.GetByName(material);
            }
        }

        /// <summary>
        /// The distance used to orbiting.</summary>
        public float FarDistance
        {
            get { return mFarDistance; }
            set { mFarDistance = value; }
        }

        // Methods --------------------------------------------------------------------

        ~CaelumBaseSkyLight()
        {
            Dispose();
        }

        public override void Dispose()
        {
            if (mDisposed)
                return;

            if(mMainLight != null)
                mMainLight.Dispose();

            if (mBillboardSet != null)
                mBillboardSet.Dispose();

            mMainLight = null;
            mBillboardSet = null;

            base.Dispose();
        }

        /// <summary>
        /// Creates the element in the world. It automatically
        /// sets up the mesh and the node.</summary>
        protected virtual void Initialise(RenderQueueGroupID renderGroup, string materialName, Vec3 scale)
        {
            mBillboardSet = SceneManager.Instance.CreateBillboardSet(1);
            mBillboardSet.CastShadows = false;
            mBillboardSet.RenderQueueGroup = renderGroup;
            mBillboardSet.MaterialName = materialName;
            mBillboardSet.DefaultSize = new Vec2(1, 1);

            // Attaches the mesh on a node
            mNode = new SceneNode();
            mNode.Attach(mBillboardSet);

            // Sets up the node (Position, Scale and Rotation)
            mBillboardSet.CreateBillboard(Vec3.Zero);
            mNode.Position = Vec3.Zero;
            mNode.Scale = scale;

            mMainLight = SceneManager.Instance.CreateLight();
            mMainLight.Type = RenderLightType.Directional;
            mMainLight.AllowStaticLighting = false;
            mMainLight.CastShadows = true;
            mMainLight.Position = new Vec3(0, 0, 150);
        }

        /// <summary>
        /// Sets the billboard body color.</summary>
        protected void setBodyColor(ColorValue color)
        {
            mBillboardSet.Billboards[0].Color = color;
        }

        /// <summary>
        /// Sets up the directional lightning</summary>
        /// <param name="color">The color of the light</param>
        /// <param name="direction">The direction of the light</param>
        protected void setLighting(ColorValue color, Vec3 direction)
        {
            if (CaelumManager.Instance == null || mMainLight == null)
                return;

            // Retrieves parameters from CaelumManager
            ColorValue diffuse = color * CaelumManager.Instance.DiffuseMultiplier;
            ColorValue specular = color * CaelumManager.Instance.SpecularMultiplier;

            float sinAngle = Vec3.Cross(direction.GetNormalize(), CaelumUtils.XAxis).Length();

            mMainLight.Direction = direction;
            mMainLight.Visible = CaelumManager.Instance.CastShadows;
            mMainLight.CastShadows = mMainLight.Visible;

            // Disables casting shadows when is rising
            if (sinAngle < 0.087f || direction.Z >= 0)
                mMainLight.CastShadows = false;

            mMainLight.DiffuseColor = diffuse;
            mMainLight.SpecularColor = specular;
        }
    }
}