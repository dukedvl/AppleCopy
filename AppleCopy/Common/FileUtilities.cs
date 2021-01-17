using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AppleCopy
{
    public static class FileUtilities
    {
        public static long GetDirectorySize(string path)
        {
            DirectoryInfo directory = new DirectoryInfo(path);

            var FileList = directory.GetFiles("*.*", SearchOption.AllDirectories);

            long fileTotalSize = FileList.Sum(t => t.Length);


            return fileTotalSize;
        }


        public static int GetFileCount(string path)
        {
            return GetFilePaths(path).Count();
        }

        public static List<string> GetFilePaths(string path)
        {
            return Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).ToList();
        }
    }
}
