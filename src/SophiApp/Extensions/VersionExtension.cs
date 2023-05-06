// <copyright file="VersionExtension.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System;

    /// <summary>
    /// The <see cref="Version"/> class extensions.
    /// </summary>
    public static class VersionExtension
    {
        /// <summary>
        /// Gets major, minor, and build version separated by a dot.
        /// </summary>
        /// <param name="version"><see cref="Version"/>.</param>
        /// <returns><see cref="string"/>.</returns>
        public static string ToShortString(this Version version) => $"{version.Major}.{version.Minor}.{version.Build}";
    }
}
