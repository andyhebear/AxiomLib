using Axiom.SoundSystems.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Axiom.Animating;
using Axiom.Core;
using Axiom.Demos;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.ParticleSystems;
using Axiom.SoundSystems.Controllers;
using Axiom.SoundSystems.Effects;

namespace Axiom.SoundSystems.Demos
{
    /// <summary>
    /// Demonstrate Doppler effect feature
    /// </summary>
    [SoundDemo]
    public class SoundDemoDoppler : TechDemo
    {
        #region Const

        /// <summary>
        /// Number of cars in the scene
        /// </summary>
        public const int cpuCarCount = 2;

        /// <summary>
        /// The relative speed of the cars (1 == finish track in one sec)
        /// </summary>
        public const float cpuCarSpeed = 0.20f;

        #endregion
      
        #region Nested

        /// <summary>
        /// A spline that interpolates positions, scales and orientations
        /// </summary>
        public class Spline //: Spline<Matrix4>
        {
            public Spline()
            {
                Positions = new PositionalSpline();
                Positions.AutoCalculate = true;
                Scales = new PositionalSpline();
                Scales.AutoCalculate = true;
                Rotations = new RotationalSpline();
                Rotations.AutoCalculate = true;
            }

            public PositionalSpline Positions { get; protected set; }

            public PositionalSpline Scales { get; protected set; }

            public RotationalSpline Rotations { get; protected set; }

            public void AddPoint(Matrix4 point)
            {
                Vector3 p, s;
                Quaternion r;
                point.Decompose(out p, out s, out r);

                Positions.AddPoint(p);
                Scales.AddPoint(s);
                Rotations.AddPoint(r);
            }

            public void AddPoint(Vector3 pos, Vector3 sca, Quaternion rot)
            {
                Positions.AddPoint(pos);
                Scales.AddPoint(sca);
                Rotations.AddPoint(rot);
            }

            public Matrix4 Interpolate(float t)
            {
                Vector3 p, s;
                Quaternion r;
                p = Positions.Interpolate(t);
                s = Scales.Interpolate(t);
                r = Rotations.Interpolate(t, false);

                return Matrix4.Compose(p, s, r);
            }

            public void MakeClosed()
            {
                Positions.AddPoint(Positions.GetPoint(0));
                Scales.AddPoint(Scales.GetPoint(0));
                Rotations.AddPoint(Rotations.GetPoint(0));
            }
        }

        /// <summary>
        /// Very simple car definition and factory.
        /// </summary>
        public class CarFactory
        {
            #region Fields

            string bodyMesh;
            string frontTireMesh;
            string backTireMesh;
            Vector3 bodyPosition;
            Vector3 bodyScale;
            Quaternion bodyOrientation;
            Vector3 chassisPosition;
            Vector2 chassisSize;

            #endregion

            #region Constructor

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="bodyMesh">Name of the car body mesh</param>
            /// <param name="frontTireMesh">Name of the front tire mesh</param>
            /// <param name="backTireMesh">Name of the back tire mesh</param>
            /// <param name="bodyPosition">Position of the car body</param>
            /// <param name="bodyOrientation">Rotation of the car body</param>
            /// <param name="bodyScale">Scale of the car body</param>
            /// <param name="chassisPosition">Chassis position relative to body</param>
            /// <param name="chassisSize">Chassis size relative to body</param>
            public CarFactory(string bodyMesh,
                string frontTireMesh,
                string backTireMesh,
                Vector3 bodyPosition,
                Quaternion bodyOrientation,
                Vector3 bodyScale,
                Vector3 chassisPosition,
                Vector2 chassisSize
                )
            {
                this.bodyMesh = bodyMesh;
                this.frontTireMesh = frontTireMesh;
                this.backTireMesh = backTireMesh;
                this.chassisSize = chassisSize;
                this.chassisPosition = chassisPosition;
                this.bodyPosition = bodyPosition;
                this.bodyScale = bodyScale;
                this.bodyOrientation = bodyOrientation;
            }

            #endregion

            #region Factory methods

