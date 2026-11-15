// <copyright file="UwpPackage.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// A UWP app that can be uninstalled.
    /// </summary>
    public sealed class UwpPackage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UwpPackage"/> class.
        /// </summary>
        /// <param name="name">Package name, e.g. Microsoft.BingWeather.</param>
        /// <param name="displayName">Localized app name.</param>
        /// <param name="logo">Path to the app logo, if it has one.</param>
        /// <param name="fullNames">Full names of all the packages with this name.</param>
        public UwpPackage(string name, string displayName, Uri? logo, IReadOnlyList<string> fullNames)
        {
            Name = name;
            DisplayName = displayName;
            Logo = logo;
            FullNames = fullNames;
        }

        /// <summary>
        /// Gets package name, e.g. Microsoft.BingWeather.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets localized app name.
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// Gets path to the app logo, if it has one.
        /// </summary>
        public Uri? Logo { get; }

        /// <summary>
        /// Gets full names of all the packages with this name: there can be several of them for different architectures.
        /// </summary>
        public IReadOnlyList<string> FullNames { get; }
    }
}
