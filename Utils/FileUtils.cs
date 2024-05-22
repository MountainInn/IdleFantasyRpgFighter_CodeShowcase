using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Utils
{
    public static class FileUtils 
    {
        public static void WriteAllText(string path, string text)
        {
            CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, text);
        }
        public static bool CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
               Directory.CreateDirectory(path);
                return true;
            }

            return false;
        }
    }

}
