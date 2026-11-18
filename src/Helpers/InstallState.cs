// <copyright file="InstallState.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// The state of a package that the app can install.
    /// </summary>
    public enum InstallState
    {
        /// <summary>
        /// The package is not installed.
        /// </summary>
        NotInstalled,

        /// <summary>
        /// The latest version of the package is installed.
        /// </summary>
        Installed,

        /// <summary>
        /// The package is installed, and a newer version is available.
        /// </summary>
        UpdateAvailable,
    }
}
