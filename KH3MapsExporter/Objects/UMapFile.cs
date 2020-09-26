using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KH3MapsExporter
{
    public class UMapFile
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public override string ToString() { return this.Name; }
    }
}
