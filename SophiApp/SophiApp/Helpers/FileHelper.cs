using System;
using System.IO;
using System.Runtime.InteropServices;

namespace SophiApp.Helpers
{
    internal class FileHelper
    {
        private enum MoveFileFlags
        {
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004
        }

        private static void MarkFileDelete(params string[] files)
        {
            foreach (var file in files)
                _ = MoveFileEx(file, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        internal static DirectoryInfo CreateDirectory(string dirPath) => Directory.CreateDirectory(dirPath);

        internal static void CreateDirectory(params string[] dirsPath)
        {
            foreach (var path in dirsPath)
                _ = CreateDirectory(path);
        }

        internal static bool DirIsEmpty(string dirPath)
        {
            byte count = 0;

            foreach (var entry in Directory.GetFileSystemEntries(dirPath))
            {
                count++;
                break;
            }

            return count == 0;
        }

        internal static void DirTryDelete(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, recursive: true);
            }
            catch (Exception)
            {
            }
        }

        internal static void LazyRemoveDirectory(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, true);
            }
            catch (Exception)
            {
                MarkFileDelete(Directory.GetFiles(dirPath));
            }
        }
    }
}