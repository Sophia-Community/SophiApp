using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SophiApp.Helpers
{
    internal class FileManager
    {
        internal static bool Save(List<string> list, string path)
        {
            try
            {
                File.WriteAllLines(path, list, Encoding.UTF8);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}