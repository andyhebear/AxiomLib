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
using AxiomContrib.Samples.Jitter.PhysicsObjects;
using AxiomContrib.Samples.Jitter.Primitives3D;
using AxiomContrib.Samples.Jitter.Scenes;
using Jitter;
using Jitter.Collision;
using Jitter.Collision.Shapes;
using Jitter.Dynamics;
using Jitter.Dynamics.Constraints;
using Jitter.LinearMath;

#endregion

namespace AxiomContrib.Samples.Jitter
{
	public enum BodyTag { DrawMe, DontDrawMe }

	public class JitterSample : SdkSample
	{
		private GeometricPrimitive[] _primitives = new GeometricPrimitive[ (int)PrimitiveType.Count ];

		private int _currentScene = 0;
		private bool _multithreadingEnabled = true;
		private bool _leftMouseButtonDown = false;
		private Vector2 _currentMousePos;

		private Random random = new Random();
		private RigidBody lastBody = null;
		private int _activeBodiesCount = 0;
		private List<ConvexHullObject> _convexHulls = new List<ConvexHullObject>();

		// Store information for drag and drop
		private JVector hitPoint, hitNormal;
		private WorldPointConstraint grabConstraint;
		private RigidBody grabBody;
		private float hitDistance = 0.0f;
		//private int scrollWheel = 0;

		//public DebugDrawer DebugDrawer { get; private set; }
		public World PhysicWorld { get; private set; }
		public IList<Scene> PhysicScenes { get; private set; }
		public JitterSampleController StatsHandler { get; private set; }

		public JitterSample()
		{
			Metadata[ "Title" ] = "Jitter Sample";
			Metadata[ "Description" ] = "Demonstrates how to use Jitter Physics Engine with Axiom.";
			Metadata[ "Thumbnail" ] = "thumb_Jitter.png";
			Metadata[ "Category" ] = "Physics";
		}

		public override bool FrameRenderingQueued( FrameEventArgs evt )
		{
			float step = evt.TimeSinceLastFrame;
			if ( step > 1.0f / 100.0f )
				step = 1.0f / 100.0f;

			PhysicWorld.Step( step, _multithreadingEnabled );
			_activeBodiesCount = 0;

			foreach ( RigidBody body in PhysicWorld.RigidBodies )
			{
				if ( body.IsActive )
					_activeBodiesCount++;

				_addBodyToUpdateList( body );
			}

			foreach ( GeometricPrimitive prim in _primitives )
			{
				if ( null != prim )
					prim.ResetIndex();
			}

			foreach ( ConvexHullObject prim in _convexHulls )
			{
				if ( null != prim )
					prim.PhysicUpdate();
			}

			if ( _currentScene > 0 && _currentScene < PhysicScenes.Count && null != PhysicScenes[ _currentScene ] )
				PhysicScenes[ _currentScene ].PhysicUpdate();

			#region drag and drop physical objects with the mouse
			if ( _leftMouseButtonDown )
			{
				if ( grabBody != null )
				{
					Vector3 ray = _rayTo( _currentMousePos.x, _currentMousePos.y );
					ray.Normalize();
					grabConstraint.Anchor = Conversion.ToJitterVector( Camera.Position + ray * hitDistance );
				}
			}
			else
			{
				grabBody = null;

				if ( grabConstraint != null )
					PhysicWorld.RemoveConstraint( grabConstraint );
			}
			#endregion

			// Draw the debug data provided by Jitter
			//DrawIslands();
			//DrawJitterDebugInfo();

			//Update Stats
			if ( StatsHandler.AreStatsVisible )
			{
				int contactCount = 0;
				int entries = (int)World.DebugType.num;
				double total = 0;

				for ( int i = 0; i < entries; i++ )
				{
					World.DebugType type = (World.DebugType)i;
					StatsHandler.SetValue( type.ToString(), PhysicWorld.DebugTimes[ i ] );
					total += PhysicWorld.DebugTimes[ i ];
				}

				foreach ( Arbiter ar in PhysicWorld.ArbiterMap.Values )
					contactCount += ar.ContactList.Count;

				StatsHandler.SetValue( "ArbiterCount", PhysicWorld.ArbiterMap.Values.Count );
				StatsHandler.SetValue( "ContactCount", contactCount );
				StatsHandler.SetValue( "IslandCount", PhysicWorld.Islands.Count );
				StatsHandler.SetValue( "BodyCount", string.Format( "{0} ( {1} )", PhysicWorld.RigidBodies.Count, _activeBodiesCount ) );
				StatsHandler.SetValue( "MultiThreaded", _multithreadingEnabled );
				StatsHandler.SetValue( "Gen", string.Format( "gen0: {0} gen1: {1} gen2: {2}", GC.CollectionCount( 0 ), GC.CollectionCount( 1 ), GC.CollectionCount( 2 ) ) );

				StatsHandler.SetValue( "TotalPhysicsTime", total );
				StatsHandler.SetValue( "PhysicsFramerate", ( 1000.0d / total ).ToString( "0" ) + " fps" );

				StatsHandler.Update();
			}

			return base.FrameRenderingQueued( evt );  // don't forget the parent class updates!
		}

