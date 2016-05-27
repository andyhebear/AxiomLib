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
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Collections.Generic;
using System.Drawing.Design;
using Engine;
using Engine.EntitySystem;
using Engine.MathEx;
using Engine.Utils;
using Engine.Renderer;
using Engine.MapSystem;


namespace Caelum
{
    /// <summary>
    /// Root of the Caelum system.
    /// Caelum is built from several classes for different sky elements (the sun,
    /// clouds, etc). This class is responsible for tracking and 
    /// updating sub-components.
    /// All parameters set in the Map Editor are retrieved in this unique
    /// class.</summary>
    /// <remarks>It can be a better way to make all components
    /// independants but it's also difficult to manage with too many
    /// entity in the Map Editor. Maybe finding a better way to set up all
    /// components will clean up the code and avoid some memory gaps.</remarks>
    public class CaelumManagerType : MapGeneralObjectType
    {
        // Attributes -----------------------------------------------------------------

        [FieldSerialize]
        protected CaelumComponent mComponentMask;

        [FieldSerialize]
        protected List<CaelumItem> mCaelumItems = new List<CaelumItem>();

        [FieldSerialize]
        protected float mFarDistance = 990.0f;

        [FieldSerialize("GradientImage")]
        protected string mGradientImage = "Caelum\\Sun\\SunGradient.png";

        [FieldSerialize("SkyGradientImage")]
        protected string mSkyGradientsImage = "Caelum\\Skydome\\EarthClearSky.png";

        [FieldSerialize("CloudsLookupImage")]
        protected string mCoverLookupImage = "Caelum\\Clouds\\CloudCoverLookup.png";

        // Accessors --------------------------------------------------------------------

        /// <summary>
        /// Describes all components which will be created in the world.</summary>
        [Editor(typeof(EnumListBoxEditor), typeof(UITypeEditor))]
        public CaelumComponent ComponentsToCreate
        {
            get { return mComponentMask; }
            set { mComponentMask = value; }
        }

        /// <summary>
        /// Describes all components' parameters which will be created 
        /// in the world. If there isn't any parameters the component
        /// won't be created.</summary>
        [TypeConverter(typeof(CollectionTypeConverter))]
        public List<CaelumItem> CaelumItems
        {
            get { return mCaelumItems; }
        }

        /// <summary>
        /// The distance used to sun and moon orbiting.</summary>
        [Category("Settings")]
        public float FarDistance
        {
            get { return mFarDistance; }
            set { mFarDistance = value; }
        }

