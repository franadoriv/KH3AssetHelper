using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UE;

namespace KH3MapsExporter
{
    public class UMapFile
    {
        public string mapNamespace { get; set; }
        public string name { get; set; }
        public string path { get; set; }
        public bool loaded { get; set; }

        public UAssetParserResult parsedResult { get; set; }
        public override string ToString() { return this.name; }
    }
}
