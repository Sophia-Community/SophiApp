// <copyright file="FolderRemover.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// Removes a folder with everything that can be removed in it.
    /// </summary>
    public static class FolderRemover
    {
        /// <summary>
        /// Removes a folder. Read-only and hidden files are removed too, the files in use are skipped,
        /// as the cmdlet "Remove-Item -Recurse -Force -ErrorAction Ignore" does.
        /// </summary>
        /// <param name="folder">The path of the folder.</param>
        public static void Remove(string folder)
        {
            var directory = new DirectoryInfo(folder);

            if (!directory.Exists)
            {
                return;
            }

            // A link to another folder is removed itself, what it points to is left alone
            if (!directory.Attributes.HasFlag(FileAttributes.ReparsePoint))
            {
                foreach (var entry in GetEntries(directory))
                {
                    if (entry is DirectoryInfo)
                    {
                        Remove(entry.FullName);
                    }
                    else
                    {
                        TryRemove(entry);
                    }
                }
            }

            TryRemove(directory);
        }

        private static FileSystemInfo[] GetEntries(DirectoryInfo directory)
        {
            try
            {
                return directory.GetFileSystemInfos();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // The folder cannot be read: it is left as it is
                return [];
            }
        }

        private static void TryRemove(FileSystemInfo entry)
        {
            try
            {
                entry.Attributes = FileAttributes.Normal;
                entry.Delete();
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException)
            {
                // The file is in use, or the folder is not empty because of such a file
            }
        }
    }
}
