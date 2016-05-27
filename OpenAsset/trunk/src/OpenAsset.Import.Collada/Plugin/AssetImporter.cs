using System;

using Axiom.Component.OpenAsset;

namespace Axiom.Component.OpenAsset
{
    public class AssetImporter: AssetImporterBase
    {
        internal AssetImporter()
        {
            this.LoadingOrder = 100.0f;
            ScriptPatterns.Add("*.collada");
            ScriptPatterns.Add("*.dae");
            base.Register();
        }

        public override void  ParseScript(System.IO.Stream stream, string groupName, string fileName)
        {

            throw new NotImplementedException();
        }

    }
}
