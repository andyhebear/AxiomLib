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

using System.Collections.Generic;
using Axiom.Collections;
using Axiom.Overlays;
using Axiom.Overlays.Elements;
using Axiom.Samples;
using AxiomContrib.Samples.Jitter.Scenes;
using System;

#endregion

namespace AxiomContrib.Samples.Jitter
{
	public class JitterSampleController
	{
		private JitterSample _sample;
		private ParamsPanel _statsPanel;
		private SortedList<string, string> _stats;
		private Queue<string> _updatedStats = new Queue<string>();

		private SdkTrayManager TrayManager;

		public bool AreStatsVisible
		{
			get;
			set;
		}

		public JitterSampleController( JitterSample sample , SdkTrayManager trayManager )
		{
			_sample = sample;
			AreStatsVisible = true;			
			TrayManager = trayManager;
		}

		public void Initialize()
		{
			DecorWidget logo = TrayManager.CreateLogoWidget( TrayLocation.TopLeft, "Jitter/Logo", "Panel", "JitterSample/JitterLogo" );
			SelectMenu sceneSelector = TrayManager.CreateLongSelectMenu( TrayLocation.TopLeft, "SceneSelector", "Scene:", 250, 140, 15 );

			foreach ( Scene scene in _sample.PhysicScenes )
			{
				sceneSelector.AddItem( scene.GetType().Name );
			}

			_stats = new SortedList<string, string>() {
														 {"ArbiterCount" , string.Empty},
														 {"ContactCount" , string.Empty},
														 {"IslandCount" , string.Empty},
														 {"BodyCount" , string.Empty},
														 {"MultiThreaded" , string.Empty},
														 {"Gen" , string.Empty},
														 {"CollisionDetect" , string.Empty},
														 {"BuildIslands" , string.Empty},
														 {"HandleArbiter" , string.Empty},
														 {"UpdateContacts" , string.Empty},
														 {"PreStep" , string.Empty},
														 {"DeactivateBodies" , string.Empty},
														 {"IntegrateForces" , string.Empty},
														 {"Integrate" , string.Empty},
														 {"PostStep" , string.Empty},
														 {"TotalPhysicsTime" , string.Empty},
														 {"PhysicsFramerate" , string.Empty}
													};
			_statsPanel = TrayManager.CreateParamsPanel( TrayLocation.TopLeft, "Jitter/StatsPanel", 250, _stats.Keys );

			// Enqueue all of available stats to force the first time update
			foreach ( string key in _stats.Keys )
			{
				_updatedStats.Enqueue( key );
			}

			sceneSelector.SelectedIndexChanged += sceneSelector_SelectedIndexChanged;

			if ( _sample.PhysicScenes.Count > 0 )
			{
				sceneSelector.SelectItem( 0 );
			}

		}

		public void SetValue( string key, int value )
		{
			SetValue( key, value.ToString() );
		}

		public void SetValue( string key, float value )
		{
			SetValue( key, value.ToString( "F2" ) );
		}

		public void SetValue( string key, double value )
		{
			SetValue( key, value.ToString( "0.00" ) );
		}

		public void SetValue( string key, bool value )
		{
			SetValue( key, value ? "true" : "false" );
		}

		public void SetValue( string key, string value )
		{
			if ( !_stats.ContainsKey( key ) || !AreStatsVisible )
				return;

			_stats[ key ] = value;

			if ( !_updatedStats.Contains( key ) )
				_updatedStats.Enqueue( key );
		}

		public void Update()
		{
			if ( _updatedStats.Count == 0 || !AreStatsVisible )
				return;

			_statsPanel.ParamValues = _stats.Values;
		}

		private void sceneSelector_SelectedIndexChanged( object sender, EventArgs e )
		{
			SelectMenu menu = sender as SelectMenu;

			if ( menu == null )
				return;

			_sample.ChangeCurrentScene( menu.SelectionIndex, false );
		}


	}
}
