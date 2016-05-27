using Axiom.SoundSystems.Tools;
using System;
using System.Reflection;
using Axiom.Animating;
using Axiom.Controllers;
using Axiom.Core;
using Axiom.Demos;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.SoundSystems;
using Axiom.SoundSystems.Controllers;
using Axiom.SoundSystems.Effects;

namespace Axiom.SoundSystems.Demos
{
    /// <summary>
    /// Demonstrate 3D sound features using the available sound system implementations.
    /// </summary>
    [SoundDemo]    
    public class SoundDemoNature : Grass
    {
        #region Fields

        AnimationState _cameraAnimState;

        AnimationState _lightAnimState;
        
        SoundContext _context;

        /// <summary>
        /// This can be used by a demo launcher to pass-in a preferred sound system
        /// to use. The demo launcher shall utilize reflection for that.
        /// </summary>
        protected string SelectedSoundSystem = "Axiom.SoundSystems.Xna.Simple";

        #endregion 

        #region CreateScene

        public override void CreateScene()
        {
            #region Graphics setup

            // get the grass scene
            base.CreateScene();

            // set another skybox
            scene.SetSkyBox(true, "Skybox/Stormy", 5000f);

            // create a scene node to attach the camera to
            SceneNode cameraNode = scene.RootSceneNode.CreateChildSceneNode("CameraNode");
            cameraNode.AttachObject(camera);

            // create new animation
            Animation animation = scene.CreateAnimation("CameraAnimation", 15.0f);

            // nice smooth animation
            animation.InterpolationMode = InterpolationMode.Spline;

            // create the main animation track
            NodeAnimationTrack track = animation.CreateNodeTrack(0, cameraNode);

            // create a few keyframes to move the camera around
            TransformKeyFrame frame;

            frame = track.CreateNodeKeyFrame(0.0f);
            frame.Translate = new Vector3(0, 20, 30);

            frame = track.CreateNodeKeyFrame(3.0f);
            frame.Translate = new Vector3(80, 70, -60);

            frame = track.CreateNodeKeyFrame(6.0f);
            frame.Translate = new Vector3(60, 0, -100);

            frame = track.CreateNodeKeyFrame(9.0f);
            frame.Translate = new Vector3(-60, 40, -100);

            frame = track.CreateNodeKeyFrame(12.0f);
            frame.Translate = new Vector3(-150, -0, -60);

            frame = track.CreateNodeKeyFrame(15.0f);
            frame.Translate = new Vector3(0, 20, 30);

            // create animation state to control the animation
            _cameraAnimState = scene.CreateAnimationState("CameraAnimation");
            _cameraAnimState.IsEnabled = true;

            // create animation of the light source, discard that one from base class
            // as we want the later attached sound demonstrate stereo effect some better
            AnimState.Parent.AllAnimationStates.Remove("LightTrack");
            scene.DestroyAnimation("LightTrack");

            Animation anim = scene.CreateAnimation("LightTrack", 14);
            
            // spline it for nice curves
            anim.InterpolationMode = InterpolationMode.Spline;
            
            // create a track to animate the camera's node
            NodeAnimationTrack nodeTrack = anim.CreateNodeTrack(0, LightNode);
            
            // setup keyframes
            TransformKeyFrame key = nodeTrack.CreateNodeKeyFrame(0);
            key.Translate = new Vector3(0, 550, -300);

            key = nodeTrack.CreateNodeKeyFrame(2);
            key.Translate = new Vector3(-250, 450, -200);

            key = nodeTrack.CreateNodeKeyFrame(4);
            key.Translate = new Vector3(-300, 450, 0);

            key = nodeTrack.CreateNodeKeyFrame(6);
            key.Translate = new Vector3(-150, 350, 600);

            key = nodeTrack.CreateNodeKeyFrame(8);
            key.Translate = new Vector3(250, 340, 180);

            key = nodeTrack.CreateNodeKeyFrame(10);
            key.Translate = new Vector3(300, 450, -200);

            key = nodeTrack.CreateNodeKeyFrame(12);
            key.Translate = new Vector3(300, 500, -400);

            key = nodeTrack.CreateNodeKeyFrame(14);
            key.Translate = new Vector3(0, 550, -300);

            // create a new animation state to track this
            _lightAnimState = scene.CreateAnimationState("LightTrack");
            _lightAnimState.IsEnabled = true;

            // tweak
            HeadNode.Translate(Vector3.UnitY * 100);
            camera.LookAt(HeadNode.DerivedPosition + Vector3.UnitY * 30);

            #endregion

            #region Sound setup

            // get a sound context
            _context = SoundsRoot.Instance.CreateSoundContext(SelectedSoundSystem, camera, window);

            // init environment
            _context.DefaultSoundSettings.DistanceScale = 200f;

            // load sounds
            Sound back1 = _context.LoadSound("34112__dobroide__20070418_nightingale.mp3", SoundKind.Simple);
            back1.Volume = 0.6f;
            back1.Loop = true;

            Sound back2 = _context.LoadSound("7168__ingeos___Vent_Wind_87_Bellac_France.ogg", SoundKind.Simple);
            back2.Volume = 0.4f;
            back2.Loop = true;

            Sound music = _context.LoadSound("18972__bebeto__Loop002_ambient.wav", SoundKind.Simple);
            music.Volume = 0.5f;
            music.Loop = true;

            // 3D sounds
            Sound buzz = _context.LoadSound("buzz_loop.wav", SoundKind.Spatial);
            buzz.Loop = true;

            Sound head = _context.LoadSound("21732__Anton__fast_8bar_key_d1.wav", SoundKind.Spatial);
            head.Loop = true;

            // setup fade-in effects (no master volume yet supported)
            // increase volume by the given value per second
            LinearAttenuationFunction fadeInfunc = new LinearAttenuationFunction(0.2f);
            back1.Effects.Add(new VolumeEffect(0, back1.Volume, fadeInfunc, EffectKind.Once));
            back2.Effects.Add(new VolumeEffect(0, back2.Volume, fadeInfunc, EffectKind.Once));
            music.Effects.Add(new VolumeEffect(0, music.Volume, fadeInfunc, EffectKind.Once));
            buzz.Effects.Add(new VolumeEffect(0, buzz.Volume, fadeInfunc, EffectKind.Once));
            head.Effects.Add(new VolumeEffect(0, head.Volume, fadeInfunc, EffectKind.PingPong));

            // play them
            back1.Play();
            back2.Play();
            music.Play();

            buzz.Play(LightNode);
            head.Play(HeadNode);

            // register a frame event handler for replay update
            Axiom.Core.Root.Instance.FrameStarted += new EventHandler<FrameEventArgs>(_context.Update);

            #endregion
        }

        #endregion
       
        #region Event Handlers

        protected override void OnFrameStarted(object source, FrameEventArgs e)
        {
            // add time to the animations
            _cameraAnimState.AddTime(e.TimeSinceLastFrame);
            _lightAnimState.AddTime(e.TimeSinceLastFrame);

            base.OnFrameStarted(source, e);        
        }

        #endregion
    }
}
