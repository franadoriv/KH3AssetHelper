using APPLIB;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using UE.UE4;

namespace UE
{
    public static class Utilities
    {
        public static void Printprops(List<Prop> plist,MemoryStream fs,BinaryReader br,StreamWriter sw, string[] expnames = null, string[] impnames=null)
        {
            foreach (Prop prop in plist)
            {
                sw.Write(prop.name + " = ");
                if (prop.type == "IntProperty")
                    sw.Write(prop.ivalue);
                else if (prop.type == "StructProperty")
                {
                    sw.Write(prop.svalue);
                    if (prop.svalue == "Color")
                    {
                        fs.Seek(prop.fpos, SeekOrigin.Begin);
                        sw.Write(" [" + (object)br.ReadByte() + " " + (object)br.ReadByte() + " " + (object)br.ReadByte() + " " + (object)br.ReadByte() + "]");
                    }
                }
                else if (prop.type == "ObjectProperty")
                {
                    sw.Write("[" + (object)prop.ivalue);
                    string str = prop.ivalue >= 0 ? expnames[prop.ivalue] : impnames[-prop.ivalue];
                    sw.Write("] " + str);
                }
                else if (prop.type == "ArrayProperty")
                    sw.Write(prop.svalue + " (" + (object)prop.size + ")");
                else if (prop.type == "FloatProperty")
                    sw.Write(prop.fvalue);
                else if (prop.type == "EnumProperty")
                    sw.Write(prop.svalue);
                else if (prop.type == "BoolProperty")
                    sw.Write(prop.ivalue);
                else if (prop.type == "ByteProperty")
                    sw.Write(prop.svalue);
                else if (prop.type == "NameProperty")
                    sw.Write(prop.svalue);
                sw.WriteLine();
            }
        }

        public static void readprops(List<Prop> plist, MemoryStream fs, BinaryReader br, string[] names = null)
        {
            while (true)
            {
                string name1 = names[br.ReadInt64()];
                if (!(name1 == "None"))
                {
                    Prop prop1 = new Prop();
                    prop1.name = name1;
                    string name2 = names[br.ReadInt64()];
                    prop1.type = name2;
                    prop1.size = br.ReadInt32();
                    prop1.id = br.ReadInt32();
                    prop1.fpos = fs.Position;
                    if (name2 == "IntProperty")
                    {
                        int num = (int)br.ReadByte();
                        prop1.ivalue = br.ReadInt32();
                    }
                    else if (name2 == "StructProperty")
                    {
                        prop1.svalue = names[br.ReadInt64()];
                        int num = (int)br.ReadByte();
                        br.ReadInt64();
                        br.ReadInt64();
                        prop1.fpos = fs.Position;
                        fs.Seek((long)prop1.size, SeekOrigin.Current);
                    }
                    else if (name2 == "ObjectProperty")
                    {
                        int num = (int)br.ReadByte();
                        prop1.ivalue = br.ReadInt32();
                    }
                    else if (name2 == "ArrayProperty")
                    {
                        prop1.svalue = names[br.ReadInt64()];
                        int num = (int)br.ReadByte();
                        fs.Seek((long)prop1.size, SeekOrigin.Current);
                    }
                    else if (name2 == "MapProperty")
                        fs.Seek((long)prop1.size, SeekOrigin.Current);
                    else if (name2 == "FloatProperty")
                    {
                        int num = (int)br.ReadByte();
                        prop1.fvalue = br.ReadSingle();
                    }
                    else if (name2 == "QWordProperty")
                    {
                        int num1 = (int)br.ReadByte();
                        long num2 = (long)br.ReadUInt64();
                    }
                    else if (name2 == "EnumProperty")
                    {
                        int num = (int)br.ReadByte();
                        prop1.svalue = names[br.ReadInt64()];
                        string name3 = names[br.ReadInt64()];
                    }
                    else if (name2 == "BoolProperty")
                    {
                        prop1.ivalue = (int)br.ReadByte();
                        int num = (int)br.ReadByte();
                    }
                    else if (name2 == "StrProperty")
                    {
                        int num1 = (int)br.ReadByte();
                        int num2 = br.ReadInt32();
                        prop1.svalue = "";
                        if (num2 > 0)
                        {
                            for (int index = 0; index < num2 - 1; ++index)
                                prop1.svalue += (string)(object)(char)br.ReadByte();
                            int num3 = (int)br.ReadByte();
                        }
                    }
                    else if (name2 == "ByteProperty")
                    {
                        prop1.svalue = names[br.ReadInt64()];
                        int num = (int)br.ReadByte();
                        fs.Seek((long)prop1.size, SeekOrigin.Current);
                    }
                    else if (name2 == "NameProperty")
                    {
                        int num1 = (int)br.ReadByte();
                        prop1.svalue = names[br.ReadInt32()];
                        int num2 = br.ReadInt32();
                        if (num2 > 0)
                        {
                            Prop prop2 = prop1;
                            prop2.svalue = prop2.svalue + "_" + (num2 - 1).ToString();
                        }
                    }
                    else
                    {
                        int num = (int)br.ReadByte();
                        fs.Seek((long)prop1.size, SeekOrigin.Current);
                    }
                    plist.Add(prop1);
                }
                else
                    break;
            }
        }

