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

	public class DejaVueSample : SdkSample
	{

		public const string AtlasName = "dejavue";
		public Real Timer { get; set; }
		public Real Timer2 { get; set; }
		/// <summary>
		/// 
		/// </summary>
		public Screen Screen { get; set; }
		public Silverback Silverback { get; set; }
		public Layer Layer { get; set; }
		public Polygon Poly { get; set; }
		public LineList List { get; set; }
		public Caption Caption { get; set; }
		public SharpGorilla.Rectangle Rectangle { get; set; }
		public QuadList QuadList { get; set; }
		public MarkupText Markup { get; set; }

		Caption _fps;
		AxiomConsole _console;

		public DejaVueSample()
		{
			Metadata[ "Title" ] = "SharpGorilla DejaVue Sample";
			Metadata[ "Description" ] = "Demonstrates how to use SharpGorilla with Axiom.";
			Metadata[ "Thumbnail" ] = "thumb_gorilla.png";
			Metadata[ "Category" ] = "Gui";
		}

		protected override void SetupContent()
		{
			// Create Silverback and load in dejavu
			Silverback = new Silverback();
			Silverback.LoadAtlas( AtlasName );
			Screen = Silverback.CreateScreen( base.Viewport, AtlasName );
			Real vpW = base.Viewport.ActualWidth;
			Real vpH = base.Viewport.ActualHeight;

			// Create our drawing layer
			Layer = Screen.CreateLayer();
			Rectangle = Layer.CreateRectangle( 0, 0, vpW, vpH );
			Rectangle.SetBackgroundGradient( Gradient.Diagonal, Converter.ToRGB( 98, 0, 63 ), Converter.ToRGB( 255, 180, 174 ) );

			Markup = Layer.CreateMarkupText( 9, 5, 5 + 80, "%@24%A Haiku\n%@14%Written by Betajaen%@14%\nAnd ported by Bostich%@9%\nSo many to choose from\nPretty typefaces on Axiom screen\nTime to update Git" );
			Caption = Layer.CreateCaption( 9, vpW - 55, 5 + 80, "9" );
			Caption.Width = 50;
			Caption.HorizontalAlign = TextAlignment.Right;
			Caption.Color = ColorEx.Yellow;
			Caption = Layer.CreateCaption( 14, vpW - 55, 18 + 80, "14" );
			Caption.Width = 50;
			Caption.HorizontalAlign = TextAlignment.Right;

			Caption = Layer.CreateCaption( 24, vpW - 55, 33 + 80, "14" );
			Caption.Width = 50;
			Caption.HorizontalAlign = TextAlignment.Right;

			_fps = Layer.CreateCaption( 14, 0, vpH - 33, "" );
			_console = new AxiomConsole();
			_console.Init( Screen );
			_console.AddCommand( "version", new AxiomConsole.CommandDelegate( PrintAxiomVersion ) );
			_console.AddCommand( "make", new AxiomConsole.CommandDelegate( Make ) );
			_console.AddCommand( "quit", new AxiomConsole.CommandDelegate( Quit ) );

			base.SetupContent();
		}

		public override bool KeyPressed( SharpInputSystem.KeyEventArgs evt )
		{
			if ( null != _console )
			{
				_console.OnKeyPressed( evt );
				return true;
			}
			else
				return base.KeyPressed( evt );
		}

		public override void Paused()
		{
			Layer.Hide();

			base.Paused();
		}

		public override void Unpaused()
		{
			Layer.Show();

			base.Unpaused();
		}

		#region Console Commands

		public void PrintAxiomVersion( List<string> parms )
		{
			_console.Print( Root.Instance.Version );
		
		}
		public void Quit( List<string> parms )
		{
			Root.Instance.QueueEndRendering();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parms"></param>
		public void Make( List<string> parms )
		{
			if ( parms.Count == 1 )
				_console.Print( "Make you what?" );
			else
			{
				string toMake = parms[ 1 ].ToLower();
				switch ( toMake )
				{
					case "sandwich":
						_console.Print( "Make it yourself!" );
						break;
					case "universe":
						_console.Print( "Boom!" );
						break;
					case "axiom":
						_console.Print( "Is a way cooler than Ogre :)" );
						break;
					case "gorilla":
						_console.Print( "He wouldn't like that!" );
						break;
					default:
						_console.Print( "Go check your fridge!" );
						break;
				}
			}
		}

		#endregion
	}
}