		private Vector3 _rayTo( float x, float y )
		{
			Ray mouseRay = Camera.GetCameraToViewportRay( x, y );
			return mouseRay.Direction;
		}

		private bool _raycastCallback( RigidBody body, JVector normal, float fraction )
		{
			return !body.IsStatic;
		}

		public override bool MouseMoved( SharpInputSystem.MouseEventArgs evt )
		{
			float x = evt.State.X.Absolute / (float)Camera.Viewport.ActualWidth;
			float y = evt.State.Y.Absolute / (float)Camera.Viewport.ActualHeight;
			_currentMousePos = new Vector2( x, y );

			return base.MouseMoved( evt );
		}

		public override bool MousePressed( SharpInputSystem.MouseEventArgs evt, SharpInputSystem.MouseButtonID id )
		{
			if ( id == SharpInputSystem.MouseButtonID.Left )
			{
				_leftMouseButtonDown = true;

				JVector ray = Conversion.ToJitterVector( _rayTo( _currentMousePos.x, _currentMousePos.y ) );
				JVector camp = Conversion.ToJitterVector( Camera.Position );

				ray = JVector.Normalize( ray ) * 100;

				float fraction;
				bool result = PhysicWorld.CollisionSystem.Raycast( camp, ray, _raycastCallback, out grabBody, out hitNormal, out fraction );

				if ( result )
				{
					hitPoint = camp + fraction * ray;

					if ( grabConstraint != null )
						PhysicWorld.RemoveConstraint( grabConstraint );

					JVector lanchor = hitPoint - grabBody.Position;
					lanchor = JVector.Transform( lanchor, JMatrix.Transpose( grabBody.Orientation ) );

					grabConstraint = new WorldPointConstraint( grabBody, lanchor );

					PhysicWorld.AddConstraint( grabConstraint );
					hitDistance = ( Conversion.ToVector3( hitPoint ) - Camera.Position ).Length;
					//scrollWheel = evt.State. mouseState.ScrollWheelValue;
					grabConstraint.Anchor = hitPoint;
				}
			}

			if ( id == SharpInputSystem.MouseButtonID.Right )
			{
				this.DragLook = false;
			}

			return base.MousePressed( evt, id ); ;
		}

		public override bool MouseReleased( SharpInputSystem.MouseEventArgs evt, SharpInputSystem.MouseButtonID id )
		{
			if ( id == SharpInputSystem.MouseButtonID.Left )
			{
				_leftMouseButtonDown = false;
			}

			if ( id == SharpInputSystem.MouseButtonID.Right )
			{
				this.CameraManager.manualStop();
				this.DragLook = true;
			}

			return base.MouseReleased( evt, id );
		}

