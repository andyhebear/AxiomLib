#region LGPL License
/*
Axiom Graphics Engine Library
Copyright (C) 2003-2010 Axiom Project Team

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*/
#endregion


#region Namespace Declarations
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using Axiom;
using Axiom.Animating;
using Axiom.FileSystem;
using Axiom.Graphics;
using Axiom.Core;
using Axiom.Math;

using Axiom.Components.Terrain;
using Axiom.ParticleSystems;

#endregion Namespace Declarations

namespace Axiom.Components.DotScene
{
	/// <summary>
	/// Implements the necessary functionality to load an Axiom Scene from a DotScene file
	/// </summary>
	public class DotSceneLoader
	{
		#region Declarations

		private SceneManager _sceneManager;
        public SceneManager SceneManager
        {
            get { return _sceneManager; }
        }

		private RenderWindow _renderWindow;
        public RenderWindow Window
        {
            get { return _renderWindow; }
        }

		private SceneNode _rootSceneNode;
        public SceneNode RootSceneNode
        {
            get { return _rootSceneNode; }
        }

		private bool _doMaterials;
		private bool _forceShadowBuffers;
		private string _groupName;
		public float DefaultLightAttenuation = 10000.0f;
		protected TerrainGlobalOptions _terrainGlobals;
		protected TerrainGroup _terrainGroup;
		protected Camera _firstCamera;
        public Camera FirstCamera
        {
            get { return _firstCamera; }
        }

		public bool VerboseLogging = false;
		
		#endregion

		/// <summary>
		/// Loads the specified scene based on the implementation provided
		/// </summary>
		/// <param name="fileName">.scene file containing scene data</param>
		/// <param name="sceneManager">Scene manager to build the scene</param>
		/// <param name="renderWindow">Window to render the scene to</param>
		/// <param name="groupName">Group name of the scene's resources</param>
		/// <param name="rootSceneNode">Root scene node</param>
		/// <param name="doMaterials">Boolean indicating whether materials should also be loaded</param>
		/// <param name="forceShadowBuffers">Boolean indicating whether ShadowBuffers should be forced</param>
        public void LoadScene(string fileName, SceneManager sceneManager, RenderWindow renderWindow, string groupName, SceneNode rootSceneNode, bool doMaterials, bool forceShadowBuffers)
		{


			LogManager.Instance.Write("Info: Loading '{0}' with Axiom.Components.DotScene.", fileName);

			FileInfoList fl = ResourceGroupManager.Instance.ListResourceFileInfo(groupName);
            
			for (int q = 0; q < fl.Count; q++)
			{
				if (fileName == fl[q].Basename)
				{
					XmlDocument scene = new XmlDocument();
					scene.Load(fl[q].Filename);
					LoadScene(scene, sceneManager, renderWindow, groupName, rootSceneNode, doMaterials, forceShadowBuffers);
                    Root.Instance.SceneManager = _sceneManager;
					LogManager.Instance.Write("Info: Completed loading '{0}' with Axiom.Components.DotScene.", fileName);
                    return;
				}
			}
			throw new Exception("Cannot load file: " + fileName);
		}

		/// <summary>
		/// Loads the specified scene based on the implementation provided
		/// </summary>
		/// <param name="sceneXmlDocument">XmlDocument containing scene data</param>
		/// <param name="sceneManager">Scene manager to build the scene</param>
		/// <param name="renderWindow">Window to render the scene to</param>
		/// <param name="groupName">Group name of the scene's resources</param>
		/// <param name="rootSceneNode">Root scene node</param>
		/// <param name="doMaterials">Boolean indicating whether materials should also be loaded</param>
		/// <param name="forceShadowBuffers">Boolean indicating whether ShadowBuffers should be forced</param>
		public void LoadScene(XmlDocument sceneXmlDocument, SceneManager sceneManager, RenderWindow renderWindow, string groupName, SceneNode rootSceneNode, bool doMaterials, bool forceShadowBuffers)
		{
			// Sanity checks
			if (sceneXmlDocument == null)
			{
				throw new Exception("Error loading scene: XmlDocument null");
			}

			XmlNode rootXmlNode = sceneXmlDocument.FirstChild.NextSibling ;
			
			if (rootXmlNode == null)
			{
				throw new Exception("Error loading scene: Root node does not exist");
			}

			// Set member values
			_sceneManager = sceneManager;
			_renderWindow = renderWindow;
			_rootSceneNode = rootSceneNode;
			_doMaterials = doMaterials;
			_forceShadowBuffers = forceShadowBuffers;
			_groupName = groupName;

			ProcessEnvironment(rootXmlNode.SelectSingleNode("environment"));
			ProcessExternals(rootXmlNode.SelectSingleNode("externals"));
			ProcessNodes(rootXmlNode.SelectSingleNode("nodes"), _rootSceneNode);
            ProcessTerrain(_rootSceneNode, rootXmlNode.SelectSingleNode("terrain"));

		}

