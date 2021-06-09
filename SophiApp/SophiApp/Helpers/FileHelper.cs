using System.Collections.Generic;
using System.IO;

namespace SophiApp.Helpers
{
    internal class FileHelper
    {
        internal static void Save(List<string> list, string path) => File.WriteAllLines(path, list);
    }
}