            /// <summary>
            /// Creates a new instance of <see cref="Car"/> from the car definition
            /// </summary>
            /// <param name="name"></param>
            /// <param name="scene"></param>
            /// <returns></returns>
            public Car CreateInstance(string name, SceneManager scene)
            {
                // create chasis and tire entities
                Entity body = scene.CreateEntity(name + "-chasis", bodyMesh);

                Entity[] tires = new Entity[4];
                tires[0] = scene.CreateEntity(name + "-tire-" + Car.TireKind.FrontRight, frontTireMesh);
                tires[1] = scene.CreateEntity(name + "-tire-" + Car.TireKind.FrontLeft, frontTireMesh);
                tires[2] = scene.CreateEntity(name + "-tire-" + Car.TireKind.BackRight, backTireMesh);
                tires[3] = scene.CreateEntity(name + "-tire-" + Car.TireKind.BackLeft, backTireMesh);

                // build scene nodes
                SceneNode bodyNode = scene.CreateSceneNode();
                bodyNode.AttachObject(body);

                SceneNode[] tireNodes = new SceneNode[4];
                for (int i = 0; i < 4; i++)
                {
                    tireNodes[i] = scene.CreateSceneNode();
                    tireNodes[i].AttachObject(tires[i]);

                    // attach tire to body
                    bodyNode.AddChild(tireNodes[i]);
                }

                // position body
                bodyNode.Translate(bodyPosition);
                bodyNode.Rotate(bodyOrientation);
                bodyNode.Scale = bodyScale;

                // position chassis
                Vector2 half = chassisSize * 0.5f;
                Quaternion q180 = Quaternion.FromAngleAxis(Utility.DegreesToRadians(180), Vector3.UnitY);
                // fr
                tireNodes[0].Position = new Vector3(half.y, 0, half.x) + chassisPosition;
                tireNodes[0].Orientation = Quaternion.Identity;
                // fl
                tireNodes[1].Position = new Vector3(half.y, 0, -half.x) + chassisPosition;
                tireNodes[1].Orientation = q180;
                // br
                tireNodes[2].Position = new Vector3(-half.y, 0, half.x) + chassisPosition;
                tireNodes[2].Orientation = Quaternion.Identity;
                // bl
                tireNodes[3].Position = new Vector3(-half.y, 0, -half.x) + chassisPosition;
                tireNodes[3].Orientation = q180;

                return new Car(body, tires);
            }

            #endregion
        }

        /// <summary>
        /// Very simple car object
        /// </summary>
        public class Car
        {
            public enum TireKind { FrontRight = 0, FrontLeft, BackRight, BackLeft }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="body"></param>
            /// <param name="tires"></param>
            public Car(MovableObject body, MovableObject[] tires)
            {
                this.Body = body;
                this.BodyNode = body.ParentSceneNode;

                this.Tires = new MovableObject[4];
                this.TireNodes = new SceneNode[4];

                for (int i = 0; i < 4; i++)
                {
                    this.Tires[i] = tires[i];
                    this.TireNodes[i] = tires[i].ParentSceneNode;
                }
            }

