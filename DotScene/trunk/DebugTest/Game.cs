using System;
using Axiom.Core;
using Axiom.Graphics;
using Axiom.Math;
using Axiom.Components.DotScene;

namespace Axiom.Game.DirectX9
{
	public sealed class Game
	{
		private readonly Root _root;
		private readonly RenderWindow _window;
		private SceneManager _scene;
		private Camera _camera;

		public Game( Root root, RenderWindow window )
		{
			_root = root;
			_window = window;
		}

		public void OnLoad()
		{

            ResourceGroupManager.Instance.AddResourceLocation("Media/Archives/AxiomCore.zip", "ZipFile");
            ResourceGroupManager.Instance.AddResourceLocation("Media/Materials", "Folder", true);
            ResourceGroupManager.Instance.AddResourceLocation("Media/Models", "Folder", false);
            ResourceGroupManager.Instance.AddResourceLocation("Media/Scene", "Folder", false);
            ResourceGroupManager.Instance.AddResourceLocation("Media/Skeletons", "Folder", false);
            ResourceGroupManager.Instance.AddResourceLocation("Media/Terrain", "Folder", false);
            ResourceGroupManager.Instance.AddResourceLocation("Media/Materials/Textures/SkyBoxes.zip", "ZipFile");

            ResourceGroupManager.Instance.InitializeAllResourceGroups();

            DotSceneLoader dsl = new DotSceneLoader();
            dsl.VerboseLogging = true;
            dsl.LoadScene("adam.scene", null, this._window, "General", null, true, false);
            this._scene = dsl.SceneManager;
            this._camera = dsl.FirstCamera;
        }

		public void OnUnload()
		{
		}

		public void OnRenderFrame( object s, FrameEventArgs e )
		{

		}

	}
}
