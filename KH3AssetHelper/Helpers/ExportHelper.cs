using APPLIB;
using DDSReader;
using KH3AssetHelper.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using UE;
using UE.UE4;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Linq;
using System.Runtime;

namespace KH3AssetHelper.Helpers
{
    public  class ExportHelper
    {
        UAssetParserResult uAssetParserResult;

        public ExportHelper(UAssetParserResult uAssetParserResult)
        {
            this.uAssetParserResult = uAssetParserResult;
        }

       public  void ExportMeshToASCII(UStaticMeshActor subMesh,string dstPath){
            ExportRawStatic($@"{uAssetParserResult.config.StaticMeshRawFolder}\{subMesh.staticMeshFile}.raw", dstPath);
       }

       public  void ExportRawStatic(string srcPath, string dstPath)
        {
            var materialDB = uAssetParserResult.materialDB;
            string materialsPath = $"{srcPath.Substring(0, srcPath.Length - 4)}.txt";
            Console.WriteLine($"ExportRawStatic.srcPath: {srcPath}");
            Console.WriteLine($"ExportRawStatic.dstPath: {dstPath}");
            Console.WriteLine($"ExportRawStatic.materialsPath: {materialsPath}");
            NumberFormatInfo numberFormatInfo = new NumberFormatInfo();
            numberFormatInfo.NumberDecimalSeparator = ".";
            int num33 = 0;
            StreamWriter SW_MAP_ASCII = null;
            if (SW_MAP_ASCII == null)
            {
                SW_MAP_ASCII = new StreamWriter(dstPath);
                SW_MAP_ASCII.WriteLine("0");
                SW_MAP_ASCII.WriteLine("     ");
            }
            FileStream fileStream4 = new FileStream(srcPath, FileMode.Open, FileAccess.Read);
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
            if (File.Exists(materialsPath))
            {
                subMeshMaterialNames = File.ReadAllLines(materialsPath);
            }
            else
            {
                staticMeshRaTxtExist = false;
                subMeshMaterialNames = new string[length2];
                for (var impIndex = 0; impIndex < length2; ++impIndex)
                    subMeshMaterialNames[impIndex] = "Submesh_" + (object)(num33 + impIndex) + "_material";
            }

            for (var impIndex = 0; impIndex < length2; ++impIndex)
            {
                int index2 = numArray4[impIndex];
                SW_MAP_ASCII.WriteLine();
                SW_MAP_ASCII.WriteLine(num14);
                if (staticMeshRaTxtExist)
                {
                    string materialKeyB = subMeshMaterialNames[numArray3[impIndex]];
                    if (materialDB.countListB.ContainsKey(materialKeyB))
                    {
                        int count3 = materialDB.texparamListList[materialDB.countListB[materialKeyB]].Count;
                        if (count3 == 0)
                        {
                            SW_MAP_ASCII.WriteLine("1");
                            SW_MAP_ASCII.WriteLine(materialKeyB);
                            SW_MAP_ASCII.WriteLine("0");
                        }
                        else
                        {
                            SW_MAP_ASCII.WriteLine(count3);
                            foreach (TexParam texparam in materialDB.texparamListList[materialDB.countListB[materialKeyB]])
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
                    }
                }
                else
                {
                    SW_MAP_ASCII.WriteLine("1");
                    SW_MAP_ASCII.WriteLine(subMeshMaterialNames[impIndex]);
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
                FileStream fileStream5 = new FileStream(dstPath, FileMode.Open);
                fileStream5.Seek(3L, SeekOrigin.Begin);
                string str2 = num33.ToString();
                for (var impIndex = 0; impIndex < str2.Length; ++impIndex)
                    fileStream5.WriteByte((byte)str2[impIndex]);
                fileStream5.Close();
                num33 = 0;
            }
            if (SW_MAP_ASCII != null)
            {
                SW_MAP_ASCII.Close();
                var fileStream5 = new FileStream(dstPath, FileMode.Open);
                fileStream5.Seek(3L, SeekOrigin.Begin);
                foreach (byte num20 in num33.ToString())
                    fileStream5.WriteByte(num20);
                fileStream5.Close();
            }

        }

       public  void ASCII2FBX(UStaticMeshActor subMesh,string dstPath)
        {
            if (!File.Exists(dstPath)) {
                string tempPath = uAssetParserResult.config.ExportFolder;
                string tempASCIIPath = $"{tempPath}\\temp.ascii";
                if (File.Exists(tempASCIIPath))
                    File.Delete(tempASCIIPath);

                ExportMeshToASCII(subMesh, tempASCIIPath);

                string scriptPath = $"{tempPath}/BlenderScript_ASCII2FBX.py";
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
                File.WriteAllText(scriptPath, Resources.BlenderScript_ASCII2FBX);
                string blenderPath = uAssetParserResult.config.BlenderPath;
                string strCmdText = $"'/C \"{blenderPath}\" --python {scriptPath} --background -- \"{tempASCIIPath}\" \"{dstPath}\" '";
                Console.WriteLine($"CMD.exe {strCmdText}");
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = blenderPath;
                startInfo.Arguments = $"--python \"{scriptPath}\" --background -- \"{tempASCIIPath}\" \"{dstPath}\"";
                var process = Process.Start(startInfo);
                Console.WriteLine($"Waiting export... ");
                process.WaitForExit();
                Console.Write($"OK");
            }
       }

        public static void BlenderPSK2FBX(string srcPath, string dstPath,string blenderPath) {
            if (!File.Exists(dstPath)) {
                string tempPath = Path.GetTempPath();
                string scriptPath = $"{tempPath}/BlenderScript_PSK2FBX.py";
                string pluginPath = $"{tempPath}/pskpsab280.py";
                if (File.Exists(scriptPath))
                    File.Delete(scriptPath);
                if (File.Exists(pluginPath))
                    File.Delete(pluginPath);
                File.WriteAllText(scriptPath, Resources.BlenderScript_PSK2FBX);
                File.WriteAllText(pluginPath, Encoding.Default.GetString(Resources.pskpsab280));
                string strCmdText = $"'/C \"{blenderPath}\" --python {scriptPath} --background -- \"{srcPath}\" \"{dstPath}\" \"{pluginPath}\" '";
                Console.WriteLine($"CMD.exe {strCmdText}");
                var startInfo = new ProcessStartInfo();
                startInfo.FileName = blenderPath;
                startInfo.Arguments = $"--python \"{scriptPath}\" --background -- \"{srcPath}\" \"{dstPath}\" \"{pluginPath}\"";
                var process = Process.Start(startInfo);
                Console.WriteLine($"Waiting export... ");
                process.WaitForExit();
                Console.Write($"OK");
            }
        }

       public static void NoesisConvert(string srcPath,string dstPath,string noesisPath) {
            Process.Start(new ProcessStartInfo() {
                Verb = "runas",
                FileName = noesisPath,
                Arguments = $"?cmode \"{srcPath}\" \"{dstPath}\" -animoutex \".fbx\""
            }).WaitForExit();
        }

        public static void DDS2TGA(string srcPath, string dstPath) {
            if (!File.Exists(dstPath)) {
                try {
                    using (var fs = new FileStream(dstPath, FileMode.Create)) {
                        var img = new DDSImage(srcPath);
                        img.SaveAsPng(fs);
                    }
                }
                catch (Exception ex) {
                    Console.WriteLine($"Error exporting texture {srcPath}: {ex.Message}");
                }
            }
       }

       public static void uAsset2Json(UAsset uAsset,string dstPath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string strJsn = JsonSerializer.Serialize(uAsset,options);
            File.WriteAllText(dstPath, strJsn);
        }

        public static void uModelConvert (string root, string partialRoute,string dstPath,string uModelPath,string noesisPath,string blenderPath) {
            var aes = "1u3t4WOI0bN6Gvz76w9GR0nWTc23lWQ8eE2MFJw3fSbupa5SD735c8bsOa1nY2V";

            var gameDir = $"{dstPath}/Game";
            var anmDir = $@"{dstPath}/anm";
            var skMeshDir = $@"{dstPath}/skMdl";
            var stMeshDir = $@"{dstPath}/stMdl";
            var texDir = $@"{dstPath}/tex";
            DirectoryHelper.CreateIfNotExist(gameDir, false);
            DirectoryHelper.CreateIfNotExist(anmDir, false);
            DirectoryHelper.CreateIfNotExist(skMeshDir, false);
            DirectoryHelper.CreateIfNotExist(stMeshDir, false);
            DirectoryHelper.CreateIfNotExist(texDir, false);
     
            Action<string> Export = (string partialRoute2) => {
                Process.Start(new ProcessStartInfo() {
                    Verb = "runas",
                    FileName = uModelPath,
                    Arguments = $"-path=\"{root}\"  -notgacomp -game=kh3 -ps4 -nooverwrite -aes={aes} -export \"{partialRoute2}\" -out=\"{gameDir}\""
                }).WaitForExit();
            };
            Export(partialRoute);
            
            foreach (string filePath in Directory.GetFiles(gameDir, "*.pskx", SearchOption.AllDirectories)) {
                var fileDst = $@"{stMeshDir}/{Path.GetFileNameWithoutExtension(filePath)}.fbx";
                BlenderPSK2FBX(filePath, fileDst,blenderPath);
            }

            /*foreach (string filePath in Directory.GetFiles(gameDir, "*.psk", SearchOption.AllDirectories)) {
                var fileDst = $@"{skMeshDir}/{Path.GetFileNameWithoutExtension(filePath)}.fbx";
                if (!File.Exists(fileDst)) {
                    NoesisConvert(filePath, fileDst, noesisPath);
                }
            }*/

            foreach (string filePath in Directory.GetFiles(gameDir, "*.psa", SearchOption.AllDirectories)) {
                var fileDst = $@"{anmDir}/{Path.GetFileNameWithoutExtension(filePath)}.fbx";
                if (!File.Exists(fileDst)) {
                    NoesisConvert(filePath, fileDst, noesisPath);
                }
            }

            foreach (string filePath in Directory.GetFiles(gameDir, "*.tga", SearchOption.AllDirectories)) {
                var fileDst = $@"{texDir}/{Path.GetFileNameWithoutExtension(filePath)}.tga";
                if (!File.Exists(fileDst)) 
                    File.Move(filePath, fileDst);
            }
        }

        public void uMapAssetExport(string root, UAsset uMapAsset, string dstPath) {
            var metaPath = $"{dstPath}/{uMapAsset.name}.json";
            if (!File.Exists(metaPath))
                uAsset2Json(uMapAsset, metaPath);
            var mapName = string.Join("_", uMapAsset.name.Split('_').Take(1));
            var blenderPath = @"C:\Program Files\Blender Foundation\Blender 2.90\blender.exe";
            Console.WriteLine($"MapName: {mapName}");
            uModelConvert(root, $"*/*SM_{mapName}_*.uasset", dstPath, uAssetParserResult.config.UModelPath, uAssetParserResult.config.NoesisPath, blenderPath);
            /*uMapAsset.staticMeshActorList.ForEach(actor => {
                uModelConvert(root, $"/{actor.staticMesh}.uasset",dstPath);
            });*/
        }
    }
}
