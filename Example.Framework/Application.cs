﻿
#region Namespace Declarations

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;

using Axiom.Core;
using Axiom.Input;
using Axiom.Overlays;
using Axiom.Math;
using Axiom.Graphics;
using MouseButtons = Axiom.Input.MouseButtons;

using SWF = System.Windows.Forms;
using Example.Framework.Configuration;
using Axiom.Configuration;

#endregion Namespace Declarations

namespace Example.Framework
{
    /// <summary>
    ///     Base class for Axiom examples.
    /// </summary>
    public abstract class ExampleApplication : IDisposable
    {
        #region Protected Fields
        protected Root Root;
        protected Camera Camera;
        protected Viewport Viewport;
        protected SceneManager SceneManager;
        protected RenderWindow Window;
        protected InputReader Input;
        protected Vector3 cameraVector = Vector3.Zero;
        protected float cameraScale;
        protected bool showDebugOverlay = true;
        protected float statDelay = 0.0f;
        protected float debugTextDelay = 0.0f;
        protected string debugText = "";
        protected float toggleDelay = 0.0f;
        protected Vector3 camVelocity = Vector3.Zero;
        protected Vector3 camAccel = Vector3.Zero;
        protected Vector3 mouseRotateVector = Vector3.Zero;
        protected bool isUsingKbCameraLook = false;
        protected float camSpeed = 2.5f;
        protected int aniso = 1;
        protected TextureFiltering filtering = TextureFiltering.Bilinear;
        private const string CONFIG_FILE = @"EngineConfig.xml";
        protected EngineConfig config = new EngineConfig();
        #endregion Protected Fields

        #region Protected Methods

        /// <summary>
        /// Creates the Camera object for the scene.
        /// </summary>
        protected virtual void CreateCamera()
        {
            // create a camera and initialize its position
            Camera = SceneManager.CreateCamera( "MainCamera" );
            Camera.Position = new Vector3( 0, 0, 500 );
            Camera.LookAt( new Vector3( 0, 0, -300 ) );

            // set the near clipping plane to be very close
            Camera.Near = 5;
        }

        /// <summary>
        ///    Shows the debug overlay, which displays performance statistics.
        /// </summary>
        protected void ShowDebugOverlay( bool show )
        {
            // gets a reference to the default overlay

            Overlay o = OverlayManager.Instance.GetByName("Core/DebugOverlay");

            if ( o == null )
            {
                LogManager.Instance.Write( string.Format( "Could not find overlay named '{0}'.", "Core/DebugOverlay" ) );
            }

            if ( show )
            {
                o.Show();
            }
            else
            {
                o.Hide();
            }
        }

        /// <summary>
        /// Takes a screenshot... used where?
        /// </summary>
        /// <param name="fileName"></param>
        protected void TakeScreenshot( string fileName )
        {
            Window.WriteContentsToFile( fileName );
        }

        #endregion Protected Methods

        #region Protected Virtual Methods

        /// <summary>
        /// Handles the selection of an appropriate scene manager
        /// </summary>
        protected virtual void ChooseSceneManager()
        {
            // Get the SceneManager, a generic one by default
            SceneManager = Root.CreateSceneManager( SceneType.Generic );
            SceneManager.ClearScene();
        }

        /// <summary>
        /// Handles creating the viewport for a window.
        /// </summary>
        protected virtual void CreateViewports()
        {
            Debug.Assert( Window != null, "Attempting to use a null RenderWindow." );

            // create a new viewport and set it's background color
            Viewport = Window.AddViewport( Camera, 0, 0, 1.0f, 1.0f, 100 );
            Viewport.BackgroundColor = ColorEx.Black;
        }

        /// <summary>
        /// Establishes the Engine.FrameStarted/Engine.FrameEnded events
        /// </summary>
        protected virtual void SetupFrameHandlers()
        {

            // add event handlers for frame events
            Root.FrameStarted += OnFrameStarted;
            Root.FrameEnded += OnFrameEnded;

        }

