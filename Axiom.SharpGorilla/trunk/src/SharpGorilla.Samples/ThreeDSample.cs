#region MIT License
/*
-----------------------------------------------------------------------------
This source file is part of the Jitter Sample
Jitter Physics Engine Copyright (c) 2010 Thorben Linneweber (admin@jitter-physics.com)

This a port for Axiom of samples using Jitter Physics Engine,
developed by Thorben Linneweber and ported by Francesco Guastella (aka romeoxbm).
 
Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:
																			  
The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.
																			  
THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
-----------------------------------------------------------------------------
*/
#endregion

#region Using Statements

using System;
using System.Collections.Generic;
using System.Reflection;
using Axiom.Core;
using Axiom.Math;
using Axiom.Samples;
using SharpGorilla;

#endregion

namespace SharpGorilla.Samples
{

	public class ThreeDSample : SdkSample
	{

        public class Button
        {
            public bool IsHovered;
            public Caption Caption;
            /// <summary>
            /// 
            /// </summary>
            /// <param name="x"></param>
            /// <param name="y"></param>
            /// <param name="text"></param>
            /// <param name="layer"></param>
            public Button(Real x, Real y, string text, Layer layer)
            {
                Caption = layer.CreateCaption(14, x, y, text);
                Caption.SetSize(64, 25);
                Caption.HorizontalAlign = TextAlignment.Center;
                Caption.VerticalAlign = VerticalAlignment.Middle;
                Caption.BackgroundColor = Converter.ToRGB(255, 255, 255, 32);
                IsHovered = false;
            }
            /// <summary>
            /// 
            /// </summary>
            /// <param name="position"></param>
            /// <returns></returns>
            public bool IsOver(Vector2 position)
            {
                bool result = Caption.Intersects(position);
                if (result && !IsHovered)
                {
                    Caption.BackgroundColor = Converter.ToRGB(255, 255, 255, 128);
                }
                else if (!result && IsHovered)
                {
                    Caption.BackgroundColor = Converter.ToRGB(255, 255, 255, 28);
                }
                IsHovered = result;
                return result;
            }
        }

        public struct D3Panel
		{
			public ScreenRendable Screen;
			public SceneNode Node;
			public Layer GuiLayer;
			public Layer MousePointerLayer;
			public SharpGorilla.Rectangle Background;
			public SharpGorilla.Rectangle MousePointer;
			public Vector2 Size;
			public List<Button> Buttons;

