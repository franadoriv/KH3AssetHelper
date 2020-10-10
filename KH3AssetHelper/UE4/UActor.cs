// Decompiled with JetBrains decompiler
// Type: UE4.actor
// Assembly: UE4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E2D17942-E9F2-4CD8-8DC3-683E9AA1D216
// Assembly location: C:\Users\Adolfo\Desktop\New folder\kh3.exe

using APPLIB;
using System.Collections.Generic;

namespace UE {

    public class UActor {
        public string id { set; get; }
        public string name { get; set; }
        public Vector3D relativeLocation { get; set; }
        public Vector3D relativeRotation { get; set; }
        public Vector3D relativeScale3D { get; set; }
        public int attachParentDicIdx { get; set; }
        public int attachParent { get; set; }
        public int[] overmat { get; set; }
        internal bool hastrans { get; set; }
        internal float[,] tm { get; set; }

        public UActor(string name = "") {
            this.name = name;
            this.overmat = new int[] { };
            this.relativeLocation = new Vector3D();
            this.relativeRotation = new Vector3D();
            this.relativeScale3D = new Vector3D(1f, 1f, 1f); ;
            this.attachParentDicIdx = -1;
        }

    }
}
