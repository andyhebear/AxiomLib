using System;
using System.IO;
using System.Xml;

namespace Axiom.Component.OpenAsset
{
    public abstract class AssetParserBase
    {
        protected Stream _Stream;
        protected string _GroupName;
        protected string _FileName;

        public AssetParserBase(Stream stream, string groupName, string fileName)
        {
            _Stream = stream;
            _GroupName = groupName;
            _FileName = fileName;

            //if (OpenFile())
            //{
            //    ReadContents();
            //}
        }

        ~AssetParserBase()
        {
            //_Stream.Close;
        }

        public abstract void OpenFile();
        public abstract void ReadContents();
    }
}
