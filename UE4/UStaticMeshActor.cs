using APPLIB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace UE {
    public class UStaticMeshActor : UActor {


        internal string _name = "";
        
        public string name { get {
                return String.IsNullOrEmpty(this._name) ? this.staticMeshFile : this.name;
            } set {
                this._name = value;
            } }

        public List<string> materials { get; set; }

        public string staticMeshFile { get; set; }

        public UStaticMeshActor(string name = "", string staticMeshFile = "") {
            this.name = name;
            this.staticMeshFile = staticMeshFile;
            this.materials = new List<string>();
            this.staticMeshFile = staticMeshFile;
        }


    }
}
