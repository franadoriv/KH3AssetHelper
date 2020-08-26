using APPLIB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using UEKH3.UE4;

namespace UEKH3
{
    public class Tools
    {
        private int game = 0;
        private string[] names = (string[])null;
        private string[] expnames = (string[])null;
        private string[] impnames = (string[])null;

        BlueprintDB blueprintDB;
        MaterialDB materialDB;

        public bool exportTextures = false;
        public bool exportLights = false;

        public void LoadDBs(string bpPath = null, string mtPath = null)
        {
            blueprintDB = new BlueprintDB(bpPath);
            materialDB = new MaterialDB(mtPath);
        }

        public Tools(string bpPath = null, string mtPath = null)
        {
            LoadDBs(bpPath, mtPath);

        }

        public void potatoSalada(string path)
        {
            string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            var uassetList = new List<string>();
            if (!string.IsNullOrEmpty(path))
                uassetList.Add(path);
            else
            {
                foreach (FileInfo file in new DirectoryInfo(localPath).GetFiles("*.uasset", SearchOption.AllDirectories))
                    uassetList.Add(file.FullName);
            }
            LoadUAssets(uassetList.ToArray());
        }

        private void LoadUAssets(string[] uassetList, string[] args = null)
        {
            var numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            var mn = new MagicNumbers();
            bool flag1 = false;
            bool dirty = false;
            string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
     
            var hasArgs = false;

            foreach (var uassetPath in uassetList)
            {
                try
                {
                    var uassetFileStream = new FileStream(uassetPath, FileMode.Open, FileAccess.Read);
                    string withoutExtension = Path.GetFileNameWithoutExtension(uassetPath);
                    string str1 = Path.GetDirectoryName(uassetFileStream.Name) + "\\" + Path.GetFileNameWithoutExtension(uassetFileStream.Name);
                    int uexpSize = 0;
                    int ubulkSize = 0;
                    FileStream uexpFileStream = null;
                    FileStream ubulkFileStream = null;

                    if (File.Exists(str1 + ".uexp"))
                    {
                        uexpFileStream = new FileStream(str1 + ".uexp", FileMode.Open, FileAccess.Read);
                        uexpSize = (int)uexpFileStream.Length;
                    }
                    if (File.Exists(str1 + ".ubulk"))
                    {
                        ubulkFileStream = new FileStream(str1 + ".ubulk", FileMode.Open, FileAccess.Read);
                        ubulkSize = (int)ubulkFileStream.Length;
                    }

                    byte[] buffer1 = new byte[uassetFileStream.Length + (long)uexpSize + (long)ubulkSize];
                    long num1 = uassetFileStream.Length + (long)uexpSize;

                    uassetFileStream.Read(buffer1, 0, (int)uassetFileStream.Length);

                    if (File.Exists(str1 + ".uexp"))
                        uexpFileStream.Read(buffer1, (int)uassetFileStream.Length, uexpSize);

                    if (File.Exists(str1 + ".ubulk"))
                        ubulkFileStream.Read(buffer1, (int)num1, ubulkSize);

                    MemoryStream fs = new MemoryStream(buffer1);
                    BinaryReader br = new BinaryReader(fs);
   
                    StreamWriter namesStreamWriter = null;
                    StreamWriter exportStreamWriter = null;
                    StreamWriter importStreamWriter = null;

                    if (hasArgs)
                    {
                        namesStreamWriter = new StreamWriter(localPath + withoutExtension + ".names.txt");
                        exportStreamWriter = new StreamWriter(localPath + withoutExtension + ".export.txt");
                        importStreamWriter = new StreamWriter(localPath + withoutExtension + ".import.txt");
                    }
                    if (File.Exists(localPath + withoutExtension + "_lights.txt"))
                        File.Delete(localPath + withoutExtension + "_lights.txt");

                    fs.Seek(20L, SeekOrigin.Begin);
                    fs.Seek((long)(br.ReadInt32() * 20), SeekOrigin.Current);
                    fs.Seek(17L, SeekOrigin.Current);
                    int length1 = br.ReadInt32();
                    long offset1 = (long)br.ReadInt32();
                    br.ReadInt32();
                    br.ReadInt32();
                    int num2 = br.ReadInt32();
                    long offset2 = (long)br.ReadInt32();
                    int num3 = br.ReadInt32();
                    long offset3 = (long)br.ReadInt32();
                    fs.Seek(offset1, SeekOrigin.Begin);
                    this.names = new string[length1];
                    int impIndex;

                    #region Fill names.txt
                    for (impIndex = 0; impIndex < length1; ++impIndex)
                    {
                        int num4 = br.ReadInt32();
                        this.names[impIndex] = "";
                        if (num4 < 0)
                        {
                            int num5 = -num4;
                            for (int index2 = 0; index2 < num5 - 1; ++index2)
                            {
                                string[] names;
                                int index3;
                                (names = this.names)[(int)(index3 = impIndex)] = names[index3] + (object)(char)br.ReadByte();
                                int num6 = (int)br.ReadByte();
                            }
                            fs.Seek(2L, SeekOrigin.Current);
                        }
                        else
                        {
                            for (int index2 = 0; index2 < num4 - 1; ++index2)
                            {
                                string[] names;
                                int index3;
                                (names = this.names)[(int)(index3 = impIndex)] = names[index3] + (object)br.ReadChar();
                            }
                            fs.Seek(1L, SeekOrigin.Current);
                        }
                        br.ReadInt32();
                        namesStreamWriter?.WriteLine(impIndex.ToString("X") + "\t" + this.names[impIndex]);
                    }
                    #endregion

                    #region Fill expnames and  impnames
                    this.expnames = new string[num2 + 1];
                    this.impnames = new string[num3 + 1];
                    int num7 = 0;
                    int num8 = 0;
                    var stringList1 = new List<string>();
                    var stringList2 = new List<string>();

                    fs.Seek(offset3, SeekOrigin.Begin);
                    for (impIndex = 1; impIndex <= num3; ++impIndex)
                    {
                        int index2 = br.ReadInt32();
                        br.ReadInt32();
                        int nameIndex = br.ReadInt32();
                        br.ReadInt32();
                        int num4 = -br.ReadInt32();
                        this.impnames[impIndex] = this.names[br.ReadInt32()];
                        if (this.names[nameIndex].ToLower() == "skeletalmesh")
                        {
                            ++num7;
                            stringList1.Add(this.impnames[impIndex]);
                        }
                        else if (this.names[nameIndex].ToLower() == "staticmesh")
                        {
                            ++num8;
                            stringList2.Add(this.impnames[impIndex]);
                        }
                        int num5 = br.ReadInt32();
                        if (num5 > 0)
                        {
                            string[] impnames;
                            int index4;
                            string str2 = (impnames = this.impnames)[(index4 = impIndex)] + "_" + (num5 - 1).ToString();
                            impnames[index4] = str2;
                        }
                        if (hasArgs)
                            importStreamWriter.WriteLine(impIndex.ToString("X") + "\t" + this.names[index2] + "\t" + this.names[nameIndex] + "\t->" + num4.ToString("X") + "\t" + this.impnames[impIndex]);
                    }
                    #endregion


                    int[] numArray1 = new int[num2 + 1];
                    int[] numArray2 = new int[num2 + 1];
                    int num9 = 0;

                    var stringList5 = new List<string>();
                    stringList5.Add("");

                    fs.Seek(offset2, SeekOrigin.Begin);
                    string key1 = "";

                    #region Blueprints
                    for (int index2 = 1; index2 <= num2; ++index2)
                    {
                        int index3 = br.ReadInt32();
                        if (index3 < 0)
                            stringList5.Add(this.impnames[-index3]);
                        else
                            stringList5.Add(this.expnames[index3]);
                        br.ReadInt32();
                        int num4 = br.ReadInt32();
                        br.ReadInt32();
                        this.expnames[index2] = this.names[br.ReadInt32()];
                        int num5 = br.ReadInt32();
                        if (num5 > 0)
                        {
                            string[] expnames;
                            int index4;
                            string str2 = (expnames = this.expnames)[(index4 = index2)] + "_" + (num5 - 1).ToString();
                            expnames[index4] = str2;
                        }
                        br.ReadInt32();
                        numArray2[index2] = br.ReadInt32();
                        if (this.game != 1)
                            br.ReadInt32();
                        numArray1[index2] = br.ReadInt32();
                        if (this.game != 1)
                            br.ReadInt32();
                        fs.Seek(60L, SeekOrigin.Current);
                        if (args.Length <= 1 || this.expnames[index2].ToLower().Contains(args[1].ToLower()))
                        {
                            if (hasArgs)
                                exportStreamWriter.WriteLine(index2.ToString("X") + "\t" + numArray1[index2].ToString("X") + "\t" + numArray2[index2].ToString("X") + "\t->" + num4.ToString("X") + "\t" + this.expnames[index2] + "." + stringList5[index2]);
                            if (stringList5[index2].ToLower() == "blueprintgeneratedclass")
                            {
                                key1 = this.expnames[index2];
                                ++num9;
                            }
                        }
                    }


                    if (num9 > 0 && !this.blueprintDB.dictionary1.ContainsKey(key1))
                    {
                        this.blueprintDB.dictionary1.Add(key1, this.blueprintDB.dictionary1.Count);
                        this.blueprintDB.stringListList2.Add(stringList1);
                        this.blueprintDB.stringListList1.Add(stringList2);
                        flag1 = true;
                    }
                    #endregion

                    var dictionary4 = new Dictionary<int, int>();
                    var dictionary5 = new Dictionary<int, int>();
                    var landList = new List<land>();
                    var intSet1 = new HashSet<int>();
                    int num10 = 0;

                    #region Actors
                    var staticMeshActorList = new List<actor>();
                    var lightActorList = new List<actor>();
                    var staticSkeletalMeshActorList = new List<actor>();
                    for (int expNameIndex = 1; expNameIndex <= num2; ++expNameIndex)
                    {
                        if (stringList5[expNameIndex].ToLower() == "texture2d")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            Utilities.readprops(new List<prop>(), fs, br);
                            fs.Seek(12L, SeekOrigin.Current);
                            string name = this.names[br.ReadInt32()];
                            fs.Seek(20L, SeekOrigin.Current);
                            fs.Seek((long)br.ReadInt32(), SeekOrigin.Current);
                            br.ReadInt32();
                            br.ReadInt32();
                            br.ReadInt32();
                            string textureOutName = this.expnames[expNameIndex] + ".dds";
                            int num4 = br.ReadInt32();
                            int count3 = br.ReadInt32();
                            br.ReadInt32();
                            long offset4 = br.ReadInt64();
                            if ((num4 & (int)byte.MaxValue) != 1)
                                fs.Seek((long)count3, SeekOrigin.Current);
                            int num5 = br.ReadInt32();
                            int num6 = br.ReadInt32();
                            if (exportTextures)
                            {
                                if (!Directory.Exists("textures"))
                                    Directory.CreateDirectory("textures");
                                if (!File.Exists("textures\\" + textureOutName))
                                {
                                    Console.WriteLine("Exporting texture: " + this.expnames[expNameIndex]);
                                    FileStream fileStream4 = new FileStream("textures\\" + textureOutName, FileMode.Create);
                                    BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream4);
                                    int num11 = 808540228;
                                    int index3 = 98;
                                    if (name == "PF_DXT1")
                                        index3 = 71;
                                    else if (name == "PF_DXT3")
                                        index3 = 74;
                                    else if (name == "PF_DXT5")
                                        index3 = 77;
                                    else if (name == "PF_B8G8R8A8")
                                        index3 = 87;
                                    else if (name == "PF_G8")
                                        index3 = 61;
                                    else if (name == "PF_FloatRGBA")
                                        index3 = 10;
                                    else if (name == "PF_BC4")
                                        index3 = 80;
                                    else if (name == "PF_BC5")
                                        index3 = 83;
                                    else if (name == "PF_BC6")
                                        index3 = 95;
                                    else if (name == "PF_BC6H")
                                        index3 = 95;
                                    else if (name != "PF_BC7")
                                        Console.WriteLine("Unsupported texture format " + name);
                                    int num12 = mn.pixbl[index3];
                                    int num13 = mn.bpp[index3] * 2;
                                    if (num12 == 1)
                                        num13 = mn.bpp[index3] / 8;
                                    binaryWriter.Write(533118272580L);
                                    binaryWriter.Write(4103);
                                    binaryWriter.Write(num6);
                                    binaryWriter.Write(num5);
                                    binaryWriter.Write(count3);
                                    binaryWriter.Write(0);
                                    binaryWriter.Write(1);
                                    fileStream4.Seek(44L, SeekOrigin.Current);
                                    binaryWriter.Write(32);
                                    binaryWriter.Write(4);
                                    binaryWriter.Write(num11);
                                    fileStream4.Seek(40L, SeekOrigin.Current);
                                    if (num11 == 808540228)
                                    {
                                        binaryWriter.Write(index3);
                                        binaryWriter.Write(3);
                                        binaryWriter.Write(0);
                                        binaryWriter.Write(1);
                                        binaryWriter.Write(0);
                                    }
                                    if ((num4 & (int)byte.MaxValue) == 1)
                                        fs.Seek(num1 * 2L - 4L + offset4, SeekOrigin.Begin);
                                    else
                                        fs.Seek(offset4, SeekOrigin.Begin);
                                    byte[] buffer2 = new byte[count3 * 2];
                                    byte[] buffer3 = new byte[64];
                                    int num14 = num6 / num12;
                                    int num15 = num5 / num12;
                                    for (int index4 = 0; index4 < (num14 + 7) / 8; ++index4)
                                    {
                                        for (int index5 = 0; index5 < (num15 + 7) / 8; ++index5)
                                        {
                                            for (int t = 0; t < 64; ++t)
                                            {
                                                int num16 = Utilities.morton(t, 8, 8);
                                                int num17 = num16 / 8;
                                                int num18 = num16 % 8;
                                                fs.Read(buffer3, 0, num13);
                                                if (index5 * 8 + num18 < num15 && index4 * 8 + num17 < num14)
                                                {
                                                    int destinationIndex = num13 * ((index4 * 8 + num17) * num15 + index5 * 8 + num18);
                                                    if (destinationIndex + num13 <= buffer2.Length)
                                                        Array.Copy((Array)buffer3, 0, (Array)buffer2, destinationIndex, num13);
                                                }
                                            }
                                        }
                                    }
                                    fileStream4.Write(buffer2, 0, count3);
                                    fileStream4.Close();
                                }
                            }
                        }
                        else if (stringList5[expNameIndex].ToLower() == "PointLightComponent".ToLower() || stringList5[expNameIndex].ToLower() == "SpotLightComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist = new List<prop>();
                            Utilities.readprops(plist, fs, br);
                            ++num10;
                            actor actor = new actor();
                            foreach (prop prop in plist)
                            {
                                if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.pos = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.rot = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    actor.parent = prop.ivalue;
                            }
                            if (exportLights)
                            {
                                StreamWriter sw = new StreamWriter(localPath + withoutExtension + "_lights.txt", true);
                                sw.WriteLine(stringList5[expNameIndex] + " " + this.expnames[expNameIndex]);
                                sw.WriteLine("-------------------------------------- [" + (object)num10 + "]");
                                Utilities.Printprops(plist, fs, br, sw);
                                sw.WriteLine();
                                sw.Close();
                            }
                            lightActorList.Add(actor);
                            Console.Write("L");
                        }
                        else if (stringList5[expNameIndex].ToLower() == "skeletalmesh")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            int num4 = 0;
                            bool flag4 = false;
                            List<int> intList = new List<int>();
                            List<prop> plist1 = new List<prop>();
                            Utilities.readprops(plist1, fs, br);
                            foreach (prop prop1 in plist1)
                            {
                                if (prop1.name == "bHasVertexColors" && prop1.ivalue > 0)
                                    flag4 = true;
                                else if (prop1.type == "ObjectProperty" && prop1.name == "Skeleton")
                                    num4 = prop1.ivalue;
                                else if (prop1.type == "ArrayProperty" && prop1.name == "LODInfo")
                                {
                                    long position1 = fs.Position;
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int num5 = br.ReadInt32();
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < num5; ++impIndex)
                                    {
                                        List<prop> plist2 = new List<prop>();
                                        Utilities.readprops(plist2, fs, br);
                                        foreach (prop prop2 in plist2)
                                        {
                                            if (prop2.type == "ObjectProperty" && prop2.name == "MorphTargetSet" && prop2.ivalue != 0)
                                            {
                                                long position2 = fs.Position;
                                                fs.Seek((long)(numArray1[prop2.ivalue] + 24), SeekOrigin.Begin);
                                                int num6 = br.ReadInt32();
                                                for (int index3 = 0; index3 < num6; ++index3)
                                                    intList.Add(br.ReadInt32());
                                                fs.Seek(position2, SeekOrigin.Begin);
                                            }
                                        }
                                    }
                                    fs.Seek(position1, SeekOrigin.Begin);
                                }
                            }
                            Console.Write(".");
                            string expname = this.expnames[expNameIndex];
                            MemoryStream memoryStream1 = new MemoryStream();
                            StreamWriter streamWriter5 = new StreamWriter((Stream)memoryStream1);
 
                            long position = fs.Position;
                            fs.Seek(10L, SeekOrigin.Current);
                            fs.Seek(24L, SeekOrigin.Current);
                            int length2 = br.ReadInt32();
                            string[] strArray1 = new string[length2];
                            for (int index3 = 0; index3 < length2; ++index3)
                            {
                                int index4 = br.ReadInt32();
                                strArray1[index3] = index4 >= 0 ? this.expnames[index4] : this.impnames[-index4];
                                fs.Seek(32L, SeekOrigin.Current);
                            }
                            if (num4 != 0)
                            {
                                if (!Directory.Exists("skeletalmesh"))
                                    Directory.CreateDirectory("skeletalmesh");
                                StreamWriter SW_SKM_SMD = new StreamWriter("skeletalmesh\\" + expname + ".smd");
                                int length3 = br.ReadInt32();
                                Quaternion3D q = new Quaternion3D();
                                Vector3D[] vector3DArray1 = new Vector3D[length3];
                                Vector3D[] vector3DArray2 = new Vector3D[length3];
                                Vector3D[] vector3DArray3 = new Vector3D[length3];
                                Quaternion3D[] quaternion3DArray1 = new Quaternion3D[length3];
                                Quaternion3D[] quaternion3DArray2 = new Quaternion3D[length3];
                                int[] numArray3 = new int[length3];
                                string[] strArray2 = new string[length3];
                                streamWriter5.WriteLine(length3);
                                SW_SKM_SMD.WriteLine("version 1");
                                SW_SKM_SMD.WriteLine("nodes");
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    strArray2[index3] = this.names[br.ReadInt32()];
                                    int num5 = br.ReadInt32();
                                    if (num5 > 0)
                                    {
                                        string[] strArray3;
                                        int index4;
                                        string str2 = (strArray3 = strArray2)[(index4 = index3)] + "_" + (num5 - 1).ToString();
                                        strArray3[index4] = str2;
                                    }
                                    numArray3[index3] = br.ReadInt32();
                                }
                                br.ReadInt32();
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    q.i = br.ReadSingle();
                                    q.j = br.ReadSingle();
                                    q.k = br.ReadSingle();
                                    q.real = br.ReadSingle();
                                    quaternion3DArray1[index3] = new Quaternion3D(q);
                                    vector3DArray1[index3] = C3D.ToEulerAngles(q);
                                    vector3DArray2[index3] = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    Vector3D vector3D = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                fs.Seek((long)(12 * br.ReadInt32()), SeekOrigin.Current);
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    int index4 = numArray3[index3];
                                    if (index4 < 0)
                                    {
                                        vector3DArray3[index3] = vector3DArray2[index3];
                                        quaternion3DArray2[index3] = quaternion3DArray1[index3];
                                    }
                                    else
                                    {
                                        quaternion3DArray2[index3] = quaternion3DArray2[index4] * quaternion3DArray1[index3];
                                        Quaternion3D quaternion3D1 = new Quaternion3D(vector3DArray2[index3], 0.0f);
                                        Quaternion3D quaternion3D2 = quaternion3DArray2[index4] * quaternion3D1 * new Quaternion3D(quaternion3DArray2[index4].real, -quaternion3DArray2[index4].i, -quaternion3DArray2[index4].j, -quaternion3DArray2[index4].k);
                                        vector3DArray3[index3] = quaternion3D2.xyz;
                                        vector3DArray3[index3] += vector3DArray3[index4];
                                    }
                                }
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    vector3DArray3[index3].Y = -vector3DArray3[index3].Y;
                                    quaternion3DArray2[index3].i = -quaternion3DArray2[index3].i;
                                    quaternion3DArray2[index3].k = -quaternion3DArray2[index3].k;
                                }
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    int index4 = numArray3[index3];
                                    if (index4 < 0)
                                    {
                                        vector3DArray2[index3] = vector3DArray3[index3];
                                        quaternion3DArray1[index3] = quaternion3DArray2[index3];
                                    }
                                    else
                                    {
                                        quaternion3DArray1[index3] = Quaternion3D.Invert(quaternion3DArray2[index4]) * quaternion3DArray2[index3];
                                        vector3DArray1[index3] = C3D.ToEulerAngles(quaternion3DArray1[index3]);
                                        vector3DArray2[index3] = vector3DArray3[index3] - vector3DArray3[index4];
                                        Quaternion3D quaternion3D = Quaternion3D.Invert(quaternion3DArray2[index4]) * new Quaternion3D(vector3DArray2[index3], 0.0f) * quaternion3DArray2[index4];
                                        vector3DArray2[index3] = quaternion3D.xyz;
                                    }
                                }
                                for (int index3 = 0; index3 < length3; ++index3)
                                    SW_SKM_SMD.WriteLine(index3.ToString() + " \"" + strArray2[index3] + "\" " + (object)numArray3[index3]);
                                SW_SKM_SMD.WriteLine("end");
                                SW_SKM_SMD.WriteLine("skeleton");
                                SW_SKM_SMD.WriteLine("time 0");
                                for (int index3 = 0; index3 < length3; ++index3)
                                {
                                    SW_SKM_SMD.Write(index3.ToString() + "  ");
                                    SW_SKM_SMD.Write(vector3DArray2[index3].X.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    SW_SKM_SMD.Write(" " + vector3DArray2[index3].Y.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    SW_SKM_SMD.Write(" " + vector3DArray2[index3].Z.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    SW_SKM_SMD.Write("  " + vector3DArray1[index3].X.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    SW_SKM_SMD.Write(" " + vector3DArray1[index3].Y.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    SW_SKM_SMD.WriteLine(" " + vector3DArray1[index3].Z.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.WriteLine(strArray2[index3]);
                                    streamWriter5.WriteLine(numArray3[index3]);
                                    streamWriter5.Write(vector3DArray3[index3].X.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + vector3DArray3[index3].Y.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + vector3DArray3[index3].Z.ToString("0.000000", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + quaternion3DArray2[index3].i.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + quaternion3DArray2[index3].j.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + quaternion3DArray2[index3].k.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.Write(" " + quaternion3DArray2[index3].real.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    streamWriter5.WriteLine();
                                }
                                SW_SKM_SMD.WriteLine("end");
                                SW_SKM_SMD.Close();
                            }
                            else
                                streamWriter5.WriteLine("0");
                            if (!Directory.Exists("skeletalmesh.raw"))
                                Directory.CreateDirectory("skeletalmesh.raw");
                            int count3 = (int)((long)(numArray1[expNameIndex] + numArray2[expNameIndex]) - fs.Position);
                            byte[] numArray4 = new byte[count3 + 1];
                            fs.Read(numArray4, 1, count3);
                            if (flag4)
                                numArray4[0] = (byte)1;
                            File.WriteAllBytes("skeletalmesh.raw\\" + expname + ".raw", numArray4);
                            fs.Seek((long)-count3, SeekOrigin.Current);
                            StreamWriter SW_SKM_RAW_TXT = new StreamWriter("skeletalmesh.raw\\" + expname + ".txt");
                            foreach (string str2 in strArray1)
                                SW_SKM_RAW_TXT.WriteLine(str2);
                            SW_SKM_RAW_TXT.Close();
                            int num11 = br.ReadInt32();
                            for (int index3 = 0; index3 < num11; ++index3)
                            {
                                int num5 = 0;
                                bool flag5 = false;
                                int num6 = (int)br.ReadByte();
                                int num12 = (int)br.ReadByte();
                                int length3 = br.ReadInt32();
                                int[] numArray3 = new int[length3];
                                int[] numArray5 = new int[length3];
                                int[] numArray6 = new int[length3];
                                int[] numArray7 = new int[length3];
                                int[][] numArray8 = new int[length3][];
                                int[] numArray9 = new int[length3];
                                int[] numArray10 = new int[length3];
                                for (int index4 = 0; index4 < length3; ++index4)
                                {
                                    int num13 = (int)br.ReadInt16();
                                    numArray3[index4] = (int)br.ReadInt16();
                                    numArray5[index4] = br.ReadInt32();
                                    numArray6[index4] = br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    numArray9[index4] = br.ReadInt32();
                                    int length4 = br.ReadInt32();
                                    numArray8[index4] = new int[length4];
                                    for (int index5 = 0; index5 < length4; ++index5)
                                        numArray8[index4][index5] = (int)br.ReadInt16();
                                    numArray10[index4] = br.ReadInt32();
                                    br.ReadInt32();
                                    fs.Seek(34L, SeekOrigin.Current);
                                    int num14 = br.ReadInt32();
                                    int num15 = br.ReadInt32();
                                    if (num14 > 0)
                                    {
                                        if (this.game == 2)
                                        {
                                            flag5 = true;
                                            fs.Seek((long)(num14 * num15 * 16), SeekOrigin.Current);
                                        }
                                        else
                                            fs.Seek((long)(num14 * num15 * 24), SeekOrigin.Current);
                                    }
                                }
                                int num16 = (int)br.ReadByte();
                                int num17 = (int)br.ReadByte();
                                int num18 = (int)br.ReadByte();
                                int num19 = (int)br.ReadByte();
                                int num20 = (int)br.ReadByte();
                                int length5 = 1;
                                int num21 = 0;
                                int[][] numArray11 = new int[length5][];
                                Vector3D[][] vector3DArray1 = new Vector3D[length5][];
                                Vector3D[][] vector3DArray2 = new Vector3D[length5][];
                                float[][,] numArray12 = new float[length5][,];
                                float[][,] numArray13 = new float[length5][,];
                                int[][,] numArray14 = new int[length5][,];
                                int[][,] numArray15 = new int[length5][,];
                                int num22 = 0;
                                int num23 = 0;
                                for (int index4 = 0; index4 < length5; ++index4)
                                {
                                    int length4 = br.ReadInt32();
                                    numArray11[index4] = new int[length4];
                                    for (int index5 = 0; index5 < length4; ++index5)
                                        numArray11[index4][index5] = num16 != 4 ? (int)br.ReadUInt16() : br.ReadInt32();
                                    fs.Seek((long)(br.ReadInt32() * 2), SeekOrigin.Current);
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    fs.Seek((long)(br.ReadInt32() * 2), SeekOrigin.Current);
                                    fs.Seek((long)(br.ReadInt32() * 4), SeekOrigin.Current);
                                    br.ReadInt32();
                                    num21 = br.ReadInt32();
                                    int num13 = (int)br.ReadInt16();
                                    br.ReadInt32();
                                    int num14 = br.ReadInt32();
                                    Vector3D vector3D1 = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    Vector3D vector3D2 = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    br.ReadInt32();
                                    int length6 = br.ReadInt32();
                                    vector3DArray1[index4] = new Vector3D[length6];
                                    vector3DArray2[index4] = new Vector3D[length6];
                                    numArray12[index4] = new float[length6, 4];
                                    numArray13[index4] = new float[length6, 4];
                                    numArray14[index4] = new int[length6, 8];
                                    numArray15[index4] = new int[length6, 8];
                                    for (int index5 = 0; index5 < length6; ++index5)
                                    {
                                        br.ReadInt32();
                                        float xx1 = (float)((int)br.ReadByte() - 128) / 128f;
                                        float yy1 = (float)((int)br.ReadByte() - 128) / 128f;
                                        float zz1 = (float)((int)br.ReadByte() - 128) / 128f;
                                        int num15 = (int)br.ReadByte();
                                        vector3DArray2[index4][index5] = new Vector3D(xx1, yy1, zz1);
                                        float xx2 = br.ReadSingle();
                                        float yy2 = br.ReadSingle();
                                        float zz2 = br.ReadSingle();
                                        vector3DArray1[index4][index5] = new Vector3D(xx2, yy2, zz2);
                                        for (int index6 = 0; index6 < num21; ++index6)
                                        {
                                            if (num14 == 1)
                                            {
                                                numArray12[index4][index5, index6] = br.ReadSingle();
                                                numArray13[index4][index5, index6] = br.ReadSingle();
                                            }
                                            else
                                            {
                                                numArray12[index4][index5, index6] = (float)SystemHalf.Half.ToHalf(br.ReadUInt16());
                                                numArray13[index4][index5, index6] = (float)SystemHalf.Half.ToHalf(br.ReadUInt16());
                                            }
                                        }
                                    }
                                    br.ReadInt32();
                                    int num24 = (int)br.ReadInt16();
                                    num22 = br.ReadInt32();
                                    int num25 = br.ReadInt32();
                                    int num26 = br.ReadInt32();
                                    if (num25 == 4)
                                    {
                                        for (int index5 = 0; index5 < num26; ++index5)
                                        {
                                            numArray14[index4][index5, 0] = br.ReadInt32();
                                            numArray15[index4][index5, 0] = (int)byte.MaxValue;
                                        }
                                    }
                                    else
                                    {
                                        num23 = 4;
                                        if (num25 == 16)
                                            num23 = 8;
                                        for (int index5 = 0; index5 < num26; ++index5)
                                        {
                                            for (int index6 = 0; index6 < num23; ++index6)
                                                numArray14[index4][index5, index6] = (int)br.ReadByte();
                                            for (int index6 = 0; index6 < num23; ++index6)
                                                numArray15[index4][index5, index6] = (int)br.ReadByte();
                                        }
                                    }
                                    if (flag5)
                                    {
                                        int num15 = (int)br.ReadInt16();
                                        int num27 = br.ReadInt32();
                                        int num28 = br.ReadInt32();
                                        fs.Seek((long)(num28 * num27), SeekOrigin.Current);
                                        int num29 = (int)br.ReadInt16();
                                        num16 = br.ReadInt32();
                                        int num30 = br.ReadInt32();
                                        fs.Seek((long)(num16 * num30), SeekOrigin.Current);
                                    }
                                    if (flag4)
                                    {
                                        int num15 = (int)br.ReadInt16();
                                        br.ReadInt32();
                                        br.ReadInt32();
                                        int num27 = br.ReadInt32();
                                        int num28 = br.ReadInt32();
                                        fs.Seek((long)(num27 * num28), SeekOrigin.Current);
                                    }
                                }
                                int num31 = num5 + length3;
                                MemoryStream memoryStream2 = new MemoryStream();
                                StreamWriter streamWriter6 = new StreamWriter((Stream)memoryStream2);
                                streamWriter6.WriteLine(num31);
                                for (int index4 = 0; index4 < length3; ++index4)
                                {
                                    int index5 = numArray5[index4];
                                    streamWriter6.WriteLine("Submesh_" + (object)index4);
                                    streamWriter6.WriteLine(num21);
                                    string key2 = strArray1[numArray3[index4]];
                                    if (this.materialDB.dictionary2.ContainsKey(key2))
                                    {
                                        int count4 = this.materialDB.texparamListList[this.materialDB.dictionary2[key2]].Count;
                                        streamWriter6.WriteLine(count4);
                                        foreach (texparam texparam in this.materialDB.texparamListList[this.materialDB.dictionary2[key2]])
                                        {
                                            streamWriter6.WriteLine(texparam.value + ".dds");
                                            streamWriter6.WriteLine("0");
                                        }
                                    }
                                    else
                                    {
                                        streamWriter6.WriteLine("1");
                                        streamWriter6.WriteLine(key2);
                                        streamWriter6.WriteLine("0");
                                        Console.WriteLine("Material not found in database: " + key2);
                                    }
                                    streamWriter6.WriteLine(numArray10[index4]);
                                    for (int index6 = numArray9[index4]; index6 < numArray9[index4] + numArray10[index4]; ++index6)
                                    {
                                        Vector3D vector3D = vector3DArray1[numArray7[index4]][index6];
                                        streamWriter6.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter8 = streamWriter6;
                                        float num13 = -vector3D.Y;
                                        string str2 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter8.Write(str2);
                                        streamWriter6.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        streamWriter6.Write(vector3DArray2[numArray7[index4]][index6].X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter9 = streamWriter6;
                                        num13 = -vector3DArray2[numArray7[index4]][index6].Y;
                                        string str3 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter9.Write(str3);
                                        streamWriter6.WriteLine(" " + vector3DArray2[numArray7[index4]][index6].Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        streamWriter6.WriteLine("0 0 0 0");
                                        for (int index7 = 0; index7 < num21; ++index7)
                                            streamWriter6.WriteLine(numArray12[numArray7[index4]][index6, index7].ToString("0.######", (IFormatProvider)numberFormatInfo) + " " + numArray13[numArray7[index4]][index6, index7].ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        streamWriter6.Write(numArray8[index4][numArray14[numArray7[index4]][index6, 0]]);
                                        streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 1]]);
                                        streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 2]]);
                                        streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 3]]);
                                        if (num23 > 4)
                                        {
                                            streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 4]]);
                                            streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 5]]);
                                            streamWriter6.Write(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 6]]);
                                            streamWriter6.WriteLine(" " + (object)numArray8[index4][numArray14[numArray7[index4]][index6, 7]]);
                                        }
                                        else
                                            streamWriter6.WriteLine();
                                        StreamWriter streamWriter10 = streamWriter6;
                                        num13 = (float)numArray15[numArray7[index4]][index6, 0] / (float)byte.MaxValue;
                                        string str4 = num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter10.Write(str4);
                                        StreamWriter streamWriter11 = streamWriter6;
                                        num13 = (float)numArray15[numArray7[index4]][index6, 1] / (float)byte.MaxValue;
                                        string str5 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter11.Write(str5);
                                        StreamWriter streamWriter12 = streamWriter6;
                                        num13 = (float)numArray15[numArray7[index4]][index6, 2] / (float)byte.MaxValue;
                                        string str6 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter12.Write(str6);
                                        StreamWriter streamWriter13 = streamWriter6;
                                        num13 = (float)numArray15[numArray7[index4]][index6, 3] / (float)byte.MaxValue;
                                        string str7 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter13.Write(str7);
                                        if (num23 > 4)
                                        {
                                            StreamWriter streamWriter14 = streamWriter6;
                                            num13 = (float)numArray15[numArray7[index4]][index6, 4] / (float)byte.MaxValue;
                                            string str8 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                            streamWriter14.Write(str8);
                                            StreamWriter streamWriter15 = streamWriter6;
                                            num13 = (float)numArray15[numArray7[index4]][index6, 5] / (float)byte.MaxValue;
                                            string str9 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                            streamWriter15.Write(str9);
                                            StreamWriter streamWriter16 = streamWriter6;
                                            num13 = (float)numArray15[numArray7[index4]][index6, 6] / (float)byte.MaxValue;
                                            string str10 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                            streamWriter16.Write(str10);
                                            StreamWriter streamWriter17 = streamWriter6;
                                            num13 = (float)numArray15[numArray7[index4]][index6, 7] / (float)byte.MaxValue;
                                            string str11 = " " + num13.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                            streamWriter17.WriteLine(str11);
                                        }
                                        else
                                            streamWriter6.WriteLine();
                                    }
                                    streamWriter6.WriteLine(numArray6[index4]);
                                    for (int index6 = 0; index6 < numArray6[index4]; ++index6)
                                    {
                                        streamWriter6.Write(numArray11[numArray7[index4]][index5 + 2] - numArray9[index4]);
                                        streamWriter6.Write(" " + (object)(numArray11[numArray7[index4]][index5 + 1] - numArray9[index4]));
                                        streamWriter6.Write(" " + (object)(numArray11[numArray7[index4]][index5] - numArray9[index4]));
                                        streamWriter6.WriteLine();
                                        index5 += 3;
                                    }
                                }
                                if (!Directory.Exists("skeletalmesh"))
                                    Directory.CreateDirectory("skeletalmesh");
                                FileStream fileStream4 = new FileStream("skeletalmesh\\" + expname + "_lod" + (object)index3 + ".ascii", FileMode.Create);
                                streamWriter5.Flush();
                                byte[] buffer2 = memoryStream1.GetBuffer();
                                fileStream4.Write(buffer2, 0, (int)memoryStream1.Length);
                                streamWriter6.Flush();
                                byte[] buffer3 = memoryStream2.GetBuffer();
                                fileStream4.Write(buffer3, 0, (int)memoryStream2.Length);
                                fileStream4.Close();
                                streamWriter6.Close();
                            }
                        }
                        else if (stringList5[expNameIndex].ToLower() == "InstancedStaticMeshComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist = new List<prop>();
                            Utilities.readprops(plist, fs, br);
                            br.ReadInt32();
                            int num4 = br.ReadInt32();
                            fs.Seek((long)(18 * num4), SeekOrigin.Current);
                            int num5 = br.ReadInt32();
                            int num6 = br.ReadInt32();
                            if (num5 != 80)
                                Console.WriteLine("Unknown matrix size " + (object)num5);
                            long position = fs.Position;
                            actor actor1 = new actor();
                            foreach (prop prop in plist)
                            {
                                if (prop.type == "ObjectProperty" && prop.name == "StaticMesh")
                                    actor1.meshname = prop.ivalue >= 0 ? this.expnames[prop.ivalue] : this.impnames[-prop.ivalue];
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    actor1.parent = prop.ivalue;
                                else if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor1.pos = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor1.rot = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor1.scale = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                            }
                            fs.Seek(position, SeekOrigin.Begin);
                            for (int index3 = 0; index3 < num6; ++index3)
                            {
                                actor actor2 = new actor();
                                actor2.meshname = actor1.meshname;
                                actor2.pos = actor1.pos;
                                actor2.rot = actor1.rot;
                                actor2.scale = actor1.scale;
                                actor2.hastrans = true;
                                actor2.tm = new float[5, 4];
                                for (int index4 = 0; index4 < 5; ++index4)
                                {
                                    for (int index5 = 0; index5 < 4; ++index5)
                                        actor2.tm[index4, index5] = br.ReadSingle();
                                }
                                staticMeshActorList.Add(actor2);
                            }
                            Console.Write("i");
                        }
                        else if (stringList5[expNameIndex].ToLower() == "MaterialInstanceConstant".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist1 = new List<prop>();
                            Utilities.readprops(plist1, fs, br);
                            if (!Directory.Exists("materials"))
                                Directory.CreateDirectory("materials");
                            StreamWriter SW_MAT_TXT = new StreamWriter("materials\\" + withoutExtension + ".txt");
                            SW_MAT_TXT.WriteLine(stringList5[expNameIndex] + " " + this.expnames[expNameIndex]);
                            SW_MAT_TXT.WriteLine("======================================");
                            Utilities.Printprops(plist1, fs, br, SW_MAT_TXT);
                            SW_MAT_TXT.WriteLine();
                            List<texparam> texparamList = new List<texparam>();
                            foreach (prop prop1 in plist1)
                            {
                                if (prop1.type == "ArrayProperty" && prop1.name == "TextureParameterValues")
                                {
                                    Console.WriteLine("Material " + this.expnames[expNameIndex]);
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int num4 = br.ReadInt32();
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < num4; ++impIndex)
                                    {
                                        List<prop> plist2 = new List<prop>();
                                        Utilities.readprops(plist2, fs, br);
                                        int num5 = 0;
                                        string str2 = "";
                                        foreach (prop prop2 in plist2)
                                        {
                                            if (prop2.name == "ParameterName")
                                            {
                                                if (!this.materialDB.dictionary3.ContainsKey(prop2.svalue))
                                                {
                                                    num5 = this.materialDB.dictionary3.Count;
                                                    this.materialDB.dictionary3.Add(prop2.svalue, this.materialDB.dictionary3.Count);
                                                    this.materialDB.stringList3.Add(prop2.svalue);
                                                }
                                                else
                                                    num5 = this.materialDB.dictionary3[prop2.svalue];
                                            }
                                            if (prop2.name == "ParameterValue")
                                                str2 = prop2.ivalue >= 0 ? this.expnames[prop2.ivalue] : this.names[-prop2.ivalue];
                                        }
                                        if (str2 != null)
                                            texparamList.Add(new texparam()
                                            {
                                                type = num5,
                                                value = str2
                                            });
                                    }
                                }
                                if (prop1.type == "ArrayProperty" && prop1.svalue == "StructProperty")
                                {
                                    SW_MAT_TXT.WriteLine(prop1.name);
                                    SW_MAT_TXT.WriteLine("--------------------------------------");
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int num4 = br.ReadInt32();
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < num4; ++impIndex)
                                    {
                                        List<prop> plist2 = new List<prop>();
                                        Utilities.readprops(plist2, fs, br);
                                        Utilities.Printprops(plist2, fs, br, SW_MAT_TXT);
                                        SW_MAT_TXT.WriteLine();
                                    }
                                }
                            }
                            if (!this.materialDB.dictionary2.ContainsKey(this.expnames[expNameIndex]))
                            {
                                this.materialDB.dictionary2.Add(this.expnames[expNameIndex], this.materialDB.dictionary2.Count);
                                this.materialDB.texparamListList.Add(texparamList);
                                dirty = true;
                            }
                            SW_MAT_TXT.Close();
                        }
                        else if (stringList5[expNameIndex].ToLower() == "LandscapeComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist = new List<prop>();
                            Utilities.readprops(plist, fs, br);
                            land land = new land();
                            foreach (prop prop in plist)
                            {
                                if (prop.name == "SectionBaseX")
                                    land.hmx = prop.ivalue;
                                else if (prop.name == "SectionBaseY")
                                    land.hmy = prop.ivalue;
                                else if (prop.name == "HeightmapTexture")
                                    land.hmtexture = prop.ivalue;
                                else if (prop.name == "HeightmapScaleBias")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    land.scale = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    land.parent = prop.ivalue;
                            }
                            landList.Add(land);
                        }
                        else if (stringList5[expNameIndex].ToLower() == "staticmeshcomponent" || stringList5[expNameIndex].ToLower() == "scenecomponent")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist = new List<prop>();
                            Utilities.readprops(plist, fs, br);
                            actor actor = new actor();
                            foreach (prop prop in plist)
                            {
                                if (prop.type == "ObjectProperty" && prop.name == "StaticMesh")
                                    actor.meshname = prop.ivalue >= 0 ? this.expnames[prop.ivalue] : this.impnames[-prop.ivalue];
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    actor.parent = prop.ivalue;
                                else if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.pos = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.rot = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.scale = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "ArrayProperty" && prop.name == "OverrideMaterials")
                                {
                                    long position = fs.Position;
                                    fs.Seek(prop.fpos + 9L, SeekOrigin.Begin);
                                    int length2 = br.ReadInt32();
                                    actor.overmat = new int[length2];
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                        actor.overmat[impIndex] = br.ReadInt32();
                                    fs.Seek(position, SeekOrigin.Begin);
                                }
                            }
                            dictionary4.Add(expNameIndex, staticMeshActorList.Count);
                            staticMeshActorList.Add(actor);
                        }
                        else if (stringList5[expNameIndex].ToLower() == "staticmesh")
                        {
                            Console.WriteLine("Exporting staticmesh: " + this.expnames[expNameIndex]);
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist1 = new List<prop>();
                            Utilities.readprops(plist1, fs, br);
                            string[] strArray = (string[])null;
                            foreach (prop prop1 in plist1)
                            {
                                if (prop1.type == "ArrayProperty" && prop1.name == "Materials")
                                {
                                    long position = fs.Position;
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int length2 = br.ReadInt32();
                                    strArray = new string[length2];
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                        strArray[impIndex] = this.impnames[-br.ReadInt32()];
                                    fs.Seek(position, SeekOrigin.Begin);
                                }
                                else if (prop1.type == "ArrayProperty" && prop1.name == "StaticMaterials")
                                {
                                    long position = fs.Position;
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int length2 = br.ReadInt32();
                                    strArray = new string[length2];
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                    {
                                        List<prop> plist2 = new List<prop>();
                                        Utilities.readprops(plist2, fs, br);
                                        foreach (prop prop2 in plist2)
                                        {
                                            if (prop2.name == "MaterialInterface")
                                                strArray[impIndex] = prop2.ivalue >= 0 ? this.expnames[prop2.ivalue] : this.impnames[-prop2.ivalue];
                                        }
                                    }
                                    fs.Seek(position, SeekOrigin.Begin);
                                }
                            }
                            string expname = this.expnames[expNameIndex];
                            if (!Directory.Exists("staticmesh.raw"))
                                Directory.CreateDirectory("staticmesh.raw");
                            int count3 = (int)((long)(numArray1[expNameIndex] + numArray2[expNameIndex]) - fs.Position);
                            byte[] numArray3 = new byte[count3];
                            fs.Read(numArray3, 0, count3);
                            File.WriteAllBytes("staticmesh.raw\\" + expname + ".raw", numArray3);
                            fs.Seek((long)-count3, SeekOrigin.Current);
                            StreamWriter SW_STM_RAW_TXT = new StreamWriter("staticmesh.raw\\" + expname + ".txt");
                            foreach (string str2 in strArray)
                                SW_STM_RAW_TXT.WriteLine(str2);
                            SW_STM_RAW_TXT.Close();
                            fs.Seek(34L, SeekOrigin.Current);
                            int num4 = br.ReadInt32();
                            fs.Seek((long)(num4 * 4), SeekOrigin.Current);
                            int num5 = br.ReadInt32();
                            for (int index3 = 0; index3 < num5; ++index3)
                            {
                                int num6 = (int)br.ReadByte();
                                int num11 = (int)br.ReadByte();
                                int length2 = (int)br.ReadByte();
                                int num12 = (int)br.ReadByte();
                                int num13 = (int)br.ReadByte();
                                int num14 = (int)br.ReadByte();
                                int[] numArray4 = new int[length2];
                                int[] numArray5 = new int[length2];
                                int[] numArray6 = new int[length2];
                                int[] numArray7 = new int[length2];
                                int[] numArray8 = new int[length2];
                                int[] numArray9 = new int[length2];
                                for (int index4 = 0; index4 < length2; ++index4)
                                {
                                    numArray4[index4] = br.ReadInt32();
                                    numArray5[index4] = br.ReadInt32();
                                    numArray6[index4] = br.ReadInt32();
                                    numArray8[index4] = br.ReadInt32();
                                    numArray9[index4] = br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                }
                                br.ReadInt32();
                                int length3 = 1;
                                int num15 = 0;
                                int[][] numArray10 = new int[length3][];
                                Vector3D[][] vector3DArray1 = new Vector3D[length3][];
                                Vector3D[][] vector3DArray2 = new Vector3D[length3][];
                                float[][,] numArray11 = new float[length3][,];
                                float[][,] numArray12 = new float[length3][,];
                                for (int index4 = 0; index4 < length3; ++index4)
                                {
                                    br.ReadInt32();
                                    int length4 = br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    vector3DArray1[index4] = new Vector3D[length4];
                                    for (int index5 = 0; index5 < length4; ++index5)
                                    {
                                        float xx = br.ReadSingle();
                                        float yy = br.ReadSingle();
                                        float zz = br.ReadSingle();
                                        vector3DArray1[index4][index5] = new Vector3D(xx, yy, zz);
                                    }
                                    int num16 = (int)br.ReadInt16();
                                    num15 = br.ReadInt32();
                                    br.ReadInt32();
                                    br.ReadInt32();
                                    int num17 = br.ReadInt32();
                                    int num18 = br.ReadInt32();
                                    br.ReadInt32();
                                    int length5 = br.ReadInt32();
                                    vector3DArray2[index4] = new Vector3D[length5];
                                    numArray11[index4] = new float[length5, 8];
                                    numArray12[index4] = new float[length5, 8];
                                    for (int index5 = 0; index5 < length5; ++index5)
                                    {
                                        if (num18 == 1)
                                        {
                                            br.ReadInt64();
                                            float xx = (float)((int)br.ReadUInt16() - 32768) / 32768f;
                                            float yy = (float)((int)br.ReadUInt16() - 32768) / 32768f;
                                            float zz = (float)((int)br.ReadUInt16() - 32768) / 32768f;
                                            int num19 = (int)br.ReadUInt16();
                                            vector3DArray2[index4][index5] = new Vector3D(xx, yy, zz);
                                        }
                                        else
                                        {
                                            br.ReadInt32();
                                            float xx = (float)((int)br.ReadByte() - 128) / 128f;
                                            float yy = (float)((int)br.ReadByte() - 128) / 128f;
                                            float zz = (float)((int)br.ReadByte() - 128) / 128f;
                                            int num19 = (int)br.ReadByte();
                                            vector3DArray2[index4][index5] = new Vector3D(xx, yy, zz);
                                        }
                                        for (int index6 = 0; index6 < num15; ++index6)
                                        {
                                            if (num17 == 1)
                                            {
                                                numArray11[index4][index5, index6] = br.ReadSingle();
                                                numArray12[index4][index5, index6] = br.ReadSingle();
                                            }
                                            else
                                            {
                                                numArray11[index4][index5, index6] = (float)SystemHalf.Half.ToHalf(br.ReadUInt16());
                                                numArray12[index4][index5, index6] = (float)SystemHalf.Half.ToHalf(br.ReadUInt16());
                                            }
                                        }
                                    }
                                    int num20 = (int)br.ReadInt16();
                                    int num21 = br.ReadInt32();
                                    br.ReadInt32();
                                    if (num21 > 0)
                                    {
                                        int num19 = br.ReadInt32();
                                        int num22 = br.ReadInt32();
                                        fs.Seek((long)(num19 * num22), SeekOrigin.Current);
                                    }
                                    int num23 = br.ReadInt32();
                                    br.ReadInt32();
                                    int length6 = br.ReadInt32() / 2;
                                    if (num23 == 1)
                                        length6 /= 2;
                                    numArray10[index4] = new int[length6];
                                    for (int index5 = 0; index5 < length6; ++index5)
                                        numArray10[index4][index5] = num23 != 1 ? (int)br.ReadUInt16() : br.ReadInt32();
                                    int num24 = 3;
                                    if (this.game == 1)
                                        num24 = 4;
                                    if (this.game == 3)
                                        num24 = 4;
                                    for (int index5 = 0; index5 < num24; ++index5)
                                    {
                                        br.ReadInt32();
                                        br.ReadInt32();
                                        int num19 = br.ReadInt32();
                                        fs.Seek((long)num19, SeekOrigin.Current);
                                    }
                                }
                                if (this.game != 1)
                                    fs.Seek((long)(12 * (length2 + 1)), SeekOrigin.Current);

                                if (!Directory.Exists("staticmesh"))
                                    Directory.CreateDirectory("staticmesh");
                                StreamWriter SW_STM_ASCII = new StreamWriter("staticmesh\\" + expname + "_lod" + (object)index3 + ".ascii");
                                SW_STM_ASCII.WriteLine(0);
                                SW_STM_ASCII.WriteLine(length2);
                                for (int index4 = 0; index4 < length2; ++index4)
                                {
                                    int index5 = numArray5[index4];
                                    SW_STM_ASCII.WriteLine("Submesh_" + (object)index4);
                                    SW_STM_ASCII.WriteLine(num15);
                                    SW_STM_ASCII.WriteLine("1");
                                    SW_STM_ASCII.WriteLine(strArray[numArray4[index4]]);
                                    SW_STM_ASCII.WriteLine("0");
                                    SW_STM_ASCII.WriteLine(numArray9[index4] - numArray8[index4] + 1);
                                    for (int index6 = numArray8[index4]; index6 <= numArray9[index4]; ++index6)
                                    {
                                        Vector3D vector3D = vector3DArray1[numArray7[index4]][index6];
                                        SW_STM_ASCII.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter7 = SW_STM_ASCII;
                                        float num16 = -vector3D.Y;
                                        string str2 = " " + num16.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter7.Write(str2);
                                        SW_STM_ASCII.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_STM_ASCII.Write(vector3DArray2[numArray7[index4]][index6].X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter8 = SW_STM_ASCII;
                                        num16 = -vector3DArray2[numArray7[index4]][index6].Y;
                                        string str3 = " " + num16.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter8.Write(str3);
                                        SW_STM_ASCII.WriteLine(" " + vector3DArray2[numArray7[index4]][index6].Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_STM_ASCII.WriteLine("0 0 0 0");
                                        for (int index7 = 0; index7 < num15; ++index7)
                                            SW_STM_ASCII.WriteLine(numArray11[numArray7[index4]][index6, index7].ToString("0.######", (IFormatProvider)numberFormatInfo) + " " + numArray12[numArray7[index4]][index6, index7].ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    }
                                    SW_STM_ASCII.WriteLine(numArray6[index4]);
                                    for (int index6 = 0; index6 < numArray6[index4]; ++index6)
                                    {
                                        SW_STM_ASCII.Write(numArray10[numArray7[index4]][index5 + 2] - numArray8[index4]);
                                        SW_STM_ASCII.Write(" " + (object)(numArray10[numArray7[index4]][index5 + 1] - numArray8[index4]));
                                        SW_STM_ASCII.Write(" " + (object)(numArray10[numArray7[index4]][index5] - numArray8[index4]));
                                        SW_STM_ASCII.WriteLine();
                                        index5 += 3;
                                    }
                                }
                                SW_STM_ASCII.Close();
                            }
                        }
                        else if (this.blueprintDB.dictionary1.ContainsKey(stringList5[expNameIndex]))
                        {
                            int index3 = 0;
                            int index4 = 0;
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<prop> plist1 = new List<prop>();
                            Utilities.readprops(plist1, fs, br);
                            foreach (prop prop in plist1)
                            {
                                if (prop.name == "SkeletalMeshComponent")
                                    index3 = prop.ivalue;
                                else if (prop.name == "StaticMeshComponent")
                                    index4 = prop.ivalue;
                            }
                            if (index4 != 0)
                            {
                                fs.Seek((long)numArray1[index4], SeekOrigin.Begin);
                                List<prop> plist2 = new List<prop>();
                                Utilities.readprops(plist2, fs, br);
                                actor actor = new actor();
                                if (!dictionary4.ContainsKey(expNameIndex))
                                    dictionary4.Add(expNameIndex, staticMeshActorList.Count);
                                foreach (prop prop in plist2)
                                {
                                    if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.pos = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.rot = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.scale = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                }
                                foreach (string str2 in this.blueprintDB.stringListList1[this.blueprintDB.dictionary1[stringList5[expNameIndex]]])
                                    staticMeshActorList.Add(new actor()
                                    {
                                        pos = actor.pos,
                                        rot = actor.rot,
                                        scale = actor.scale,
                                        meshname = str2
                                    });
                            }
                            if (index3 != 0)
                            {
                                fs.Seek((long)numArray1[index3], SeekOrigin.Begin);
                                List<prop> plist2 = new List<prop>();
                                Utilities.readprops(plist2, fs, br);
                                actor actor = new actor();
                                if (!dictionary5.ContainsKey(expNameIndex))
                                    dictionary5.Add(expNameIndex, staticSkeletalMeshActorList.Count);
                                foreach (prop prop in plist2)
                                {
                                    if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.pos = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.rot = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.scale = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                }
                                foreach (string str2 in this.blueprintDB.stringListList2[this.blueprintDB.dictionary1[stringList5[expNameIndex]]])
                                    staticSkeletalMeshActorList.Add(new actor()
                                    {
                                        pos = actor.pos,
                                        rot = actor.rot,
                                        scale = actor.scale,
                                        meshname = str2
                                    });
                            }
                        }
                    }
                    namesStreamWriter?.Close();
                    exportStreamWriter?.Close();
                    importStreamWriter?.Close();
                    #endregion

                    #region LandMap
                    if (landList.Count > 0)
                    {
                        int num4 = 0;
                        foreach (land land in landList)
                        {
                            if (!intSet1.Contains(land.hmtexture))
                            {
                                intSet1.Add(land.hmtexture);
                                ++num4;
                            }
                        }
                        HashSet<int> intSet2 = new HashSet<int>();
                        StreamWriter SW_LNDS_ASCII = new StreamWriter(localPath + withoutExtension + "_landscape.ascii");
                        SW_LNDS_ASCII.WriteLine("0");
                        SW_LNDS_ASCII.WriteLine(num4);
                        foreach (land land in landList)
                        {
                            if (!intSet2.Contains(land.hmtexture))
                            {
                                intSet2.Add(land.hmtexture);
                                fs.Seek((long)numArray1[land.hmtexture], SeekOrigin.Begin);
                                Utilities.readprops(new List<prop>(), fs, br);
                                fs.Seek(24L, SeekOrigin.Current);
                                int num5 = br.ReadInt32();
                                int num6 = br.ReadInt32();
                                br.ReadInt32();
                                fs.Seek((long)br.ReadInt32(), SeekOrigin.Current);
                                br.ReadInt32();
                                br.ReadInt32();
                                br.ReadInt32();
                                int num11 = br.ReadInt32();
                                int num12 = 1;
                                fs.Seek(16L, SeekOrigin.Current);
                                if (num11 == 1281)
                                {
                                    num5 /= 2;
                                    num6 /= 2;
                                    fs.Seek(32L, SeekOrigin.Current);
                                    num12 *= 2;
                                }
                                int index2 = dictionary4[land.parent];
                                Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(staticMeshActorList[index2].rot.X, staticMeshActorList[index2].rot.Y, staticMeshActorList[index2].rot.Z);
                                Quaternion3D quaternion3D1 = Quaternion3D.Invert(quaternion);
                                SW_LNDS_ASCII.WriteLine("Section_" + (object)land.hmx + "_" + (object)land.hmy);
                                SW_LNDS_ASCII.WriteLine(1);
                                SW_LNDS_ASCII.WriteLine(0);
                                SW_LNDS_ASCII.WriteLine(num5 * num6);
                                byte[] buffer2 = new byte[num5 * num6 * 4];
                                byte[] buffer3 = new byte[64];
                                if (this.game == 0 || this.game == 2)
                                {
                                    for (int index3 = 0; index3 < num6 / 8; ++index3)
                                    {
                                        for (int index4 = 0; index4 < num5 / 8; ++index4)
                                        {
                                            for (int t = 0; t < 64; ++t)
                                            {
                                                int num13 = Utilities.morton(t, 8, 8);
                                                int num14 = num13 / 8;
                                                int num15 = num13 % 8;
                                                fs.Read(buffer3, 0, 4);
                                                if (index4 * 8 + num15 < num5 && index3 * 8 + num14 < num6)
                                                {
                                                    int destinationIndex = 4 * ((index3 * 8 + num14) * num5 + index4 * 8 + num15);
                                                    Array.Copy((Array)buffer3, 0, (Array)buffer2, destinationIndex, 4);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                    fs.Read(buffer2, 0, buffer2.Length);
                                BinaryReader binaryReader = new BinaryReader((Stream)new MemoryStream(buffer2));
                                for (int index3 = 0; index3 < num6; ++index3)
                                {
                                    for (int index4 = 0; index4 < num5; ++index4)
                                    {
                                        float zz = (float)(((int)binaryReader.ReadUInt32() & 16777215) - 8388608) / 32768f;
                                        Quaternion3D quaternion3D2 = new Quaternion3D(new Vector3D((float)(land.hmx + index4 * num12), (float)(land.hmy + index3 * num12), zz) * staticMeshActorList[index2].scale, 0.0f);
                                        Vector3D vector3D = (quaternion * quaternion3D2 * quaternion3D1).xyz + staticMeshActorList[index2].pos;
                                        SW_LNDS_ASCII.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_LNDS_ASCII.Write(" " + (-vector3D.Y).ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_LNDS_ASCII.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_LNDS_ASCII.WriteLine("0 0 0");
                                        SW_LNDS_ASCII.WriteLine("0 0 0 0");
                                        SW_LNDS_ASCII.WriteLine(((float)index4 / (float)num5).ToString("0.######", (IFormatProvider)numberFormatInfo) + " " + ((float)index3 / (float)num6).ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    }
                                }
                                SW_LNDS_ASCII.WriteLine((num5 - 1) * (num6 - 1) * 2);
                                for (int index3 = 0; index3 < num6 - 1; ++index3)
                                {
                                    for (int index4 = 0; index4 < num5 - 1; ++index4)
                                    {
                                        int num13 = index3 * num5 + index4;
                                        SW_LNDS_ASCII.WriteLine(num13.ToString() + " " + (object)(num13 + 1) + " " + (object)(num13 + num5));
                                        SW_LNDS_ASCII.WriteLine((num13 + 1).ToString() + " " + (object)(num13 + num5 + 1) + " " + (object)(num13 + num5));
                                    }
                                }
                            }
                        }
                        SW_LNDS_ASCII.Close();
                    }
                    fs.Close();
                    #endregion

                    #region LevelMap
                    if (staticMeshActorList.Count == 0)
                    {
                        if (staticSkeletalMeshActorList.Count == 0)
                            continue;
                    }
                    int num32 = 0;
                    int num33 = 0;
                    int num34 = 0;

                    StreamWriter SW_MAP_ASCII = null;
                    //Fill maps statick meshes
                    foreach (actor actor in staticMeshActorList)
                    {
                        Console.WriteLine($"ActorPos '{actor.meshname}' -> Vector({actor.pos.X},{actor.pos.Y},{actor.pos.Z}) ");
                        //Console.Write(".");
                        ++num34;
                        if (actor.meshname != null && !(actor.meshname == ""))
                        {
                            if (!File.Exists("staticmesh.raw\\" + actor.meshname + ".raw"))
                            {
                                Console.WriteLine("Missing model: " + actor.meshname);
                            }
                            else
                            {
                                //Create map file if doesnt exists
                                if (SW_MAP_ASCII == null)
                                {
                                    SW_MAP_ASCII = new StreamWriter(localPath + withoutExtension + "_map" + (object)num32 + ".ascii");
                                    SW_MAP_ASCII.WriteLine("0");
                                    SW_MAP_ASCII.WriteLine("     ");
                                }

                                FileStream fileStream4 = new FileStream("staticmesh.raw\\" + actor.meshname + ".raw", FileMode.Open, FileAccess.Read);
                                BinaryReader binaryReader = new BinaryReader(fileStream4);
                                fileStream4.Seek(34L, SeekOrigin.Current);
                                int num4 = binaryReader.ReadInt32();
                                fileStream4.Seek(num4 * 4, SeekOrigin.Current);
                                binaryReader.ReadInt32();
                                bool flag4 = false;
                                int num5 = (int)binaryReader.ReadByte();
                                int num6 = (int)binaryReader.ReadByte();
                                int length2 = (int)binaryReader.ReadByte();
                                int num11 = (int)binaryReader.ReadByte();
                                int num12 = (int)binaryReader.ReadByte();
                                int num13 = (int)binaryReader.ReadByte();
                                int[] numArray3 = new int[length2];
                                int[] numArray4 = new int[length2];
                                int[] numArray5 = new int[length2];
                                int[] numArray6 = new int[length2];
                                int[] numArray7 = new int[length2];
                                int[] numArray8 = new int[length2];
                                for (impIndex = 0; impIndex < length2; ++impIndex)
                                {
                                    numArray3[impIndex] = binaryReader.ReadInt32();
                                    numArray4[impIndex] = binaryReader.ReadInt32();
                                    numArray5[impIndex] = binaryReader.ReadInt32();
                                    numArray7[impIndex] = binaryReader.ReadInt32();
                                    numArray8[impIndex] = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                }
                                binaryReader.ReadInt32();
                                int length3 = 1;
                                int num14 = 0;
                                int[][] numArray9 = new int[length3][];
                                Vector3D[][] vector3DArray1 = new Vector3D[length3][];
                                Vector3D[][] vector3DArray2 = new Vector3D[length3][];
                                float[][,] numArray10 = new float[length3][,];
                                float[][,] numArray11 = new float[length3][,];
                                int[,] numArray12 = (int[,])null;
                                for (int index2 = 0; index2 < length3; ++index2)
                                {
                                    int index3 = num34 - 1;
                                    binaryReader.ReadInt32();
                                    int length4 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    vector3DArray1[index2] = new Vector3D[length4];
                                    for (int index4 = 0; index4 < length4; ++index4)
                                    {
                                        float xx = binaryReader.ReadSingle();
                                        float yy = binaryReader.ReadSingle();
                                        float zz = binaryReader.ReadSingle();
                                        vector3DArray1[index2][index4] = new Vector3D(xx, yy, zz);
                                    }
                                    int num15 = (int)binaryReader.ReadInt16();
                                    num14 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    int num16 = binaryReader.ReadInt32();
                                    int num17 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    int length5 = binaryReader.ReadInt32();
                                    vector3DArray2[index2] = new Vector3D[length5];
                                    numArray10[index2] = new float[length5, 8];
                                    numArray11[index2] = new float[length5, 8];
                                    numArray12 = new int[length5, 4];
                                    for (int index4 = 0; index4 < length5; ++index4)
                                    {
                                        if (num17 == 1)
                                        {
                                            binaryReader.ReadInt64();
                                            float xx = (float)((int)binaryReader.ReadUInt16() - 32768) / 32768f;
                                            float yy = (float)((int)binaryReader.ReadUInt16() - 32768) / 32768f;
                                            float zz = (float)((int)binaryReader.ReadUInt16() - 32768) / 32768f;
                                            int num18 = (int)binaryReader.ReadUInt16();
                                            vector3DArray2[index2][index4] = new Vector3D(xx, yy, zz);
                                        }
                                        else
                                        {
                                            binaryReader.ReadInt32();
                                            float xx = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                            float yy = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                            float zz = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                            int num18 = (int)binaryReader.ReadByte();
                                            vector3DArray2[index2][index4] = new Vector3D(xx, yy, zz);
                                        }
                                        for (int index5 = 0; index5 < num14; ++index5)
                                        {
                                            if (num16 == 1)
                                            {
                                                numArray10[index2][index4, index5] = binaryReader.ReadSingle();
                                                numArray11[index2][index4, index5] = binaryReader.ReadSingle();
                                            }
                                            else
                                            {
                                                numArray10[index2][index4, index5] = (float)SystemHalf.Half.ToHalf(binaryReader.ReadUInt16());
                                                numArray11[index2][index4, index5] = (float)SystemHalf.Half.ToHalf(binaryReader.ReadUInt16());
                                            }
                                        }
                                    }
                                    for (; index3 >= 0; index3 = staticMeshActorList[index3].parent < 0 || !dictionary4.ContainsKey(staticMeshActorList[index3].parent) ? -1 : dictionary4[staticMeshActorList[index3].parent])
                                    {
                                        Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(staticMeshActorList[index3].rot.X, staticMeshActorList[index3].rot.Y, staticMeshActorList[index3].rot.Z);
                                        Quaternion3D quaternion3D1 = Quaternion3D.Invert(quaternion);
                                        float[,] tm = staticMeshActorList[index3].tm;
                                        for (int index4 = 0; index4 < length5; ++index4)
                                        {
                                            if (staticMeshActorList[index3].hastrans)
                                            {
                                                float x1 = vector3DArray1[index2][index4].X;
                                                float y1 = vector3DArray1[index2][index4].Y;
                                                float z1 = vector3DArray1[index2][index4].Z;
                                                float xx1 = (float)((double)tm[0, 0] * (double)x1 + (double)tm[1, 0] * (double)y1 + (double)tm[2, 0] * (double)z1) + tm[3, 0];
                                                float yy1 = (float)((double)tm[0, 1] * (double)x1 + (double)tm[1, 1] * (double)y1 + (double)tm[2, 1] * (double)z1) + tm[3, 1];
                                                float zz1 = (float)((double)tm[0, 2] * (double)x1 + (double)tm[1, 2] * (double)y1 + (double)tm[2, 2] * (double)z1) + tm[3, 2];
                                                vector3DArray1[index2][index4] = new Vector3D(xx1, yy1, zz1);
                                                float x2 = vector3DArray2[index2][index4].X;
                                                float y2 = vector3DArray2[index2][index4].Y;
                                                float z2 = vector3DArray2[index2][index4].Z;
                                                float xx2 = (float)((double)tm[0, 0] * (double)x2 + (double)tm[1, 0] * (double)y2 + (double)tm[2, 0] * (double)z2);
                                                float yy2 = (float)((double)tm[0, 1] * (double)x2 + (double)tm[1, 1] * (double)y2 + (double)tm[2, 1] * (double)z2);
                                                float zz2 = (float)((double)tm[0, 2] * (double)x2 + (double)tm[1, 2] * (double)y2 + (double)tm[2, 2] * (double)z2);
                                                vector3DArray2[index2][index4] = new Vector3D(xx2, yy2, zz2);
                                                vector3DArray2[index2][index4].Normalize();
                                            }
                                            Quaternion3D quaternion3D2 = new Quaternion3D(vector3DArray1[index2][index4] * staticMeshActorList[index3].scale, 0.0f);
                                            Quaternion3D quaternion3D3 = quaternion * quaternion3D2 * quaternion3D1;
                                            vector3DArray1[index2][index4] = quaternion3D3.xyz + staticMeshActorList[index3].pos;
                                            Quaternion3D quaternion3D4 = new Quaternion3D(vector3DArray2[index2][index4] * staticMeshActorList[index3].scale, 0.0f);
                                            Quaternion3D quaternion3D5 = quaternion * quaternion3D4 * quaternion3D1;
                                            quaternion3D5.xyz.Normalize();
                                            vector3DArray2[index2][index4] = quaternion3D5.xyz;
                                        }
                                    }
                                    int num19 = (int)binaryReader.ReadInt16();
                                    int num20 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    if (num20 > 0)
                                    {
                                        flag4 = true;
                                        binaryReader.ReadInt32();
                                        int num18 = binaryReader.ReadInt32();
                                        for (int index4 = 0; index4 < num18; ++index4)
                                        {
                                            numArray12[index4, 0] = (int)binaryReader.ReadByte();
                                            numArray12[index4, 1] = (int)binaryReader.ReadByte();
                                            numArray12[index4, 2] = (int)binaryReader.ReadByte();
                                            numArray12[index4, 3] = (int)binaryReader.ReadByte();
                                        }
                                    }
                                    int num21 = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    int length6 = binaryReader.ReadInt32() / 2;
                                    if (num21 == 1)
                                        length6 /= 2;
                                    numArray9[index2] = new int[length6];
                                    for (impIndex = 0; impIndex < length6; ++impIndex)
                                        numArray9[index2][impIndex] = num21 != 1 ? (int)binaryReader.ReadUInt16() : binaryReader.ReadInt32();
                                }
                                bool staticMeshRaTxtExist = true;
                                string[] strArray;
                                if (File.Exists("staticmesh.raw\\" + actor.meshname + ".txt"))
                                {
                                    strArray = File.ReadAllLines("staticmesh.raw\\" + actor.meshname + ".txt");
                                    if (actor.overmat != null)
                                    {
                                        for (impIndex = 0; impIndex < actor.overmat.Length && impIndex < strArray.Length; ++impIndex)
                                        {
                                            if (actor.overmat[impIndex] < 0)
                                                strArray[impIndex] = this.impnames[-actor.overmat[impIndex]];
                                        }
                                    }
                                }
                                else
                                {
                                    staticMeshRaTxtExist = false;
                                    strArray = new string[length2];
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                        strArray[impIndex] = "Submesh_" + (object)(num33 + impIndex) + "_material";
                                }
                                for (impIndex = 0; impIndex < length2; ++impIndex)
                                {
                                    int index2 = numArray4[impIndex];
                                    SW_MAP_ASCII.WriteLine("Submesh_" + (object)(num33 + impIndex) + "_" + actor.meshname);
                                    SW_MAP_ASCII.WriteLine(num14);
                                    if (staticMeshRaTxtExist)
                                    {
                                        string key2 = strArray[numArray3[impIndex]];
                                        if (this.materialDB.dictionary2.ContainsKey(key2))
                                        {
                                            int count3 = this.materialDB.texparamListList[this.materialDB.dictionary2[key2]].Count;
                                            if (count3 == 0)
                                            {
                                                SW_MAP_ASCII.WriteLine("1");
                                                SW_MAP_ASCII.WriteLine(key2);
                                                SW_MAP_ASCII.WriteLine("0");
                                            }
                                            else
                                            {
                                                SW_MAP_ASCII.WriteLine(count3);
                                                foreach (texparam texparam in this.materialDB.texparamListList[this.materialDB.dictionary2[key2]])
                                                {
                                                    SW_MAP_ASCII.WriteLine(texparam.value + ".dds");
                                                    SW_MAP_ASCII.WriteLine("0");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            SW_MAP_ASCII.WriteLine("1");
                                            SW_MAP_ASCII.WriteLine(key2);
                                            SW_MAP_ASCII.WriteLine("0");
                                            Console.WriteLine("Material not found in database: " + key2);
                                        }
                                    }
                                    else
                                    {
                                        SW_MAP_ASCII.WriteLine("1");
                                        SW_MAP_ASCII.WriteLine(strArray[impIndex]);
                                        SW_MAP_ASCII.WriteLine("0");
                                    }
                                    SW_MAP_ASCII.WriteLine(numArray8[impIndex] - numArray7[impIndex] + 1);
                                    for (int index3 = numArray7[impIndex]; index3 <= numArray8[impIndex]; ++index3)
                                    {
                                        Vector3D vector3D = vector3DArray1[numArray6[impIndex]][index3];
                                        SW_MAP_ASCII.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter5 = SW_MAP_ASCII;
                                        float num15 = -vector3D.Y;
                                        string str2 = " " + num15.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter5.Write(str2);
                                        SW_MAP_ASCII.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_MAP_ASCII.Write(vector3DArray2[numArray6[impIndex]][index3].X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter6 = SW_MAP_ASCII;
                                        num15 = -vector3DArray2[numArray6[impIndex]][index3].Y;
                                        string str3 = " " + num15.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter6.Write(str3);
                                        SW_MAP_ASCII.WriteLine(" " + vector3DArray2[numArray6[impIndex]][index3].Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        if (flag4)
                                            SW_MAP_ASCII.WriteLine(numArray12[index3, 0].ToString() + " " + (object)numArray12[index3, 1] + " " + (object)numArray12[index3, 2] + " " + (object)numArray12[index3, 3]);
                                        else
                                            SW_MAP_ASCII.WriteLine("0 0 0 0");
                                        for (int index4 = 0; index4 < num14; ++index4)
                                            SW_MAP_ASCII.WriteLine(numArray10[numArray6[impIndex]][index3, index4].ToString("0.######", (IFormatProvider)numberFormatInfo) + " " + numArray11[numArray6[impIndex]][index3, index4].ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    }
                                    bool flag6 = true;
                                    if (actor.scale.X < 0.0)
                                        flag6 = !flag6;
                                    if (actor.scale.Y < 0.0)
                                        flag6 = !flag6;
                                    if (actor.scale.Z < 0.0)
                                        flag6 = !flag6;
                                    SW_MAP_ASCII.WriteLine(numArray5[impIndex]);
                                    for (int index3 = 0; index3 < numArray5[impIndex]; ++index3)
                                    {
                                        if (flag6)
                                        {
                                            SW_MAP_ASCII.Write(numArray9[numArray6[impIndex]][index2 + 2] - numArray7[impIndex]);
                                            SW_MAP_ASCII.Write(" " + (object)(numArray9[numArray6[impIndex]][index2 + 1] - numArray7[impIndex]));
                                            SW_MAP_ASCII.Write(" " + (object)(numArray9[numArray6[impIndex]][index2] - numArray7[impIndex]));
                                        }
                                        else
                                        {
                                            SW_MAP_ASCII.Write(numArray9[numArray6[impIndex]][index2] - numArray7[impIndex]);
                                            SW_MAP_ASCII.Write(" " + (object)(numArray9[numArray6[impIndex]][index2 + 1] - numArray7[impIndex]));
                                            SW_MAP_ASCII.Write(" " + (object)(numArray9[numArray6[impIndex]][index2 + 2] - numArray7[impIndex]));
                                        }
                                        SW_MAP_ASCII.WriteLine();
                                        index2 += 3;
                                    }
                                }
                                fileStream4.Close();
                                num33 += length2;
                                if (SW_MAP_ASCII.BaseStream.Length > 100000000L)
                                {
                                    SW_MAP_ASCII.Close();
                                    SW_MAP_ASCII = (StreamWriter)null;
                                    FileStream fileStream5 = new FileStream(localPath + withoutExtension + "_map" + (object)num32 + ".ascii", FileMode.Open);
                                    fileStream5.Seek(3L, SeekOrigin.Begin);
                                    string str2 = num33.ToString();
                                    for (impIndex = 0; impIndex < str2.Length; ++impIndex)
                                        fileStream5.WriteByte((byte)str2[impIndex]);
                                    fileStream5.Close();
                                    ++num32;
                                    num33 = 0;
                                }
                            }
                        }
                    }
                    if (SW_MAP_ASCII != null)
                    {
                        SW_MAP_ASCII.Close();
                        FileStream fileStream4 = new FileStream(localPath + withoutExtension + "_map" + (object)num32 + ".ascii", FileMode.Open);
                        fileStream4.Seek(3L, SeekOrigin.Begin);
                        foreach (byte num4 in num33.ToString())
                            fileStream4.WriteByte(num4);
                        fileStream4.Close();
                    }
                    #endregion

                    #region SkyMap
                    StreamWriter SW_SKMMAP_ASCII =  null;
                    int num35 = 0;
                    int num36 = 0;
                    foreach (actor actor in staticSkeletalMeshActorList)
                    {
                        Console.Write("+");
                        ++num36;
                        if (actor.meshname == null || !(actor.meshname != ""))
                        {
                            if (!File.Exists("skeletalmesh.raw\\" + actor.meshname + ".raw"))
                            {
                                Console.WriteLine("Missing skeletal model: " + actor.meshname);
                            }
                            else
                            {
                                if (SW_SKMMAP_ASCII == null)
                                {
                                    SW_SKMMAP_ASCII = new StreamWriter(localPath + withoutExtension + "_skmap" + (object)num32 + ".ascii");
                                    SW_SKMMAP_ASCII.WriteLine("0");
                                    SW_SKMMAP_ASCII.WriteLine("     ");
                                }
                                FileStream skMeshRawFS = new FileStream("skeletalmesh.raw\\" + actor.meshname + ".raw", FileMode.Open, FileAccess.Read);
                                BinaryReader binaryReader = new BinaryReader((Stream)skMeshRawFS);
                                int num4 = (int)binaryReader.ReadByte();
                                binaryReader.ReadInt32();
                                bool flag4 = false;
                                if (num4 != 0)
                                    flag4 = true;
                                bool flag5 = false;
                                int num5 = (int)binaryReader.ReadByte();
                                int num6 = (int)binaryReader.ReadByte();
                                int length2 = binaryReader.ReadInt32();
                                int[] numArray3 = new int[length2];
                                int[] numArray4 = new int[length2];
                                int[] numArray5 = new int[length2];
                                int[] numArray6 = new int[length2];
                                int[][] numArray7 = new int[length2][];
                                int[] numArray8 = new int[length2];
                                int[] numArray9 = new int[length2];
                                for (int index2 = 0; index2 < length2; ++index2)
                                {
                                    int num11 = (int)binaryReader.ReadInt16();
                                    numArray3[index2] = (int)binaryReader.ReadInt16();
                                    numArray4[index2] = binaryReader.ReadInt32();
                                    numArray5[index2] = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    numArray8[index2] = binaryReader.ReadInt32();
                                    int length3 = binaryReader.ReadInt32();
                                    numArray7[index2] = new int[length3];
                                    for (int index3 = 0; index3 < length3; ++index3)
                                        numArray7[index2][index3] = (int)binaryReader.ReadInt16();
                                    numArray9[index2] = binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    skMeshRawFS.Seek(34L, SeekOrigin.Current);
                                    int num12 = binaryReader.ReadInt32();
                                    int num13 = binaryReader.ReadInt32();
                                    if (num12 > 0)
                                    {
                                        if (this.game == 2)
                                        {
                                            flag5 = true;
                                            skMeshRawFS.Seek((long)(num12 * num13 * 16), SeekOrigin.Current);
                                        }
                                        else
                                            skMeshRawFS.Seek((long)(num12 * num13 * 24), SeekOrigin.Current);
                                    }
                                }
                                int num14 = (int)binaryReader.ReadByte();
                                int num15 = (int)binaryReader.ReadByte();
                                int num16 = (int)binaryReader.ReadByte();
                                int num17 = (int)binaryReader.ReadByte();
                                int num18 = (int)binaryReader.ReadByte();
                                int length4 = 1;
                                int num19 = 0;
                                int[][] numArray10 = new int[length4][];
                                Vector3D[][] vector3DArray1 = new Vector3D[length4][];
                                Vector3D[][] vector3DArray2 = new Vector3D[length4][];
                                float[][,] numArray11 = new float[length4][,];
                                float[][,] numArray12 = new float[length4][,];
                                int[][,] numArray13 = new int[length4][,];
                                int[][,] numArray14 = new int[length4][,];
                                int num20 = 0;
                                for (int index2 = 0; index2 < length4; ++index2)
                                {
                                    int index3 = num36 - 1;
                                    int length3 = binaryReader.ReadInt32();
                                    numArray10[index2] = new int[length3];
                                    for (int index4 = 0; index4 < length3; ++index4)
                                        numArray10[index2][index4] = num14 != 4 ? (int)binaryReader.ReadUInt16() : binaryReader.ReadInt32();
                                    skMeshRawFS.Seek((long)(binaryReader.ReadInt32() * 2), SeekOrigin.Current);
                                    binaryReader.ReadInt32();
                                    binaryReader.ReadInt32();
                                    skMeshRawFS.Seek((long)(binaryReader.ReadInt32() * 2), SeekOrigin.Current);
                                    skMeshRawFS.Seek((long)(binaryReader.ReadInt32() * 4), SeekOrigin.Current);
                                    binaryReader.ReadInt32();
                                    num19 = binaryReader.ReadInt32();
                                    int num11 = (int)binaryReader.ReadInt16();
                                    binaryReader.ReadInt32();
                                    int num12 = binaryReader.ReadInt32();
                                    Vector3D vector3D1 = new Vector3D(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                                    Vector3D vector3D2 = new Vector3D(binaryReader.ReadSingle(), binaryReader.ReadSingle(), binaryReader.ReadSingle());
                                    binaryReader.ReadInt32();
                                    int length5 = binaryReader.ReadInt32();
                                    vector3DArray1[index2] = new Vector3D[length5];
                                    vector3DArray2[index2] = new Vector3D[length5];
                                    numArray11[index2] = new float[length5, 4];
                                    numArray12[index2] = new float[length5, 4];
                                    numArray13[index2] = new int[length5, 8];
                                    numArray14[index2] = new int[length5, 8];
                                    for (int index4 = 0; index4 < length5; ++index4)
                                    {
                                        binaryReader.ReadInt32();
                                        float xx1 = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                        float yy1 = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                        float zz1 = (float)((int)binaryReader.ReadByte() - 128) / 128f;
                                        int num13 = (int)binaryReader.ReadByte();
                                        vector3DArray2[index2][index4] = new Vector3D(xx1, yy1, zz1);
                                        float xx2 = binaryReader.ReadSingle();
                                        float yy2 = binaryReader.ReadSingle();
                                        float zz2 = binaryReader.ReadSingle();
                                        vector3DArray1[index2][index4] = new Vector3D(xx2, yy2, zz2);
                                        for (int index5 = 0; index5 < num19; ++index5)
                                        {
                                            if (num12 == 1)
                                            {
                                                numArray11[index2][index4, index5] = binaryReader.ReadSingle();
                                                numArray12[index2][index4, index5] = binaryReader.ReadSingle();
                                            }
                                            else
                                            {
                                                numArray11[index2][index4, index5] = (float)SystemHalf.Half.ToHalf(binaryReader.ReadUInt16());
                                                numArray12[index2][index4, index5] = (float)SystemHalf.Half.ToHalf(binaryReader.ReadUInt16());
                                            }
                                        }
                                    }
                                    for (; index3 >= 0; index3 = staticSkeletalMeshActorList[index3].parent < 0 || !dictionary4.ContainsKey(staticSkeletalMeshActorList[index3].parent) ? -1 : dictionary4[staticSkeletalMeshActorList[index3].parent])
                                    {
                                        Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(staticSkeletalMeshActorList[index3].rot.X, staticSkeletalMeshActorList[index3].rot.Y, staticSkeletalMeshActorList[index3].rot.Z);
                                        Quaternion3D quaternion3D1 = Quaternion3D.Invert(quaternion);
                                        for (int index4 = 0; index4 < length5; ++index4)
                                        {
                                            Quaternion3D quaternion3D2 = new Quaternion3D(vector3DArray1[index2][index4] * staticSkeletalMeshActorList[index3].scale, 0.0f);
                                            Quaternion3D quaternion3D3 = quaternion * quaternion3D2 * quaternion3D1;
                                            vector3DArray1[index2][index4] = quaternion3D3.xyz + staticSkeletalMeshActorList[index3].pos;
                                            Quaternion3D quaternion3D4 = new Quaternion3D(vector3DArray2[index2][index4] * staticSkeletalMeshActorList[index3].scale, 0.0f);
                                            Quaternion3D quaternion3D5 = quaternion * quaternion3D4 * quaternion3D1;
                                            quaternion3D5.xyz.Normalize();
                                            vector3DArray2[index2][index4] = quaternion3D5.xyz;
                                        }
                                    }
                                    binaryReader.ReadInt32();
                                    int num21 = (int)binaryReader.ReadInt16();
                                    num20 = binaryReader.ReadInt32();
                                    int num22 = binaryReader.ReadInt32();
                                    int num23 = binaryReader.ReadInt32();
                                    if (num22 == 4)
                                    {
                                        for (int index4 = 0; index4 < num23; ++index4)
                                        {
                                            numArray13[index2][index4, 0] = binaryReader.ReadInt32();
                                            numArray14[index2][index4, 0] = (int)byte.MaxValue;
                                        }
                                    }
                                    else
                                    {
                                        int num13 = 4;
                                        if (num22 == 16)
                                            num13 = 8;
                                        for (int index4 = 0; index4 < num23; ++index4)
                                        {
                                            for (int index5 = 0; index5 < num13; ++index5)
                                                numArray13[index2][index4, index5] = (int)binaryReader.ReadByte();
                                            for (int index5 = 0; index5 < num13; ++index5)
                                                numArray14[index2][index4, index5] = (int)binaryReader.ReadByte();
                                        }
                                    }
                                    if (flag5)
                                    {
                                        int num13 = (int)binaryReader.ReadInt16();
                                        int num24 = binaryReader.ReadInt32();
                                        int num25 = binaryReader.ReadInt32();
                                        skMeshRawFS.Seek((long)(num25 * num24), SeekOrigin.Current);
                                        int num26 = (int)binaryReader.ReadInt16();
                                        num14 = binaryReader.ReadInt32();
                                        int num27 = binaryReader.ReadInt32();
                                        skMeshRawFS.Seek((long)(num14 * num27), SeekOrigin.Current);
                                    }
                                    if (flag4)
                                    {
                                        int num13 = (int)binaryReader.ReadInt16();
                                        binaryReader.ReadInt32();
                                        binaryReader.ReadInt32();
                                        int num24 = binaryReader.ReadInt32();
                                        int num25 = binaryReader.ReadInt32();
                                        skMeshRawFS.Seek((long)(num24 * num25), SeekOrigin.Current);
                                    }
                                }
                                bool flag6 = true;
                                string[] matNameArray;
                                if (File.Exists("skeletalmesh.raw\\" + actor.meshname + ".txt"))
                                {
                                    matNameArray = File.ReadAllLines("skeletalmesh.raw\\" + actor.meshname + ".txt");
                                }
                                else
                                {
                                    flag6 = false;
                                    matNameArray = new string[length2];
                                    for (int i = 0; i < length2; ++i)
                                        matNameArray[i] = "Submesh_" + (object)(num35 + i) + "_material";
                                }
                                for (int index2 = 0; index2 < length2; ++index2)
                                {
                                    int index3 = numArray4[index2];
                                    SW_SKMMAP_ASCII.WriteLine("Submesh_" + (object)(num35 + index2) + "_" + actor.meshname);
                                    SW_SKMMAP_ASCII.WriteLine(num19);
                                    if (flag6)
                                    {
                                        string key2 = matNameArray[numArray3[index2]];
                                        if (this.materialDB.dictionary2.ContainsKey(key2))
                                        {
                                            int count3 = this.materialDB.texparamListList[this.materialDB.dictionary2[key2]].Count;
                                            SW_SKMMAP_ASCII.WriteLine(count3);
                                            foreach (texparam texparam in this.materialDB.texparamListList[this.materialDB.dictionary2[key2]])
                                            {
                                                SW_SKMMAP_ASCII.WriteLine(texparam.value + ".dds");
                                                SW_SKMMAP_ASCII.WriteLine("0");
                                            }
                                        }
                                        else
                                        {
                                            SW_SKMMAP_ASCII.WriteLine("1");
                                            SW_SKMMAP_ASCII.WriteLine(key2);
                                            SW_SKMMAP_ASCII.WriteLine("0");
                                            Console.WriteLine("Material not found in database: " + key2);
                                        }
                                    }
                                    else
                                    {
                                        SW_SKMMAP_ASCII.WriteLine("1");
                                        SW_SKMMAP_ASCII.WriteLine(matNameArray[index2]);
                                        SW_SKMMAP_ASCII.WriteLine("0");
                                    }
                                    SW_SKMMAP_ASCII.WriteLine(numArray9[index2]);
                                    for (int index4 = numArray8[index2]; index4 < numArray8[index2] + numArray9[index2]; ++index4)
                                    {
                                        Vector3D vector3D = vector3DArray1[numArray6[index2]][index4];
                                        SW_SKMMAP_ASCII.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter5 = SW_SKMMAP_ASCII;
                                        float num11 = -vector3D.Y;
                                        string str2 = " " + num11.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter5.Write(str2);
                                        SW_SKMMAP_ASCII.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_SKMMAP_ASCII.Write(vector3DArray2[numArray6[index2]][index4].X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        StreamWriter streamWriter6 = SW_SKMMAP_ASCII;
                                        num11 = -vector3DArray2[numArray6[index2]][index4].Y;
                                        string str3 = " " + num11.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                        streamWriter6.Write(str3);
                                        SW_SKMMAP_ASCII.WriteLine(" " + vector3DArray2[numArray6[index2]][index4].Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                        SW_SKMMAP_ASCII.WriteLine("0 0 0 0");
                                        for (int index5 = 0; index5 < num19; ++index5)
                                            SW_SKMMAP_ASCII.WriteLine(numArray11[numArray6[index2]][index4, index5].ToString("0.######", (IFormatProvider)numberFormatInfo) + " " + numArray12[numArray6[index2]][index4, index5].ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    }
                                    SW_SKMMAP_ASCII.WriteLine(numArray5[index2]);
                                    for (int index4 = 0; index4 < numArray5[index2]; ++index4)
                                    {
                                        SW_SKMMAP_ASCII.Write(numArray10[numArray6[index2]][index3 + 2] - numArray8[index2]);
                                        SW_SKMMAP_ASCII.Write(" " + (object)(numArray10[numArray6[index2]][index3 + 1] - numArray8[index2]));
                                        SW_SKMMAP_ASCII.Write(" " + (object)(numArray10[numArray6[index2]][index3] - numArray8[index2]));
                                        SW_SKMMAP_ASCII.WriteLine();
                                        index3 += 3;
                                    }
                                }
                                skMeshRawFS.Close();
                                num35 += length2;
                                if (SW_SKMMAP_ASCII.BaseStream.Length > 100000000L)
                                {
                                    SW_SKMMAP_ASCII.Close();
                                    SW_SKMMAP_ASCII = (StreamWriter)null;
                                    FileStream fileStream5 = new FileStream(localPath + withoutExtension + "_skmap" + (object)num32 + ".ascii", FileMode.Open);
                                    fileStream5.Seek(3L, SeekOrigin.Begin);
                                    foreach (byte num11 in num35.ToString())
                                        fileStream5.WriteByte(num11);
                                    fileStream5.Close();
                                    ++num32;
                                    num35 = 0;
                                }
                            }
                        }
                    }
                    if (SW_SKMMAP_ASCII != null)
                    {
                        SW_SKMMAP_ASCII.Close();
                        FileStream fileStream4 = new FileStream(localPath + withoutExtension + "_skmap" + (object)num32 + ".ascii", FileMode.Open);
                        fileStream4.Seek(3L, SeekOrigin.Begin);
                        foreach (byte num4 in num35.ToString())
                            fileStream4.WriteByte(num4);
                        fileStream4.Close();
                    }
                    #endregion

                    #region LightMap
                    StreamWriter SW_LMAP_ASCII = null;
                    int num37 = 0;
                    foreach (actor actor in lightActorList)
                    {
                        Console.Write(".");
                        ++num36;
                        if (SW_LMAP_ASCII == null)
                        {
                            SW_LMAP_ASCII = new StreamWriter(localPath + withoutExtension + "_lmap" + (object)num32 + ".ascii");
                            SW_LMAP_ASCII.WriteLine("0");
                            SW_LMAP_ASCII.WriteLine("     ");
                        }
                        Vector3D[] vector3DArray = new Vector3D[4]
                        {
              new Vector3D(50f, 0.0f, -50f),
              new Vector3D(0.0f, 50f, -50f),
              new Vector3D(-50f, 0.0f, 50f),
              new Vector3D(0.0f, -50f, 50f)
                        };
                        for (int index2 = 0; index2 < 4; ++index2)
                            vector3DArray[index2] += actor.pos;
                        if (actor.parent >= 0)
                        {
                            if (dictionary4.ContainsKey(actor.parent))
                            {
                                for (int index2 = 0; index2 < 4; ++index2)
                                    vector3DArray[index2] += staticMeshActorList[dictionary4[actor.parent]].pos;
                            }
                            if (dictionary5.ContainsKey(actor.parent))
                            {
                                for (int index2 = 0; index2 < 4; ++index2)
                                    vector3DArray[index2] += staticSkeletalMeshActorList[dictionary5[actor.parent]].pos;
                            }
                        }
                        ++num37;
                        SW_LMAP_ASCII.WriteLine("Light_" + (object)num37);
                        SW_LMAP_ASCII.WriteLine(0);
                        SW_LMAP_ASCII.WriteLine("0");
                        SW_LMAP_ASCII.WriteLine(4);
                        for (int index2 = 0; index2 < 4; ++index2)
                        {
                            Vector3D vector3D = vector3DArray[index2];
                            SW_LMAP_ASCII.Write(vector3D.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                            SW_LMAP_ASCII.Write(" " + (-vector3D.Y).ToString("0.######", (IFormatProvider)numberFormatInfo));
                            SW_LMAP_ASCII.WriteLine(" " + vector3D.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
                            SW_LMAP_ASCII.WriteLine("0 0 0");
                            SW_LMAP_ASCII.WriteLine("0 0 0 0");
                        }
                        SW_LMAP_ASCII.WriteLine(2);
                        SW_LMAP_ASCII.WriteLine("0 1 2");
                        SW_LMAP_ASCII.WriteLine("2 3 0");
                        if (SW_LMAP_ASCII.BaseStream.Length > 100000000L)
                        {
                            SW_LMAP_ASCII.Close();
                            SW_LMAP_ASCII = (StreamWriter)null;
                            FileStream fileStream4 = new FileStream(localPath + withoutExtension + "_lmap" + (object)num32 + ".ascii", FileMode.Open);
                            fileStream4.Seek(3L, SeekOrigin.Begin);
                            foreach (byte num4 in num37.ToString())
                                fileStream4.WriteByte(num4);
                            fileStream4.Close();
                            ++num32;
                            num37 = 0;
                        }
                    }
                    if (SW_LMAP_ASCII != null)
                    {
                        SW_LMAP_ASCII.Close();
                        FileStream fileStream4 = new FileStream(localPath + withoutExtension + "_lmap" + (object)num32 + ".ascii", FileMode.Open);
                        fileStream4.Seek(3L, SeekOrigin.Begin);
                        foreach (byte num4 in num37.ToString())
                            fileStream4.WriteByte(num4);
                        fileStream4.Close();
                    }
                    #endregion
                }


                catch (Exception ex)
                {
                    Console.WriteLine("Error exporting asset " + uassetPath + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }

            }
            //FOREACH END
            
            if (dirty)
            {
                FileStream fileStream = new FileStream("material.db", FileMode.Create);
                BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream);
                binaryWriter.Write(this.materialDB.dictionary2.Count);
                foreach (string key in this.materialDB.dictionary2.Keys)
                {
                    binaryWriter.Write(key.Length);
                    binaryWriter.Write(key.ToCharArray());
                }
                binaryWriter.Write(this.materialDB.dictionary3.Count);
                foreach (string key in this.materialDB.dictionary3.Keys)
                {
                    binaryWriter.Write(key.Length);
                    binaryWriter.Write(key.ToCharArray());
                }
                for (int index = 0; index < this.materialDB.dictionary2.Count; ++index)
                {
                    binaryWriter.Write(this.materialDB.texparamListList[index].Count);
                    foreach (texparam texparam in this.materialDB.texparamListList[index])
                    {
                        binaryWriter.Write((byte)texparam.type);
                        binaryWriter.Write(texparam.value.Length);
                        binaryWriter.Write(texparam.value.ToCharArray());
                    }
                }
                fileStream.Close();
            }
            
            
            if (!flag1)
                return;

            blueprintDB.Save();
        }
    }
 }



