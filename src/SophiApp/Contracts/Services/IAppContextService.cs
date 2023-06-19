// <copyright file="IAppContextService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// Service for app data.
    /// </summary>
    public interface IAppContextService
    {
        /// <summary>
        /// Get app name, version with build and type.
        /// </summary>
        string GetFullName();
    }
}
