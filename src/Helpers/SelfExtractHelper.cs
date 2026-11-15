// <copyright file="SelfExtractHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;

    /// <summary>
    /// The app is a single file that unpacks itself to "%TEMP%\.net\SophiApp" at start. This helper removes the unpacked files.
    /// </summary>
    public static class SelfExtractHelper
    {
        private static readonly string ExeName = Path.GetFileName(Environment.ProcessPath) ?? string.Empty;

        // Every folder with the unpacked app has this file
        private static readonly string AppLibrary = $"{typeof(SelfExtractHelper).Assembly.GetName().Name}.dll";

        // The folder the running app was unpacked to, or null if the app was not unpacked
        private static readonly string? ExtractFolder = GetExtractFolder();

        /// <summary>
        /// Removes the files left by the previous runs and removes the files of this run when the app exits.
        /// </summary>
        public static void RemoveOnExit()
        {
            if (ExtractFolder is null)
            {
                return;
            }

            RemoveStaleFolders();
            AppDomain.CurrentDomain.ProcessExit += (_, _) => RemoveAfterExit();
        }

        private static string? GetExtractFolder()
        {
            var folder = new DirectoryInfo(AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar));

            // The app runs from its own folder: there is nothing to remove
            if (ExeName.Length == 0 || folder.FullName.Equals(EnvironmentHelper.AppFolder, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            // Nothing but "<any folder>\.net\<exe name>\<id>" with the unpacked app inside may be removed
            var isExtractFolder = folder.Parent?.Parent?.Name == ".net"
                && folder.Parent.Name.Equals(Path.GetFileNameWithoutExtension(ExeName), StringComparison.OrdinalIgnoreCase)
                && HasUnpackedApp(folder);

            return isExtractFolder ? folder.FullName : null;
        }

        private static bool HasUnpackedApp(DirectoryInfo folder) => File.Exists(Path.Combine(folder.FullName, AppLibrary));

        private static void RemoveStaleFolders()
        {
            // The app could crash or be updated, so its old files are still there
            foreach (var folder in Directory.GetParent(ExtractFolder!)!.GetDirectories())
            {
                if (folder.FullName.Equals(ExtractFolder, StringComparison.OrdinalIgnoreCase) || !HasUnpackedApp(folder))
                {
                    continue;
                }

                try
                {
                    folder.Delete(recursive: true);
                }
                catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
                {
                    // The folder is in use
                }
            }
        }

        private static void RemoveAfterExit()
        {
            // The app cannot remove its own files while it is running, so a hidden command does it a few seconds later.
            // The files are kept if the app is running again by that time.
            // The parent folders are removed only if they are empty: ".net" is shared with other apps
            var parent = Directory.GetParent(ExtractFolder!)!;
            var arguments = $"/c ping -n 4 127.0.0.1 >nul & tasklist /fi \"imagename eq {ExeName}\" /nh | find /i \"{ExeName}\" >nul || rmdir /s /q \"{ExtractFolder}\" & rmdir \"{parent.FullName}\" 2>nul & rmdir \"{parent.Parent!.FullName}\" 2>nul";

            try
            {
                using var cleaner = Process.Start(new ProcessStartInfo(Path.Combine(Environment.SystemDirectory, "cmd.exe"), arguments)
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    WorkingDirectory = Environment.SystemDirectory,
                });
            }
            catch (Exception)
            {
                // The files will be removed at the next start
            }
        }
    }
}
