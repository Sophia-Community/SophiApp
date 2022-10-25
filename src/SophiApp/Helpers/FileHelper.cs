using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace SophiApp.Helpers
{
    internal class FileHelper
    {
        private const int maxRelativePathLengthUnicodeChars = 260;
        private const int targetIsADirectory = 1;

        private enum MoveFileFlags
        {
            MOVEFILE_DELAY_UNTIL_REBOOT = 0x00000004
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CreateSymbolicLink(string lpSymlinkFileName, string lpTargetFileName, int dwFlags);

        private static string GetTargetPathRelativeToLink(string linkPath, string targetPath, bool linkAndTargetAreDirectories = false)
        {
            string returnPath;

            FileAttributes relativePathAttribute = 0;
            if (linkAndTargetAreDirectories)
            {
                relativePathAttribute = FileAttributes.Directory;
                // set the link path to the parent directory, so that PathRelativePathToW returns a path that works
                // for directory symlink traversal
                linkPath = Path.GetDirectoryName(linkPath.TrimEnd(Path.DirectorySeparatorChar));
            }

            StringBuilder relativePath = new StringBuilder(maxRelativePathLengthUnicodeChars);
            if (!PathRelativePathToW(relativePath, linkPath, relativePathAttribute, targetPath, relativePathAttribute))
            {
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                returnPath = targetPath;
            }
            else
            {
                returnPath = relativePath.ToString();
            }

            return returnPath;
        }

        private static void MarkFileDelete(params string[] files)
        {
            foreach (var file in files)
                _ = MoveFileEx(file, null, MoveFileFlags.MOVEFILE_DELAY_UNTIL_REBOOT);
        }

        [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool MoveFileEx(string lpExistingFileName, string lpNewFileName, MoveFileFlags dwFlags);

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool PathRelativePathToW(
            StringBuilder pszPath,
            string pszFrom,
            FileAttributes dwAttrFrom,
            string pszTo,
            FileAttributes dwAttrTo);

        internal static void CreateDirectory(string dirPath)
        {
            try
            {
                _ = Directory.CreateDirectory(dirPath);
            }
            catch (Exception)
            {
            }
        }

        internal static void CreateDirectory(params string[] dirsPath)
        {
            foreach (var path in dirsPath)
                CreateDirectory(path);
        }

        internal static void CreateDirectoryLink(string linkPath, string targetPath)
        {
            CreateDirectoryLink(linkPath, targetPath, false);
        }

        internal static void CreateDirectoryLink(string linkPath, string targetPath, bool makeTargetPathRelative)
        {
            if (makeTargetPathRelative)
            {
                targetPath = GetTargetPathRelativeToLink(linkPath, targetPath, true);
            }

            if (!CreateSymbolicLink(linkPath, targetPath, targetIsADirectory) || Marshal.GetLastWin32Error() != 0)
            {
                try
                {
                    Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());
                }
                catch (COMException exception)
                {
                    throw new IOException(exception.Message, exception);
                }
            }
        }

        internal static bool DirectoryIsEmpty(string dirPath)
        {
            byte count = 0;

            foreach (var entry in Directory.GetFileSystemEntries(dirPath))
            {
                count++;
                break;
            }

            return count == 0;
        }

        internal static void DirectoryLazyDelete(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                foreach (var file in Directory.GetFiles(dirPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (Exception)
                    {
                        MarkFileDelete(file);
                    }
                }

                foreach (var dir in Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        Directory.Delete(dir);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }

                TryDeleteDirectory(dirPath);
            }
        }

        internal static void DirectoryLazyDelete(params string[] dirsPath)
        {
            foreach (var dir in dirsPath)
                DirectoryLazyDelete(dir);
        }

        internal static void FileDelete(params string[] filesPath)
        {
            foreach (var file in filesPath)
                TryDeleteFile(file);
        }

        internal static bool IsSymbolicLink(string dirPath)
        {
            var di = new DirectoryInfo(dirPath);
            return di.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        internal static void TryDeleteDirectory(string dirPath)
        {
            if (Directory.Exists(dirPath))
            {
                try
                {
                    Directory.Delete(dirPath, recursive: true);
                }
                catch (Exception)
                {
                }
            }
        }

        internal static void TryDeleteFile(string filePath)
        {
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        internal static void TryDirectoryDelete(string dirPath)
        {
            try
            {
                Directory.Delete(dirPath, recursive: true);
            }
            catch (Exception)
            {
            }
        }

        internal static void WriteAllLines(string path, List<string> list)
        {
            var dirPath = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

            if (Directory.Exists(dirPath).Invert())
                Directory.CreateDirectory(dirPath);

            File.WriteAllLines(path, list);
        }

        internal static void WriteAllText(string path, string text)
        {
            var dirPath = path.Substring(0, path.LastIndexOf(Path.DirectorySeparatorChar));

            if (Directory.Exists(dirPath).Invert())
                Directory.CreateDirectory(dirPath);

            File.WriteAllText(path, text);
        }
    }
}