using APPLIB;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using UE.UE4;
using static UE.UStaticMeshActor;

namespace UE {



    public class UAssetParser
    {
        public UAParserConfig config = new UAParserConfig();
 
        private int game = 0;
        private string[] names = new string[] { };
        private string[] expnames = new string[] { };
        private string[] impnames = new string[] { };
        public BlueprintDB blueprintDB;
        public MaterialDB materialDB;
        public bool exportTextures = false;
        public bool exportLights = false;
        public bool exportMaps = false;
        NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
        public Action<string> OnDebugMessage=null;

        public UAssetParser(UAParserConfig config)
        {
            this.setConfig(config);
        }

        public void setConfig(UAParserConfig config)
        {
            this.config = config;
            this.LoadDBs();
        }

        public void DebugWrite(string txt)
        {
            OnDebugMessage?.Invoke(txt);
        }

        public void DebugWriteLine(string txt)
        {
            OnDebugMessage?.Invoke(txt + "\r\n");
        }

        public void LoadDBs()
        {
            blueprintDB = new BlueprintDB(config.BlueprintDBPath);
            materialDB = new MaterialDB(config.MaterialDBPath);
        }
        
        public UAssetParserResult ParseUAsset(string path)
        {
            var result = new UAssetParserResult(ref this.config);
            
            //string localPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            var uassetList = new List<string>();
            if (!string.IsNullOrEmpty(path))
                uassetList.Add(path);
            /*else
            {
                foreach (FileInfo file in new DirectoryInfo(localPath).GetFiles("*.uasset", SearchOption.AllDirectories))
                    uassetList.Add(file.FullName);
            }*/
            result.uAssets = ProcessUAssets(uassetList.ToArray());
            result.blueprintDB = this.blueprintDB;
            result.materialDB = this.materialDB;
            result.impnames = this.impnames;
            return result;
        }

