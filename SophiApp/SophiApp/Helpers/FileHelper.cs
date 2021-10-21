using System;
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
                DebugHelper.WriteStatusLog($"Try create directory: {dirPath}");
                _ = Directory.CreateDirectory(dirPath);
            }
            catch (Exception e)
            {
                DebugHelper.WriteStatusLog($"Create directory has error: {e.Message}");
            }
        }

        internal static void CreateDirectory(params string[] dirsPath)
        {
            foreach (var path in dirsPath)
                CreateDirectory(path);
        }

        internal static void CreateDirectoryLink(string linkPath, string targetPath)
        {
            DebugHelper.WriteStatusLog($"Try create symbolic link dir: {linkPath}");
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

        internal static void DirectoryDelete(string dirPath)
        {
            try
            {
                DebugHelper.WriteStatusLog($"Try delete directory: {dirPath}");
                Directory.Delete(dirPath, recursive: true);
            }
            catch (Exception e)
            {
                DebugHelper.WriteStatusLog($"Delete directory has error: {e.Message}");
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

            DebugHelper.WriteStatusLog($"Dir: \"{dirPath}\" is empty: {count == 0}");
            return count == 0;
        }

        internal static void DirectoryLazyDelete(string dirPath)
        {
            try
            {
                DebugHelper.WriteStatusLog($"Try lazy delete dir: {dirPath}");
                Directory.Delete(dirPath, true);
                DebugHelper.WriteStatusLog($"Success delete dir: {dirPath}");
            }
            catch (Exception e)
            {
                DebugHelper.WriteStatusLog($"Delete dir {dirPath} has error {e.Message}");
                DebugHelper.WriteStatusLog($"Send files to MarkFileDelete func:");
                Array.ForEach(Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories), f => DebugHelper.WriteStatusLog($"{f}"));
                MarkFileDelete(Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories));
            }

            //var regpathSessionManager = @"SYSTEM\CurrentControlSet\Control\Session Manager";
            //var regkeyPendingFile = "PendingFileRenameOperations";
            ////var regvaluePendingFile = RegHelper.GetValue(RegistryHive.LocalMachine, regpathSessionManager, regkeyPendingFile) as string[];

            //try
            //{
            //    DebugHelper.WriteStatusLog($"Try lazy delete dir: {dirPath}");
            //    Directory.Delete(dirPath, true);
            //    DebugHelper.WriteStatusLog($"Success delete dir: {dirPath}");
            //}
            //catch (Exception e)
            //{
            //    DebugHelper.WriteStatusLog($"Delete dir {dirPath} has error {e.Message}");
            //    var arr = Directory.GetFiles(dirPath);
            //    var arr1 = new List<string>();

            //    for (int i = 0; i < arr.Length; i++)
            //    {
            //        arr1.Add(i % 2 == 0 ? arr[i] : string.Empty);
            //    }

            //    var regvaluePendingFile = arr1.ToArray();

            //    RegHelper.SetValue(RegistryHive.LocalMachine, regpathSessionManager, regkeyPendingFile, regvaluePendingFile, RegistryValueKind.MultiString);
            //    DebugHelper.WriteStatusLog($@"Write to registry {RegistryHive.LocalMachine}\{regpathSessionManager}\{regkeyPendingFile} value:");
            //    arr1.ForEach(str => DebugHelper.WriteStatusLog($"{str}"));
            //}
        }

        internal static bool IsSymbolicLink(string dirPath)
        {
            var di = new DirectoryInfo(dirPath);
            DebugHelper.WriteStatusLog($"Directory {dirPath} is symbolic link: {di.Attributes.HasFlag(FileAttributes.ReparsePoint)}");
            return di.Attributes.HasFlag(FileAttributes.ReparsePoint);
        }

        internal static void TryDeleteDirectory(string dirPath)
        {
            try
            {
                DebugHelper.WriteStatusLog($"Try delete directory: {dirPath}");
                Directory.Delete(dirPath, recursive: true);
            }
            catch (Exception e)
            {
                DebugHelper.WriteStatusLog($"Delete directory has error: {e.Message}");
            }
        }
    }
}