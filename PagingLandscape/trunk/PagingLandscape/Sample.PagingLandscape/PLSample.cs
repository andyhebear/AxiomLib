using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;

namespace Axiom.Samples.PagingLandscape
{
	public class PLSample : SdkSample
	{
		protected SinbadCharacterController chara;

		SceneNode waterNode;
		float flowAmount;
		bool flowUp = true;
		const float FLOW_HEIGHT = 0.8f;
		const float FLOW_SPEED = 0.2f;
		RaySceneQuery raySceneQuery = null;

		// move the Camera like a human at 3m/sec
		bool humanSpeed = true;

		// keep Camera 2m above the ground
		bool followTerrain = true;

		/// <summary>
		/// 
		/// </summary>
		public PLSample()
		{
			Metadata[ "Title" ] = "Landscape";
			Metadata[ "Description" ] = "An example demonstrating huge landscape using paging algorithms.";
			Metadata[ "Thumbnail" ] = "thumb_terrain.png";
			Metadata[ "Category" ] = "Environment";
		}

		protected override void CreateSceneManager()
		{
			SceneManager = Root.CreateSceneManager( SceneType.ExteriorFar, "TechDemoSMInstance" );
			SceneManager.ClearScene();
		}

		protected override void LocateResources()
		{
			base.LocateResources();
			ResourceGroupManager.Instance.CreateResourceGroup( "TerrainMedia" );
			ResourceGroupManager.Instance.AddResourceLocation( @"../../Media/PagingLandscape", "Folder", "TerrainMedia" );
			ResourceGroupManager.Instance.AddResourceLocation( @"../../Media/PagingLandscape/ps_height_1k", "Folder", "TerrainMedia" );
			ResourceGroupManager.Instance.LoadResourceGroup( "TerrainMedia" );
		}

		protected override void UnloadResources()
		{
			// unload the map so we don't interfere with subsequent samples
			ResourceGroupManager.Instance.UnloadResourceGroup( "TerrainMedia" );
			ResourceGroupManager.Instance.RemoveResourceLocation( @"../../Media/PagingLandscape", "TerrainMedia" );
			ResourceGroupManager.Instance.RemoveResourceLocation( @"../../Media/PagingLandscape/ps_height_1k", "TerrainMedia" );
		}

		/// <summary>
		/// <p
		/// </summary>
		protected override void SetupContent()
		{
			base.SetupContent();

			Viewport.BackgroundColor = ColorEx.White;

			SceneManager.AmbientLight = new ColorEx( 0.5f, 0.5f, 0.5f );

			//SceneManager.SetFog( FogMode.Linear, new ColorEx( 0.2f, 0.2f, 0.2f ), 0, 250, 2500 );
			SceneManager.SetSkyBox( true, "Examples/EveningSkyBox", 10000 );

			// set shadow properties
			SceneManager.ShadowTechnique = ShadowTechnique.TextureModulative;
			SceneManager.ShadowColor = new ColorEx( 0.5f, 0.5f, 0.5f );
			SceneManager.ShadowTextureSize = 1024;
			SceneManager.ShadowTextureCount = 1;

			Light light = SceneManager.CreateLight( "MainLight" );
			light.Type = LightType.Point;
			light.Position = new Vector3( 20, 80, 50 );
			light.Diffuse = ColorEx.White;

			SceneManager.LoadWorldGeometry( "Landscape.xml" );

			SceneManager.Options[ "" ] = "";
			//Material mat = (Material)MaterialManager.Instance.Load( "SplattingMaterial5", "TerrainMedia" );
			// SceneManager.SetFog(FogMode.Exp2, ColorEx.White, .008f, 0, 250);

			// water plane setup
			//Plane waterPlane = new Plane( Vector3.UnitY, 1.5f );

			//MeshManager.Instance.CreatePlane(
			//    "WaterPlane",
			//    ResourceGroupManager.DefaultResourceGroupName,
			//    waterPlane,
			//    2800, 2800,
			//    20, 20,
			//    true, 1,
			//    10, 10,
			//    Vector3.UnitZ );

			//Entity waterEntity = SceneManager.CreateEntity( "Water", "WaterPlane" );
			//waterEntity.MaterialName = "Terrain/WaterPlane";

			//waterNode = SceneManager.RootSceneNode.CreateChildSceneNode( "WaterNode" );
			//waterNode.AttachObject( waterEntity );
			//waterNode.Translate( new Vector3( 1000, 0, 1000 ) );

			// disable default camera control so the character can do its own
			CameraManager.setStyle( CameraStyle.Manual );

			// create our character controller
			chara = new SinbadCharacterController( Camera );

			//CameraManager.Camera.Position = new Vector3( 128, 400, 128 );
			//CameraManager.Camera.LookAt( new Vector3( 0, 0, 150 ) );

		}

		protected override void CleanupContent()
		{
			base.CleanupContent();
			if ( chara != null )
				chara = null;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool FrameRenderingQueued( FrameEventArgs evt )
		{
			// let character update animations and camera
			var height = Axiom.SceneManagers.PagingLandscape.Data2D.Data2DManager.Instance.GetRealWorldHeight( Camera.WorldPosition.x, Camera.WorldPosition.z);
			chara.Height = height;
			chara.AddTime( evt.TimeSinceLastFrame );
			return base.FrameRenderingQueued( evt );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool KeyPressed( SharpInputSystem.KeyEventArgs evt )
		{
			// relay input events to character controller
			if ( !TrayManager.IsDialogVisible )
				chara.InjectKeyDown( evt );

			return base.KeyPressed( evt );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool KeyReleased( SharpInputSystem.KeyEventArgs evt )
		{
			if ( !TrayManager.IsDialogVisible )
				chara.InjectKeyUp( evt );

			return base.KeyReleased( evt );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool MouseMoved( SharpInputSystem.MouseEventArgs evt )
		{
			// relay input events to character controller
			if ( !TrayManager.IsDialogVisible )
				chara.InjectMouseMove( evt );

			return base.MouseMoved( evt );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <param name="id"></param>
		/// <returns></returns>
		public override bool MousePressed( SharpInputSystem.MouseEventArgs evt, SharpInputSystem.MouseButtonID id )
		{
			// relay input events to character controller
			if ( !TrayManager.IsDialogVisible )
				chara.InjectMouseDown( evt, id );

			return base.MousePressed( evt, id );
		}

	}
}
