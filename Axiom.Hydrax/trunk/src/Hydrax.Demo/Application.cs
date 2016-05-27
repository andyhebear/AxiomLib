using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

using Axiom.Core;
using Axiom.Graphics;
using Axiom.Hydrax.Noise;
using Axiom.Input;
using Axiom.Math;
using Axiom.Hydrax.Modules;
using Axiom.Overlays;

using MaterialManager=Axiom.Hydrax.MaterialManager;

namespace Hydrax.Demo
{
    class Application : IDisposable
    {
        protected Root engine;
        protected Camera camera;
        protected Viewport viewport;
        protected SceneManager scene;
        protected RenderWindow window;
        protected InputReader input;
        protected bool showDebugOverlay = true;

        public const int _def_SkyBoxNum = 3;
        protected Axiom.Hydrax.Hydrax mHydrax;

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
        bool IsObserver = false;
        float mKeyBuffer = 0;
        bool ha = false;

        protected Vector3 camVelocity = Vector3.Zero;
        protected Vector3 camAccel = Vector3.Zero;
        protected float camSpeed = 2.5f;
        protected float cameraScale;
        
        public void Run()
        {
            try
            {
                if ( Setup() )
                {
                    // start the engines rendering loop
                    engine.StartRendering();
                }
            }
            catch ( Exception ex )
            {
                // try logging the error here first, before Root is disposed of
                if ( LogManager.Instance != null )
                {
                    LogManager.Instance.Write( LogManager.BuildExceptionString( ex ) );
                }
            }
        }

        bool Setup()
        {
            // instantiate the Root singleton
            engine = new Root( "AxiomEngine.log" );
            engine = Root.Instance;

            // Set Active Render System
            foreach(RenderSystem iter in engine.RenderSystems)
            {
            	engine.RenderSystem = iter;
            }

            // add event handlers for frame events
            engine.FrameStarted += OnFrameStarted;
            
            window = Root.Instance.Initialize( true, "Axiom Engine Demo Window" );

            ApplicationWindowEventListener rwl = new ApplicationWindowEventListener( window );
            WindowEventMonitor.Instance.RegisterListener( window, rwl );

            ResourceGroupManager.Instance.AddResourceLocation( "Media/Hydrax", "Folder" );
            ResourceGroupManager.Instance.AddResourceLocation( "Media/Skyboxes.zip", "ZipFile" );
            ResourceGroupManager.Instance.InitializeAllResourceGroups();

            ShowDebugOverlay( showDebugOverlay );

            ChooseSceneManager();
            CreateCamera();
            CreateViewports();

            // set default mipmap level
            TextureManager.Instance.DefaultMipmapCount = 5;

            input = SetupInput();

            // call the overridden CreateScene method
            CreateScene();
            return true;
        }

        /// <summary>
        ///    Shows the debug overlay, which displays performance statistics.
        /// </summary>
        void ShowDebugOverlay( bool show )
        {
            // gets a reference to the default overlay
            Overlay o = OverlayManager.Instance.GetByName( "Core/DebugOverlay" );

            if ( o == null )
            {
                LogManager.Instance.Write( string.Format( "Could not find overlay named '{0}'.", "Core/DebugOverlay" ) );
            }
            else
            {

                if ( show )
                {
                    o.Show();
                }
                else
                {
                    o.Hide();
                }
            }
        }

        void ChooseSceneManager()
        {
            // Get the SceneManager, a generic one by default
            scene = engine.CreateSceneManager( "TerrainSceneManager", "HydraxSMInstance" );
            scene.ClearScene();
        }

        void CreateCamera()
        {
            // create a camera and initialize its position
            camera = scene.CreateCamera( "MainCamera" );
            camera.Position = new Vector3( 0, 0, 500 );
            camera.LookAt( new Vector3( 0, 0, -300 ) );

            // set the near clipping plane to be very close
            camera.Near = 5;
            camera.AutoAspectRatio = true;
        }

        void CreateViewports()
        {
            Debug.Assert( window != null, "Attempting to use a null RenderWindow." );

            // create a new viewport and set it's background color
            viewport = window.AddViewport( camera, 0, 0, 1.0f, 1.0f, 100 );
            viewport.BackgroundColor = ColorEx.Black;
        }

        InputReader SetupInput()
        {
            InputReader ir = null;
#if  !( XBOX || XBOX360 ) && !( SIS )
            // retrieve and initialize the input system
            ir = PlatformManager.Instance.CreateInputReader();
            ir.Initialize( window, true, true, false, false );
#endif
			return ir;
        }

