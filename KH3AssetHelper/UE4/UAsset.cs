using System;
using System.Collections.Generic;
using System.Text;

namespace UE {
    public class UAsset
    {

        public string name { get; set; }

        public List<UActor> staticSkeletalMeshActorList { get; set; }
        public List<UStaticMeshActor> staticMeshActorList { get; set; }
        public List<UActor> lightActorList { get; set; }
        public List<ULand> landList { get; set; }
        public Dictionary<string, UMaterial> uMaterials { get; set; }

        internal List<string> staticMeshNameList { get; set; }
        internal List<string> SkeletalMeshNameList { get; set; }

        public UAsset(string name)
        {
            this.name = name;
            this.staticMeshNameList = new List<string>();
            this.SkeletalMeshNameList = new List<string>();
            this.staticSkeletalMeshActorList = new List<UActor>();
            this.staticMeshActorList = new List<UStaticMeshActor>();
            this.lightActorList = new List<UActor>();
            this.landList = new List<ULand>();
            this.uMaterials = new Dictionary<string, UMaterial>();
        }
    }
}