        /// <summary>
        /// The image's virtual path used to calculate sun color.</summary>
        [Category("Settings")]
        [Description("Sets the image used to calculate sun color")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public string GradientImage
        {
            get { return mGradientImage; }
            set { mGradientImage = value; }
        }

        /// <summary>
        /// The sky color gradients image's name.</summary>
        [Category("Settings")]
        [Description("Sets the image used to calculate sky gradient")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public string SkyGradientImage
        {
            get { return mSkyGradientsImage; }
            set { mSkyGradientsImage = value; }
        }

        /// <summary>
        /// Sets the image used to lookup the cloud coverage threshold.
        /// This image is used to calculate the cloud coverage threshold
        /// based on the desired cloud cover.</summary>
        [Category("Settings")]
        [Description("Sets the image used to lookup the cloud coverage threshold.")]
        [Editor(typeof(EditorTextureUITypeEditor), typeof(UITypeEditor))]
        public string CoverLookupImage
        {
            get { return mCoverLookupImage; }
            set { mCoverLookupImage = value; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumManagerType()
        {
            this.UniqueEntityInstance = true;
            this.AllowEmptyName = true;
        }

        protected override bool OnSave(TextBlock block)
        {
            block.SetAttribute("ComponentsToCreate", mComponentMask.ToString() );
            return base.OnSave(block);
        }

        protected override bool OnLoad(TextBlock block)
        {
            string data = block.GetAttribute("ComponentsToCreate");
            mComponentMask = (CaelumComponent)Enum.Parse(mComponentMask.GetType(), data);
            return base.OnLoad(block);
        }
    }


    public class CaelumManager : MapGeneralObject
    {
        // Attributes -----------------------------------------------------------------

        [FieldSerialize("ObserverLatitude")]
        protected Degree mObserverLatitude = new Degree(45.0f);

        [FieldSerialize("ObserverLongitude")]
        protected Degree mObserverLongitude = new Degree(45.0f);

        //// 

        [FieldSerialize("ManageAmbientLight")]
        protected bool mManageAmbientLight = true;

        [FieldSerialize("CastShadows")]
        protected bool mCastShadows = true;

        [FieldSerialize("SpecularMultiplier")]
        protected ColorValue mSpecularMultiplier = new ColorValue(1.0f, 1.0f, 1.0f);

        [FieldSerialize("DiffuseMultiplier")]
        protected ColorValue mDiffuseMultiplier = new ColorValue(1.0f, 1.0f, 0.9f);

        [FieldSerialize("AmbientMultiplier")]
        protected ColorValue mAmbientMultiplier = new ColorValue(1.0f, 1.0f, 1.0f);

        [FieldSerialize("MinAmbientLight")]
        protected ColorValue mMinAmbientLight = new ColorValue(0.15f, 0.15f, 0.15f);

        ////

        [FieldSerialize("StarFieldInclination")]
        protected float mStarInclination = 13.0f;

        ////

        [FieldSerialize("CloudsCover")]
        protected float mCloudCover = 0.3f;

        [FieldSerialize("IsCloudsAnimated")]
        protected bool mAnimating = true;

        [FieldSerialize("CloudsBlendTime")]
        protected float mCloudBlendTime = 86400.0f;

        [FieldSerialize("CloudsSpeed")]
        [Description("Sets cloud movement speed")]
        protected Vec2 mCloudSpeed = new Vec2(5.0f, 9.0f);

        ////

        [FieldSerialize("FogDensityMultiplier")]
        protected float mFogDensityMultiplier = 0.0015f;

        ////

        [FieldSerialize("TimeScale")]
        protected double mTimeScale = 512.0d;

        [FieldSerialize("StartYear")]
        protected int mStartYear = 2007;

        [FieldSerialize("StartMonth")]
        protected int mStartMonth = 4;

        [FieldSerialize("StartDay")]
        protected int mStartDay = 9;

        [FieldSerialize("StartHour")]
        protected int mStartHour = 23;

        ////

        [FieldSerialize("LensFlareEffect")]
        protected bool mLensFlare = true;

        [FieldSerialize("LensFlareScale")]
        protected Vec2 mFlareScale = new Vec2(1.0f, 1.0f);

        ////

        protected bool mManageFog = false;

        protected bool onGame = true;

        protected List<CaelumBase> mChildren = new List<CaelumBase>();

        protected static CaelumManager mInstance;

        CaelumManagerType _type = null; public new CaelumManagerType Type { get { return _type; } }

        // Accessors --------------------------------------------------------------------

        public static CaelumManager Instance
        {
            get { return mInstance; }
        }

        /// <summary>
        /// The observer's latitude. North is positive, south is negative.</summary>
        [Category("SolarSystem")]
        [Description("The observer's latitude. North is positive, south is negative.")]
        public Degree ObserverLatitude
        {
            get { return mObserverLatitude; }
            set { mObserverLatitude = value; }
        }

        /// <summary>
        /// The observer's latitude. North is positive, south is negative.</summary>
        [Category("SolarSystem")]
        [Description("The observer's longitude. East is positive, west is negative.")]
        public Degree ObserverLongitude
        {
            get { return mObserverLongitude; }
            set { mObserverLongitude = value; }
        }

        /// <summary>
        /// Enables/disables Caelum managing ambient light.</summary>
        [Category("Sun")]
        [Description("Enables/disables sun managing ambient lighting.")]
        public bool ManageAmbientLight
        {
            get { return mManageAmbientLight; }
            set { mManageAmbientLight = value; }
        }

        /// <summary>
        /// Enables/disables Caelum casting shadows.</summary>
        [Category("Sun")]
        [Description("Enables/disables sun casting shadows.")]
        public bool CastShadows
        {
            get { return mCastShadows; }
            set { mCastShadows = value; }
        }

        /// <summary>
        /// The diffuse multiplier for light colour</summary>
        [Category("LightColor")]
        [Description("Colour multiplier for diffuse light")]
        public ColorValue DiffuseMultiplier
        {
            get { return mDiffuseMultiplier; }
            set { mDiffuseMultiplier = value; }
        }

        /// <summary>
        /// The specular multiplier for light colour</summary>
        [Category("LightColor")]
        [Description("Colour multiplier for specular light")]
        public ColorValue SpecularMultiplier
        {
            get { return mSpecularMultiplier; }
            set { mSpecularMultiplier = value; }
        }

        /// <summary>
        /// The ambient multiplier for light colour</summary>
        [Category("LightColor")]
        [Description("Colour multiplier for ambient light colour")]
        public ColorValue AmbientMultiplier
        {
            get { return mAmbientMultiplier; }
            set { mAmbientMultiplier = value; }
        }

        /// <summary>
        /// The minimal value for ambient light colour. It's 
        /// useful when it's night to avoid gettting a black map.</summary>
        [Category("LightColor")]
        [Description("Minimal value for ambient light colour")]
        public ColorValue MinAmbientLight
        {
            get { return mMinAmbientLight; }
            set { mMinAmbientLight = value; }
        }
        
        /// <summary>
        /// The inclination of the starfield to UpAxis in degrees</summary>
        [Category("StarField")]
        [EditorLimitsRange(0, 180)]
        [Description("The inclination of the starfield to UpAxis in degrees")]
        [Editor(typeof(SingleValueEditor), typeof(UITypeEditor))]
        public float StarFieldInclination
        {
            get { return mStarInclination; }
            set { mStarInclination = value; }
        }

        /// <summary>
        /// The clouds' cover, between 0 (completely clear) and 1 (completely covered).
        /// It's used to calculate the clouds' coverage threshold.</summary>
        [Category("LayeredClouds")]
        [EditorLimitsRange(0, 1)]
        [Description("Sets cloud cover, between 0 (completely clear) and 1 (completely covered)")]
        [Editor(typeof(SingleValueEditor), typeof(UITypeEditor))]
        public float CloudCover
        {
            get { return mCloudCover; }
            set { mCloudCover = value; }
        }

        /// <summary>
        /// Switch internal clouds' animation on/off</summary>
        [Category("LayeredClouds")]
        [Description("Sets if the cloud layer is animating itself")]
        public bool AnimatedClouds
        {
            get { return mAnimating; }
            set { mAnimating = value; }
        }

        /// <summary>
        /// The time it takes to blend two clouds shaped together, in seconds.</summary>
        [Category("LayeredClouds")]
        [Description("Sets the time it takes to blend two cloud shaped together, in seconds")]
        public float CloudsBlendTime
        {
            get { return mCloudBlendTime; }
            set { mCloudBlendTime = value; }
        }

        /// <summary>
        /// The clouds' movements speed.</summary>
        [Category("LayeredClouds")]
        [EditorLimitsRange(-20, 20)]
        [Editor(typeof(Vec2ValueEditor), typeof(UITypeEditor))]
        public Vec2 CloudsSpeed
        {
            get { return mCloudSpeed; }
            set { mCloudSpeed = value; }
        }
        
        /// <summary>
        /// The global fog's density multiplier.</summary>
        [Category("Fog")]
        public float FogDensityMultiplier
        {
            get { return mFogDensityMultiplier; }
            set { mFogDensityMultiplier = value; }
        }

        /// <summary>
        /// The relative time scale. If negative, time will move backwards; 
        /// 2.0 means double speed...</summary>
        [Category("Time")]
        public double TimeScale
        {
            get { return mTimeScale; }
            set { mTimeScale = value; }
        }

        [Category("Time")]
        [Editor(typeof(DateTimeEditor), typeof(UITypeEditor))]
        public DateTime StartDate
        {
            get { return new DateTime(mStartYear, mStartMonth, mStartDay); }
            set 
            {
                mStartYear = value.Year; 
                mStartMonth = value.Month;
                mStartDay = value.Day;
            }
        }

        [Category("Time")]
        public int StarHour
        {
            get { return mStartHour; }
            set { mStartHour = value; }
        }

        /// <summary>
        /// Enables/disables the lens flare effect.</summary>
        [Category("LensFlare")]
        [Description("Enables/disables the lens flare effect.")]
        public bool LensFlareEffect
        {
            get { return mLensFlare; }
            set { mLensFlare = value; }
        }

        [Category("LensFlare")]
        [EditorLimitsRange(0, 10)]
        [Editor(typeof(Vec2ValueEditor), typeof(UITypeEditor))]
        public Vec2 LensFlareScale
        {
            get { return mFlareScale; }
            set { mFlareScale = value; }
        }

        // Methods --------------------------------------------------------------------

        public CaelumManager()
        {
            mInstance = this;
        }

        /// <summary>
        /// Creates all Caelum's components needed,
        /// register them in <c>mChildren</c> and sets up them.
        /// </summary>
        protected override void  OnPostCreate(bool loaded)
        {
 	        base.OnPostCreate(loaded);

            Log.Info("Initialising Caelum system...");

            // Avoid undo bug
            if(mChildren == null)
                mChildren = new List<CaelumBase>();

            // Creates and sets up a new UniversalClock
            UniversalClock clock = new UniversalClock();
            clock.SetGregorianDateTime(mStartYear, mStartMonth, mStartDay, mStartHour, 0, 0);

            // Sets up Sky color's calculation model
            SkyColorModel.GradientImage = _type.GradientImage;
            SkyColorModel.SkyGradientImage = _type.SkyGradientImage;

            /* Creates all Caelum's element
             * TODO: Find a better way to do that and clean up this code */
            CaelumItem item = getItembyType(CaelumComponent.Sun);
            if (item != null && CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Sun))
            {
                CaelumSun sun = new CaelumSun(item);
                sun.FarDistance = _type.FarDistance;
                mChildren.Add(sun);
            }

            item = getItembyType(CaelumComponent.Moon);
            if (item != null && CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Moon))
            {
                CaelumMoon moon = new CaelumMoon(item);
                moon.FarDistance = _type.FarDistance;
                mChildren.Add(moon);
            }

            item = getItembyType(CaelumComponent.Starfield);
            if (item != null && CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Starfield))
            {
                    mChildren.Add(new CaelumStarfield(item));
            }

            item = getItembyType(CaelumComponent.Skydome);
            if (item != null && CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Skydome))
            {
                    mChildren.Add(new CaelumSkydome(item));
            } 
            
            item = getItembyType(CaelumComponent.Clouds);
            if (item != null && CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Clouds))
            {
                CaelumClouds clouds = new CaelumClouds(item);
                clouds.SetCoverImage(_type.CoverLookupImage);
                mChildren.Add(clouds);
            }

