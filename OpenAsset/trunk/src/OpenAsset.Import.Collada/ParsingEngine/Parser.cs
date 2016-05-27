using System;
using System.Xml;

using Axiom.Component.OpenAsset;
using System.IO;

namespace Axiom.Component.OpenAsset.Collada
{
    public class Parser:AssetParserBase
    {
        private XmlReader _Reader;
        private XmlNode _RootNode;
        private float _UnitSize = 1.0f;
        private UpDirection _UpDirection = UpDirection.Z;
        private FormatVersion _Format = FormatVersion.v1_5;

		Parser( Stream stream, string groupName, string fileName ) : base(stream, groupName, fileName )
		{}

        public override void OpenFile()
        {
            
        }

        public override void ReadContents()
        {
            throw new NotImplementedException();
        }
    }
}