        void OnFrameStarted( Object source, FrameEventArgs e )
        {
        	input.Capture();
        	
        	float scaleMove = 200 * e.TimeSinceLastFrame;
        	
        	// reset acceleration zero
            camAccel = Vector3.Zero;

            if ( input.IsKeyPressed( KeyCodes.Escape ) )
            {
                //Root.Instance.QueueEndRendering();
                e.StopRendering = true;
            }

            if ( input.IsKeyPressed( KeyCodes.A ) )
            {
                camAccel.x = -0.5f;
            }

            if ( input.IsKeyPressed( KeyCodes.D ) )
            {
                camAccel.x = 0.5f;
            }

            if ( input.IsKeyPressed( KeyCodes.W ) )
            {
                camAccel.z = -1.0f;
            }

            if ( input.IsKeyPressed( KeyCodes.S ) )
            {
                camAccel.z = 1.0f;
            }

            //camAccel.y += (float)( input.RelativeMouseZ * 0.1f );

            if ( input.IsKeyPressed( KeyCodes.Left ) )
            {
                camera.Yaw( cameraScale );
            }

            if ( input.IsKeyPressed( KeyCodes.Right ) )
            {
                camera.Yaw( -cameraScale );
            }

            if ( input.IsKeyPressed( KeyCodes.Up ) )
            {
                camera.Pitch( cameraScale );
            }

            if ( input.IsKeyPressed( KeyCodes.Down ) )
            {
                camera.Pitch( -cameraScale );
            }
            
            if ( !input.IsMousePressed( MouseButtons.Left ) )
			{
				float cameraYaw = -input.RelativeMouseX * .13f;
				float cameraPitch = -input.RelativeMouseY * .13f;

				camera.Yaw( cameraYaw );
				camera.Pitch( cameraPitch );
			}
			else
			{
				// TODO unused
				//cameraVector.x += input.RelativeMouseX * 0.13f;
			}
            
            camVelocity += ( camAccel * scaleMove * camSpeed );

            // move the camera based on the accumulated movement vector
            camera.MoveRelative( camVelocity * e.TimeSinceLastFrame );

            // Now dampen the Velocity - only if user is not accelerating
            if ( camAccel == Vector3.Zero )
            {
                camVelocity *= ( 1 - ( 6 * e.TimeSinceLastFrame ) );
            }

            
            if ( mHydrax != null )
            {
                mHydrax.Update( e.TimeSinceLastFrame );
            }
            
        }

        void TakeScreenshot( string fileName )
        {
            window.WriteContentsToFile( fileName );
        }

        void CreateScene()
        {
			ResourceGroupManager.Instance.InitializeAllResourceGroups();
			
            // Set default ambient light.
            scene.AmbientLight = new ColorEx( 1, 1, 1 );

            // Create the SkyBox
            scene.SetSkyBox( true, mSkyBoxes[ mCurrentSkyBox ], 99999 * 3 );
            camera.Far = 99999 * 6;
            camera.Position = new Vector3( 312.902f, 206.419f, 1524.02f );
            camera.Orientation = new Quaternion( 0.998f, -0.0121f, -0.0608f, -0.00074f );
            Light mLight = scene.CreateLight( "Light0" );
            mLight.Position = mSunPosition[ mCurrentSkyBox ];
            mLight.Diffuse = new ColorEx( 1, 1, 1 );
            mLight.Specular = new ColorEx(
                mSunColor[ mCurrentSkyBox ].x,
                mSunColor[ mCurrentSkyBox ].y,
                mSunColor[ mCurrentSkyBox ].z );

            ProjectedGrid.GridOptions mOptions;
            mOptions = new ProjectedGrid.GridOptions();
            mOptions.Complexity = 264;
            mOptions.Strength = 25f;
            mOptions.Elevation = 50.0f;
            mOptions.Smooth = false;
            mOptions.ForceRecalculateGeometry = false;
            mOptions.ChoppyWaves = true;
            mOptions.ChoppyStrength = 2.9f;
            
            Perlin.Options perlinOptions = new Perlin.Options(8,0.185f,0.49f,1.4f,1.27f,2,new Vector3(0.5f,50,150000));
            
            Perlin perlinNoise = new Perlin();
            FFT fftNoise = new FFT( new FFT.Options( 64 ) );
            
            //create hydrax
            mHydrax = new Axiom.Hydrax.Hydrax( scene, camera, window.GetViewport( 0 ) );
            mHydrax.Position = new Vector3( 1500, 50, 1500 );
            
            

            SimpleGrid simpleGrid 
			= new SimpleGrid(mHydrax,perlinNoise,
								Axiom.Hydrax.MaterialManager.NormalMode.NM_VERTEX,
								new SimpleGrid.Options(512,new Axiom.Hydrax.Size(5000,5000),45,false,true,0.05f));
			
            
            ProjectedGrid projectedGrid = new ProjectedGrid(
                mHydrax,
 				perlinNoise,

 				new Plane( new Vector3( 0, 1, 0 ), new Vector3( 0, 0, 0 ) ),
                MaterialManager.NormalMode.NM_VERTEX,
                mOptions );

            mHydrax.SetComponents(Axiom.Hydrax.HydraxComponent.Sun | 
                                  Axiom.Hydrax.HydraxComponent.Foam | 
                                  Axiom.Hydrax.HydraxComponent.Depth | Axiom.Hydrax.HydraxComponent.Caustics);
            
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


            //   scene.LoadWorldGeometry("Terrain.xml");
            //  CreatePalms();
            scene.LoadWorldGeometry("Terrain.xml");
            Axiom.Graphics.Material mDepth = (Axiom.Graphics.Material)Axiom.Graphics.MaterialManager.Instance.GetByName("Terrain");

            mHydrax.MaterialManager.AddDepthTechnique(mDepth.CreateTechnique());
        }

