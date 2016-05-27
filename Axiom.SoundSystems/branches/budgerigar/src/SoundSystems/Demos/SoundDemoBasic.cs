using Axiom.Animating;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.SoundSystems;
using Axiom.Demos;
using System;

namespace Axiom.SoundSystems.Demos
{
    /// <summary>
    /// Demonstrate 3D sound features using the available sound system implementations.
    /// </summary>
    [SoundDemo]
    public class SoundDemoBasic : TechDemo
    {
        #region Fields

        AnimationState _animationState;

        SceneNode _headNode;

        SoundContext _context;

        string _backSoundName = "background.wav";
        
        string _growlSoundName = "growl.ogg";

        #endregion

        #region CreateScene

        public override void CreateScene()
        {
            #region Graphics setup

            // set some ambient light
            scene.AmbientLight = new ColorEx(1.0f, 0.2f, 0.2f, 0.2f);

            // create a skydome
            scene.SetSkyDome(true, "Examples/CloudySky", 5, 8);

            // turn on some fog
            scene.SetFog(FogMode.Exp, ColorEx.White, 0.0002f);

            // create a simple default point light
            Light light = scene.CreateLight("MainLight");
            light.Position = new Vector3(20, 80, 50);

            // create a plane for the plane mesh
            Plane plane = new Plane();
            plane.Normal = Vector3.UnitY;
            plane.D = 200;

            // create a plane mesh
            MeshManager.Instance.CreatePlane(
                "FloorPlane", ResourceGroupManager.DefaultResourceGroupName,
                plane, 200000, 200000, 20, 20, true, 1, 50, 50, Vector3.UnitZ);

            // create an entity to reference this mesh
            Entity planeEntity = scene.CreateEntity("Floor", "FloorPlane");
            planeEntity.MaterialName = "Examples/RustySteel";
            scene.RootSceneNode.CreateChildSceneNode().AttachObject(planeEntity);

            // create an entity
            Entity ogreHead = scene.CreateEntity("OgreHead", "ogrehead.mesh");

            // create a scene node for the entity and attach the entity
            _headNode = scene.RootSceneNode.CreateChildSceneNode("OgreHeadNode", Vector3.Zero, Quaternion.Identity);
            _headNode.AttachObject(ogreHead);

            // create a scene node to attach the camera to
            SceneNode cameraNode = scene.RootSceneNode.CreateChildSceneNode("CameraNode");
            cameraNode.AttachObject(camera);

            // create new animation
            Animation animation = scene.CreateAnimation("Animation1", 10.0f);

            // nice smooth animation
            animation.InterpolationMode = InterpolationMode.Spline;

            // create the main animation track
            AnimationTrack track = animation.CreateNodeTrack(0, cameraNode);

            // create a few keyframes to move the camera around
            TransformKeyFrame frame;

            frame = (TransformKeyFrame)track.CreateKeyFrame(0.0f);

            frame = (TransformKeyFrame)track.CreateKeyFrame(2.5f);
            frame.Translate = new Vector3(500, 500, -1000);

            frame = (TransformKeyFrame)track.CreateKeyFrame(5.0f);
            frame.Translate = new Vector3(-1500, 1000, -600);

            frame = (TransformKeyFrame)track.CreateKeyFrame(7.5f);
            frame.Translate = new Vector3(0, -100, 0);

            frame = (TransformKeyFrame)track.CreateKeyFrame(10.0f);
            frame.Translate = Vector3.Zero;

            // create a new animation state to control the animation
            _animationState = scene.CreateAnimationState("Animation1");

            // enable the animation
            _animationState.IsEnabled = true;

            #endregion

            #region Sound setup

            // NOTE: use the OpenAL sound system implicitly, because the sounds could not be converted to xna's format correctly
            Console.WriteLine("Using Axiom.SoundSystems.OpenAL.OpenTK sound system.");
            
            // get a sound context
            _context = SoundsRoot.Instance.CreateSoundContext("Axiom.SoundSystems.OpenAL.OpenTK", window);

            // set a sound listener
            _context.Listener = _context.CreateCameraListener(camera);

            // init environment
            _context.DefaultSoundSettings.DistanceScale = 100f;

            // ambient sound
            Sound back = _context.LoadSound(_backSoundName, SoundKind.Simple);
            back.Loop = true;

            // 3D sound
            Sound growl = _context.LoadSound(_growlSoundName, SoundKind.Spatial);
            growl.Loop = true;

            // play them
            back.Play();
            growl.Play(_headNode);

            // register an frame event for replay update
            Axiom.Core.Root.Instance.FrameStarted += new EventHandler<FrameEventArgs>(_context.Update);

            #endregion
        }

        #endregion

        #region Event Handlers

        protected override void OnFrameStarted(object source, FrameEventArgs e)
        {

            // add time to the animation which is driven off of rendering time per frame
            _animationState.AddTime(e.TimeSinceLastFrame);

            base.OnFrameStarted(source, e);
        }

        #endregion
    }
}
