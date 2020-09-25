using System.Collections.Generic;
using System.IO;


namespace UE.UE4
{
    public class BlueprintDB
    {
        public Dictionary<string, int> dictionary1 = new Dictionary<string, int>();
        public List<List<string>> countListA = new List<List<string>>();
        public List<List<string>> countListB = new List<List<string>>();
        private string bpPath;

        public void Load(string bpPath)
        {
            if (File.Exists(bpPath))
            {
                this.bpPath = bpPath;
                var fileStream = new FileStream(bpPath, FileMode.Open, FileAccess.Read);
                var br = new BinaryReader(fileStream);
                int num1 = br.ReadInt32();
                for (int index = 0; index < num1; ++index)
                    dictionary1.Add(Utilities.readnamef(br), dictionary1.Count);
                for (int index1 = 0; index1 < dictionary1.Count; ++index1)
                {
                    int num2 = br.ReadInt32();
                    List<string> stringList1 = new List<string>();
                    for (int index2 = 0; index2 < num2; ++index2)
                        stringList1.Add(Utilities.readnamef(br));
                    countListA.Add(stringList1);
                    int num3 = br.ReadInt32();
                    List<string> stringList2 = new List<string>();
                    for (int index2 = 0; index2 < num3; ++index2)
                        stringList2.Add(Utilities.readnamef(br));
                    countListB.Add(stringList2);
                }
                fileStream.Close();
            }
        }

        public void Save()
        {

            FileStream fileStream6 = new FileStream(bpPath, FileMode.Create);
            BinaryWriter binaryWriter1 = new BinaryWriter((Stream)fileStream6);
            binaryWriter1.Write(dictionary1.Count);
            foreach (string key in dictionary1.Keys)
            {
                binaryWriter1.Write(key.Length);
                binaryWriter1.Write(key.ToCharArray());
            }
            for (int index = 0; index < dictionary1.Count; ++index)
            {
                binaryWriter1.Write(countListA[index].Count);
                foreach (string str in countListA[index])
                {
                    binaryWriter1.Write(str.Length);
                    binaryWriter1.Write(str.ToCharArray());
                }
                binaryWriter1.Write(countListB[index].Count);
                foreach (string str in countListB[index])
                {
                    binaryWriter1.Write(str.Length);
                    binaryWriter1.Write(str.ToCharArray());
                }
            }
            fileStream6.Close();
        }

        public BlueprintDB(string bluePrintFilePath = null)
        {
            if (!string.IsNullOrEmpty(bluePrintFilePath))
            {
                Load(bluePrintFilePath);
            }
        }
    }
}
