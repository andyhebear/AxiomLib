using System;
using System.Collections.Generic;
using System.IO;

using Axiom.Core;
using Axiom.Scripting;

namespace Axiom.Component.OpenAsset
{
    public abstract class AssetImporterBase: IScriptLoader
    {
        public float LoadingOrder { get; protected set; }
        public List<string> ScriptPatterns { get; private set; }
        public bool verboseLogging { get; set; } 


        public AssetImporterBase()
        {
            ScriptPatterns = new List<string>();
            verboseLogging = false;
        }

        #region IScriptLoader Members

        public abstract void ParseScript( System.IO.Stream stream, string groupName, string fileName );

        public void ParseScript(string filePath, string groupName, string fileName)
        {

            // A FileStream is needed to read the XML document.
            FileStream fs = new FileStream(filePath, FileMode.Open);
            ParseScript(fs, groupName, fileName);
        }   

        protected void Register()
        {
            ResourceGroupManager.Instance.RegisterScriptLoader(this);
        }

        #endregion
    }
}
