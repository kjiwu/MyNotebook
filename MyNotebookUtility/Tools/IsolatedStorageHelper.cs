using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.IsolatedStorage;
using System.IO;

namespace MyNotebookUtility.Tools
{
    public class IsolatedStorageHelper
    {
        public static void CreateDir(string path)
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.DirectoryExists(path))
                    isf.CreateDirectory(path);
            }
        }

        public static Stream GetFileStream(string file)
        {
            Stream stream = null;

            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!isf.FileExists(file))
                {
                    using (IsolatedStorageFileStream fs = new IsolatedStorageFileStream(file, FileMode.Open, isf))
                    {
                        fs.CopyTo(stream, (int)fs.Length);
                    }
                }
            }

            return stream;
        }
    }
}