        /// <summary>
        /// Handles the initialization and ordering of all of the various Create*,Setup*, etc
        /// methods in an ExampleApplication instance
        /// </summary>
        /// <returns>returns true on successful setup. Used by Run()</returns>
        protected virtual bool Setup()
        {
            // instantiate the Root singleton
            Root = new Root( "AxiomEngine.log" );

            // this actually loads the resource information
            // stored in EngineConfig.xml
            SetupResources();

            // This runs the ConfigDialog for per-session
            // variables like which renderer to use, fullscreen, etc
            if ( !Configure() )
            {
                // shutting right back down
                Root.Shutdown();

                return false;
            }

            // initialize a RenderWindow
            Window = Root.Instance.Initialize( true, "Axiom Engine Demo Window" );

            // intialize resources .. need a window initialized before running this step
            ResourceGroupManager.Instance.InitializeAllResourceGroups();

            // establish the debug overlay
            ShowDebugOverlay( showDebugOverlay );

            // select a scene manager
            ChooseSceneManager();

            // set up the PlayerCamera
            CreateCamera();

            // set up the Viewports
            CreateViewports();

            // sets up input
            SetupInput();

            // call the overridden CreateScene method
            CreateScene();

            // set default mipmap level
            TextureManager.Instance.DefaultMipmapCount = 5;

            // sets up the FrameStarted/FrameEnded events
            SetupFrameHandlers();

            return true;
        }

        /// <summary>
        /// Handles the setup of the Input system for the ExampleApplication
        /// </summary>
        protected virtual void SetupInput()
        {

            // retreive and initialize the input system
            Input = PlatformManager.Instance.CreateInputReader();
            Input.Initialize( Window, true, true, false, false );

        }

        /// <summary>
        ///		Loads default resource configuration if one exists.
        /// </summary>
        protected virtual void SetupResources()
        {
            string resourceConfigPath = Path.GetFullPath( CONFIG_FILE );

            if ( File.Exists( resourceConfigPath ) )
            {
                // load the config file
                // relative from the location of debug and releases executables
                config.ReadXml( CONFIG_FILE );

                // interrogate the available resource paths
                foreach ( EngineConfig.FilePathRow row in config.FilePath )
                {
                    ResourceGroupManager.Instance.AddResourceLocation( row.src, row.type );
                }
            }
        }

        /// <summary>
        /// Attempts to get a session configuration.
        /// </summary>
        /// <returns>returns true on success</returns>
        protected virtual bool Configure()
        {
            ConfigDialog dlg = new ConfigDialog();
            
            //theMouse added 
            dlg.LoadRenderSystemConfig += new ConfigDialog.LoadRenderSystemConfigEventHandler(LoadRenderSystemConfiguration);
            dlg.SaveRenderSystemConfig += new ConfigDialog.SaveRenderSystemConfigEventHandler(SaveRenderSystemConfiguration);

            SWF.DialogResult result = dlg.ShowDialog();
            if ( result == SWF.DialogResult.Cancel )
            {
                Root.Instance.Dispose();
                Root = null;
                return false;
            }

            return true;
        }

        private void SaveRenderSystemConfiguration(object sender, RenderSystem rs)
        {
            string renderSystemId = rs.GetType().FullName;

            EngineConfig.ConfigOptionDataTable codt = ((EngineConfig.ConfigOptionDataTable)config.Tables["ConfigOption"]);
            foreach (ConfigOption opt in rs.ConfigOptions)
            {
                EngineConfig.ConfigOptionRow coRow = codt.FindByNameRenderSystem(opt.Name, renderSystemId);
                if (coRow == null)
                {
                    coRow = codt.NewConfigOptionRow();
                    coRow.RenderSystem = renderSystemId;
                    coRow.Name = opt.Name;
                    codt.AddConfigOptionRow(coRow);
                }
                coRow.Value = opt.Value;
            }
            config.AcceptChanges();
            config.WriteXml(CONFIG_FILE);
        }

