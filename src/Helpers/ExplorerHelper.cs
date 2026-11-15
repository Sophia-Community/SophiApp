// <copyright file="ExplorerHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Diagnostics;

    /// <summary>
    /// Saves and restores File Explorer windows around its restart.
    /// </summary>
    public static class ExplorerHelper
    {
        private static readonly string ExplorerPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "explorer.exe");

        /// <summary>
        /// Gets paths of the folders that are opened in File Explorer windows.
        /// </summary>
        public static List<string> GetOpenedFolders()
        {
            var folders = new List<string>();

            try
            {
                var shellType = Type.GetTypeFromProgID("Shell.Application");

                if (shellType is null)
                {
                    return folders;
                }

                dynamic shell = Activator.CreateInstance(shellType) !;

                foreach (var window in shell.Windows())
                {
                    try
                    {
                        string? path = window.Document.Folder.Self.Path;

                        if (!string.IsNullOrEmpty(path))
                        {
                            folders.Add(path);
                        }
                    }
                    catch (Exception)
                    {
                        // The window is not a folder window
                    }
                }
            }
            catch (Exception)
            {
                // Opened folders cannot be received. File Explorer will be restarted without restoring them
            }

            return folders;
        }

        /// <summary>
        /// Starts File Explorer if it was not restarted automatically and opens the folders again.
        /// </summary>
        /// <param name="openedFolders">Paths of the folders to open.</param>
        public static async Task RestoreAsync(IEnumerable<string> openedFolders)
        {
            // Wait for File Explorer to be restarted by Windows
            await Task.Delay(3000);

            if (Process.GetProcessesByName("explorer").Length == 0)
            {
                // Start File Explorer if it was not restarted automatically
                using var explorer = Process.Start(ExplorerPath);
                await Task.Delay(3000);
            }

            // Restoring closed folders
            foreach (var folder in openedFolders.Where(Directory.Exists))
            {
                using var window = Process.Start(ExplorerPath, $"\"{folder}\"");
            }
        }
    }
}
