// <copyright file="InstalledPrograms.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.Win32;

    /// <summary>
    /// Looks for the programs listed in "Installed apps" of Windows Settings.
    /// </summary>
    public static class InstalledPrograms
    {
        // The programs installed for the current user, for all users, and the 32-bit ones installed for all users
        private static readonly (RegistryKey Root, string Path)[] UninstallKeys =
        [
            (Registry.CurrentUser, "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
            (Registry.LocalMachine, "Software\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
            (Registry.LocalMachine, "Software\\WOW6432Node\\Microsoft\\Windows\\CurrentVersion\\Uninstall"),
        ];

        /// <summary>
        /// Determines whether a program with this name is installed.
        /// </summary>
        /// <param name="displayName">The name of the program as Windows shows it.</param>
        public static bool Exists(string displayName)
        {
            foreach (var (root, path) in UninstallKeys)
            {
                using var uninstallKey = root.OpenSubKey(path);

                foreach (var name in uninstallKey?.GetSubKeyNames() ?? [])
                {
                    using var programKey = uninstallKey!.OpenSubKey(name);

                    if (displayName.Equals(programKey?.GetValue("DisplayName") as string, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
