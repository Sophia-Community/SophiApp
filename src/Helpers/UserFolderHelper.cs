// <copyright file="UserFolderHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Runtime.InteropServices;
    using System.Text;
    using Microsoft.Win32;
    using SophiApp.Extensions;

    /// <summary>
    /// Reads and changes the locations of the user folders: Desktop, Documents, Downloads, Music, Pictures and Videos.
    /// </summary>
    internal static class UserFolderHelper
    {
        private const string ShellFoldersKey = "Software\\Microsoft\\Windows\\CurrentVersion\\Explorer\\User Shell Folders";
        private const string DesktopIniName = "desktop.ini";

        // The screenshots folder has a registry value only while it is not in its default place
        private const string ScreenshotsRegistryName = "{B7BEDE81-DF94-4682-A7D8-57A52620B86F}";

        // SHCNE_UPDATEITEM, SHCNE_UPDATEDIR, SHCNF_PATHW | SHCNF_FLUSH
        private const int UpdateItem = 0x00002000;
        private const int UpdateDirectory = 0x00001000;
        private const uint PathAndFlush = 0x0005 | 0x1000;

        private static readonly Dictionary<UserFolder, FolderData> Folders = new()
        {
            [UserFolder.Desktop] = new("Desktop", "{754AC886-DF64-4CBA-86B5-F7FBF4FBCEF5}", ["B4BFCC3A-DB2C-424C-B029-7FE99A87C641"], GetDesktopIni(nameId: 21769, iconId: 183)),
            [UserFolder.Documents] = new("Personal", "{F42EE2D3-909F-4907-8871-4C22FC0BF756}", ["FDD39AD0-238F-46AF-ADB4-6C85480369C7", "F42EE2D3-909F-4907-8871-4C22FC0BF756"], GetDesktopIni(nameId: 21770, iconId: 112, oldIconId: 235)),
            [UserFolder.Downloads] = new("{374DE290-123F-4565-9164-39C4925E467B}", "{7D83EE9B-2244-4E70-B1F5-5393042AF1E4}", ["374DE290-123F-4565-9164-39C4925E467B", "7D83EE9B-2244-4E70-B1F5-5393042AF1E4"], GetDesktopIni(nameId: 21798, iconId: 184)),
            [UserFolder.Music] = new("My Music", "{A0C69A99-21C8-4671-8703-7934162FCF1D}", ["4BD8D571-6D19-48D3-BE97-422220080E43", "A0C69A99-21C8-4671-8703-7934162FCF1D"], GetDesktopIni(nameId: 21790, iconId: 108, oldIconId: 237, infoTipId: 12689)),
            [UserFolder.Pictures] = new("My Pictures", "{0DDD015D-B06C-45D5-8C4C-F59713854639}", ["33E28130-4E1E-4676-835A-98395C3BC3BB", "0DDD015D-B06C-45D5-8C4C-F59713854639"], GetDesktopIni(nameId: 21779, iconId: 113, oldIconId: 236, infoTipId: 12688)),
            [UserFolder.Videos] = new("My Video", "{35286A68-3C57-41A1-BBB1-0EAE73D76C95}", ["18989B1D-99B5-455B-841C-AB7C74E4DDFC", "35286A68-3C57-41A1-BBB1-0EAE73D76C95"], GetDesktopIni(nameId: 21791, iconId: 189, oldIconId: 238, infoTipId: 12690)),
        };

        /// <summary>
        /// Gets the location a user folder has in a new Windows user profile.
        /// </summary>
        /// <param name="folder">A user folder.</param>
        public static string GetDefaultPath(UserFolder folder) => folder == UserFolder.Screenshots
            ? Path.Combine(GetPath(UserFolder.Pictures), folder.ToString())
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), folder.ToString());

        /// <summary>
        /// Gets the location of a user folder.
        /// </summary>
        /// <param name="folder">A user folder.</param>
        public static string GetPath(UserFolder folder)
        {
            var registryName = folder == UserFolder.Screenshots ? ScreenshotsRegistryName : Folders[folder].RegistryName;
            return Registry.CurrentUser.GetSubKeyValue(ShellFoldersKey, registryName) is string { Length: > 0 } path ? Normalize(path) : GetDefaultPath(folder);
        }

        /// <summary>
        /// Gets the fixed drives a user folder can be moved to. The system drive is not one of them.
        /// </summary>
        public static IReadOnlyList<string> GetOtherDrives()
        {
            var systemDrive = Path.GetPathRoot(Environment.SystemDirectory) ?? string.Empty;

            return DriveInfo.GetDrives()
                .Where(drive => drive.DriveType == DriveType.Fixed && drive.IsReady && !drive.Name.Equals(systemDrive, StringComparison.OrdinalIgnoreCase))
                .Select(drive => drive.Name)
                .Order(StringComparer.OrdinalIgnoreCase)
                .ToArray();
        }

        /// <summary>
        /// Gets a value indicating whether a path is the root of a drive or of a network share.
        /// </summary>
        /// <param name="path">A path to a folder.</param>
        public static bool IsRoot(string path) => Normalize(path).Equals(Normalize(Path.GetPathRoot(path) ?? string.Empty), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether two paths lead to the same folder.
        /// </summary>
        /// <param name="first">A path to a folder.</param>
        /// <param name="second">Another path to a folder.</param>
        public static bool IsSamePath(string first, string second) => Normalize(first).Equals(Normalize(second), StringComparison.OrdinalIgnoreCase);

        /// <summary>
        /// Gets a value indicating whether a folder has files or folders in it. The file that keeps the look of the folder does not count.
        /// </summary>
        /// <param name="path">A path to a folder.</param>
        public static bool HasFiles(string path)
        {
            try
            {
                return Directory.Exists(path)
                    && Directory.EnumerateFileSystemEntries(path).Any(entry => !Path.GetFileName(entry).Equals(DesktopIniName, StringComparison.OrdinalIgnoreCase));
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.LogSkippedException($"Looking for files in the folder \"{path}\"", ex);
                return false;
            }
        }

        /// <summary>
        /// Changes the location of a user folder. The files are not moved to the new location.
        /// </summary>
        /// <param name="folder">A user folder.</param>
        /// <param name="path">The new location. The folder is created if it does not exist.</param>
        public static void SetPath(UserFolder folder, string path)
        {
            if (folder == UserFolder.Screenshots)
            {
                SetScreenshotsPath(path);
                return;
            }

            var data = Folders[folder];
            var currentPath = GetPath(folder);
            var newPath = Normalize(path);

            if (IsSamePath(currentPath, newPath))
            {
                return;
            }

            _ = Directory.CreateDirectory(newPath);
            var isMoved = false;

            foreach (var id in data.KnownFolderIds)
            {
                var knownFolder = Guid.Parse(id);
                var result = SHSetKnownFolderPath(ref knownFolder, 0, nint.Zero, newPath);

                // The first id is the folder itself. The second one is its twin for the local files of a profile kept in the cloud
                if (result < 0 && !isMoved)
                {
                    Marshal.ThrowExceptionForHR(result);
                }

                isMoved = true;
            }

            // File Explorer reads the name and the icon of a folder from its desktop.ini only if the folder is read-only
            var directory = new DirectoryInfo(newPath);
            directory.Attributes |= FileAttributes.ReadOnly;
            Registry.CurrentUser.CreateKeyAndSetValue(ShellFoldersKey, data.RegistryGuid, newPath, RegistryValueKind.ExpandString);

            // The old folder is not a user folder anymore
            DeleteDesktopIni(currentPath);
            DeleteDesktopIni(newPath);
            var desktopIni = Path.Combine(newPath, DesktopIniName);
            File.WriteAllText(desktopIni, data.DesktopIni, Encoding.Unicode);
            File.SetAttributes(desktopIni, FileAttributes.Hidden | FileAttributes.System | FileAttributes.Archive);

            UpdateFolder(newPath);

            if (Directory.Exists(currentPath))
            {
                UpdateFolder(currentPath);
            }
        }

        [DllImport("shell32.dll")]
        private static extern int SHSetKnownFolderPath(ref Guid folderId, uint flags, nint token, [MarshalAs(UnmanagedType.LPWStr)] string path);

        [DllImport("shell32.dll")]
        private static extern void SHChangeNotify(int eventId, uint flags, [MarshalAs(UnmanagedType.LPWStr)] string item1, nint item2);

        // Windows keeps only the location of the screenshots folder: it is not a folder of the user profile with a name and an icon of its own
        private static void SetScreenshotsPath(string path)
        {
            var newPath = Normalize(path);

            if (IsSamePath(newPath, GetDefaultPath(UserFolder.Screenshots)))
            {
                Registry.CurrentUser.DeleteSubKeyValue(ShellFoldersKey, ScreenshotsRegistryName);
                return;
            }

            _ = Directory.CreateDirectory(newPath);
            Registry.CurrentUser.CreateKeyAndSetValue(ShellFoldersKey, ScreenshotsRegistryName, newPath, RegistryValueKind.ExpandString);
        }

        // Makes File Explorer read the name and the icon of a folder again
        private static void UpdateFolder(string path)
        {
            SHChangeNotify(UpdateItem, PathAndFlush, path, nint.Zero);
            SHChangeNotify(UpdateDirectory, PathAndFlush, path, nint.Zero);
        }

        private static void DeleteDesktopIni(string folderPath)
        {
            var desktopIni = Path.Combine(folderPath, DesktopIniName);

            try
            {
                if (File.Exists(desktopIni))
                {
                    // A read-only file cannot be deleted
                    File.SetAttributes(desktopIni, FileAttributes.Normal);
                    File.Delete(desktopIni);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                App.Logger.LogSkippedException($"Deleting the file \"{desktopIni}\"", ex);
            }
        }

        // A path without the backslash at the end, with the variables replaced by their values. The root of a drive keeps its backslash
        private static string Normalize(string path)
        {
            var expanded = Environment.ExpandEnvironmentVariables(path.Trim());
            return expanded.Length > 3 ? expanded.TrimEnd('\\') : expanded;
        }

        // The file starts with an empty line
        private static string GetDesktopIni(int nameId, int iconId, int oldIconId = 0, int infoTipId = 0)
        {
            var lines = new List<string> { string.Empty, "[.ShellClassInfo]", $"LocalizedResourceName=@%SystemRoot%\\system32\\shell32.dll,-{nameId}" };

            if (infoTipId > 0)
            {
                lines.Add($"InfoTip=@%SystemRoot%\\system32\\shell32.dll,-{infoTipId}");
            }

            lines.Add($"IconResource=%SystemRoot%\\System32\\imageres.dll,-{iconId}");

            if (oldIconId > 0)
            {
                lines.Add("IconFile=%SystemRoot%\\System32\\shell32.dll");
                lines.Add($"IconIndex=-{oldIconId}");
            }

            return string.Join("\r\n", lines) + "\r\n";
        }

        private sealed record FolderData(string RegistryName, string RegistryGuid, string[] KnownFolderIds, string DesktopIni);
    }
}
