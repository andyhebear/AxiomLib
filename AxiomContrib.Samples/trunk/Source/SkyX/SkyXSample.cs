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
using SX = Axiom.SkyX;

namespace Axiom.Samples.SkyX
{
	public class SkyXSample : SdkSample
	{
		private SX.SkyX _skyX;
		private bool _showInformation;

		public SkyXSample()
		{
			Metadata[ "Title" ] = "SkyX";
			Metadata[ "Description" ] = "Day & Night cycle including nice clouds!";
			Metadata[ "Thumbnail" ] = "thumb_skyX.png";
			Metadata[ "Category" ] = "Simulation";
		}

		protected override void SetupContent()
		{
			Camera.Far = 30000;
			Camera.Near = 20;
			Camera.Position = new Vector3( 20000, 500, 20000 );
			Camera.Direction = new Vector3( 1, 0, 0 );

			_skyX = new SX.SkyX( SceneManager, Camera );
			_skyX.Create();
			//_skyX.VCloudsManager.Create();

			//SX.AtmosphereManager.AtmosphereOptions opt = (SX.AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();
			//opt.RayleighMultiplier = 0.0045f;
			//_skyX.AtmosphereManager.Options = opt;

			//_skyX.CloudsManager.Add( new SX.CloudLayer.Options(/* Default options */) );
			base.SetupContent();
		}

		public override bool FrameRenderingQueued( Core.FrameEventArgs evt )
		{
			// var 
			var skyXOptions = (Axiom.SkyX.AtmosphereManager.AtmosphereOptions)_skyX.AtmosphereManager.Options.Clone();
			_skyX.TimeMultiplier = !_showInformation ? 0.1f : 0.0f;
			//skyXOptions.HeightPosition += evt.TimeSinceLastFrame * 0.05f;

			_skyX.AtmosphereManager.Options = skyXOptions;

			_skyX.Update( evt.TimeSinceLastFrame );

			return base.FrameRenderingQueued( evt );
		}

		private string getConfigStringFromSkyXAtmosphereOptions( Axiom.SkyX.AtmosphereManager.AtmosphereOptions Options )
		{
			var hour = Options.Time.x;
			var min = (int)( ( Options.Time.x - hour ) * 60 );

			var timeStr = hour + ":" + min;
			string str = "Axiom SkyX Plugin demo (Press F1 to show/hide information)" + ( _showInformation ? " - Simuation paused - \n" : "\n-------------------------------------------------------------\nTime: " + timeStr.ToString() + "\n" );

			if ( _showInformation )
			{
				str += "-------------------------------------------------------------\n";
				str += "Time: " + timeStr.ToString() + " [1, Shift+1] (+/-).\n";
				str += "Rayleigh multiplier: " + Options.RayleighMultiplier + " [2, Shift+2] (+/-).\n";
				str += "Mie multiplier: " + Options.MieMultiplier + " [3, Shift+3] (+/-).\n";
				str += "Exposure: " + Options.Exposure + " [4, Shift+4] (+/-).\n";
				str += "Inner radius: " + Options.InnerRadius + " [5, Shift+5] (+/-).\n";
				str += "Outer radius: " + Options.OuterRadius + " [6, Shift+6] (+/-).\n";
				str += "Number of samples: " + Options.NumberOfSamples + " [7, Shift+7] (+/-).\n";
				str += "Height position: " + Options.HeightPosition + " [8, Shift+8] (+/-).\n";

				if ( _skyX.VCloudsManager.IsCreated )
				{
					str += "Wind direction: " + _skyX.VCloudsManager.VClouds.WindDirection.InDegrees.ToString() + " degrees [0, Shift+0] (+/-).\n";
					str += "Clouds roughness: " + _skyX.VCloudsManager.VClouds.NoiseScale + " [p, Shift+p] (+/-).\n";
				}
			}

			return str;
		}
	}
}
