#region MIT/X11 License
//Copyright © 2003-2011 Axiom 3D Rendering Engine Project
//
//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:
//
//The above copyright notice and this permission notice shall be included in
//all copies or substantial portions of the Software.
//
//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//THE SOFTWARE.
#endregion License

using System;
using Axiom.Math;
using Axiom.Core;

using HX = Axiom.Hydrax;
using Axiom.Hydrax.Noise;
using Axiom.Hydrax.Modules;

namespace Axiom.Samples.Sample
{
	/// <summary>
	/// 
	/// </summary>
	public class HydraxSample : SdkSample
	{
		public const int NumSkybox = 3;
		protected HX.Hydrax mHydrax;

		protected string[] mSkyBoxes =
            {
                "Skybox/Stormy",
                "Skybox/Morning",
                "Skybox/CloudHills"
            };

		protected Vector3[] mSunPosition =
            {
                new Vector3( 0, 10000, 0 ),
                new Vector3( 0, 10000, 90000 ),
                new Vector3( 0, 10000, 0 )
            };

		protected Vector3[] mSunColor =
            {
                new Vector3( 1, 0.9f, 0.6f ),
                new Vector3( 1, 0.6f, 0.4f ),
                new Vector3( 0.45f, 0.45f, 0.45f )
            };

		int mLastSkyBox = 0;
		int mCurrentSkyBox = 0;

		/// <summary>
		/// 
		/// </summary>
		public HydraxSample()
		{
			Metadata[ "Title" ] = "YourSampleTitle";
			Metadata[ "Description" ] = "YourSampleDescription";
			Metadata[ "Thumbnail" ] = "thumb_jitter.png";
			Metadata[ "Category" ] = "YourCategory";
		}

		protected override void SetupContent()
		{
			Camera.Position = new Vector3( 0, 0, 500 );
			Camera.LookAt( new Vector3( 0, 0, -300 ) );
			// set the near clipping plane to be very close
			Camera.Near = 5;

			// Set default ambient light.
			SceneManager.AmbientLight = new ColorEx( 1, 1, 1 );

			// Create the SkyBox
			SceneManager.SetSkyBox( true, mSkyBoxes[ mCurrentSkyBox ], 99999 * 3 );
			Camera.Far = 99999 * 6;
			Camera.Position = new Vector3( 312.902f, 206.419f, 1524.02f );
			Camera.Orientation = new Quaternion( 0.998f, -0.0121f, -0.0608f, -0.00074f );
			Light mLight = SceneManager.CreateLight( "Light0" );
			mLight.Position = mSunPosition[ mCurrentSkyBox ];
			mLight.Diffuse = new ColorEx( 1, 1, 1 );
			mLight.Specular = new ColorEx(
				mSunColor[ mCurrentSkyBox ].x,
				mSunColor[ mCurrentSkyBox ].y,
				mSunColor[ mCurrentSkyBox ].z );

			ProjectedGrid.GridOptions mOptions;
			mOptions = new ProjectedGrid.GridOptions();
			mOptions.Complexity = 64;
			mOptions.Strength = 25f;
			mOptions.Elevation = 50.0f;
			mOptions.Smooth = false;
			mOptions.ForceRecalculateGeometry = false;
			mOptions.ChoppyWaves = true;
			mOptions.ChoppyStrength = 2.9f;

			Perlin.Options perlinOptions = new Perlin.Options( 8, 0.185f, 0.49f, 1.4f, 1.27f, 2, new Vector3( 0.5f, 50, 150000 ) );

			Perlin perlinNoise = new Perlin();
			FFT fftNoise = new FFT( new FFT.Options( 64 ) );

			//create hydrax
			mHydrax = new Axiom.Hydrax.Hydrax( SceneManager, Camera, Window.GetViewport( 0 ) );
			mHydrax.Position = new Vector3( 1500, 50, 1500 );



			SimpleGrid simpleGrid
			= new SimpleGrid( mHydrax, perlinNoise,
								Axiom.Hydrax.MaterialManager.NormalMode.NM_VERTEX,
								new SimpleGrid.Options( 64, new Axiom.Hydrax.Size( 5000, 5000 ), 45, false, true, 0.05f ) );


			ProjectedGrid projectedGrid = new ProjectedGrid(
				mHydrax,
				perlinNoise,

				new Plane( new Vector3( 0, 1, 0 ), new Vector3( 0, 0, 0 ) ),
				HX.MaterialManager.NormalMode.NM_VERTEX,
				mOptions );

			mHydrax.SetComponents( HX.HydraxComponent.Sun | HX.HydraxComponent.Smooth | HX.HydraxComponent.Caustics );

			mHydrax.Module = projectedGrid;
			/* mHydrax.LoadCfg( "HydraxDemo.hdx" );*/

			mHydrax.Create();

			mHydrax.PlanesError = 10.5f;
			mHydrax.FullReflectionDistance = 1e+011f;
			mHydrax.GlobalTransparency = 0;

			mHydrax.NormalDistortion = 0.075f;
			mHydrax.WaterColor = new Vector3( 0.058209f, 0.535822f, 0.679105f );
			mHydrax.SunPosition = new Vector3( 0, 10000, 0 );
			mHydrax.SunStrenght = 0.75f;
			mHydrax.SunColor = new Vector3( 1.0f, 0.8f, 0.4f );
			mHydrax.FoamTransparency = 1;
			mHydrax.DepthLimit = 15;
			mHydrax.SmoothPower = 5;
			mHydrax.FoamMaxDistance = 7.5e+007f;
			mHydrax.FoamScale = 0.0075f;
			mHydrax.FoamStart = 0;
			mHydrax.CausticsScale = 135;
			mHydrax.CausticsPower = 10.5f;
			mHydrax.CausticsEnd = 0.8f;
			mHydrax.GodRaysExposure = new Vector3( 0.76f, 2.46f, 2.29f );
			mHydrax.GodRaysIntensity = 0.015f;
			mHydrax.Position = new Vector3( 0, 0, 0 );

			//   SceneManager.LoadWorldGeometry("Terrain.xml");
			//  CreatePalms();
			//SceneManager.LoadWorldGeometry( "Terrain.xml" );
			//Axiom.Graphics.Material mDepth = (Axiom.Graphics.Material)Axiom.Graphics.MaterialManager.Instance.GetByName( "Terrain" );

			//mHydrax.MaterialManager.AddDepthTechnique( mDepth.CreateTechnique() );
			base.SetupContent();
		}

		public override bool FrameRenderingQueued( Core.FrameEventArgs evt )
		{
			if ( mHydrax != null )
			{
				mHydrax.Update( evt.TimeSinceLastFrame );
			}
			return base.FrameRenderingQueued( evt );
		}
	}
}
