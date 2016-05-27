#region Namespace Declarations
using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Linq;
using Axiom.Core;
using Axiom.Overlays;
using Axiom.Math;
using Axiom.Graphics;

using SIS = SharpInputSystem;

using SWF = System.Windows.Forms;

#endregion Namespace Declarations

namespace Demo.SkyX
{
    /// <summary>
    ///     Base class for Axiom examples.
    /// </summary>
    public abstract class Application : IDisposable
    {
        #region Protected Fields
        protected Root engine;
        protected Camera camera;
        protected Viewport viewport;
        protected SceneManager scene;
        protected RenderWindow window;
        protected Vector3 cameraVector = Vector3.Zero;
        protected float cameraScale;
        protected bool showDebugOverlay = true;
        protected float statDelay = 0.0f;
        protected float debugTextDelay = 0.0f;
        protected string debugText = "";
        protected float toggleDelay = 0.0f;
        protected Vector3 camVelocity = Vector3.Zero;
        protected Vector3 camAccel = Vector3.Zero;
        protected float camSpeed = 2.5f;
        protected int aniso = 1;
        protected TextureFiltering filtering = TextureFiltering.Bilinear;

        //SIS Variables
        protected SIS.InputManager inputManager;
        protected SIS.Keyboard inputKeyboard;
        protected SIS.Mouse inputMouse;
        protected EventHandler eventHandler = new EventHandler();

        #endregion Protected Fields

        #region Protected Methods

        protected virtual void CreateCamera()
        {
            // create a camera and initialize its position
            camera = scene.CreateCamera( "MainCamera" );
            camera.Position = new Vector3( 0, 0, 500 );
            camera.LookAt( new Vector3( 0, 0, -300 ) );

            // set the near clipping plane to be very close
            camera.Near = 5;
            camera.AutoAspectRatio = true;
        }

