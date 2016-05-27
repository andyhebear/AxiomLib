using System;
using System.Collections.Generic;

using Axiom.Core;

namespace Axiom.Component.OpenAsset
{
    public interface IAssetPlugin
    {
        string Name { get; }
        string Version { get; }
        AssetImporterBase AssetImport { get; set; }
        List<string> SupportedFileFormats { get; set; }
        void Initialize();
        void Shutdown();
    }
}