            if (CaelumItem.IsContainType(_type.ComponentsToCreate, CaelumComponent.Fog))
                mManageFog = true;

            new CaelumLensFlare();

			AddTimer();
            onGame = EntitySystemWorld.Instance.WorldSimulationType != WorldSimulationTypes.Editor;

            Log.Info("Done: Initialising Caelum system");
        }

        protected override void OnRender(Camera camera)
        {
            base.OnRender(camera);

            // Avoid some errors on destruction
            if (UniversalClock.Instance == null)
                return;

            if(!onGame)
                UniversalClock.Instance.Update(onGame, 0);
            
            float relDayTime = Convert.ToSingle(UniversalClock.Instance.JulianDay % 1d);

            // Updates each elements one by one
            foreach(CaelumBase element in mChildren)
                element.Update(relDayTime, camera);

            if(CaelumLensFlare.Instance != null)
                CaelumLensFlare.Instance.update(relDayTime, camera);

            // Manages fog
            if (Fog.Instance != null && mManageFog)
            {
                Fog.Instance.ExpDensity = SkyColorModel.GetFogDensity() * FogDensityMultiplier;
                Fog.Instance.Color = SkyColorModel.GetFogColor();
            }
        }

        protected override void OnTick()
        {
            base.OnTick();

            if (UniversalClock.Instance != null && onGame)
                UniversalClock.Instance.Update(onGame, TickDelta);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (mChildren != null)
            {
                foreach (CaelumBase element in mChildren)
                    element.Dispose();

                mChildren.Clear();
            }

            SkyColorModel.Dispose();
            CaelumLensFlare.Instance.Dispose();

            if(UniversalClock.Instance != null)
                UniversalClock.Instance.Dispose();

            mChildren = null;
            mInstance = null;
        }

        /// <summary>
        /// A shortcut function to get parameters by type.</summary>
        protected CaelumItem getItembyType(CaelumComponent Type)
        {
            foreach( CaelumItem item in _type.CaelumItems)
                if (item.IsContainType(Type))
                    return item;

            return null;
        }
    }
}
