using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UEKH3.UE4
{
    public class MaterialDB
    {
        public Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
        public Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
        public List<string> stringList3 = new List<string>();
        public List<List<texparam>> texparamListList = new List<List<texparam>>();

        public void Load(string maerialDbFilePath)
        {
            if (File.Exists(maerialDbFilePath))
            {
                var fileStream = new FileStream(maerialDbFilePath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fileStream);
                int num1 = br.ReadInt32();
                for (int index = 0; index < num1; ++index)
                    dictionary2.Add(Utilities.readnamef(br), dictionary2.Count);
                int num2 = br.ReadInt32();
                for (int index = 0; index < num2; ++index)
                {
                    string key = Utilities.readnamef(br);
                    dictionary3.Add(key, dictionary3.Count);
                    stringList3.Add(key);
                }
                for (int index1 = 0; index1 < dictionary2.Count; ++index1)
                {
                    int num3 = br.ReadInt32();
                    List<texparam> texparamList = new List<texparam>();
                    for (int index2 = 0; index2 < num3; ++index2)
                        texparamList.Add(new texparam()
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