        private List<UAsset> ProcessUAssets(string[] uAssetsPaths, string[] args = null)
        {
            var uAssetsList = new List<UAsset>();
            if (args == null)
                args = new string[] { };
            
            numberFormatInfo.NumberDecimalSeparator = ".";
            var mn = new MagicNumbers();
            bool saveBPDB = false;
            bool dirty = false;
            //string localPath = rootPath;//Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "\\";
            var hasArgs = false;
            MemoryStream fs;
            BinaryReader br;
            var texDirFiles = Directory.GetFiles(this.config.TexturesFolder);
            foreach (var uassetPath in uAssetsPaths)
            {
                var uAsset = new UAsset(Path.GetFileNameWithoutExtension(uassetPath));
                try
                {
                    Console.WriteLine("Reading: " + uassetPath);
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

                    fs = new MemoryStream(buffer1);
                    br = new BinaryReader(fs);
   
                    StreamWriter namesStreamWriter = null;
                    StreamWriter exportStreamWriter = null;
                    StreamWriter importStreamWriter = null;

                   /* if (File.Exists(localPath + withoutExtension + "_lights.txt"))
                        File.Delete(localPath + withoutExtension + "_lights.txt");

                    if (hasArgs)
                    {
                        namesStreamWriter = new StreamWriter(localPath + withoutExtension + ".names.txt");
                        exportStreamWriter = new StreamWriter(localPath + withoutExtension + ".export.txt");
                        importStreamWriter = new StreamWriter(localPath + withoutExtension + ".import.txt");
                    }*/


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
                            uAsset.SkeletalMeshNameList.Add(this.impnames[impIndex]);
                        }
                        else if (this.names[nameIndex].ToLower() == "staticmesh")
                        {
                            ++num8;
                            uAsset.staticMeshNameList.Add(this.impnames[impIndex]);
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

                    var namesList = new List<string>();
                    namesList.Add("");

                    fs.Seek(offset2, SeekOrigin.Begin);
                    string key1 = "";

                    #region Blueprints
                    for (int index2 = 1; index2 <= num2; ++index2)
                    {
                        int index3 = br.ReadInt32();
                        if (index3 < 0)
                            namesList.Add(this.impnames[-index3]);
                        else
                            namesList.Add(this.expnames[index3]);
                        br.ReadInt32();
                        int num4 = br.ReadInt32();
                        br.ReadInt32();
                        this.expnames[index2] = this.names[br.ReadInt32()];
                        //UNUSED
                        
                        int num5 = br.ReadInt32();
                        if (num5 > 0)
                        {
                            string[] unused_expnames;
                            int index4;
                            string str2 = (unused_expnames = this.expnames)[(index4 = index2)] + "_" + (num5 - 1).ToString();
                            unused_expnames[index4] = str2;
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
                                exportStreamWriter?.WriteLine(index2.ToString("X") + "\t" + numArray1[index2].ToString("X") + "\t" + numArray2[index2].ToString("X") + "\t->" + num4.ToString("X") + "\t" + this.expnames[index2] + "." + namesList[index2]);
                            if (namesList[index2].ToLower() == "blueprintgeneratedclass")
                            {
                                key1 = this.expnames[index2];
                                ++num9;
                            }
                        }
                    }


                    if (num9 > 0 && !this.blueprintDB.dictionary1.ContainsKey(key1))
                    {
                        this.blueprintDB.dictionary1.Add(key1, this.blueprintDB.dictionary1.Count);
                        this.blueprintDB.countListB.Add(uAsset.SkeletalMeshNameList);
                        this.blueprintDB.countListA.Add(uAsset.staticMeshNameList);
                        saveBPDB = true;
                    }
                    #endregion

                    
                    var dictionary4 = new Dictionary<int, int>();
                    var dictionary5 = new Dictionary<int, int>();
                    
                    
                    int num10 = 0;

                    #region Actors

                    for (int expNameIndex = 1; expNameIndex <= num2; ++expNameIndex)
                    {
                        //DebugWriteLine(namesList[expNameIndex].ToLower());
                        if (namesList[expNameIndex].ToLower() == "texture2d")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            Utilities.readprops(new List<Prop>(), fs, br, names);
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
                               // if (!Directory.Exists("textures"))
                                 //   Directory.CreateDirectory("textures");
                                if (!File.Exists($"{config.TexturesFolder}\\{textureOutName}"))
                                {
                                    DebugWriteLine("Exporting texture: " + this.expnames[expNameIndex]);
                                    FileStream textureFileStream = new FileStream($"{config.TexturesFolder}\\{textureOutName}", FileMode.Create);
                                    BinaryWriter binaryWriter = new BinaryWriter((Stream)textureFileStream);
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
                                        DebugWriteLine("Unsupported texture format " + name);
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
                                    textureFileStream.Seek(44L, SeekOrigin.Current);
                                    binaryWriter.Write(32);
                                    binaryWriter.Write(4);
                                    binaryWriter.Write(num11);
                                    textureFileStream.Seek(40L, SeekOrigin.Current);
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
                                    textureFileStream.Write(buffer2, 0, count3);
                                    textureFileStream.Close();
                                }
                            }
                        }
                        else if (namesList[expNameIndex].ToLower() == "PointLightComponent".ToLower() || namesList[expNameIndex].ToLower() == "SpotLightComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist = new List<Prop>();
                            Utilities.readprops(plist, fs, br, names);
                            ++num10;
                            UActor actor = new UActor();
                            foreach (Prop prop in plist)
                            {
                                if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.relativeLocation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.relativeRotation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    actor.attachParent = prop.ivalue;
                            }
                            if (exportLights)
                            {
                                StreamWriter sw = new StreamWriter($"{config.ExportFolder}/{withoutExtension}_lights.txt", true);
                                sw.WriteLine(namesList[expNameIndex] + " " + this.expnames[expNameIndex]);
                                sw.WriteLine("-------------------------------------- [" + (object)num10 + "]");
                                Utilities.Printprops(plist, fs, br, sw);
                                sw.WriteLine();
                                sw.Close();
                            }
                            uAsset.lightActorList.Add(actor);
                            DebugWriteLine("L");
                        }
                        else if (namesList[expNameIndex].ToLower() == "skeletalmesh")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            int num4 = 0;
                            bool flag4 = false;
                            List<int> intList = new List<int>();
                            List<Prop> plist1 = new List<Prop>();
                            Utilities.readprops(plist1, fs, br, names);
                            foreach (Prop prop1 in plist1)
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
                                        List<Prop> plist2 = new List<Prop>();
                                        Utilities.readprops(plist2, fs, br, names);
                                        foreach (Prop prop2 in plist2)
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
                            DebugWrite(".");
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
                               // if (!Directory.Exists("skeletalmesh"))
                               ///     Directory.CreateDirectory("skeletalmesh");
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
                            //if (!Directory.Exists("skeletalmesh.raw"))
                            //  Directory.CreateDirectory("skeletalmesh.raw");
                            int count3 = (int)((long)(numArray1[expNameIndex] + numArray2[expNameIndex]) - fs.Position);
                            byte[] numArray4 = new byte[count3 + 1];
                            fs.Read(numArray4, 1, count3);
                            if (flag4)
                                numArray4[0] = (byte)1;
                            File.WriteAllBytes($"{config.StaticMeshRawFolder}\\{expname}.raw", numArray4);
                            fs.Seek((long)-count3, SeekOrigin.Current);
                            StreamWriter SW_SKM_RAW_TXT = new StreamWriter($"{config.StaticMeshRawFolder}\\{expname}.txt");
                            foreach (string materialName in strArray1)
                            {
                               //TODO:In this point you should save the materials in the actor
                                SW_SKM_RAW_TXT.WriteLine(materialName);
                            }
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
                                    string matKeyA = strArray1[numArray3[index4]];
                                    if (this.materialDB.countListA.ContainsKey(matKeyA))
                                    {
                                        DebugWriteLine("Material found in database: " + matKeyA);
                                        int count4 = this.materialDB.texparamListList[this.materialDB.countListA[matKeyA]].Count;
                                        streamWriter6.WriteLine(count4);
                                        foreach (TexParam texparam in this.materialDB.texparamListList[this.materialDB.countListA[matKeyA]])
                                        {
                                            streamWriter6.WriteLine(texparam.value + ".dds");
                                            streamWriter6.WriteLine("0");
                                        }
                                    }
                                    else
                                    {
                                        streamWriter6.WriteLine("1");
                                        streamWriter6.WriteLine(matKeyA);
                                        streamWriter6.WriteLine("0");
                                        DebugWriteLine("Material not found in database: " + matKeyA);
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
                        else if (namesList[expNameIndex].ToLower() == "InstancedStaticMeshComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist = new List<Prop>();
                            Utilities.readprops(plist, fs, br,names);
                            br.ReadInt32();
                            int num4 = br.ReadInt32();
                            fs.Seek((long)(18 * num4), SeekOrigin.Current);
                            int num5 = br.ReadInt32();
                            int num6 = br.ReadInt32();
                            if (num5 != 80)
                                DebugWriteLine("Unknown matrix size " + (object)num5);
                            long position = fs.Position;
                            var ISMCActor = new UStaticMeshActor();
                            foreach (Prop prop in plist)
                            {
                                if (prop.type == "ObjectProperty" && prop.name == "StaticMesh")
                                    ISMCActor.staticMeshFile = prop.ivalue >= 0 ? this.expnames[prop.ivalue] : this.impnames[-prop.ivalue];
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    ISMCActor.attachParent = prop.ivalue;
                                else if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    ISMCActor.relativeLocation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    ISMCActor.relativeRotation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    ISMCActor.relativeScale3D = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                            }
                            fs.Seek(position, SeekOrigin.Begin);
                            for (int index3 = 0; index3 < num6; ++index3)
                            {
                                ISMCActor.hastrans = true;
                                ISMCActor.tm = new float[5, 4];
                                for (int i = 0; i < 5; ++i) {
                                    for (int k = 0; k < 4; ++k)
                                        ISMCActor.tm[i, k] = br.ReadSingle();
                                }
                                uAsset.staticMeshActorList.Add(ISMCActor);
                            }
                        }
                        else if (namesList[expNameIndex].ToLower() == "MaterialInstanceConstant".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist1 = new List<Prop>();
                            Utilities.readprops(plist1, fs, br,names);
                            if (!Directory.Exists("materials"))
                                Directory.CreateDirectory("materials");
                            StreamWriter SW_MAT_TXT = new StreamWriter("materials\\" + withoutExtension + ".txt");
                            SW_MAT_TXT.WriteLine(namesList[expNameIndex] + " " + this.expnames[expNameIndex]);
                            SW_MAT_TXT.WriteLine("======================================");
                            Utilities.Printprops(plist1, fs, br, SW_MAT_TXT);
                            SW_MAT_TXT.WriteLine();
                            List<TexParam> texparamList = new List<TexParam>();
                            foreach (Prop prop1 in plist1)
                            {
                                if (prop1.type == "ArrayProperty" && prop1.name == "TextureParameterValues")
                                {
                                    DebugWriteLine("Material " + this.expnames[expNameIndex]);
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int num4 = br.ReadInt32();
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < num4; ++impIndex)
                                    {
                                        List<Prop> plist2 = new List<Prop>();
                                        Utilities.readprops(plist2, fs, br,names);
                                        int num5 = 0;
                                        string str2 = "";
                                        foreach (Prop prop2 in plist2)
                                        {
                                            if (prop2.name == "ParameterName")
                                            {
                                                if (!this.materialDB.countListB.ContainsKey(prop2.svalue))
                                                {
                                                    num5 = this.materialDB.countListB.Count;
                                                    this.materialDB.countListB.Add(prop2.svalue, this.materialDB.countListB.Count);
                                                    this.materialDB.stringList3.Add(prop2.svalue);
                                                }
                                                else
                                                    num5 = this.materialDB.countListB[prop2.svalue];
                                            }
                                            if (prop2.name == "ParameterValue")
                                                str2 = prop2.ivalue >= 0 ? this.expnames[prop2.ivalue] : this.names[-prop2.ivalue];
                                        }
                                        if (str2 != null)
                                            texparamList.Add(new TexParam()
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
                                        List<Prop> plist2 = new List<Prop>();
                                        Utilities.readprops(plist2, fs, br,names);
                                        Utilities.Printprops(plist2, fs, br, SW_MAT_TXT);
                                        SW_MAT_TXT.WriteLine();
                                    }
                                }
                            }
                            if (!this.materialDB.countListA.ContainsKey(this.expnames[expNameIndex]))
                            {
                                this.materialDB.countListA.Add(this.expnames[expNameIndex], this.materialDB.countListA.Count);
                                this.materialDB.texparamListList.Add(texparamList);
                                dirty = true;
                            }
                            SW_MAT_TXT.Close();
                        }
                        else if (namesList[expNameIndex].ToLower() == "LandscapeComponent".ToLower())
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist = new List<Prop>();
                            Utilities.readprops(plist, fs, br,names);
                            ULand land = new ULand();
                            foreach (Prop prop in plist)
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
                            uAsset.landList.Add(land);
                        }
                        else if (namesList[expNameIndex].ToLower() == "staticmeshcomponent" || namesList[expNameIndex].ToLower() == "scenecomponent")
                        {
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist = new List<Prop>();
                            Utilities.readprops(plist, fs, br,names);
                            var actor = new UStaticMeshActor();
                            foreach (Prop prop in plist)
                            {
                                if (prop.type == "ObjectProperty" && prop.name == "StaticMesh")
                                    actor.staticMeshFile = prop.ivalue >= 0 ? this.expnames[prop.ivalue] : this.impnames[-prop.ivalue];
                                else if (prop.type == "ObjectProperty" && prop.name == "AttachParent")
                                    actor.attachParent = prop.ivalue;
                                else if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.relativeLocation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.relativeRotation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                }
                                else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                {
                                    fs.Seek(prop.fpos, SeekOrigin.Begin);
                                    actor.relativeScale3D = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
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
                            dictionary4.Add(expNameIndex, uAsset.staticMeshActorList.Count);
                            uAsset.staticMeshActorList.Add(actor);
                        }
                        //for export only
                        else if (namesList[expNameIndex].ToLower() == "staticmesh")
                        {
                            DebugWriteLine("Exporting staticmesh: " + this.expnames[expNameIndex]);
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist1 = new List<Prop>();
                            Utilities.readprops(plist1, fs, br,names);
                            string[] matNames = (string[])null;
                            foreach (Prop prop1 in plist1)
                            {
                                if (prop1.type == "ArrayProperty" && prop1.name == "Materials")
                                {
                                    long position = fs.Position;
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int length2 = br.ReadInt32();
                                    matNames = new string[length2];
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                        matNames[impIndex] = this.impnames[-br.ReadInt32()];
                                    fs.Seek(position, SeekOrigin.Begin);
                                }
                                else if (prop1.type == "ArrayProperty" && prop1.name == "StaticMaterials")
                                {
                                    long position = fs.Position;
                                    fs.Seek(prop1.fpos + 9L, SeekOrigin.Begin);
                                    int length2 = br.ReadInt32();
                                    matNames = new string[length2];
                                    fs.Seek(49L, SeekOrigin.Current);
                                    for (impIndex = 0; impIndex < length2; ++impIndex)
                                    {
                                        List<Prop> plist2 = new List<Prop>();
                                        Utilities.readprops(plist2, fs, br,names);
                                        foreach (Prop prop2 in plist2)
                                        {
                                            if (prop2.name == "MaterialInterface")
                                                matNames[impIndex] = prop2.ivalue >= 0 ? this.expnames[prop2.ivalue] : this.impnames[-prop2.ivalue];
                                        }
                                    }
                                    fs.Seek(position, SeekOrigin.Begin);
                                }
                            }
                            //TODO: In this point we need to save the material x meshname in some new material database
                            string expname = this.expnames[expNameIndex];
                            materialDB.meshXMaterials[expname] = matNames;
                            //if (!Directory.Exists("staticmesh.raw"))
                              //  Directory.CreateDirectory("staticmesh.raw");
                            int count3 = (int)((long)(numArray1[expNameIndex] + numArray2[expNameIndex]) - fs.Position);
                            byte[] numArray3 = new byte[count3];
                            fs.Read(numArray3, 0, count3);
                            File.WriteAllBytes($"{config.StaticMeshRawFolder}\\" + expname + ".raw", numArray3);
                            fs.Seek((long)-count3, SeekOrigin.Current);
                            StreamWriter SW_STM_RAW_TXT = new StreamWriter($"{config.StaticMeshRawFolder}\\" + expname + ".txt");
                            //TODO: Static mesh material write point
                            foreach (string matName in matNames)
                                SW_STM_RAW_TXT.WriteLine(matName);
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
                                    SW_STM_ASCII.WriteLine(matNames[numArray4[index4]]);
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
                        else if (this.blueprintDB.dictionary1.ContainsKey(namesList[expNameIndex]))
                        {
                            int index3 = 0;
                            int index4 = 0;
                            fs.Seek((long)numArray1[expNameIndex], SeekOrigin.Begin);
                            List<Prop> plist1 = new List<Prop>();
                            Utilities.readprops(plist1, fs, br,names);
                            foreach (Prop prop in plist1)
                            {
                                if (prop.name == "SkeletalMeshComponent")
                                    index3 = prop.ivalue;
                                else if (prop.name == "StaticMeshComponent")
                                    index4 = prop.ivalue;
                            }
                            if (index4 != 0)
                            {
                                fs.Seek((long)numArray1[index4], SeekOrigin.Begin);
                                List<Prop> plist2 = new List<Prop>();
                                Utilities.readprops(plist2, fs, br,names);
                                UActor actor = new UActor();
                                if (!dictionary4.ContainsKey(expNameIndex))
                                    dictionary4.Add(expNameIndex, uAsset.staticMeshActorList.Count);
                                foreach (Prop prop in plist2)
                                {
                                    if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeLocation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeRotation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeScale3D = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                }
                                foreach (string str2 in this.blueprintDB.countListA[this.blueprintDB.dictionary1[namesList[expNameIndex]]])
                                    uAsset.staticMeshActorList.Add(new UStaticMeshActor(str2, str2)
                                    {
                                        relativeLocation = actor.relativeLocation,
                                        relativeRotation = actor.relativeRotation,
                                        relativeScale3D = actor.relativeScale3D,
                                    });
                            }
                            if (index3 != 0)
                            {
                                fs.Seek((long)numArray1[index3], SeekOrigin.Begin);
                                List<Prop> plist2 = new List<Prop>();
                                Utilities.readprops(plist2, fs, br,names);
                                UActor actor = new UActor();
                                if (!dictionary5.ContainsKey(expNameIndex))
                                    dictionary5.Add(expNameIndex, uAsset.staticSkeletalMeshActorList.Count);
                                foreach (Prop prop in plist2)
                                {
                                    if (prop.type == "StructProperty" && prop.name == "RelativeLocation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeLocation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeRotation")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeRotation = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                    else if (prop.type == "StructProperty" && prop.name == "RelativeScale3D")
                                    {
                                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                                        actor.relativeScale3D = new Vector3D(br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                                    }
                                }
                                foreach (string str2 in this.blueprintDB.countListB[this.blueprintDB.dictionary1[namesList[expNameIndex]]])
                                    uAsset.staticSkeletalMeshActorList.Add(new UStaticMeshActor(str2, str2)
                                    {
                                        relativeLocation = actor.relativeLocation,
                                        relativeRotation = actor.relativeRotation,
                                        relativeScale3D = actor.relativeScale3D
                                    });
                            }
                        }
                    }
                    namesStreamWriter?.Close();
                    exportStreamWriter?.Close();
                    importStreamWriter?.Close();
                    #endregion


                    ExportAll(withoutExtension,impIndex,numArray1,dictionary4,dictionary5,uAsset,fs,br,uAsset.uMaterials);
                    uAsset.uMaterials.Keys.ToList().ForEach(textureBaseName => {
                        var m = uAsset.uMaterials[textureBaseName];
                        m.texturesList = texDirFiles.Where(texPath => {
                            var fName = Path.GetFileName(texPath);
                            var A = fName.Left(m.textureBaseName.Length + 1);
                            var B = $"{m.textureBaseName}_";
                            return A == B;
                        }).Select(Path.GetFileNameWithoutExtension).ToList();
                    });

                    for(var i=0;i<uAsset.staticMeshActorList.Count(); i++) 
                        uAsset.staticMeshActorList[i].id = $"{uAsset.staticMeshActorList[i].name}_{i}";

                    uAssetsList.Add(uAsset);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error exporting asset " + uassetPath + "\r\n" + ex.Message + "\r\n" + ex.StackTrace);
                }                
            }
            //FOREACH END

            if (dirty)
                UpdateDBs(saveBPDB);


            return uAssetsList;
        }

        public void UpdateDBs(bool saveBPDB)
        {
            FileStream fileStream = new FileStream(config.MaterialDBPath, FileMode.Create);
            BinaryWriter binaryWriter = new BinaryWriter((Stream)fileStream);
            binaryWriter.Write(this.materialDB.countListA.Count);
            foreach (string key in this.materialDB.countListA.Keys)
            {
                binaryWriter.Write(key.Length);
                binaryWriter.Write(key.ToCharArray());
            }
            binaryWriter.Write(this.materialDB.countListB.Count);
            foreach (string key in this.materialDB.countListB.Keys)
            {
                binaryWriter.Write(key.Length);
                binaryWriter.Write(key.ToCharArray());
            }
            for (int index = 0; index < this.materialDB.countListA.Count; ++index)
            {
                binaryWriter.Write(this.materialDB.texparamListList[index].Count);
                foreach (TexParam texparam in this.materialDB.texparamListList[index])
                {
                    binaryWriter.Write((byte)texparam.type);
                    binaryWriter.Write(texparam.value.Length);
                    binaryWriter.Write(texparam.value.ToCharArray());
                }
            }
            fileStream.Close();
            if (saveBPDB)
                blueprintDB.Save();
        }

        public void ExportAll(
            string withoutExtension,
            int impIndex,
            int[] numArray1,
            Dictionary<int, int> dictionary4,
            Dictionary<int, int> dictionary5,
            UAsset uAsset,
            MemoryStream fs,
            BinaryReader br,
            Dictionary<string,UMaterial> uMaterialsDic)
        {
            var intSet1 = new HashSet<int>();
            #region LandMap
            if (uAsset.landList.Count > 0)
            {
                int num4 = 0;
                foreach (ULand land in uAsset.landList)
                {
                    if (!intSet1.Contains(land.hmtexture))
                    {
                        intSet1.Add(land.hmtexture);
                        ++num4;
                    }
                }
                HashSet<int> intSet2 = new HashSet<int>();
                StreamWriter SW_LNDS_ASCII = new StreamWriter($"{config.ExportFolder}/{withoutExtension}_landscape.ascii");
                SW_LNDS_ASCII.WriteLine("0");
                SW_LNDS_ASCII.WriteLine(num4);
                foreach (ULand land in uAsset.landList)
                {
                    if (!intSet2.Contains(land.hmtexture))
                    {
                        intSet2.Add(land.hmtexture);
                        fs.Seek((long)numArray1[land.hmtexture], SeekOrigin.Begin);
                        Utilities.readprops(new List<Prop>(), fs, br, names);
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
                        Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(uAsset.staticMeshActorList[index2].relativeRotation.X, uAsset.staticMeshActorList[index2].relativeRotation.Y, uAsset.staticMeshActorList[index2].relativeRotation.Z);
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
                                Quaternion3D quaternion3D2 = new Quaternion3D(new Vector3D((float)(land.hmx + index4 * num12), (float)(land.hmy + index3 * num12), zz) * uAsset.staticMeshActorList[index2].relativeScale3D, 0.0f);
                                Vector3D vector3D = (quaternion * quaternion3D2 * quaternion3D1).xyz + uAsset.staticMeshActorList[index2].relativeLocation;
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
            if (uAsset.staticMeshActorList.Count == 0)
            {
                if (uAsset.staticSkeletalMeshActorList.Count == 0)
                    return;
            }

            //This for get all sub meshes    
            for(var n = 0; n <uAsset.staticMeshActorList.Count; n++)
                FillActorSubMeshes(uAsset.staticMeshActorList, n, dictionary4,ref uMaterialsDic);
            

            return;

            int num32 = 0;
            int num33 = 0;
            int num34 = 0;

            StreamWriter SW_MAP_ASCII = null;

            if (exportMaps) { 
                //Fill maps statick meshes
                foreach (UStaticMeshActor actor in uAsset.staticMeshActorList)
                {
                    DebugWriteLine($"ActorPos '{actor.staticMeshFile}' -> Vector({actor.relativeLocation.X},{actor.relativeLocation.Y},{actor.relativeLocation.Z}) ");
                    //DebugWrite(".");
                    ++num34;
                    if (actor.staticMeshFile != null && !(actor.staticMeshFile == ""))
                    {
                        if (!File.Exists($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.raw"))
                        {
                            DebugWriteLine("Missing model: " + actor.staticMeshFile);
                        }
                        else
                        {
                            //Create map file if doesnt exists
                            if (exportMaps && SW_MAP_ASCII == null)
                            {
                                SW_MAP_ASCII = new StreamWriter($"{config.ExportFolder}/{withoutExtension}_map{num32}.ascii");
                                SW_MAP_ASCII.WriteLine("0");
                                SW_MAP_ASCII.WriteLine("     ");
                            }

                            FileStream fileStream4 = new FileStream($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.raw", FileMode.Open, FileAccess.Read);
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
                                for (; index3 >= 0; index3 = uAsset.staticMeshActorList[index3].attachParent < 0 || !dictionary4.ContainsKey(uAsset.staticMeshActorList[index3].attachParent) ? -1 : dictionary4[uAsset.staticMeshActorList[index3].attachParent])
                                {
                                    Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(uAsset.staticMeshActorList[index3].relativeRotation.X, uAsset.staticMeshActorList[index3].relativeRotation.Y, uAsset.staticMeshActorList[index3].relativeRotation.Z);
                                    Quaternion3D quaternion3D1 = Quaternion3D.Invert(quaternion);
                                    float[,] tm = uAsset.staticMeshActorList[index3].tm;
                                    for (int index4 = 0; index4 < length5; ++index4)
                                    {
                                        if (uAsset.staticMeshActorList[index3].hastrans)
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
                                        Quaternion3D quaternion3D2 = new Quaternion3D(vector3DArray1[index2][index4] * uAsset.staticMeshActorList[index3].relativeScale3D, 0.0f);
                                        Quaternion3D quaternion3D3 = quaternion * quaternion3D2 * quaternion3D1;
                                        vector3DArray1[index2][index4] = quaternion3D3.xyz + uAsset.staticMeshActorList[index3].relativeLocation;
                                        Quaternion3D quaternion3D4 = new Quaternion3D(vector3DArray2[index2][index4] * uAsset.staticMeshActorList[index3].relativeScale3D, 0.0f);
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
                            string[] subMeshMaterialNames;
                            if (File.Exists($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.txt"))
                            {
                                subMeshMaterialNames = File.ReadAllLines($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.txt");
                                if (actor.overmat != null)
                                {
                                    for (impIndex = 0; impIndex < actor.overmat.Length && impIndex < subMeshMaterialNames.Length; ++impIndex)
                                    {
                                        if (actor.overmat[impIndex] < 0)
                                            subMeshMaterialNames[impIndex] = this.impnames[-actor.overmat[impIndex]];
                                    }
                                }
                                else
                                    DebugWriteLine($"actor overmat is null: {actor.staticMeshFile}");
                            }
                            else
                            {
                                staticMeshRaTxtExist = false;
                                subMeshMaterialNames = new string[length2];
                                for (impIndex = 0; impIndex < length2; ++impIndex)
                                    subMeshMaterialNames[impIndex] = "Submesh_" + (object)(num33 + impIndex) + "_material";
                            }
                            //Until here is only reading the raw file, crazy
                            for (impIndex = 0; impIndex < length2; ++impIndex)
                            {
                                int index2 = numArray4[impIndex];
                                string subMeshName = "Submesh_" + (object)(num33 + impIndex) + "_" + actor.staticMeshFile;
                                SW_MAP_ASCII.WriteLine();
                                SW_MAP_ASCII.WriteLine(num14);
                                //All possible materials
                                if (staticMeshRaTxtExist)
                                {
                                    string materialKeyB = subMeshMaterialNames[numArray3[impIndex]];
                                    //actor.materials.Add(new UMaterial(materialKeyB, materialKeyB.Substring(3)) { id=$"{actor.staticMeshFile}_{num33 + impIndex}"});
                                    if (this.materialDB.countListB.ContainsKey(materialKeyB))
                                    {
                                        int count3 = this.materialDB.texparamListList[this.materialDB.countListB[materialKeyB]].Count;
                                        if (count3 == 0)
                                        {
                                            SW_MAP_ASCII.WriteLine("1");
                                            SW_MAP_ASCII.WriteLine(materialKeyB);
                                            SW_MAP_ASCII.WriteLine("0");
                                        }
                                        else
                                        {
                                            SW_MAP_ASCII.WriteLine(count3);
                                            foreach (TexParam texparam in this.materialDB.texparamListList[this.materialDB.countListB[materialKeyB]])
                                            {
                                                SW_MAP_ASCII.WriteLine(texparam.value + ".dds");
                                                SW_MAP_ASCII.WriteLine("0");
                                            }
                                        }
                                    }
                                    else
                                    {
                                        SW_MAP_ASCII.WriteLine("1");
                                        SW_MAP_ASCII.WriteLine(materialKeyB);
                                        SW_MAP_ASCII.WriteLine("0");
                                        DebugWriteLine("Material materialKeyB not found in database: " + materialKeyB);
                                    }
                                }
                                else
                                {
                                    //actor.materials.Add(new UMaterial(subMeshMaterialNames[impIndex], subMeshMaterialNames[impIndex].Substring(3)) { id= $"{actor.staticMeshFile}_{num33 + impIndex}" });
                                    SW_MAP_ASCII.WriteLine("1");
                                    SW_MAP_ASCII.WriteLine(subMeshMaterialNames[impIndex]);
                                    SW_MAP_ASCII.WriteLine("0");
                                }
                                SW_MAP_ASCII.WriteLine(numArray8[impIndex] - numArray7[impIndex] + 1);
                                
                                //This contains the vertices position
                                for (int index3 = numArray7[impIndex]; index3 <= numArray8[impIndex]; ++index3)
                                {
                                    Vector3D subMeshPosition = vector3DArray1[numArray6[impIndex]][index3];
                                    SW_MAP_ASCII.Write(subMeshPosition.X.ToString("0.######", (IFormatProvider)numberFormatInfo));
                                    StreamWriter streamWriter5 = SW_MAP_ASCII;
                                    float num15 = -subMeshPosition.Y;
                                    string str2 = " " + num15.ToString("0.######", (IFormatProvider)numberFormatInfo);
                                    streamWriter5.Write(str2);
                                    SW_MAP_ASCII.WriteLine(" " + subMeshPosition.Z.ToString("0.######", (IFormatProvider)numberFormatInfo));
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
                                if (actor.relativeScale3D.X < 0.0)
                                    flag6 = !flag6;
                                if (actor.relativeScale3D.Y < 0.0)
                                    flag6 = !flag6;
                                if (actor.relativeScale3D.Z < 0.0)
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
                                FileStream fileStream5 = new FileStream($"{config.ExportFolder}\\{withoutExtension}_map{num32}.ascii", FileMode.Open);
                                fileStream5.Seek(3L, SeekOrigin.Begin);
                                string str2 = num33.ToString();
                                if (exportMaps)
                                {
                                    for (impIndex = 0; impIndex < str2.Length; ++impIndex)
                                        fileStream5.WriteByte((byte)str2[impIndex]);
                                }
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
                    var fileStream4 = new FileStream($"{config.ExportFolder}\\{ withoutExtension}_map{num32}.ascii", FileMode.Open);
                    fileStream4.Seek(3L, SeekOrigin.Begin);

                    foreach (byte num4 in num33.ToString())
                        fileStream4.WriteByte(num4);

                    fileStream4.Close();
                }
            }
            #endregion
            
            #region SkyMap
            StreamWriter SW_SKMMAP_ASCII = null;
            int num35 = 0;
            int num36 = 0;
            foreach (UStaticMeshActor actor in uAsset.staticSkeletalMeshActorList)
            {
                DebugWrite("+");
                ++num36;
                if (actor.staticMeshFile == null || !(actor.staticMeshFile != ""))
                {
                    if (!File.Exists("skeletalmesh.raw\\" + actor.staticMeshFile + ".raw"))
                    {
                        DebugWriteLine("Missing skeletal model: " + actor.staticMeshFile);
                    }
                    else
                    {
                        if (SW_SKMMAP_ASCII == null)
                        {
                            SW_SKMMAP_ASCII = new StreamWriter($"{config.ExportFolder}\\{withoutExtension}_skmap{num32}.ascii");
                            SW_SKMMAP_ASCII.WriteLine("0");
                            SW_SKMMAP_ASCII.WriteLine("     ");
                        }
                        FileStream skMeshRawFS = new FileStream($"{config.SkeletalMeshRawFolder}\\" + actor.staticMeshFile + ".raw", FileMode.Open, FileAccess.Read);
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
                            for (; index3 >= 0; index3 = uAsset.staticSkeletalMeshActorList[index3].attachParent < 0 || !dictionary4.ContainsKey(uAsset.staticSkeletalMeshActorList[index3].attachParent) ? -1 : dictionary4[uAsset.staticSkeletalMeshActorList[index3].attachParent])
                            {
                                Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(uAsset.staticSkeletalMeshActorList[index3].relativeRotation.X, uAsset.staticSkeletalMeshActorList[index3].relativeRotation.Y, uAsset.staticSkeletalMeshActorList[index3].relativeRotation.Z);
                                Quaternion3D quaternion3D1 = Quaternion3D.Invert(quaternion);
                                for (int index4 = 0; index4 < length5; ++index4)
                                {
                                    Quaternion3D quaternion3D2 = new Quaternion3D(vector3DArray1[index2][index4] * uAsset.staticSkeletalMeshActorList[index3].relativeScale3D, 0.0f);
                                    Quaternion3D quaternion3D3 = quaternion * quaternion3D2 * quaternion3D1;
                                    vector3DArray1[index2][index4] = quaternion3D3.xyz + uAsset.staticSkeletalMeshActorList[index3].relativeLocation;
                                    Quaternion3D quaternion3D4 = new Quaternion3D(vector3DArray2[index2][index4] * uAsset.staticSkeletalMeshActorList[index3].relativeScale3D, 0.0f);
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
                        if (File.Exists($"{config.SkeletalMeshRawFolder}\\{actor.staticMeshFile}.txt"))
                        {
                            matNameArray = File.ReadAllLines($"{config.SkeletalMeshRawFolder}\\{actor.staticMeshFile}.txt");
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
                            SW_SKMMAP_ASCII.WriteLine("Submesh_" + (object)(num35 + index2) + "_" + actor.staticMeshFile);
                            SW_SKMMAP_ASCII.WriteLine(num19);
                            if (flag6)
                            {
                                string key2 = matNameArray[numArray3[index2]];
                                if (this.materialDB.countListB.ContainsKey(key2))
                                {
                                    int count3 = this.materialDB.texparamListList[this.materialDB.countListB[key2]].Count;
                                    SW_SKMMAP_ASCII.WriteLine(count3);
                                    foreach (TexParam texparam in this.materialDB.texparamListList[this.materialDB.countListB[key2]])
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
                                    DebugWriteLine("Material not found in database: " + key2);
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
                            FileStream fileStream5 = new FileStream($"{config.ExportFolder}/{withoutExtension}_skmap{num32}.ascii", FileMode.Open);
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
                FileStream fileStream4 = new FileStream($"{config.ExportFolder}/{withoutExtension}_skmap{num32}.ascii", FileMode.Open);
                fileStream4.Seek(3L, SeekOrigin.Begin);
                foreach (byte num4 in num35.ToString())
                    fileStream4.WriteByte(num4);
                fileStream4.Close();
            }
            #endregion

            #region LightMap
            StreamWriter SW_LMAP_ASCII = null;
            int num37 = 0;
            foreach (UActor actor in uAsset.lightActorList)
            {
                DebugWrite(".");
                ++num36;
                if (SW_LMAP_ASCII == null)
                {
                    SW_LMAP_ASCII = new StreamWriter($"{config.ExportFolder}/{withoutExtension}_lmap{num32}.ascii");
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
                    vector3DArray[index2] += actor.relativeLocation;
                if (actor.attachParent >= 0)
                {
                    if (dictionary4.ContainsKey(actor.attachParent))
                    {
                        for (int index2 = 0; index2 < 4; ++index2)
                            vector3DArray[index2] += uAsset.staticMeshActorList[dictionary4[actor.attachParent]].relativeLocation;
                    }
                    if (dictionary5.ContainsKey(actor.attachParent))
                    {
                        for (int index2 = 0; index2 < 4; ++index2)
                            vector3DArray[index2] += uAsset.staticSkeletalMeshActorList[dictionary5[actor.attachParent]].relativeLocation;
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
                    FileStream fileStream4 = new FileStream($"{config.ExportFolder}/{withoutExtension}_lmap{num32}.ascii", FileMode.Open);
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
                FileStream fileStream4 = new FileStream($"{config.ExportFolder}/{withoutExtension}_lmap{num32}.ascii", FileMode.Open);
                fileStream4.Seek(3L, SeekOrigin.Begin);
                foreach (byte num4 in num37.ToString())
                    fileStream4.WriteByte(num4);
                fileStream4.Close();
            }
            #endregion

        }

        void FillActorSubMeshes(List<UStaticMeshActor> staticMeshActorList, int actorIndex, Dictionary<int, int> dictionary4,ref Dictionary<string, UMaterial> uMaterialsDic)
        {

            var actor = staticMeshActorList[actorIndex];
            if (actor.staticMeshFile != null && !(actor.staticMeshFile == ""))
            {
                string actorMeshPath = $"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.raw";
                if (!File.Exists(actorMeshPath))
                {
                    DebugWriteLine("Missing model: " + actor.staticMeshFile);
                }
                else
                {
                    #region A
                    FileStream fileStream4 = new FileStream(actorMeshPath, FileMode.Open, FileAccess.Read);
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
                    for (var impIndex = 0; impIndex < length2; ++impIndex)
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
                        int index3 = actorIndex;
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
                        for (; index3 >= 0; index3 = staticMeshActorList[index3].attachParent < 0 || !dictionary4.ContainsKey(staticMeshActorList[index3].attachParent) ? -1 : dictionary4[staticMeshActorList[index3].attachParent])
                        {
                            Quaternion3D quaternion = C3D.EulerAnglesToQuaternion(staticMeshActorList[index3].relativeRotation.X, staticMeshActorList[index3].relativeRotation.Y, staticMeshActorList[index3].relativeRotation.Z);
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
                                Quaternion3D quaternion3D2 = new Quaternion3D(vector3DArray1[index2][index4] * staticMeshActorList[index3].relativeScale3D, 0.0f);
                                Quaternion3D quaternion3D3 = quaternion * quaternion3D2 * quaternion3D1;
                                vector3DArray1[index2][index4] = quaternion3D3.xyz + staticMeshActorList[index3].relativeLocation;

                                Quaternion3D quaternion3D4 = new Quaternion3D(vector3DArray2[index2][index4] * staticMeshActorList[index3].relativeScale3D, 0.0f);
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
                        for (var impIndex = 0; impIndex < length6; ++impIndex)
                            numArray9[index2][impIndex] = num21 != 1 ? (int)binaryReader.ReadUInt16() : binaryReader.ReadInt32();
                    }
                    bool staticMeshRaTxtExist = true;
                    string[] subMeshMaterialNames;
                    if (File.Exists($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.txt"))
                    {
                        subMeshMaterialNames = File.ReadAllLines($"{config.StaticMeshRawFolder}\\{actor.staticMeshFile}.txt");
                        if (actor.overmat != null)
                        {
                            for (var impIndex = 0; impIndex < actor.overmat.Length && impIndex < subMeshMaterialNames.Length; ++impIndex)
                            {
                                if (actor.overmat[impIndex] < 0)
                                    subMeshMaterialNames[impIndex] = this.impnames[-actor.overmat[impIndex]];
                            }
                        }
                        else
                            DebugWriteLine($"actor overmat is null: {actor.staticMeshFile}");
                    }
                    else
                    {
                        staticMeshRaTxtExist = false;
                        subMeshMaterialNames = new string[length2];
                        for (var impIndex = 0; impIndex < length2; ++impIndex)
                            subMeshMaterialNames[impIndex] = "Submesh_" + (impIndex) + "_material";
                    }
                    for (var impIndex = 0; impIndex < length2; ++impIndex) {
                        var material = new UMaterial() { id = $"{actor.staticMeshFile}_{impIndex}" };
                        if (staticMeshRaTxtExist) {
                            material.name = subMeshMaterialNames[numArray3[impIndex]];
                            material.textureBaseName = $"T_{material.name.Substring(3)}";
                        }
                        else {
                            material.name = subMeshMaterialNames[impIndex];
                            material.textureBaseName = $"T_{material.name.Substring(3)}";
                        }
                        actor.materials.Add(material.name);
                        if (!uMaterialsDic.ContainsKey(material.name)) 
                            uMaterialsDic.Add(material.name, material);
                        
                        #endregion
                    }

                }

            }
        }

    }
}




