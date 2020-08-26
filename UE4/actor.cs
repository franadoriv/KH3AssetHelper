// Decompiled with JetBrains decompiler
// Type: UE4.actor
// Assembly: UE4, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: E2D17942-E9F2-4CD8-8DC3-683E9AA1D216
// Assembly location: C:\Users\Adolfo\Desktop\New folder\kh3.exe

using APPLIB;

namespace UEKH3
{
  public class actor
  {
    public string meshname;
    public Vector3D pos = new Vector3D();
    public Vector3D rot = new Vector3D();
    public Vector3D scale = new Vector3D(1f, 1f, 1f);
    public int parent = -1;
    public int[] overmat;
    public bool hastrans;
    public float[,] tm;
  }
}