			public D3Panel( Silverback silverback, SceneManager sceneMgr, Vector2 size )
			{
				Size = size;
				Screen = silverback.CreateScreenRendable( Size, "dejavue" );
				Node = sceneMgr.RootSceneNode.CreateChildSceneNode();
				Node.AttachObject( Screen );

				GuiLayer = Screen.CreateLayer( 0 );
				Background = GuiLayer.CreateRectangle( 0, 0, size.x * 100, size.y * 100 );
				Background.SetBackgroundGradient( Gradient.NorthSouth, Converter.ToRGB( 94, 97, 255, 5 ), Converter.ToRGB( 94, 97, 255, 50 ) );
				Background.SetBorder( 2, Converter.ToRGB( 255, 255, 255, 150 ) );
				MousePointerLayer = Screen.CreateLayer( 15 );
				MousePointer = MousePointerLayer.CreateRectangle( 0, 0, 10, 18 );
				MousePointer.SetBackgroundImage( "mousepointer" );
				Buttons = new List<Button>();
			}
			/// <summary>
			/// 
			/// </summary>
			/// <param name="ray"></param>
			/// <param name="isOver"></param>
			/// <returns></returns>
			public Button Check( Ray ray, out bool isOver )
			{
				isOver = false;
				Matrix4 transform = Matrix4.Compose( Node.Position, Node.Scale, Node.Orientation );
				AxisAlignedBox aabb = Screen.BoundingBox;
				aabb.Transform( transform );
				Axiom.Math.IntersectResult res = Axiom.Math.Utility.Intersects( ray, aabb );

				if ( !res.Hit )
					return null;

				Vector3 a, b, c, d = Vector3.Zero;
				Vector2 halfSize = Size * 0.5f;
				a = transform * new Vector3( -halfSize.x, -halfSize.y, 0 );
				b = transform * new Vector3( halfSize.x, -halfSize.y, 0 );
				c = transform * new Vector3( -halfSize.x, halfSize.y, 0 );
				d = transform * new Vector3( halfSize.x, halfSize.y, 0 );

				res = Axiom.Math.Utility.Intersects( ray, c, b, a, true, true );
				if ( !res.Hit )
					res = Axiom.Math.Utility.Intersects( ray, c, d, b, true, true );
				if ( !res.Hit )
					return null;

				if ( res.Distance > 6.0f )
					return null;

				isOver = true;
				Vector3 hitPos = ( ray.Origin + ( ray.Direction * res.Distance ) );
				Vector3 localPos = transform.Inverse() * hitPos;

				localPos.x += halfSize.x;
				localPos.y -= halfSize.y;
				localPos.x *= 100;
				localPos.y *= 100;

				// Cursor clip
				localPos.x = Axiom.Math.Utility.Clamp<Real>( localPos.x, ( Size.x * 100 ) - 10, 0 );
				localPos.y = Axiom.Math.Utility.Clamp<Real>( -localPos.y, ( Size.y * 100 ) - 18, 0 );

				MousePointer.Position = new Vector2( localPos.x, localPos.y );

				for ( int i = 0; i < Buttons.Count; i++ )
				{
					if ( Buttons[ i ].IsOver( MousePointer.Position ) )
						return Buttons[ i ];
				}
				return null;
			}

			public Caption CreateCaption( Real x, Real y, string text )
			{
				return GuiLayer.CreateCaption( 14, x, y, text );
			}

			public Button CreateButton( Real x, Real y, string text )
			{
				Button button = new Button( x, y, text, GuiLayer );
				Buttons.Add( button );
				return button;
			}
		}


		private Silverback _silverback;
		private Screen _hud;
		private Screen _controlPanel;
		private Layer _hudLayer;
		private Layer _crossHairLayer;
		private SharpGorilla.Rectangle _crossHair;
		private D3Panel _powerPanel;
		private SharpGorilla.Rectangle _powerValue;
		private SharpGorilla.Rectangle _powerValueBackground;
		private Button _powerUpButton;
		private Button _powerDownButton;
		private Real _basePower;
		private SceneNode _node;
		float timer, timer2;

		public ThreeDSample()
		{
			Metadata[ "Title" ] = "SharpGorilla 3D Sample";
			Metadata[ "Description" ] = "Demonstrates how to use SharpGorilla with Axiom in 3D.";
			Metadata[ "Thumbnail" ] = "thumb_gorilla.png";
			Metadata[ "Category" ] = "Gui";
		}

		protected override void SetupContent()
        {
            _silverback = new Silverback();
            _silverback.LoadAtlas("dejavue");

            CreateHUD();
            CreateControlPanel();

            Camera.Position = new Vector3(10, 2, 10);
            Camera.LookAt(new Vector3(0, 2, 0));
            Camera.Near = 0.05f;
            Camera.Far = 1000;
        }

        void CreateHUD()
        {
            _hud = _silverback.CreateScreen(Viewport, "dejavue");
            _hudLayer = _hud.CreateLayer();
            Caption fakeHealth = _hudLayer.CreateCaption(24, 0, 0, "+ 100");
            fakeHealth.Width = Viewport.ActualWidth - 16;
            fakeHealth.Height = Viewport.ActualHeight - 4;
            fakeHealth.HorizontalAlign = TextAlignment.Right;
            fakeHealth.VerticalAlign = VerticalAlignment.Bottom;

            Caption fakeAmmo = _hudLayer.CreateCaption(24, 16, 0, ": 60");
            fakeAmmo.Width = Viewport.ActualWidth;
            fakeAmmo.Height = Viewport.ActualHeight - 4;
            fakeAmmo.VerticalAlign = VerticalAlignment.Bottom;

            _crossHairLayer = _hud.CreateLayer();
            _crossHair = _crossHairLayer.CreateRectangle((Viewport.ActualWidth * 0.5f) - 11, (Viewport.ActualHeight * 0.5f) - 11, 22, 22);
            _crossHair.SetBackgroundImage("crosshair");


        }

