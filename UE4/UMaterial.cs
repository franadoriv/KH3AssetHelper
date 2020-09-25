using System;
using System.Collections.Generic;
using System.Text;

namespace UE {
    public class UMaterial {
        public string id { get; set; }
        public string name { get; set; }
        public string textureBaseName { get; set; }
        public List<string> texturesList { get; set; }

        public UMaterial(string name="", string textureBaseName = "") {
            this.name = name;
            this.textureBaseName = textureBaseName;
            this.texturesList = new List<string>();
        }
    }
}
