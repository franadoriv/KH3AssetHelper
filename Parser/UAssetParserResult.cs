using KH3AssetHelper.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using UE.UE4;

namespace UE
{
    public class UAssetParserResult
    {
        public string[] names = new string[] { };
        public string[] expnames = new string[] { };
        public string[] impnames = new string[] { };

        public BlueprintDB blueprintDB;
        public MaterialDB materialDB;

        public List<UAsset> uAssets = new List<UAsset>();

        public ExportHelper exportHelper;

        public UAParserConfig config;

        public UAssetParserResult(ref UAParserConfig config)
        {
            exportHelper = new ExportHelper(this);
            this.config = config;
        }
    }
}