		public override bool KeyPressed( SharpInputSystem.KeyEventArgs evt )
		{
			bool baseRetValue = base.KeyPressed( evt );

			// change threading mode
			if ( evt.Key == SharpInputSystem.KeyCode.Key_M )
				_multithreadingEnabled = !_multithreadingEnabled;

			// create random primitives
			if ( evt.Key == SharpInputSystem.KeyCode.Key_SPACE )
			{
				_spawnRandomPrimitive( Conversion.ToJitterVector( Camera.Position ),
					Conversion.ToJitterVector( Camera.Direction * 40.0f ) );
			}

			// Show/Hide physics stats
			if ( evt.Key == SharpInputSystem.KeyCode.Key_F12 )
			{
				StatsHandler.AreStatsVisible = !StatsHandler.AreStatsVisible;
			}

			return baseRetValue;
		}

		public void ChangeCurrentScene( int newSceneIndex, bool skipDestroy )
		{
			if ( !skipDestroy )
			{
				PhysicScenes[ _currentScene ].Destroy();
				this.SceneManager.ClearScene();
				this.SceneManager.DestroyAllMovableObjectsByType( "ManualObject" );
			}

			_initPrimitives();
			_convexHulls.Clear();

			_currentScene = newSceneIndex;
			PhysicScenes[ _currentScene ].Build();
			StatsHandler.SetValue( "CurrentScene", this.PhysicScenes[ _currentScene ].GetType().Name );

			// add a skybox (disabled for now due to an issue when switching from scenes)
			//SceneManager.SetSkyBox( true, "Examples/StormySkyBox", 5000 );

			// setup some basic lighting for our scene
			SceneManager.AmbientLight = new ColorEx( 0.5f, 0.5f, 0.5f );
			SceneManager.CreateLight( "Light1" ).Position = new Vector3( 20, 80, 50 );

			// set initial camera position and style
			this.DragLook = true;
			CameraManager.TopSpeed = 10;
			Camera.Position = new Vector3( 15, 15, 30 );
		}

		private void _addBodyToUpdateList( RigidBody rb )
		{
			if ( rb.Tag is BodyTag && ( (BodyTag)rb.Tag ) == BodyTag.DontDrawMe )
				return;

			bool isCompoundShape = ( rb.Shape is CompoundShape );

			if ( !isCompoundShape )
			{
				_addShapeToUpdateList( rb.Shape, rb.Orientation, rb.Position );
			}
			else
			{
				CompoundShape cShape = rb.Shape as CompoundShape;
				JMatrix orientation = rb.Orientation;
				JVector position = rb.Position;

				foreach ( var ts in cShape.Shapes )
				{
					JVector pos = ts.Position;
					JMatrix ori = ts.Orientation;

					JVector.Transform( ref pos, ref orientation, out pos );
					JVector.Add( ref pos, ref position, out pos );

					JMatrix.Multiply( ref ori, ref orientation, out ori );

					_addShapeToUpdateList( ts.Shape, ori, pos );
				}
			}
		}

		private void _addShapeToUpdateList( Shape shape, JMatrix ori, JVector pos )
		{
			GeometricPrimitive primitive = null;
			Vector3 scale = Vector3.UnitScale;

			if ( shape is BoxShape )
			{
				primitive = _primitives[ (int)PrimitiveType.Box ];
				scale = Conversion.ToVector3( ( shape as BoxShape ).Size );
			}
			else if ( shape is SphereShape )
			{
				primitive = _primitives[ (int)PrimitiveType.Sphere ];
				scale = new Vector3( ( shape as SphereShape ).Radius );
			}
			else if ( shape is CylinderShape )
			{
				primitive = _primitives[ (int)PrimitiveType.Cylinder ];
				CylinderShape cs = shape as CylinderShape;
				scale = new Vector3( cs.Radius, cs.Height, cs.Radius );
			}
			else if ( shape is CapsuleShape )
			{
				primitive = _primitives[ (int)PrimitiveType.Capsule ];
				CapsuleShape cs = shape as CapsuleShape;
				scale = new Vector3( cs.Radius * 2, cs.Length, cs.Radius * 2 );

			}
			else if ( shape is ConeShape )
			{
				primitive = _primitives[ (int)PrimitiveType.Cone ];
				ConeShape cs = shape as ConeShape;
				scale = new Vector3( cs.Radius, cs.Height, cs.Radius );
			}

			primitive.PhysicUpdate( Conversion.ToVector3( pos ), Conversion.ToQuat( ori ), scale );
		}

