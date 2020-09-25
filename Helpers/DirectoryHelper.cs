using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace KH3AssetHelper.Helpers {
    public static class DirectoryHelper {
        public static void CreateIfNotExist(string path,bool eraseContent = false) {
            Console.WriteLine($"Creating directory: {path} ");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else if (Directory.Exists(path) && eraseContent) {
                Directory.Delete(path, true);
                Directory.CreateDirectory(path);
            }
        }
    }
}