        void CreateControlPanel()
        {

            _powerPanel = new D3Panel(_silverback, this.SceneManager, new Vector2(4, 1));
            _powerPanel.Node.Position = new Vector3(0, 1.5f, -10);
            Caption caption = _powerPanel.CreateCaption(0, 4, "Power Level");
            caption.Width = 400;
            caption.HorizontalAlign = TextAlignment.Center;

            _powerValueBackground = _powerPanel.GuiLayer.CreateRectangle(10, 35, 380, 10);
            _powerValueBackground.SetBackgroundColor(Converter.ToRGB(255, 255, 255, 100));

            _powerValue = _powerPanel.GuiLayer.CreateRectangle(10, 35, 200, 10);
            _powerValue.SetBackgroundGradient(Gradient.NorthSouth, Converter.ToRGB(255, 255, 255, 200), Converter.ToRGB(64, 64, 64, 200));
            _powerDownButton = _powerPanel.CreateButton(10, 65, "-");
            _powerUpButton = _powerPanel.CreateButton(84, 65, "+");
        }

		public override bool FrameStarted( FrameEventArgs evt )
        {
            base.FrameStarted( evt );

            //_fps.Text = "FPS: " + _renderWindow.LastFPS.ToString() + " , Batches " + _renderWindow.LastBatchCount.ToString();
            timer += evt.TimeSinceLastFrame;

            if (timer > 1.0f / 60)
            {
                _powerPanel.Node.Yaw(0.0005f);

                timer2 += Axiom.Math.Utility.RangeRandom(0, timer * 10);
                timer = 0;
                timer = Axiom.Math.Utility.Clamp<Real>(timer, 25, 0);
                Real power = _basePower + (Axiom.Math.Utility.Cos(timer2) * 5);
                power = Axiom.Math.Utility.Clamp<Real>(power, 380, 0);
                _powerValue.Width = power;
            }

			return false;
            //Vector3 trans = new Vector3(0, 0, 0);

            //if (_input.IsKeyPressed(System.Windows.Forms.Keys.W))
            //    trans.z = -1;
            //if (_input.IsKeyPressed(System.Windows.Forms.Keys.S))
            //    trans.z = 1;
            //if (_input.IsKeyPressed(System.Windows.Forms.Keys.A))
            //    trans.x = -1;
            //if (_input.IsKeyPressed(System.Windows.Forms.Keys.D))
            //    trans.x = 1;

            //if (trans.Length != 0)
            //{
            //    Vector3 pos = _camera.Position;
            //    pos += _camera.Orientation * (trans * 5.0f) * e.TimeSinceLastFrame;
            //    pos.y = 2.0f;
            //    _camera.Position = pos;
            //}
        }

		public override bool MousePressed( SharpInputSystem.MouseEventArgs evt, SharpInputSystem.MouseButtonID id )
		{
			bool isOver = false;

			if ( id == SharpInputSystem.MouseButtonID.Left )
			{
				Button button = _powerPanel.Check( Camera.GetCameraToViewportRay( 0.5f, 0.5f ), out isOver );
				//if (isOver)
				//    _crossHairLayer.Hide();
				//else
				//    _crossHairLayer.Show();

				if ( button != null )
				{
					if ( button == _powerDownButton )
					{
						_basePower -= 1.0f;
					}
					if ( button == _powerUpButton )
					{
						_basePower += 1.0f;
					}
				}
			}
			return base.MousePressed( evt, id );
		}
	}
}