        /// <summary>
        ///    Shows the debug overlay, which displays performance statistics.
        /// </summary>
        protected void ShowDebugOverlay( bool show )
        {
            // gets a reference to the default overlay
            Overlay o = OverlayManager.Instance.GetByName( "Core/DebugOverlay" );

            if ( o == null )
            {
                LogManager.Instance.Write( string.Format( "Could not find overlay named '{0}'.", "Core/DebugOverlay" ) );
            }
            else
            {
                // If we have something to play with
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

        protected void TakeScreenshot( string fileName )
        {
            window.WriteContentsToFile( fileName );
        }

        #endregion Protected Methods

        #region Protected Virtual Methods

        protected virtual void ChooseSceneManager()
        {
            // Get the SceneManager, a generic one by default
            scene = engine.CreateSceneManager( SceneType.Generic, "ExampleInstance" );
            scene.ClearScene();

            // Add it to root
            Root.Instance.SceneManager = scene;
        }

        protected virtual void CreateViewports()
        {
            Debug.Assert( window != null, "Attempting to use a null RenderWindow." );

            // create a new viewport and set it's background color
            viewport = window.AddViewport( camera, 0, 0, 1.0f, 1.0f, 100 );
            viewport.BackgroundColor = ColorEx.Black;
        }

        protected virtual bool Setup()
        {
            // instantiate the Root singleton
            engine = new Root( "Axiom.log" );

            // add event handlers for frame events
            engine.FrameStarted += new EventHandler<FrameEventArgs>( OnFrameStarted );
            engine.FrameEnded += new EventHandler<FrameEventArgs>(OnFrameEnded);

            // allow for setting up resource gathering
            SetupResources();

            Root.Instance.RenderSystem = Root.Instance.RenderSystems.First().Value;

            LoadRenderSystemConfiguration( null, Root.Instance.RenderSystem );

            try
            {
                window = Root.Instance.Initialize( true, "Axiom GUI Render Window" );
                ResourceGroupManager.Instance.InitializeAllResourceGroups();
            }
            catch ( Exception ex )
            {
                ex.ToString();
            }

            ShowDebugOverlay( showDebugOverlay );

            inputManager = SIS.InputManager.CreateInputSystem( SWF.Control.FromHandle( (IntPtr)window[ "WINDOW" ] ) );

            inputKeyboard = inputManager.CreateInputObject<SIS.Keyboard>( true, "" );
            inputKeyboard.EventListener = eventHandler;

            //inputMouse = inputManager.CreateInputObject<SIS.Mouse>(false, "");
            //inputMouse.EventListener = eventHandler;

            //inputMouse.MouseState.Width = window.Width;
            //inputMouse.MouseState.Height = window.Height;

            ChooseSceneManager();
            CreateCamera();
            CreateViewports();

            // set default mipmap level
            //TextureManager.Instance.DefaultMipmapCount = 5;

            // call the overridden CreateScene method
            CreateScene();

            return true;
        }

        /// <summary>
        /// Loads the RS config
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="rs"></param>
        private void LoadRenderSystemConfiguration( object sender, RenderSystem rs )
        {
            string renderSystemId = rs.GetType().FullName;
            EngineConfig config = new EngineConfig();
            config.ReadXml( @"EngineConfig.xml" );

            EngineConfig.ConfigOptionDataTable codt = ( (EngineConfig.ConfigOptionDataTable)config.Tables[ "ConfigOption" ] );
            foreach ( EngineConfig.ConfigOptionRow row in codt )
            {
                if ( row.RenderSystem == renderSystemId )
                {
                    if ( rs.ConfigOptions.ContainsKey( row.Name ) )
                        rs.ConfigOptions[ row.Name ].Value = row.Value;
                }
            }
        }

        /// <summary>
        /// Loads default resource configuration if one exists.
        /// </summary>
        protected virtual void SetupResources()
        {
            EngineConfig config = new EngineConfig();
            config.ReadXml( @"EngineConfig.xml" );

            // interrogate the available resource paths
            foreach ( EngineConfig.FilePathRow row in config.FilePath )
            {
                ResourceGroupManager.Instance.AddResourceLocation( row.src, row.type, false );
            }
        }

        #endregion Protected Virtual Methods

        #region Protected Abstract Methods

        /// <summary>
        /// Overridable method for the scene creation
        /// </summary>
        protected abstract void CreateScene();

        #endregion Protected Abstract Methods

        #region Public Methods

        public void Start()
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

        public void Dispose()
        {
            if ( engine != null )
            {
                // remove event handlers
                engine.FrameStarted -= OnFrameStarted;
                engine.FrameEnded -= OnFrameEnded;
            }
            if ( scene != null )
            {
                scene.RemoveAllCameras();
                scene.RemoveCamera( camera );

                camera = null;
                Root.Instance.RenderSystem.DetachRenderTarget( window );
                engine.Dispose();
            }
        }

        #endregion Public Methods

        #region Event Handlers

        protected virtual void OnFrameEnded( Object source, FrameEventArgs e )
        {
        }

        protected virtual void OnFrameStarted( Object source, FrameEventArgs e )
        {
            input.Update(e);
            float scaleMove = 200 * e.TimeSinceLastFrame;

            // reset acceleration zero
            camAccel = Vector3.Zero;

            // set the scaling of camera motion
            cameraScale = 100 * e.TimeSinceLastFrame;

            inputKeyboard.Capture();
            //inputMouse.Capture();

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_ESCAPE ) )
            {
                Root.Instance.QueueEndRendering();

                return;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_A ) )
            {
                camAccel.x = -0.5f;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_D ) )
            {
                camAccel.x = 0.5f;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_W ) )
            {
                camAccel.z = -1.0f;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_S ) )
            {
                camAccel.z = 1.0f;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_LEFT ) )
            {
                camera.Yaw( cameraScale );
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_RIGHT ) )
            {
                camera.Yaw( -cameraScale );
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_UP ) )
            {
                camera.Pitch( cameraScale );
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_DOWN ) )
            {
                camera.Pitch( -cameraScale );
            }

            // subtract the time since last frame to delay specific key presses
            toggleDelay -= e.TimeSinceLastFrame;

            // toggle rendering mode
            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_R ) && toggleDelay < 0 )
            {
                if ( camera.PolygonMode == PolygonMode.Points )
                {
                    camera.PolygonMode = PolygonMode.Solid;
                }
                else if ( camera.PolygonMode == PolygonMode.Solid )
                {
                    camera.PolygonMode = PolygonMode.Wireframe;
                }
                else
                {
                    camera.PolygonMode = PolygonMode.Points;
                }

                Console.WriteLine( "Rendering mode changed to '{0}'.", camera.PolygonMode );

                toggleDelay = 1;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_T ) && toggleDelay < 0 )
            {
                // toggle the texture settings
                switch ( filtering )
                {
                    case TextureFiltering.Bilinear:
                        filtering = TextureFiltering.Trilinear;
                        aniso = 1;
                        break;
                    case TextureFiltering.Trilinear:
                        filtering = TextureFiltering.Anisotropic;
                        aniso = 8;
                        break;
                    case TextureFiltering.Anisotropic:
                        filtering = TextureFiltering.Bilinear;
                        aniso = 1;
                        break;
                }

                Console.WriteLine( "Texture Filtering changed to '{0}'.", filtering );

                // set the new default
                MaterialManager.Instance.SetDefaultTextureFiltering( filtering );
                MaterialManager.Instance.DefaultAnisotropy = aniso;

                toggleDelay = 1;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_P ) )
            {
                string[] temp = Directory.GetFiles( Environment.CurrentDirectory, "screenshot*.jpg" );
                string fileName = string.Format( "screenshot{0}.jpg", temp.Length + 1 );

                TakeScreenshot( fileName );

                // show briefly on the screen
                debugText = string.Format( "Wrote screenshot '{0}'.", fileName );

                // show for 2 seconds
                debugTextDelay = 2.0f;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_B ) )
            {
                scene.ShowBoundingBoxes = !scene.ShowBoundingBoxes;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_F ) )
            {
                // hide all overlays, includes ones besides the debug overlay
                viewport.ShowOverlays = !viewport.ShowOverlays;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_COMMA ) )
            {
                Root.Instance.MaxFramesPerSecond = 60;
            }

            if ( inputKeyboard.IsKeyDown( SIS.KeyCode.Key_PERIOD ) )
            {
                Root.Instance.MaxFramesPerSecond = 0;
            }

            camVelocity += ( camAccel * scaleMove * camSpeed );

            // move the camera based on the accumulated movement vector
            camera.MoveRelative( camVelocity * e.TimeSinceLastFrame );

            // Now dampen the Velocity - only if user is not accelerating
            if ( camAccel == Vector3.Zero )
            {
                camVelocity *= ( 1 - ( 6 * e.TimeSinceLastFrame ) );
            }

            // update performance stats once per second
            if ( statDelay <= 0.0f && showDebugOverlay )
            {
                UpdateStats();
                statDelay = 1.0f;
            }
            else
            {
                statDelay -= e.TimeSinceLastFrame;
            }

            // turn off debug text when delay ends
            if ( debugTextDelay < 0.0f )
            {
                debugTextDelay = 0.0f;
                debugText = "";
            }
            else if ( debugTextDelay > 0.0f )
            {
                debugTextDelay -= e.TimeSinceLastFrame;
            }
            if (input.IsMouseButtonDown(Axiom.Input.MouseButtons.Right))
            {
                float cameraYaw = -input.MousePosition.x * 10;
                float cameraPitch = -input.MousePosition.y * 10;

                camera.Yaw(cameraYaw);
                camera.Pitch(cameraPitch);
            }
            OverlayElement element = OverlayManager.Instance.Elements.GetElement( "Core/DebugText" );
            element.Text = debugText;
            return;
        }
        protected Axiom.SkyX.InputManager input;
        protected void UpdateStats()
        {
            OverlayElement label;

            label = OverlayManager.Instance.Elements.GetElement( "Core/CurrFps" );
            if ( label != null )
            {
                label.Text = string.Format( "Current FPS: {0:#0.00}", engine.CurrentFPS );
                label.Color = ColorEx.Goldenrod;
            }

            label = OverlayManager.Instance.Elements.GetElement( "Core/AverageFps" );
            if ( label != null )
            {
                label.Text = string.Format( "Average FPS: {0:#0.00}", engine.AverageFPS );
            }

            label = OverlayManager.Instance.Elements.GetElement( "Core/WorstFps" );
            if ( label != null )
            {
                label.Text = string.Format( "Worst FPS: {0:#0.00}", engine.WorstFPS );
            }

            label = OverlayManager.Instance.Elements.GetElement( "Core/BestFps" );
            if ( label != null )
            {
                label.Text = string.Format( "Best FPS: {0:#0.00}", engine.BestFPS );
            }

            label = OverlayManager.Instance.Elements.GetElement( "Core/NumTris" );
            if ( label != null )
            {
                label.Text = string.Format( "Triangle Count: {0:#0}", viewport.RenderedFaceCount );
            }

            label = OverlayManager.Instance.Elements.GetElement( "Core/NumBatches" );
            if ( label != null )
            {
                label.Text = string.Format( "Batch Count: {0:#0}", viewport.RenderedBatchCount );
            }

        }

        #endregion Event Handlers
    }
}
