﻿// <copyright file="IAppContextService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with app context.
    /// </summary>
    public interface IAppContextService
    {
        /// <summary>
        /// Get app name and version.
        /// </summary>
        string GetFullName();

        /// <summary>
        /// Gets the code name of the application build.
        /// </summary>
        string GetBuildName();

        /// <summary>
        /// Returns the name of the program version: Community or Pro.
        /// </summary>
        string GetVersionName();
    }
}