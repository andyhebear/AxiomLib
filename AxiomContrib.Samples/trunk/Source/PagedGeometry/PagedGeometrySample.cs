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
using System.Collections.Generic;
using System.Text;

using Axiom.Math;
using Axiom.Core;

namespace Axiom.Samples.PagedGeometry
{
	using Forests;

	public class PagedGeometrySample : SdkSample
	{

		private PagedGeometry _grass;
		private PagedGeometry _trees;
		private PagedGeometry _bushes;
		private GrassLoader _loader;
		private GrassLayer _grassLayer;

		public PagedGeometrySample()
		{
			Metadata[ "Title" ] = "Forests";
			Metadata[ "Description" ] = "Draw many geometry";
			Metadata[ "Thumbnail" ] = "thumb_forests.png";
			Metadata[ "Category" ] = "Geometry";
		}

		/// <summary>
		/// 
		/// </summary>
		protected override void SetupContent()
		{
			Camera.Position = new Vector3( 700, 5, 700 );
			Camera.LookAt( new Vector3( 0, 0, -300 ) );

			SceneManager.SetSkyBox( true, "3D-Diggers/SkyBox", 2000 );
			//-------------------------------------- LOAD GRASS --------------------------------------
			_grass = new PagedGeometry( Camera, 30 );
			_grass.AddDetailLevel<GrassPage>( 60 );
			_loader = new GrassLoader( _grass );
			_grass.PageLoader = _loader;
			//_loader.HeightFunction = mheight;
			_grassLayer = _loader.AddLayer( "grass" );
			_grassLayer.SetMinimumSize( 0.7f, 0.7f );
			_grassLayer.SetMaximumSize( 0.9f, 0.9f );
			_grassLayer.IsAnimationEnabled = true;
			_grassLayer.SwayDistribution = 7.0f;
			_grassLayer.SwayLength = 0.1f;
			_grassLayer.SwaySpeed = 0.4f;
			_grassLayer.Density = 3.0f;
			_grassLayer.SetRenderTechnique( GrassTechnique.Sprite );
			_grassLayer.FadeTechnique = FadeTechnique.Grow;

			_grassLayer.MapBounds = new TBounds( 0, 0, 1500, 1500 );

			////-------------------------------------- LOAD TREES --------------------------------------
			_trees = new PagedGeometry( Camera, 50 );

			_trees.SetInfinite();
			_trees.AddDetailLevel<BatchPage>( 90, 30 );
			_trees.AddDetailLevel<ImpostorPage>( 700, 50 );

			////Create a new TreeLoader2D object
			TreeLoader2D treeLoader = new TreeLoader2D( _trees, new TBounds( 0, 0, 1500, 1500 ) );
			_trees.PageLoader = treeLoader;
			//treeLoader.SetHeightFunction(mheight);
			//Load a tree entity
			Entity tree1 = SceneManager.CreateEntity( "tree1", "fir05_30.mesh" );
			Vector3 position = Vector3.Zero;

			Radian yaw;
			float scale;
			for ( int i = 0; i < 10000; i++ )
			{
				yaw = new Degree( Axiom.Math.Utility.RangeRandom( 0, 360 ) );
				position.x = Axiom.Math.Utility.RangeRandom( 0, 1500 );
				position.z = Axiom.Math.Utility.RangeRandom( 0, 1500 );
				scale = Axiom.Math.Utility.RangeRandom( 0.20f, 0.60f );

				treeLoader.AddTree( tree1, position, yaw, scale );
			}
			////-------------------------------------- LOAD BUSHES --------------------------------------
			_bushes = new PagedGeometry( Camera, 50 );
			_bushes.AddDetailLevel<BatchPage>( 80, 50 );

			TreeLoader2D bushLoader = new TreeLoader2D( _bushes, new TBounds( 0, 0, 1500, 1500 ) );
			_bushes.PageLoader = bushLoader;
			//bushLoader.SetHeightFunction(mheight);
			Entity fern = SceneManager.CreateEntity( "Fern", "farn1.mesh" );
			Entity plant = SceneManager.CreateEntity( "Plant", "plant2.mesh" );
			Entity mushRoom = SceneManager.CreateEntity( "Mushroom", "shroom1_1.mesh" );

			//Randomly place 20,000 bushes on the terrain
			for ( int i = 0; i < 20000; i++ )
			{
				yaw = new Degree( (Real)Axiom.Math.Utility.RangeRandom( 0, 360 ) );
				position.x = Axiom.Math.Utility.RangeRandom( 0, 1500 );
				position.z = Axiom.Math.Utility.RangeRandom( 0, 1500 );

				float rnd = Axiom.Math.Utility.UnitRandom();
				if ( rnd < 0.8f )
				{
					scale = Axiom.Math.Utility.RangeRandom( 0.6f, 0.6f );
					bushLoader.AddTree( fern, position, yaw, scale );
				}
				else if ( rnd < 0.9f )
				{
					scale = Axiom.Math.Utility.RangeRandom( 0.4f, 0.8f );
					bushLoader.AddTree( mushRoom, position, yaw, scale );
				}
				else
				{
					scale = Axiom.Math.Utility.RangeRandom( 0.5f, 0.7f );
					bushLoader.AddTree( plant, position, yaw, scale );
				}
			}
			base.SetupContent();
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="evt"></param>
		/// <returns></returns>
		public override bool FrameRenderingQueued( Core.FrameEventArgs evt )
		{
			UpdatePagedGeometry();
			return base.FrameRenderingQueued( evt );
		}

		/// <summary>
		/// 
		/// </summary>
		void UpdatePagedGeometry()
		{
			if ( _grass != null )
				_grass.Update();
			if ( _trees != null )
				_trees.Update();
			if ( _bushes != null )
				_bushes.Update();
		}
	}
}