		#region Private Methods

		/// <summary>
		/// selected node environment (rootXmlNode.SelectSingleNode("environment"))
		/// </summary>
		/// <param name="environmentNode">XmlNode use rootXmlNode.SelectSingleNode("environment") </param>
		private void ProcessEnvironment(XmlNode environmentNode)
		{
			if (environmentNode != null)
            {

                #region SceneManager
                if (VerboseLogging == true)
                {
                    LogManager.Instance.Write("Info: Starting 'scenemanager' Axiom.Components.DotScene.");
                }

                XmlNode sceneManagerNode = environmentNode.SelectSingleNode("scenemanager");
                if (sceneManagerNode != null)
                {
                    ProcessSceneManager(sceneManagerNode);
                }
                else
                {
                    LogManager.Instance.Write("No Manager In File Creating Default");
                    _sceneManager = Root.Instance.CreateSceneManager(SceneType.Generic, "DotSceneDefault");
                    _sceneManager.ClearScene();
                }

                // Ambient color
                XmlNode ambientColorNode = environmentNode.SelectSingleNode("colourAmbient");
                _sceneManager.AmbientLight = DotSceneXmlUtility.RetrieveColor(ambientColorNode, ColorEx.Black);

                // Background color
                XmlNode backgroundColorNode = environmentNode.SelectSingleNode("colourBackground");
                ColorEx backgroundColor = DotSceneXmlUtility.RetrieveColor(backgroundColorNode, ColorEx.Black);

                if (VerboseLogging == true)
                {
                    LogManager.Instance.Write("Info: Completed 'scenemanager' Axiom.Components.DotScene.");
                }
                #endregion

                #region Camera and Viewport
                // Camera                
                if (VerboseLogging == true)
                {
                    LogManager.Instance.Write("Info: Starting 'Camera' Axiom.Components.DotScene.");
                }
                XmlNode cameraNode = environmentNode.SelectSingleNode("camera");
                if (cameraNode != null)
                {
                    ProcessCamera(null, cameraNode);
                }
                else
                {
                    Camera camera = _sceneManager.CreateCamera("defaultcamera");
                    camera.Position = new Vector3(128, 25, 128);
                    camera.LookAt(new Vector3(0, 0, 0));
                    camera.Near = 1;
                    camera.Far = 384;
                    _firstCamera = camera;
                }
                ProcessViewport(environmentNode.SelectSingleNode("viewport"));
                //for (int viewportIndex = 0; viewportIndex < _renderWindow.ViewportCount; viewportIndex++)
                //{
                //    _renderWindow.GetViewport(viewportIndex).BackgroundColor = backgroundColor;
                //}
                #endregion

                #region Sky and Fog
                // SkyBox
                XmlNode skyBoxNode = environmentNode.SelectSingleNode("skyBox");
                if (skyBoxNode != null)
                {
                    ProcessSkyBox(skyBoxNode);
                }

                // SkyDome
                XmlNode skyDomeNode = environmentNode.SelectSingleNode("skyDome");
                if (skyDomeNode != null)
                {
                    ProcessSkyDome(skyDomeNode);
                }

                // SkyPlane
                XmlNode skyPlaneNode = environmentNode.SelectSingleNode("skyPlane");
                if (skyPlaneNode != null)
                {
                    ProcessSkyPlane(skyPlaneNode);
                }

                // Fog
                XmlNode fogNode = environmentNode.SelectSingleNode("fog");
                if (fogNode != null)
                {
                    ProcessFog(fogNode);
                }
                #endregion
            }
			else
			{
				LogManager.Instance.Write("Info: no 'Environment' Axiom.Components.DotScene.");
			}
		}
		private void ProcessViewport(XmlNode ViewportNode)
		{
			Viewport viewport = _renderWindow.AddViewport(_firstCamera);
			if (ViewportNode != null)
			{
				XmlNode viewportColorNode = ViewportNode.SelectSingleNode("colour");
				viewport.BackgroundColor = DotSceneXmlUtility.RetrieveColor(viewportColorNode, ColorEx.Black);
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="externalsNode"></param>
		private void ProcessExternals(XmlNode externalsNode)
		{
			if (externalsNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", externalsNode.Name);
		}

		/// <summary>
		/// Loads a skybox
		/// </summary>
		/// <param name="skyBoxNode"></param>
		private void ProcessSkyBox(XmlNode skyBoxNode)
		{

			// Material name
			string materialName = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyBoxNode, "material", "BaseWhite");

			// Distance
            float distance = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyBoxNode, "distance", 5000f);

			// Drawfirst
			bool drawFirst = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyBoxNode, "drawFirst", true);

			// Rotation
			Quaternion rotation;
			XmlNode rotationNode = skyBoxNode.Attributes["quaternion"];
			if (rotationNode != null)
				rotation = DotSceneXmlUtility.RetrieveQuaternion(rotationNode, Quaternion.Identity);
			else
			{
				rotationNode = skyBoxNode.Attributes["rotation"];
                if (rotationNode != null)
                {
                    rotation = DotSceneXmlUtility.RetrieveRotation(rotationNode, Quaternion.Identity);
                }
                else
                {
                    rotation = new Quaternion();
                }
			}

			// Set the sky
			_sceneManager.SetSkyBox(true, materialName, distance, drawFirst, rotation, _groupName);

		}

