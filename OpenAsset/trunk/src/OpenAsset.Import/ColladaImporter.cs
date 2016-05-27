using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Axiom.Scripting;

namespace OpenAsset.Import
{
    class ColladaImporter: AssetImporter
    {
        internal ColladaImporter()
        {
            this.LoadingOrder = 100.0f;
            ScriptPatterns.Add( "*.collada" );
        }

        public override void  ParseScript(System.IO.Stream stream, string groupName, string fileName)
        {
         	 base.ParseScript(stream, groupName, fileName);
        }
    }
}