        private void ChangeSkyBox()
        {
            scene.SetSkyBox( false, mSkyBoxes[ mLastSkyBox ], 99999 * 3 );
            scene.SetSkyBox( true, mSkyBoxes[ mCurrentSkyBox ], 99999 * 3 );

            mHydrax.SunPosition = mSunPosition[ mCurrentSkyBox ];
            mHydrax.SunColor = mSunColor[ mCurrentSkyBox ];

            scene.GetLight( "Light0" ).Position = mSunPosition[ mCurrentSkyBox ];
            scene.GetLight( "Light0" ).Specular = new ColorEx( mSunColor[ mCurrentSkyBox ].x, mSunColor[ mCurrentSkyBox ].y, mSunColor[ mCurrentSkyBox ].z );
        }

        private void CreatePalms()
        {
            int NumberOfPalms = 12;

            /*
            SceneNode mPalmSceneNode = scene.RootSceneNode.CreateChildSceneNode();

            for ( int k = 0; k < NumberOfPalms; k++ )
            {
                Vector3 randomPos = new Vector3( rand( 500, 2500 ), 0, rand( 500, 2500 ) );
                RaySceneQuery raySceneQuere = scene.CreateRayQuery(
                    new Ray( randomPos + new Vector3( 0, 1000000, 0 ), Vector3.NegativeUnitY ) );
                List<RaySceneQueryResultEntry> list = raySceneQuere.Execute();

                if ( list.Count > 0 )
                {
                    for ( int i = 0; i < list.Count; i++ )
                    {

                    }
                }
                //   else
                //     {
                //         k--;
                //        continue;
                //    }
                Entity mPalmEnt = scene.CreateEntity( "Palm" + k.ToString(), "Palm.mesh" );
                SceneNode mPalmSN = mPalmSceneNode.CreateChildSceneNode();
                mPalmSN.Rotate( new Vector3( -1, 0, rand( -0.3f, 0.3f ) ), 90 );
                mPalmSN.AttachObject( mPalmEnt );
                float scale = rand( 50, 75 );
                mPalmSN.ScaleBy( new Vector3( scale, scale, scale ) );
                mPalmSN.Position = randomPos;
            }*/
        }

        float seed = 801;
        private float rand( float min, float max )
        {

            seed += (float)( System.Math.PI * 2.8574f + seed * ( 0.3424f - 0.12434f + 0.452345f ) );
            if ( seed > 10000000000 ) seed -= 10000000000;
            Real r = new Real( seed );
            return (float)( ( max - min ) * System.Math.Abs( Axiom.Math.Utility.Sin( new Radian( r ) ) ) + min );
        }

        #region IDisposable Implementation

        public void Dispose()
        {
            if ( engine != null )
            {
                // remove event handlers
                engine.FrameStarted -= OnFrameStarted;

                //engine.Dispose();
            }
            if ( scene != null )
                scene.RemoveAllCameras();
            camera = null;
            Root.Instance.RenderSystem.DetachRenderTarget( window );
            window.Dispose();

            engine.Dispose();

        }

        #endregion IDisposable Implementation
    }
}
