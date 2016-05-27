using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Overlays;
using Axiom.Overlays.Elements;
using SIS = SharpInputSystem;

namespace Demo.SkyX
{
    class Game : Application
    {
        #region Fields and Properties

        private TextArea _textArea;
        private Axiom.SkyX.SkyX _skyX;


        private bool _showInformation = false;
        private float _keyDelay = 1.0f;

        #endregion Fields and Properties

        #region Implementation of Application
        /// <summary>
        /// Overridable method for scene creation
        /// </summary>
        protected override void CreateScene()
        {
            input = new Axiom.SkyX.InputManager(window, viewport);
            // Set some camera params
            camera.Far = 30000;
            camera.Near =  20;
            camera.Position = new Vector3(20000, 500, 20000);
            camera.Direction= new Vector3( 1, 0, 0 );
            //camera.PolygonMode = PolygonMode.Wireframe;
            // Create our text area for display SkyX parameters
            _createTextArea();

            // Create SkyX
            _skyX = new Axiom.SkyX.SkyX( scene, camera );
             _skyX.Create();
            _skyX.VCloudsManager.Create();

            ////// A little change to default atmosphere settings :)
            Axiom.SkyX.AtmosphereManager.AtmosphereOptions opt = (Axiom.SkyX.AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();
            opt.RayleighMultiplier = 0.0045f;
            _skyX.AtmosphereManager.Options = opt;
            // Add our ground atmospheric scattering pass to terrain material
            //var terrainMaterial = (Material)MaterialManager.Instance.GetByName("Terrain");
            //if (terrainMaterial != null)
            //{
            //    var pass = terrainMaterial.GetTechnique(0).CreatePass();
            //    _skyX.GpuManager.AddGroundPass(pass, 5000, SceneBlendType.TransparentColor);
            //}
            ////// Create our terrain
            scene.SetWorldGeometry("Terrain.xml");

            //// Add a basic cloud layer
            _skyX.CloudsManager.Add(new Axiom.SkyX.CloudLayer.Options(/* Default options */));
        }

        protected override void OnFrameStarted( object source, FrameEventArgs e )
        {
            base.OnFrameStarted( source, e );
           // var 
            var skyXOptions = (Axiom.SkyX.AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();
            // Time
            _skyX.TimeMultiplier = !_showInformation ? 0.1f : 0.0f;
            if (_skyX.TimeMultiplier != 0)
            {

            }
		    // Show/Hide information
		    if (inputKeyboard.IsKeyDown(SIS.KeyCode.Key_F1) && _keyDelay < 0)
            {
			    _showInformation = !_showInformation;

			    _keyDelay = 0.25f;
		    }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_1) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                _skyX.TimeMultiplier = 1.0f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_1) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                _skyX.TimeMultiplier = -1.0f;
            }
            // Rayleigh multiplier
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_2) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                
                skyXOptions.RayleighMultiplier += e.TimeSinceLastFrame * 0.025f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_2) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.RayleighMultiplier -= e.TimeSinceLastFrame * 0.025f;
            }

            // Mie multiplier
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_3) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.MieMultiplier += e.TimeSinceLastFrame * 0.025f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_3) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.MieMultiplier -= e.TimeSinceLastFrame * 0.025f;
            }
            // Exposure
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_4) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.Exposure += e.TimeSinceLastFrame * 0.5f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_4) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.Exposure -= e.TimeSinceLastFrame * 0.5f;
            }
            // Inner radius
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_5) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.InnerRadius += e.TimeSinceLastFrame * 0.25f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_5) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.InnerRadius -= e.TimeSinceLastFrame * 0.25f;
            }
            // Outer radius
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_6) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.OuterRadius += e.TimeSinceLastFrame * 0.25f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_6) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.OuterRadius -= e.TimeSinceLastFrame * 0.25f;
            }
            // Number of samples
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_7) && _keyDelay < 0 && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.NumberOfSamples++;
                _keyDelay = 0.25f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_7) && _keyDelay < 0 && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.NumberOfSamples--;
                _keyDelay = 0.25f;
            }
            // Outer radius
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_8) && !(inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.HeightPosition += e.TimeSinceLastFrame * 0.05f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_8) && (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_LSHIFT) || inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_RSHIFT)))
            {
                skyXOptions.HeightPosition -= e.TimeSinceLastFrame * 0.05f;
            }
            if (inputKeyboard.IsKeyDown(SharpInputSystem.KeyCode.Key_SPACE) && _keyDelay < 0)
            {
                camera.Position = new Vector3(0, 900, 0);
                _keyDelay = 0.25f;
            }
           // 

           // _skyX.VCloudsManager.WindSpeed = _skyX.VCloudsManager.WindSpeed + 100.0f * e.TimeSinceLastFrame;
            
		    _textArea.Text = getConfigStringFromSkyXAtmosphereOptions(skyXOptions);
            _skyX.AtmosphereManager.Options = skyXOptions;
            //_skyX.AtmosphereManager.Update(skyXOptions, true);
            _skyX.Update( e.TimeSinceLastFrame );
            //_skyX.VCloudsManager.Update(e.TimeSinceLastFrame);
            _keyDelay -= e.TimeSinceLastFrame;

		    // Update terrain material
            var terrainMaterial = (Material)MaterialManager.Instance.GetByName( "Terrain" );
            if ( terrainMaterial != null)
                terrainMaterial.GetTechnique(0).GetPass(0).FragmentProgramParameters.SetNamedConstant("uLightY", -_skyX.AtmosphereManager.SunDirection.y);
            debugText = this.camera.DerivedPosition.ToString();
        }

        //protected override void ChooseSceneManager()
        //{
        //    scene = engine.CreateSceneManager( "TerrainSceneManager", "SkyXTerrainSceneManager" );
        //}

        #endregion Implementation of Application

        private string getConfigStringFromSkyXAtmosphereOptions(Axiom.SkyX.AtmosphereManager.AtmosphereOptions Options)
        {
            var hour = Options.Time.x ;
            var min = (int)( (Options.Time.x - hour) * 60);

	        var timeStr = hour + ":" + min;
	        var str = "Axiom SkyX Plugin demo (Press F1 to show/hide information)" + ( _showInformation ? " - Simuation paused - \n" : "\n-------------------------------------------------------------\nTime: " + timeStr + "\n" );

	        if (_showInformation)
	        {
                str += "-------------------------------------------------------------\n";
                str += "Time: " + timeStr + " [1, Shift+1] (+/-).\n";
                str += "Rayleigh multiplier: " +  Options.RayleighMultiplier + " [2, Shift+2] (+/-).\n";
                str += "Mie multiplier: " +  Options.MieMultiplier +" [3, Shift+3] (+/-).\n";
                str += "Exposure: " +  Options.Exposure  + " [4, Shift+4] (+/-).\n";
                str += "Inner radius: " +  Options.InnerRadius  + " [5, Shift+5] (+/-).\n";
                str += "Outer radius: " +  Options.OuterRadius + " [6, Shift+6] (+/-).\n";
                str += "Number of samples: " +  Options.NumberOfSamples + " [7, Shift+7] (+/-).\n";
                str += "Height position: " +  Options.HeightPosition + " [8, Shift+8] (+/-).\n";

                if (_skyX.VCloudsManager.IsCreated)
                {
                    str += "Wind direction: " + _skyX.VCloudsManager.VClouds.WindDirection.InDegrees.ToString() +" degrees [0, Shift+0] (+/-).\n";
                    str += "Clouds roughness: " + _skyX.VCloudsManager.VClouds.NoiseScale + " [p, Shift+p] (+/-).\n";
                }
	        }

	        return str;
        }

        // Create text area for SkyX parameters
        private void _createTextArea()
	    {
		    // Create a panel
            OverlayElementContainer panel = (OverlayElementContainer)OverlayManager.Instance.Elements.CreateElement("Panel", "SkyXParametersPanel");
            panel.MetricsMode = MetricsMode.Pixels;
		    panel.SetPosition(10, 10);
		    panel.SetDimensions(400, 400);

		    // Create a text area
            _textArea = (TextArea)OverlayManager.Instance.Elements.CreateElement("TextArea", "SkyXParametersTextArea");
		    _textArea.MetricsMode = MetricsMode.Pixels;
		    _textArea.SetPosition(0, 0);
		    _textArea.SetDimensions(100, 100);
		    _textArea.Text = "Axiom SkyX plugin demo";
		    _textArea.CharHeight = 16;
		    _textArea.FontName = "BlueHighway";
		    _textArea.ColorBottom = new ColorEx(0.3f, 0.5f, 0.3f);
		    _textArea.ColorTop= new ColorEx(0.5f, 0.7f, 0.5f);

		    // Create an overlay, and add the panel
		    Overlay overlay = OverlayManager.Instance.Create("OverlayName");
		    overlay.AddElement(panel);

		    // Add the text area to the panel
		    panel.AddChild(_textArea);

		    // Show the overlay
		    overlay.Show();
	    }

    }
}