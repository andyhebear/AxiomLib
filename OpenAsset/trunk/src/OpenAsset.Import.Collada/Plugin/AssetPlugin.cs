using System;
using System.Collections.Generic;
using Axiom.Component.OpenAsset;

namespace OpenAsset.Import.Collada
{
    [AssetPlugInAttribute("This is the Collada Importer")]
    public class AssetPlugin:IAssetPlugin 
    {
        private AssetImporter _AssetImport = new AssetImporter();
        private List<string> _SupportedFileFormats = new List<string>(){"1.5", "1.4", "1.3"};

        #region IAssetPlugin Members

        public string Name
        {
            get{ return "Collada Importer";}
        }

        public string Version
        {
            get{ return "0.0.*"; }
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

        #endregion

        #region IAssetPlugin Members


        public List<string> SupportedFileFormats
        {
            get { return _SupportedFileFormats; }
            set { _SupportedFileFormats = value; }
        }

        #endregion
    }
}