        private void LoadRenderSystemConfiguration(object sender, RenderSystem rs)
        {
            string renderSystemId = rs.GetType().FullName;

            EngineConfig.ConfigOptionDataTable codt = ((EngineConfig.ConfigOptionDataTable)config.Tables["ConfigOption"]);
            foreach (EngineConfig.ConfigOptionRow row in codt)
            {
                if (row.RenderSystem == renderSystemId)
                {
                    if (rs.ConfigOptions.ContainsKey(row.Name))
                    {
                        rs.ConfigOptions[row.Name].Value = row.Value;
                    }
                }
            }
        }
        #endregion Protected Virtual Methods

        #region Protected Abstract Methods

        /// <summary>
        /// Create a scene. Must be implemented by an inheritor.
        /// </summary>
        protected abstract void CreateScene();

        #endregion Protected Abstract Methods

        #region Public Methods

        /// <summary>
        /// Begins the execution the application.
        /// </summary>
        public void Run()
        {
            try
            {
                if ( Setup() )
                {
                    // start the engines rendering loop
                    Root.StartRendering();
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
            if ( Root != null )
            {
                // remove event handlers
                Root.FrameStarted -= OnFrameStarted;
                Root.FrameEnded -= OnFrameEnded;

                //engine.Dispose();
            }
            SceneManager.RemoveAllCameras();
            SceneManager.RemoveCamera( Camera );
            Camera = null;
            Root.Instance.RenderSystem.DetachRenderTarget( Window );
            Window.Dispose();

            Root.Dispose();
        }

        #endregion Public Methods

        #region Event Handlers

        /// <summary>
        /// Event handler that is triggered once per frame after rendering is complete. Is configured
        /// to run by default is 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected virtual void OnFrameEnded( Object source, FrameEventArgs e )
        {
        }

        protected virtual void OnFrameStarted( Object source, FrameEventArgs e )
        {

            UpdateDebugOverlay( source, e );

            UpdateInput( source, e );

        }

        protected virtual void UpdateInput( Object source, FrameEventArgs e )
        {
            // TODO: Move this into an event queueing mechanism that is processed every frame
            Input.Capture();

            float scaleMove = 200 * e.TimeSinceLastFrame;

            // reset acceleration zero
            camAccel = Vector3.Zero;

            // set the scaling of camera motion
            cameraScale = 100 * e.TimeSinceLastFrame;


            if ( Input.IsKeyPressed( KeyCodes.Escape ) )
            {
                Root.Instance.QueueEndRendering();
                return;
            }

            if ( Input.IsKeyPressed( KeyCodes.A ) )
            {
                camAccel.x = -0.5f;
            }

            if ( Input.IsKeyPressed( KeyCodes.D ) )
            {
                camAccel.x = 0.5f;
            }

            if ( Input.IsKeyPressed( KeyCodes.W ) )
            {
                camAccel.z = -1.0f;
            }

            if ( Input.IsKeyPressed( KeyCodes.S ) )
            {
                camAccel.z = 1.0f;
            }

            //camAccel.y += (float)( input.RelativeMouseZ * 0.1f );


            // knock out the mouse stuff here
            isUsingKbCameraLook = false;
            if ( Input.IsKeyPressed( KeyCodes.Left ) )
            {
                Camera.Yaw( cameraScale );
                isUsingKbCameraLook = true;
            }

            if ( Input.IsKeyPressed( KeyCodes.Right ) )
            {
                Camera.Yaw( -cameraScale );
                isUsingKbCameraLook = true;
            }

            if ( Input.IsKeyPressed( KeyCodes.Up ) )
            {
                Camera.Pitch( cameraScale );
                isUsingKbCameraLook = true;
            }

            if ( Input.IsKeyPressed( KeyCodes.Down ) )
            {
                Camera.Pitch( -cameraScale );
                isUsingKbCameraLook = true;
            }

            // Mouse camera movement.
            if ( !isUsingKbCameraLook )
            {
                mouseRotateVector = Vector3.Zero;
                mouseRotateVector.x += Input.RelativeMouseX * 0.13f;
                mouseRotateVector.y += Input.RelativeMouseY * 0.13f;
                Camera.Yaw( -mouseRotateVector.x );
                Camera.Pitch( -mouseRotateVector.y );
            }
            isUsingKbCameraLook = false;

            // subtract the time since last frame to delay specific key presses
            toggleDelay -= e.TimeSinceLastFrame;

            // toggle rendering mode
            if ( Input.IsKeyPressed( KeyCodes.R ) && toggleDelay < 0 )
            {
                if ( Camera.PolygonMode == PolygonMode.Points )
                {
                    Camera.PolygonMode = PolygonMode.Solid;
                }
                else if ( Camera.PolygonMode == PolygonMode.Solid )
                {
                    Camera.PolygonMode = PolygonMode.Wireframe;
                }
                else
                {
                    Camera.PolygonMode = PolygonMode.Points;
                }

                Console.WriteLine( "Rendering mode changed to '{0}'.", Camera.PolygonMode );

                toggleDelay = 1;
            }

            if ( Input.IsKeyPressed( KeyCodes.T ) && toggleDelay < 0 )
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

            if ( Input.IsKeyPressed( KeyCodes.P ) )
            {
                string[] temp = Directory.GetFiles( Environment.CurrentDirectory, "screenshot*.jpg" );
                string fileName = string.Format( "screenshot{0}.jpg", temp.Length + 1 );

                TakeScreenshot( fileName );

                // show briefly on the screen
                debugText = string.Format( "Wrote screenshot '{0}'.", fileName );

                // show for 2 seconds
                debugTextDelay = 2.0f;
            }

            if ( Input.IsKeyPressed( KeyCodes.B ) )
            {
                SceneManager.ShowBoundingBoxes = !SceneManager.ShowBoundingBoxes;
            }

            if ( Input.IsKeyPressed( KeyCodes.F ) )
            {
                // hide all overlays, includes ones besides the debug overlay
                Viewport.ShowOverlays = !Viewport.ShowOverlays;
            }



            //if ( !input.IsMousePressed( MouseButtons.Left ) )
            //{
            //    float cameraYaw = -input.RelativeMouseX * .13f;
            //    float cameraPitch = -input.RelativeMouseY * .13f;

            //    camera.Yaw( cameraYaw );
            //    camera.Pitch( cameraPitch );
            //}
            //else
            //{
            //    cameraVector.x += input.RelativeMouseX * 0.13f;
            //}

            camVelocity += ( camAccel * scaleMove * camSpeed );

            // move the camera based on the accumulated movement vector
            Camera.MoveRelative( camVelocity * e.TimeSinceLastFrame );

            // Now dampen the Velocity - only if user is not accelerating
            if ( camAccel == Vector3.Zero )
            {
                camVelocity *= ( 1 - ( 6 * e.TimeSinceLastFrame ) );
            }



        }

        protected void UpdateDebugOverlay( object source, FrameEventArgs e )
        {
            OverlayElement element;

            // update performance stats once per second
            if ( statDelay < 0.0f && showDebugOverlay )
            {
                // TODO: Replace with CEGUI
                element = OverlayManager.Instance.Elements.GetElement( "Core/CurrFps" );
                element.Text = string.Format( "Current FPS: {0:#.00}", Root.Instance.CurrentFPS );

                element = OverlayManager.Instance.Elements.GetElement( "Core/BestFps" );
                element.Text = string.Format( "Best FPS: {0:#.00}", Root.Instance.BestFPS );

                element = OverlayManager.Instance.Elements.GetElement( "Core/WorstFps" );
                element.Text = string.Format( "Worst FPS: {0:#.00}", Root.Instance.WorstFPS );

                element = OverlayManager.Instance.Elements.GetElement( "Core/AverageFps" );
                element.Text = string.Format( "Average FPS: {0:#.00}", Root.Instance.AverageFPS );

                element = OverlayManager.Instance.Elements.GetElement( "Core/NumTris" );
                element.Text = string.Format( "Triangle Count: {0}", SceneManager.TargetRenderSystem.FacesRendered );

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

            element = OverlayManager.Instance.Elements.GetElement( "Core/DebugText" );
            element.Text = debugText;


        }

        #endregion Event Handlers
    }
}