		/// <summary>
		/// Loads a skydome
		/// </summary>
		/// <param name="skyDomeNode"></param>
		private void ProcessSkyDome(XmlNode skyDomeNode)
		{

			// Material name
			string materialName = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyDomeNode, "material", "BaseWhite");

			// Distance
			float distance = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyDomeNode, "distance", 4000.0f);


			// Drawfirst
			bool drawFirst = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyDomeNode, "drawFirst", true);

			// Rotation
			Quaternion rotation;
			XmlNode rotationNode = skyDomeNode.Attributes["quaternion"];
			if (rotationNode != null)
				rotation = DotSceneXmlUtility.RetrieveQuaternion(rotationNode, Quaternion.Identity);
			else
			{
				rotationNode = skyDomeNode.Attributes["rotation"];
				rotation = DotSceneXmlUtility.RetrieveRotation(rotationNode, Quaternion.Identity);
			}

			// Curvature
			float curvature = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyDomeNode, "curvature", 10.0f);

			// Tiling
			float tiling = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyDomeNode, "tiling", 8.0f);

			// Set the sky
			_sceneManager.SetSkyDome(true, materialName, curvature, tiling, distance, drawFirst, rotation, _groupName);
			
		}

		/// <summary>
		/// loads a sky plane
		/// </summary>
		/// <param name="skyPlaneNode"></param>
		private void ProcessSkyPlane(XmlNode skyPlaneNode)
		{

			// Material name
			string materialName = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "material", "BaseWhite");

			// Drawfirst
			bool drawFirst = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "drawFirst", false);

			// PlaneX
			float planeX = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "planeX", 0.0f);

			// PlaneY
			float planeY = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "planeY", -1.0f);

			// PlaneZ
			float planeZ = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "planeZ", -0.0f);

			// PlaneD
			float planeD = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "planeD", 500f);

			// Scale
			float scale = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "scale", 1000.0f);

			// Tiling
			float tiling = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "tiling", 10.0f);

			// Bow
			float bow = DotSceneXmlUtility.RetrieveXmlAttributeValue(skyPlaneNode, "bow", 0.0f);

			// Set the sky
			_sceneManager.SetSkyPlane(true, new Plane(new Vector3(planeX, planeY, planeZ), planeD), materialName, scale, tiling, drawFirst, bow, _groupName);

		}

		private void ProcessFog(XmlNode fogNode)
		{

			// Exponential density
			float exponentialDensity = DotSceneXmlUtility.RetrieveXmlAttributeValue(fogNode, "exponentialDensity", 0.0f);

			// Linear start
			float linearStart = DotSceneXmlUtility.RetrieveXmlAttributeValue(fogNode, "linearStart", 0.0f);

			// Linear end
			float linearEnd = DotSceneXmlUtility.RetrieveXmlAttributeValue(fogNode, "linearEnd", 0.0f);

			// Diffuse color
			XmlNode diffuseColorNode = fogNode.SelectSingleNode("colourDiffuse");
			ColorEx diffuseColor = DotSceneXmlUtility.RetrieveColor(diffuseColorNode, ColorEx.White);

			// Fog mode
			FogMode fogMode;

			switch (DotSceneXmlUtility.RetrieveXmlAttributeValue(fogNode, "mode", String.Empty))
			{

				case "exp":
					fogMode = FogMode.Exp;
					break;
				case "exp2":
					fogMode = FogMode.Exp2;
					break;
				case "linear":
					fogMode = FogMode.Linear;
					break;
				default:
					fogMode = FogMode.None;
					break;
			}


			// Set the fog
			_sceneManager.SetFog(fogMode, diffuseColor, exponentialDensity, linearStart, linearEnd);

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nodesRootNode"></param>
		/// <param name="parentSceneNode"></param>
		private void ProcessNodes(XmlNode nodesRootNode, SceneNode parentSceneNode)
		{

			if (nodesRootNode == null) return;

			foreach (XmlNode childNode in nodesRootNode.ChildNodes)
			{
				switch (childNode.Name)
				{
					case "position":
						parentSceneNode.Position = DotSceneXmlUtility.RetrieveVector3(childNode);
						parentSceneNode.SetInitialState();
						break;
					case "quaternion":
						parentSceneNode.Orientation = DotSceneXmlUtility.RetrieveQuaternion(childNode);
						parentSceneNode.SetInitialState();
						break;
					case "rotation":
						parentSceneNode.Orientation = DotSceneXmlUtility.RetrieveRotation(childNode);
						parentSceneNode.SetInitialState();
						break;
					case "scale":
						parentSceneNode.Scale = DotSceneXmlUtility.RetrieveVector3(childNode);
						parentSceneNode.SetInitialState();
						break;
					case "node":
						ProcessNode(parentSceneNode, childNode);
						break;
				}
			}

		}

		private float AnimLength(XmlNode nodeToProcess)
		{
			return DotSceneXmlUtility.RetrieveXmlAttributeValue(nodeToProcess, "length");
		}

		private InterpolationMode AnimInterpolationMode(XmlNode nodeToProcess)
		{
			InterpolationMode ip = InterpolationMode.Spline;

			if (nodeToProcess.Attributes["interpolationMode"].Value == "linear" ||
				nodeToProcess.Attributes["rotationInterpolationMode"].Value == "linear")
				ip = InterpolationMode.Linear;
			return ip;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneNode"></param>
		/// <param name="nodeToProcess"></param>
		/// <param name="track"></param>
		private void ProcessKeyframe(SceneNode sceneNode, XmlNode nodeToProcess, AnimationTrack track)
		{
			float time = DotSceneXmlUtility.RetrieveXmlAttributeValue(nodeToProcess, "time");
			TransformKeyFrame frame = (TransformKeyFrame)track.CreateKeyFrame(time);

			foreach (XmlNode node in nodeToProcess.ChildNodes)
			{
				switch (node.Name)
				{
					case "translation":
						frame.Translate = DotSceneXmlUtility.RetrieveVector3(node);
						break;
					case "quaternion":
						frame.Rotation = DotSceneXmlUtility.RetrieveQuaternion(node);
						break;
					case "rotation":
						frame.Rotation = DotSceneXmlUtility.RetrieveRotation(node);
						break;
					case "scale":
						frame.Scale = DotSceneXmlUtility.RetrieveVector3(node);
						break;
				}
			}
		}

		private void ProcessAnimation(SceneNode sceneNode, XmlNode nodeToProcess)
		{
			string name = DotSceneXmlUtility.RetrieveOrGenerateNodeName(nodeToProcess);

			Animation animation = _sceneManager.CreateAnimation(name, AnimLength(nodeToProcess));
			animation.InterpolationMode = AnimInterpolationMode(nodeToProcess);
			AnimationTrack track = animation.CreateNodeTrack(0, sceneNode);

			// keyframes
			foreach (XmlNode node in nodeToProcess.ChildNodes)
			{
				if (node.Name == "keyframe")
					ProcessKeyframe(sceneNode, node, track);
			}
			AnimationState animationState = _sceneManager.CreateAnimationState(name);
			animationState.IsEnabled = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sceneNode"></param>
		/// <param name="nodeToProcess"></param>
		private void ProcessAnimations(SceneNode sceneNode, XmlNode nodeToProcess)
		{
			// animations
			foreach (XmlNode node in nodeToProcess.ChildNodes)
			{
				if (node.Name == "animation")
					ProcessAnimation(sceneNode, node);
			}

		}

		/// <summary>
		/// Processes nodes in .Scene tree
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="nodeToProcess"></param>
		private void ProcessNode(SceneNode parentSceneNode, XmlNode nodeToProcess)
		{
            if (parentSceneNode == null)
            {
                parentSceneNode = _sceneManager.RootSceneNode;
            }
			SceneNode newSceneNode = parentSceneNode.CreateChildSceneNode(DotSceneXmlUtility.RetrieveOrGenerateNodeName(nodeToProcess));

			// Process through PRS transformations first
			// List<XmlNode> stillToProcessNodeList = new List<XmlNode>();
			foreach (XmlNode node in nodeToProcess.ChildNodes)
			{
				switch (node.Name)
				{
					case "position":
						newSceneNode.Position = DotSceneXmlUtility.RetrieveVector3(node);
						break;
					case "quaternion":
						newSceneNode.Orientation = DotSceneXmlUtility.RetrieveQuaternion(node);
						break;
					case "rotation":
						newSceneNode.Orientation = DotSceneXmlUtility.RetrieveRotation(node);
						break;
					case "scale":
						newSceneNode.Scale = DotSceneXmlUtility.RetrieveVector3(node);
						break;
					case "entity":
						ProcessEntity(newSceneNode, node);
						break;
					case "light":
						ProcessLight(newSceneNode, node);
						break;
					case "camera":
						ProcessCamera(newSceneNode, node);
						break;
                    case "trackTarget":
                        ProcessTrackTarget(newSceneNode, node);
						break;
                    case "billboardSet":
                        ProcessBillboardSet(newSceneNode, node);
                        break;
                    case "plane":
                        ProcessPlane(newSceneNode, node);
                        break;
                    case "particleSystem":
                        ProcessParticleSystem(newSceneNode, node);
                        break;
					case "node":
						// Note: Recursive call
						ProcessNode(newSceneNode, node);
						break;
					case "animations":
						ProcessAnimations(newSceneNode, node);
						break;
				}
			}

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="entityNode"></param>
		private void ProcessEntity(SceneNode parentSceneNode, XmlNode entityNode)
		{

			string entityNodeName = DotSceneXmlUtility.RetrieveOrGenerateNodeName(entityNode);

			XmlAttribute meshNameAttribute = entityNode.Attributes["meshFile"];

			if (meshNameAttribute != null)
			{
				XmlNode vertexBufferNode = entityNode.SelectSingleNode("vertexBuffer");
				XmlNode indexBufferNode = entityNode.SelectSingleNode("indexBuffer");
				if (_forceShadowBuffers || (vertexBufferNode == null && indexBufferNode == null))
				{
					MeshManager.Instance.Load(meshNameAttribute.Value, _groupName);
				}
				else
				{
					// TODO: Load the vertex and index buffers
				}
				Entity newEntity = _sceneManager.CreateEntity(entityNodeName, meshNameAttribute.Value);
				parentSceneNode.AttachObject(newEntity);
			}

		}

		private void ProcessLight(SceneNode parentSceneNode, XmlNode lightNode)
		{
			if (lightNode != null)
			{
				string lightNodeName =  DotSceneXmlUtility.RetrieveOrGenerateNodeName(lightNode);
				Light newLight = _sceneManager.CreateLight(lightNodeName);
				parentSceneNode.AttachObject(newLight);

				// Visibility
				newLight.IsVisible = DotSceneXmlUtility.RetrieveXmlAttributeValue(lightNode, "visible", true);

				// Diffuse color
				XmlNode diffuseColorNode = lightNode.SelectSingleNode("colourDiffuse");
				newLight.Diffuse = DotSceneXmlUtility.RetrieveColor(diffuseColorNode, newLight.Diffuse);

				// Specular color
				XmlNode specularColorNode = lightNode.SelectSingleNode("colourSpecular");
				newLight.Specular = DotSceneXmlUtility.RetrieveColor(specularColorNode, newLight.Specular);

				// Light type
				string lightType = DotSceneXmlUtility.RetrieveXmlAttributeValue(lightNode, "type", "point");

				switch (lightType)
				{
					case "point":
						newLight.Type = LightType.Point;
						break;
					case "directional":
					case "targetDirectional":
						newLight.Type = LightType.Directional;
						break;
					case "spot":
					case "targetSpot":
						newLight.Type = LightType.Spotlight;
						break;
				}

				// Attenuation
				XmlNode attenuationNode = lightNode.SelectSingleNode("lightAttenuation");
				if (attenuationNode != null)
				{
					float attenuationRange = DotSceneXmlUtility.RetrieveXmlAttributeValue(attenuationNode, "range", DefaultLightAttenuation);
					float attenuationConstant = DotSceneXmlUtility.RetrieveXmlAttributeValue(attenuationNode, "constant", 0.0f);
					float attenuationLinear = DotSceneXmlUtility.RetrieveXmlAttributeValue(attenuationNode, "linear", 0.0f);
					float attenuationQuadratic = DotSceneXmlUtility.RetrieveXmlAttributeValue(attenuationNode, "quadratic", 0.0f);
					newLight.SetAttenuation(attenuationRange, attenuationConstant, attenuationLinear, attenuationQuadratic);
				}

				// Range
				XmlNode rangeNode = lightNode.SelectSingleNode("lightRange");
				if (rangeNode != null)
				{
					float inner = DotSceneXmlUtility.RetrieveXmlAttributeValue(rangeNode, "inner", 0.0f);
					float outer = DotSceneXmlUtility.RetrieveXmlAttributeValue(rangeNode, "outer", 0.0f);
					float falloff = DotSceneXmlUtility.RetrieveXmlAttributeValue(rangeNode, "falloff", 0.0f);
					newLight.SetSpotlightRange(inner, outer, falloff);
				}

				// Direction/normal
				XmlNode normalNode = lightNode.SelectSingleNode("normal");
				if (normalNode != null)
				{
					newLight.Direction = DotSceneXmlUtility.RetrieveVector3(normalNode);
				}

				// Position
				XmlNode positionNode = lightNode.SelectSingleNode("position");
				if (positionNode != null)
				{
					newLight.Position = DotSceneXmlUtility.RetrieveVector3(positionNode);
				}
			}
		}

		/// <summary>
		/// Loads the Camera
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="cameraNode"></param>
		private void ProcessCamera(SceneNode parentSceneNode, XmlNode cameraNode)
		{

			if (cameraNode != null)
			{
				string name = DotSceneXmlUtility.RetrieveOrGenerateNodeName(cameraNode);
				Camera camera = _sceneManager.CreateCamera(name);

                if (_firstCamera == null)
                {
                    _firstCamera = camera;
                }

                if (parentSceneNode == null)
                {
                    _sceneManager.RootSceneNode.AttachObject(camera);
                }
                else
                {
                    parentSceneNode.AttachObject(camera);
                }

                XmlAttribute polyModeAttribute = cameraNode.Attributes["polyMode"];
                if (polyModeAttribute != null)
                {
                    camera.PolygonMode = (PolygonMode)DotSceneXmlUtility.RetrieveXmlAttributeValue(cameraNode, "polyMode", 2);
                }
                
				// Clipping
				XmlNode clippingNode = cameraNode.SelectSingleNode("clipping");
				if (clippingNode != null)
				{
					camera.Near = DotSceneXmlUtility.RetrieveXmlAttributeValue(clippingNode, "near", camera.Near);
                    camera.Far = DotSceneXmlUtility.RetrieveXmlAttributeValue(clippingNode, "far", camera.Far);
				}

				// FOV
				XmlAttribute fovAttribute = cameraNode.Attributes["fov"];
				if (fovAttribute != null)
				{
					camera.FieldOfView = DotSceneXmlUtility.RetrieveXmlAttributeValue(cameraNode, "fov", 45);
				}


                //Use Rendering Distance
                camera.UseRenderingDistance = DotSceneXmlUtility.RetrieveXmlAttributeValue(cameraNode, "useRenderingDistance", true);

				// AspectRatio
				camera.AspectRatio = DotSceneXmlUtility.RetrieveXmlAttributeValue(cameraNode, "aspectRatio", camera.AspectRatio);

				// Projection type
				string type = DotSceneXmlUtility.RetrieveXmlAttributeValue(cameraNode, "projectionType", string.Empty);
				if (type == "perspective")
				{
					camera.ProjectionType = Projection.Perspective;
				}
                else if (type == "orthographic")
				{
					camera.ProjectionType = Projection.Orthographic;
				}

				// Direction/normal
				XmlNode normalNode = cameraNode.SelectSingleNode("normal");
				if (normalNode != null)
				{
					camera.Direction = DotSceneXmlUtility.RetrieveVector3(normalNode);
				}

				// Position
				XmlNode positionNode = cameraNode.SelectSingleNode("position");
				if (positionNode != null)
				{
					camera.Position = DotSceneXmlUtility.RetrieveVector3(positionNode);
				}

				// rotation
				XmlNode rotationNode = cameraNode.SelectSingleNode("rotation");
				if (rotationNode != null)
				{
					camera.Orientation = (DotSceneXmlUtility.RetrieveRotation(rotationNode));
				}

                camera.LookAt(_rootSceneNode.Position);

			}
		}


		/// <summary>
		/// Loads a Terrain as specified with Axiom.Components.Terrain
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="terrainNode"></param>
		private void ProcessTerrain( SceneNode parentSceneNode , XmlNode terrainNode)
		{
			if (terrainNode != null)
			{
				// Read from .scene 
				float worldSize = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "worldSize");
				ushort mapSize = (ushort)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "mapSize");
				bool colourmapEnabled = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "colourmapEnabled", false);
				int colourMapTextureSize = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "colourMapTextureSize");
				int compositeMapDistance = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "tuningCompositeMapDistance");
				int maxPixelError = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "tuningMaxPixelError");
				int minBatchSize = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "tuningMinBatchSize");
				int maxBatchSize = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainNode, "tuningMaxBatchSize");

				MaterialManager.Instance.SetDefaultTextureFiltering(TextureFiltering.Anisotropic);
				MaterialManager.Instance.DefaultAnisotropy = 7;

				Vector3 lightDir = new Vector3(0.55f, 0.3f, 0.75f);
				lightDir.Normalize();

				Light l = _sceneManager.CreateLight("dsl_Light");
				l.Type = LightType.Directional;
				l.Direction = lightDir;
				l.Diffuse = ColorEx.White;
				l.Specular = new ColorEx(0.4f, 0.4f, 0.4f);

				_sceneManager.AmbientLight = new ColorEx(0.8f, 0.8f, 0.8f);

				TerrainGlobalOptions.MaxPixelError = maxPixelError;
				TerrainGlobalOptions.CompositeMapDistance = compositeMapDistance;
				TerrainGlobalOptions.LightMapDirection = l.Direction;
				TerrainGlobalOptions.CompositeMapAmbient = _sceneManager.AmbientLight;
				TerrainGlobalOptions.CompositeMapDiffuse = l.Diffuse;

				_terrainGroup = new TerrainGroup(_sceneManager, Alignment.Align_X_Z, (ushort)mapSize, worldSize);
				_terrainGroup.Origin = Vector3.Zero;

				_terrainGroup.ResourceGroup = "General";

				foreach (XmlNode terrainPageNode in terrainNode.FirstChild.ChildNodes)
				{
					ProcessTerrainPage(terrainPageNode);
				}

                Axiom.Components.Terrain.Terrain terrain = new Components.Terrain.Terrain(_sceneManager);
				_terrainGroup.LoadAllTerrains(true);

                Axiom.Components.Terrain.Terrain t = _terrainGroup.GetTerrain(0, 0);
 

				_terrainGroup.FreeTemporaryResources();

			}           
		}

		/// <summary>
		/// Loads the terrain page
		/// </summary>
		/// <param name="terrainPageNode"></param>
		private void ProcessTerrainPage(XmlNode terrainPageNode)
		{
			string name = DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainPageNode, "name", "");
			int pageX = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainPageNode, "pageX");
			int pageY = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(terrainPageNode, "pageY");

			if (ResourceGroupManager.Instance.ResourceExists("General", name))
			{
				_terrainGroup.DefineTerrain(pageX, pageY, name);
			}
		}

		/// <summary>
		/// loads the UserDataReference Node
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="userDataReferenceNode"></param>
		private void ProcessUserDataReference(SceneNode parentSceneNode, XmlNode userDataReferenceNode)
		{

			if (userDataReferenceNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", userDataReferenceNode.Name);
		}

		/// <summary>
		/// Loads the octree Node
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="octreeNode"></param>
		private void ProcessOctree(SceneNode parentSceneNode, XmlNode octreeNode)
		{

			if (octreeNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", octreeNode.Name);

		}

		/// <summary>
		/// Loads Particle System Node
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="particleSystemNode"></param>
		private void ProcessParticleSystem(SceneNode parentSceneNode, XmlNode particleSystemNode)
		{
			if (particleSystemNode != null)
			{
				try
				{
					string name = DotSceneXmlUtility.RetrieveXmlAttributeValue(particleSystemNode, "name", "");
					string file = DotSceneXmlUtility.RetrieveXmlAttributeValue(particleSystemNode, "file", "");

					ParticleSystem particleSystem = ParticleSystemManager.Instance.CreateSystem(name, file);
				}
				catch (Exception ex)
				{
					LogManager.Instance.Write("Error: The <{0}> cannot be created Axiom.Components.DotScene. Error {}", particleSystemNode.Name, ex.Message );
				}

			}
		}

		/// <summary>
		/// Loads a Billboard Set
		/// </summary>
		/// <param name="parentSceneNode"></param>
		/// <param name="billboardSetNode"></param>
		private void ProcessBillboardSet(SceneNode parentSceneNode, XmlNode billboardSetNode)
		{

			if (billboardSetNode != null) 
				{
					string name = DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "name", "");
					//id						ID					#IMPLIED	
					int poolSize = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "poolSize");	
					bool autoextend = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "autoextend", true);
					string materialName = DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "materialName", "");
					float defaultWidth = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "defaultWidth");	
					float defaultHeight = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "defaultHeight");	
					BillboardType billboardType = (BillboardType)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "billboardType");
					BillboardOrigin billboardOrigin = (BillboardOrigin)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "billboardOrigin");
					BillboardRotationType billboardRotationType = (BillboardRotationType)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "billboardRotationType");                    
					bool sortingEnabled = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "sortingEnabled", false);
					bool cullIndividually = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "cullIndividually", false);
					bool accurateFacing = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "accurateFacing", false);
					bool billboardsInWorldSpace = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "billboardsInWorldSpace", false);
					bool pointRenderingEnabled = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "pointRenderingEnabled", false);

					BillboardSet billboardSet = _sceneManager.CreateBillboardSet( name, poolSize );
					billboardSet.AutoExtend = autoextend;
					billboardSet.MaterialName = materialName;
					billboardSet.BillboardType = billboardType;
					billboardSet.BillboardOrigin = billboardOrigin;
					billboardSet.BillboardRotationType = billboardRotationType;
					
					//TODO Check and Test, guessed on placement in node tree
					foreach (XmlNode billBoards in billboardSetNode.FirstChild.ChildNodes)
					{
						//id						ID					#IMPLIED	
						float rotation = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "rotation");	
						float width = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "width");	
						float height = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(billboardSetNode, "height");
						
						//TODO Check and test placement Vector3
						Billboard billBoard = new Billboard(billboardSet.WorldPosition, billboardSet);
						billBoard.Rotation = rotation;
						billBoard.Width = width;
						billBoard.Height = height;

					}

				}

		}

		private void ProcessPlane(SceneNode parentSceneNode, XmlNode planeNode)
		{

			if (planeNode != null)
			{

				string name = DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "name", "");
				//id						ID					#IMPLIED	
				int normalX = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "normalX");
				int normalY = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "normalY");
				int normalZ = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "normalZ");
				float distance = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "distance");
				float width = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "width");
				float height = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "height");
				int xSegments = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "xSegments", 1);
				int ySegments = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "ySegments", 1);
				int numTexCoordSets = (int)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "numTexCoordSets");
				float uTile = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "uTile", 1);
				float vTile = (float)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "vTile", 1);
				string materialName = DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "material", "");
				bool hasNormals = (bool)DotSceneXmlUtility.RetrieveXmlAttributeValue(planeNode, "hasNormals", true);

				//TODO Check and Test, guessed on placement in node tree
				Vector3 normal = DotSceneXmlUtility.RetrieveVector3(planeNode.FirstChild);
				Vector3 up = DotSceneXmlUtility.RetrieveVector3(planeNode.FirstChild);

				Plane plane = new Plane(normal, distance);
				MeshManager.Instance.CreatePlane(name, ResourceGroupManager.DefaultResourceGroupName, plane, width, height, xSegments, ySegments, hasNormals, numTexCoordSets, uTile, vTile, up);

				Entity planeEnt = _sceneManager.CreateEntity("Plane", name);
				planeEnt.MaterialName = materialName;

				parentSceneNode.AttachObject(planeEnt);
			}

		}

		private void ProcessTrackTarget(SceneNode parentSceneNode, XmlNode trackTargetNode)
		{
			if (trackTargetNode != null)
			{
				string nodeName = DotSceneXmlUtility.RetrieveXmlAttributeValue(trackTargetNode, "nodeName", "");

				Vector3 localDirection = Vector3.NegativeUnitZ;
				if (trackTargetNode.FirstChild != null)
				{
					DotSceneXmlUtility.RetrieveVector3(trackTargetNode.FirstChild);
				}
				Vector3 offset = Vector3.Zero;
				if (trackTargetNode.FirstChild != null)
				{
					DotSceneXmlUtility.RetrieveVector3(trackTargetNode.FirstChild);
				}

				try
				{
					SceneNode trackNode = _sceneManager.GetSceneNode(nodeName);
					if (trackNode != null)
					{
						parentSceneNode.SetAutoTracking(true, trackNode, localDirection, offset);

					}
				}
				catch
				{
					LogManager.Instance.Write("Warning: The track target cannot be loaded by Axiom.Components.DotScene. <{0}>", trackTargetNode.InnerXml);
				}

			}
		}

		private void ProcessShadowSettings(SceneNode parentSceneNode, XmlNode shadowSettingsNode)
		{

			if (shadowSettingsNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", shadowSettingsNode.Name);

		}

		private void ProcessSceneManager(XmlNode sceneManagerNode)
		{

			if (sceneManagerNode != null)
			{
				if (VerboseLogging == true)
				{
					LogManager.Instance.Write("Info: Creating 'scenemanager' Axiom.Components.DotScene.");
				}
				string name = DotSceneXmlUtility.RetrieveXmlAttributeValue(sceneManagerNode, "name", "DefaultSceneManager");
				string type = DotSceneXmlUtility.RetrieveXmlAttributeValue(sceneManagerNode, "type", "DefaultSceneManager");

				if (VerboseLogging == true)
				{
					LogManager.Instance.Write("Info: 'scenemanager' name = '{0}', type = '{1}'.", name, type);
				}

				_sceneManager = Root.Instance.CreateSceneManager(type, name);
                _sceneManager.ClearScene();

				if (VerboseLogging == true)
				{
					LogManager.Instance.Write("Info: creating root scenenode.");
				}


                _rootSceneNode = _sceneManager.RootSceneNode;

			}
			else
			{
				LogManager.Instance.Write("Warning: Missing SceneManager in .scene file.");
			}
		}

		private void ProcessShadowTextureConfig(SceneNode parentSceneNode, XmlNode shadowTextureConfigNode)
		{

			if (shadowTextureConfigNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", shadowTextureConfigNode.Name);

		}

		private void ProcessResourceGroup(XmlNode resourceGroupNode)
		{

			if (resourceGroupNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", resourceGroupNode.Name);

		}

		private void ProcessResourceLocation(XmlNode resourceLocationNode)
		{

			if (resourceLocationNode == null) return;
			LogManager.Instance.Write("Warning: The <{0}> xml node is not supported by Axiom.Components.DotScene.", resourceLocationNode.Name);

		}
		#endregion
	}

}