        public static string readnamef(BinaryReader br)
        {
            string str = "";
            int num = br.ReadInt32();
            for (int index = 0; index < num; ++index)
                str += br.ReadByte();
            return str;
        }

        public static Quaternion3D matrix2quat(float[,] m)
        {
            int[] numArray1 = new int[3] { 1, 2, 0 };
            Quaternion3D quaternion3D = new Quaternion3D();
            double[] numArray2 = new double[4];
            double num1 = (double)m[0, 0] + (double)m[1, 1] + (double)m[2, 2];
            if (num1 > 0.0)
            {
                double num2 = Math.Pow(num1 + 1.0, 0.5);
                quaternion3D.real = (float)(num2 / 2.0);
                double num3 = 0.5 / num2;
                quaternion3D.i = (float)(((double)m[1, 2] - (double)m[2, 1]) * num3);
                quaternion3D.j = (float)(((double)m[2, 0] - (double)m[0, 2]) * num3);
                quaternion3D.k = (float)(((double)m[0, 1] - (double)m[1, 0]) * num3);
            }
            else
            {
                int index1 = 0;
                if ((double)m[1, 1] > (double)m[0, 0])
                    index1 = 1;
                if ((double)m[2, 2] > (double)m[index1, index1])
                    index1 = 2;
                int index2 = numArray1[index1];
                int index3 = numArray1[index2];
                double num2 = Math.Pow((double)m[index1, index1] - ((double)m[index2, index2] + (double)m[index3, index3]) + 1.0, 0.5);
                numArray2[index1] = num2 * 0.5;
                if (num2 != 0.0)
                    num2 = 0.5 / num2;
                numArray2[3] = ((double)m[index2, index3] - (double)m[index3, index2]) * num2;
                numArray2[index2] = ((double)m[index1, index2] + (double)m[index2, index1]) * num2;
                numArray2[index3] = ((double)m[index1, index3] + (double)m[index3, index1]) * num2;
                quaternion3D.i = (float)numArray2[0];
                quaternion3D.j = (float)numArray2[1];
                quaternion3D.k = (float)numArray2[2];
                quaternion3D.real = (float)numArray2[3];
            }
            return quaternion3D;
        }

        public static int morton(int t, int sx, int sy)
        {
            int num1;
            int num2 = num1 = 1;
            int num3 = t;
            int num4 = sx;
            int num5 = sy;
            int num6 = 0;
            int num7 = 0;
            while (num4 > 1 || num5 > 1)
            {
                if (num4 > 1)
                {
                    num6 += num2 * (num3 & 1);
                    num3 >>= 1;
                    num2 *= 2;
                    num4 >>= 1;
                }
                if (num5 > 1)
                {
                    num7 += num1 * (num3 & 1);
                    num3 >>= 1;
                    num1 *= 2;
                    num5 >>= 1;
                }
            }
            return num7 * sx + num6;
        }

        public static void ExportRawStatic(string srcPath,string dstPath, MaterialDB materialDB)
        {
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
    
        public static void ASCII2FBX(string blenderPath,string scriptPath,string srcPath,string dstPath)
        {
            string strCmdText;
            strCmdText = $"'/C {blenderPath} --python {scriptPath} --background -- {scriptPath} {dstPath}'";
            Process.Start("CMD.exe", strCmdText);
        }

    }
}
