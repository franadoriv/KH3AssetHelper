using APPLIB;
using System;
using System.Collections.Generic;
using System.IO;

namespace UEKH3
{
    public static class Utilities
    {
        public static void Printprops(List<prop> plist,MemoryStream fs,BinaryReader br,StreamWriter sw, string[] expnames = null, string[] impnames=null)
        {
            foreach (prop prop in plist)
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

        public static void readprops(List<prop> plist, MemoryStream fs, BinaryReader br, string[] names = null)
        {
            while (true)
            {
                string name1 = names[br.ReadInt64()];
                if (!(name1 == "None"))
                {
                    prop prop1 = new prop();
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
                            prop prop2 = prop1;
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

    }
}