		private void _spawnRandomPrimitive( JVector position, JVector velocity )
		{
			RigidBody body = null;
			int rndn = random.Next( 7 );

			// less of the more advanced objects
			if ( rndn == 5 || rndn == 6 || rndn == 7 )
				rndn = random.Next( 7 );

			switch ( rndn )
			{
				case 0:
					body = new RigidBody( new ConeShape( (float)random.Next( 5, 50 ) / 20.0f, (float)random.Next( 10, 20 ) / 20.0f ) );
					break;

				case 1:
					body = new RigidBody( new BoxShape( (float)random.Next( 10, 30 ) / 20.0f, (float)random.Next( 10, 30 ) / 20.0f, (float)random.Next( 10, 30 ) / 20.0f ) );
					break;

				case 2:
					body = new RigidBody( new SphereShape( (float)random.Next( 30, 100 ) / 100.0f ) );
					break;

				case 3:
					body = new RigidBody( new CylinderShape( 1.0f, 0.5f ) );
					break;

				case 4:
					body = new RigidBody( new CapsuleShape( 1.0f, 0.5f ) );
					break;

				case 5:
					Shape b1 = new BoxShape( new JVector( 3, 1, 1 ) );
					Shape b2 = new BoxShape( new JVector( 1, 1, 3 ) );
					Shape b3 = new CylinderShape( 3.0f, 0.5f );

					CompoundShape.TransformedShape t1 = new CompoundShape.TransformedShape( b1, JMatrix.Identity, JVector.Zero );
					CompoundShape.TransformedShape t2 = new CompoundShape.TransformedShape( b2, JMatrix.Identity, JVector.Zero );
					CompoundShape.TransformedShape t3 = new CompoundShape.TransformedShape( b3, JMatrix.Identity, new JVector( 0, 0, 0 ) );

					CompoundShape ms = new CompoundShape( new CompoundShape.TransformedShape[ 3 ] { t1, t2, t3 } );

					body = new RigidBody( ms );
					break;

				case 6:
					ConvexHullObject obj2 = new ConvexHullObject( this );
					body = obj2.body;
					body.Restitution = 0.2f;
					body.StaticFriction = 0.8f;
					_convexHulls.Add( obj2 );
					break;
			}

			PhysicWorld.AddBody( body );

			body.Position = position;
			body.LinearVelocity = velocity;

			lastBody = body;
		}

		private void _initPrimitives()
		{
			for ( int i = 0; i < (int)PrimitiveType.Count; ++i )
			{
				if ( _primitives[ i ] == null )
					continue;

				if ( !_primitives[ i ].IsDisposed )
					_primitives[ i ].Dispose();

				_primitives[ i ] = null;
			}

			_primitives[ (int)PrimitiveType.Box ] = new BoxPrimitive( this );
			_primitives[ (int)PrimitiveType.Capsule ] = new CapsulePrimitive( this );
			_primitives[ (int)PrimitiveType.Cone ] = new ConePrimitive( this );
			_primitives[ (int)PrimitiveType.Cylinder ] = new CylinderPrimitive( this );
			_primitives[ (int)PrimitiveType.Sphere ] = new SpherePrimitive( this );
		}

		protected override void SetupContent()
		{
			//Setting Up Physics System
			CollisionSystem collision = new CollisionSystemSAP();
			PhysicWorld = new World( collision );
			PhysicWorld.AllowDeactivation = true;

			//DebugDrawer = new DebugDrawer();


			//Get available scenes and add them into the scene selector
			PhysicScenes = new List<Scene>();
			foreach ( Type type in Assembly.GetExecutingAssembly().GetTypes() )
			{
				if ( type.Namespace == "AxiomContrib.Samples.Jitter.Scenes" && !type.IsAbstract )
				{
					Scene scene = (Scene)Activator.CreateInstance( type, this );
					this.PhysicScenes.Add( scene );
				}
			}

			StatsHandler = new JitterSampleController( this, TrayManager );
			StatsHandler.Initialize();

		}
	}
}
