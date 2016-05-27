using System;
using System.Xml.Serialization;


using Axiom.Core;
using Axiom.Scripting;
using Axiom.Component.OpenAsset;

namespace Axiom.Component.OpenAsset
{
    public class AssetImporter : AssetImporterBase
    {

        private string _fileName = "";
        private string _groupName = "";
        private scene loadScene = null;
        
        internal AssetImporter()
        {
            this.LoadingOrder = 100.0f;
            ScriptPatterns.Add("*.scene");
            base.Register();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="groupName"></param>
        /// <param name="fileName"></param>
        public override void ParseScript(System.IO.Stream stream, string groupName, string fileName)
        {
            _fileName = fileName;
            _groupName = groupName;

            /* Create serializer */
            XmlSerializer serializer = new XmlSerializer(typeof(scene));

            /* If the XML document has been altered with unknown 
            nodes or attributes, handle them with the 
            UnknownNode and UnknownAttribute events.*/
            serializer.UnknownNode += new
            XmlNodeEventHandler(serializer_UnknownNode);

            serializer.UnknownAttribute += new
            XmlAttributeEventHandler(serializer_UnknownAttribute);

            /* Deserialize */ 
            scene loadScene = (scene)serializer.Deserialize(stream);
            // Make Scene manager here
            if (loadScene != null)
            {
                if (loadSceneManager())
                {
                    loadEnvironment();
                    loadAnimations();
                    loadAnimationStates();
                    loadCamera();
                    loadLight();
                    loadNodes();
                    loadOctree();
                    loadRenderTargets();
                    loadTerrain();
                    loadViewports();

                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serializer_UnknownNode(object sender, XmlNodeEventArgs e)
        {
            LogManager.Instance.Write("OpenAsset(DotScene) found an Unknown Node:" + e.Name + "\t" + e.Text + " in file '" + _fileName + "'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serializer_UnknownAttribute
        (object sender, XmlAttributeEventArgs e)
        {
            System.Xml.XmlAttribute attr = e.Attr;
            LogManager.Instance.Write("OpenAsset(DotScene) found an Unknown attribute " +
            attr.Name + "='" + attr.Value + "' in file '" + _fileName + "'");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool loadSceneManager() 
        {
            if (loadScene.environment != null)
            {
                //TO DO GET IT IN
                return true;
            }
            else
            {
                LogManager.Instance.Write("OpenAsset(DotScene) missing Environment in file '" + _fileName + "'");
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void loadEnvironment()
        {
            if (loadScene.environment != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                    LogManager.Instance.Write("OpenAsset(DotScene) missing Environment in file '" + _fileName + "'");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void loadAnimationStates()
        {
            if (loadScene.animationStates != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no Animation States in file '" + _fileName + "'");
                }
            }

        }
        
        /// <summary>
        /// 
        /// </summary>
        public void loadAnimations()
        {
            if (loadScene.animations != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no Animations in file '" + _fileName + "'");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void loadCamera()
        {
            if (loadScene.camera != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                LogManager.Instance.Write("OpenAsset(DotScene) no Cameras in file '" + _fileName + "'");
            }

        }        
        
        public void loadLight()
        {
            if (loadScene.light != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no Lights in file '" + _fileName + "'");
                }
            }

        }
                
        /// <summary>
        /// 
        /// </summary>
        public void loadNodes()
        {
            if (loadScene.nodes != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                    LogManager.Instance.Write("OpenAsset(DotScene) no nodes in file '" + _fileName + "'");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>        
        public void loadOctree()
        {
            if (loadScene.octree != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no octree in file '" + _fileName + "'");
                }
            }

        }

        public void loadRenderTargets()
        {
            if (loadScene.renderTargets != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no RenderTargets in file '" + _fileName + "'");
                }
            }
            
        }

        /// <summary>
        /// 
        /// </summary>
        public void loadTerrain()
        {
            if (loadScene.terrain != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                LogManager.Instance.Write("OpenAsset(DotScene) no terrain in file '" + _fileName + "'");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        public void loadViewports()
        {
            if (loadScene.viewports != null)
            {
                //TO DO GET IT IN
            }
            else
            {
                if (base.verboseLogging)
                {
                    LogManager.Instance.Write("OpenAsset(DotScene) no veiwports in file '" + _fileName + "'");
                }
            }

        }
    }
}
