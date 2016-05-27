using System;
using System.Collections.Generic;
using Axiom.Core;
using Axiom.Component.OpenAsset;

namespace Axiom.Component.OpenAsset.DotScene
{
    [AssetPlugInAttribute("This is the DotScene Importer")]
    public class AssetPlugin : IAssetPlugin
    {
        private AssetImporter _AssetImport = new AssetImporter();
        private List<string> _SupportedFileFormats = new List<string>() { "1.0" };

        #region IAssetPlugin Members

        public string Name
        {
            get { return "DotScene Importer"; }
        }

        public string Version
        {
            get { return "0.0.*"; }
        }


        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }

        public AssetImporterBase AssetImport
        {
            get { return (AssetImporterBase)_AssetImport; }
            set { _AssetImport = (AssetImporter)value; }
        }

        public List<string> SupportedFileFormats
        {
            get { return _SupportedFileFormats; }
            set { _SupportedFileFormats = value; }
        }

        #endregion
    }
}