            /// <summary>
            /// Get the scene node the car is attached to. Valid after <see cref="AttachToScene"/>
            /// </summary>
            public SceneNode ParentSceneNode { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public SceneNode[] TireNodes { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public SceneNode BodyNode { get; private set; }
            
            /// <summary>
            /// 
            /// </summary>
            public MovableObject Body { get; private set; }

            /// <summary>
            /// 
            /// </summary>
            public MovableObject[] Tires { get; private set; }
            
            /// <summary>
            /// 
            /// </summary>
            /// <param name="which"></param>
            /// <returns></returns>
            public MovableObject GetTire(TireKind which)
            {
                return Tires[(int)which];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="which"></param>
            /// <returns></returns>
            public SceneNode GetTireNode(TireKind which)
            {
            	return TireNodes[(int)which];
            }
            
            /// <summary>
            /// Re-attach the car to a scene node
            /// </summary>
            /// <param name="node"></param>
            public void AttachToScene(SceneNode node)
            {
                if (BodyNode.Parent != null)
                    BodyNode.RemoveFromParent();

                node.AddChild(BodyNode);
                ParentSceneNode = node;
            }
        }

        #endregion Nested
        
        #region Fields

        /// <summary>
        /// This can be used by a demo launcher to pass-in a preferred sound system
        /// to use. The demo launcher shall utilize reflection for that.
        /// </summary>
        protected string SelectedSoundSystem = "Axiom.SoundSystems.Xna.Simple";

        protected Spline track;
        protected Car[] cpuCars;
        protected float[] cpuCarTimes;
        protected SoundInstance[] cpuCarSounds;
        protected SceneNode roadNode;
        protected Sound carEngineSound;
        protected SoundContext soundContext;
        protected Light light;

        #endregion

        #region Scene creation

        public override void CreateScene()
        {
            #region Graphics setup

            // environment
            scene.AmbientLight = ColorEx.White;
            scene.SetFog(FogMode.Exp, ColorEx.Indigo, 0.0015f);
            scene.SetSkyBox(true, "Skybox/Stormy", 1000);

            // light
            light = scene.CreateLight("main");
            light.Type = LightType.Point;
            light.Diffuse = ColorEx.White;
            light.SetAttenuation(2000, 0.05f, 0.0003f, 0.0f);

            // ground
            Plane plane = new Plane(Vector3.UnitY, 0);
            MeshManager.Instance.CreatePlane("groundMesh1", ResourceGroupManager.DefaultResourceGroupName, plane, 20000, 20000, 20, 20, true, 1, 50, 50, Vector3.UnitZ);
            Entity ground = scene.CreateEntity("ground1", "groundMesh1");
            ground.MaterialName = "Examples/RustySteel";

            // rain
            ParticleSystem rain = ParticleSystemManager.Instance.CreateSystem("rain1", "ParticleSystems/Rain");
            rain.ParticleQuota = 1000;
            rain.SpeedFactor = 8.0f;

            // get a spline for road (do circle)
            track = new Spline();
            float radius = 3f;
            for (float f = 0; f < Utility.PI * 2; f += Utility.PI * 0.1f)
            {
                MakeCircularTrackPoint(track, radius, f);
            }
            // close track
            MakeCircularTrackPoint(track, radius, Utility.PI * 2);

            // build road mesh
            Material m = CreateRoadMaterial("RoadSurface1", "road.png");
            ManualObject road = BuildRoad("road1", m.Name, track, 0.01f, new Vector2(1.0f, 0.1f));
            Mesh roadMesh = road.ConvertToMesh("road1", ResourceGroupManager.DefaultResourceGroupName);
            Entity roadEntity = scene.CreateEntity("road1", roadMesh);

            // create cars
            CarFactory carTemplate = new CarFactory(
                "bugatti1_body.mesh",
                "bugatti1_tireRF.mesh",
                "bugatti1_tireRB.mesh",
                new Vector3(0, 3.67f, 0), // default position (to be over surface)
                Quaternion.FromAngleAxis(Utility.DegreesToRadians(-90), Vector3.UnitY), // default mesh orientation
                Vector3.UnitScale * 10, // default scale
                new Vector3(-0.17f, -0.12f, 0), // chassis position (body relative)
                new Vector2(0.866f, 1.514f) // chassis size (w/d, body relative)
                );

            cpuCars = new Car[cpuCarCount];

            for (int i = 0; i < cpuCars.Length; i++)
            {
                cpuCars[i] = carTemplate.CreateInstance("cpu" + i, scene);
            }

            // scene graph
            scene.RootSceneNode.CreateChildSceneNode().AttachObject(ground);
            ground.ParentNode.Translate(Vector3.NegativeUnitY * 10); // avoid interference with road mesh

            scene.RootSceneNode.CreateChildSceneNode().AttachObject(rain);
            rain.ParentNode.Translate(Vector3.UnitY * 1500);
            rain.ParentNode.Scale = new Vector3(3, 1, 3);

            roadNode = scene.RootSceneNode.CreateChildSceneNode();
            roadNode.AttachObject(roadEntity);
            roadNode.Scale = Vector3.UnitScale * 100;

            // attach cars to scene, position them and init car animation times
            SceneNode[] cpuCarNodes = new SceneNode[cpuCars.Length];
            cpuCarTimes = new float[cpuCars.Length];

            for (int i = 0; i < cpuCars.Length; i++)
            {
                cpuCarNodes[i] = scene.RootSceneNode.CreateChildSceneNode();
                cpuCarTimes[i] = (float)(i + 2) / (float)(cpuCars.Length + 2);
                
                Matrix4 m44 = track.Interpolate(cpuCarTimes[i]);

                cpuCarNodes[i].Position = m44.Translation;
                cpuCarNodes[i].Orientation = Quaternion.FromRotationMatrix(m44.ExtractRotation());
                cpuCars[i].AttachToScene(cpuCarNodes[i]);
            }

            // create and place player on road side
            camera.Position = new Vector3(0, roadNode.Scale.y * 0.14f, -roadNode.Scale.z * 3.20f);
            camera.LookAt(new Vector3(-roadNode.Scale.x * 4f, 0, -roadNode.Scale.z));
            camera.Near = 1;

            #endregion

            #region Sound setup

            // TODO: fix OpenAL function.
            // As to OpenAL docs, the prerequisities for a doppler effect on a source seem to be:
            //
            // 1. a spatial listener. Global distance model for it is set by default, can also be set in Axiom.SoundSystems.OpenAL.SoundContext
            // constructor through Al.alDistanceModel(). Spatial parameters are passed to the OpenAL listener through the hereby used CameraListener 
            // in Axiom.SoundSystems.OpenAL.SoundContext.Update() and Axiom.SoundSystems.OpenAL.CameraListener.Velocity)
            //
            // 2. a spatial sound source. Means to set a distance model for it, see Axiom.SoundSystems.OpenAL.SoundInstance constructor,
            //   other parameters are passed to OpenAL through the SoundInstance analogously to how listener is updated through CameraListener)
            //
            // 3. aside the spatial relation of a listener and emitter (source),
            // the doppler effect is influenced by speed of sound and doppler scale (and perhaps distance scale).
            // Default OpenAL values are reasonable, but are anyway overriden same reasonably
            // in the Axiom.SoundSystems.OpenAL.SoundInstance.SpeedOfSound (AL_SPEED_OF_SOUND),
            // .DopplerScale (AL_DOPPLER_FACTOR), .DistanceScale (AL_ROLOFF_FACTOR) properties.
            //
            // 4. A mono waveform!! Stereo samples are not spatially processed at all.
            //
            // That's how I understood OpenAL, everything looks like set up correctly, 
            // volume of the car engine fades with distance, but there's no doppler effect, whoat :(
            //
            if (SelectedSoundSystem.Contains("OpenAL"))
            {
            	Console.WriteLine("\nWARNING: you won't hear a doppler effect with the OpenAL subsystem yet");
            	Console.Write("Press a key to continue...");
            	Console.ReadKey();
            }
            
            // create a sound context. The PathString param is used only for Xna, if selected
            soundContext = SoundsRoot.Instance.CreateSoundContext(SelectedSoundSystem, camera, window);
            soundContext.DefaultSoundSettings.DistanceScale = roadNode.Scale.Length * 0.5f;
            soundContext.DefaultSoundSettings.DopplerScale = 12f;

            // car engine sound
            carEngineSound = soundContext.LoadSound("18771__djbono__Ferrari_F456-loop2.wav", SoundKind.Spatial);
            carEngineSound.Pitch = 0.1f; // higher rpm
            carEngineSound.Loop = true;
      
            // rain
            Sound raining = soundContext.LoadSound("2521__RHumphries__rbh_rain_03-loop.wav", SoundKind.Simple);
            raining.Loop = true;
            raining.Volume = 0.6f;

            // background tune
            Sound tune = soundContext.LoadSound("38516__LS__little_girls_125_hot.wav", SoundKind.Simple);
            tune.Loop = true;

            // play the loaded sounds
            tune.Play();
            raining.Play();

            cpuCarSounds = new SoundInstance[cpuCars.Length];
            for (int i = 0; i < cpuCars.Length; i++)
            {
                cpuCarSounds[i] = carEngineSound.Play(cpuCars[i].ParentSceneNode);
                cpuCarSounds[i].Velocity = GetCarVelocity(cpuCars[i].ParentSceneNode.DerivedOrientation);
            }

            #endregion
        }

        private void MakeCircularTrackPoint(Spline track, float radius, float angle)
        {
            // do circle
            float x = radius * Utility.Cos(angle);
            float y = 0;
            float z = radius * Utility.Sin(angle);

            Vector3 pos = new Vector3(x, y, z);
            Quaternion ori = Quaternion.FromEulerAngles(0, -angle, 0.2f);

            track.AddPoint(pos, Vector3.UnitScale, ori);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="textureName"></param>
        /// <returns></returns>
        Material CreateRoadMaterial(string name, string textureName)
        {
            Material m = (Material)MaterialManager.Instance.Create(name, ResourceGroupManager.DefaultResourceGroupName);
            m.Load();
            m.CullingMode = CullingMode.None;
            Pass p = m.GetBestTechnique().GetPass(0);
            p.CreateTextureUnitState(textureName);

            return m;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="roadName"></param>
        /// <param name="materialName"></param>
        /// <param name="spline"></param>
        /// <param name="step"></param>
        /// <param name="texuvStep"></param>
        /// <returns></returns>
        ManualObject BuildRoad(string roadName, string materialName, Spline spline, float step, Vector2 texuvStep)
        {
            ManualObject road = new ManualObject(roadName);
            road.Begin(materialName, OperationType.TriangleList);

            // build vertices
            float texu = 0, texv = 0;
            int segmentCount = 0;
            int segmentVertexCount = 2;

            for (float t = 0; t < 1.0; t += step)
            {
                Vector3 point = spline.Positions.Interpolate(t);
                Quaternion rot = spline.Rotations.Interpolate(t, false);
                Vector3 xAxis = rot.XAxis;
                Vector3 yAxis = rot.YAxis;

                // next road segment
                Vector3[] surface = new Vector3[segmentVertexCount];
                surface[0] = point - xAxis * 0.5f;
                surface[1] = point + xAxis * 0.5f;

                texu = 0;
                for (int i = 0; i < surface.Length; i++)
                {
                    road.Position(surface[i]);
                    road.Normal(yAxis);
                    road.TextureCoord(texu, texv);

                    texu += texuvStep.x;
                }

                texv += texuvStep.y;
                segmentCount++;
            }

            // build indices
            int istart0 = 0;
            int istart1 = segmentVertexCount;

            for (int i = 0; i < segmentCount - 1; i++)
            {
                for (int j = 0; j < segmentVertexCount - 1; j++)
                {
                    int i1, i2, i3;

                    i1 = istart0;
                    i2 = istart0 + 1;
                    i3 = istart1;
                    road.Triangle((ushort)i1, (ushort)i2, (ushort)i3);

                    i1 = istart1;
                    i2 = istart0 + 1;
                    i3 = istart1 + 1;
                    road.Triangle((ushort)i1, (ushort)i2, (ushort)i3);

                    istart0++;
                    istart1++;
                }

                istart0++;
                istart1++;
            }

            road.End();

            return road;
        }

        #endregion

        #region Frame event handler

        protected override void OnFrameStarted(object source, FrameEventArgs e)
        {
            base.OnFrameStarted(source, e);

            if (e.StopRendering)
                return;

            // the TechDemo class updates camera transforms upon user input, update the light
            light.Position = camera.Position;

            // animate cpu cars
            for (int i = 0; i < cpuCars.Length; i++)
            {
                cpuCarTimes[i] += e.TimeSinceLastFrame * cpuCarSpeed;
                cpuCarTimes[i] %= 1.0f;

                Matrix4 m44 = track.Interpolate(cpuCarTimes[i]);

                Quaternion ori = Quaternion.FromRotationMatrix(m44.ExtractRotation());
                cpuCars[i].ParentSceneNode.Position = m44.Translation * roadNode.Scale;
                cpuCars[i].ParentSceneNode.Orientation = ori;
                
                float wheelRoll = 720f * e.TimeSinceLastFrame; // not any realistic however
                cpuCars[i].GetTireNode(Car.TireKind.FrontRight).Roll(-wheelRoll);
                cpuCars[i].GetTireNode(Car.TireKind.BackRight).Roll(-wheelRoll);
                cpuCars[i].GetTireNode(Car.TireKind.FrontLeft).Roll(wheelRoll);
                cpuCars[i].GetTireNode(Car.TireKind.BackLeft).Roll(wheelRoll);
                
                // update sound
                cpuCarSounds[i].Velocity = GetCarVelocity(ori);
            }

            soundContext.Update(source, e);
        }

        private Vector3 GetCarVelocity(Quaternion ori)
        {
            return cpuCarSpeed * ori.ZAxis;
        }

        #endregion
     }
}
