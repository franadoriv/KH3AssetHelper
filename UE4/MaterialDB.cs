using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UE.UE4
{
    public class MaterialDB
    {
        public Dictionary<string, int> countListA = new Dictionary<string, int>();
        public Dictionary<string, int> countListB = new Dictionary<string, int>();
        public List<string> stringList3 = new List<string>();
        public List<List<TexParam>> texparamListList = new List<List<TexParam>>();
        public Dictionary<string, string[]> meshXMaterials = new Dictionary<string, string[]>();

        public void Load(string maerialDbFilePath)
        {
            if (File.Exists(maerialDbFilePath))
            {
                var fileStream = new FileStream(maerialDbFilePath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fileStream);
                int num1 = br.ReadInt32();
                for (int index = 0; index < num1; ++index)
                    countListA.Add(Utilities.readnamef(br), countListA.Count);
                int num2 = br.ReadInt32();
                for (int index = 0; index < num2; ++index)
                {
                    string key = Utilities.readnamef(br);
                    countListB.Add(key, countListB.Count);
                    stringList3.Add(key);
                }
                for (int index1 = 0; index1 < countListA.Count; ++index1)
                {
                    int num3 = br.ReadInt32();
                    List<TexParam> texparamList = new List<TexParam>();
                    for (int index2 = 0; index2 < num3; ++index2)
                        texparamList.Add(new TexParam()
                        {
                            type = (int)br.ReadByte(),
                            value = Utilities.readnamef(br)
                        });
                    texparamListList.Add(texparamList);
                }
                fileStream.Close();
            }

        }

        public MaterialDB(string maerialDbFilePath = null)
        {
            if (!string.IsNullOrEmpty(maerialDbFilePath))
            {
                Load(maerialDbFilePath);
            }
        }
    }
}
