// <copyright file="ICommonDataService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with common app data.
    /// </summary>
    public interface ICommonDataService
    {
        /// <summary>
        /// Gets app name and version.
        /// </summary>
        string GetFullName();

        /// <summary>
        /// Gets the code name of the application build.
        /// </summary>
        string GetBuildName();

        /// <summary>
        /// Gets the name of the program version: Community or Pro.
        /// </summary>
        string GetVersionName();

        /// <summary>
        /// Gets app name and version delimiter.
        /// </summary>
        string GetDelimiter();
    }
}
