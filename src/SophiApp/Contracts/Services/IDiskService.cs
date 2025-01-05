// <copyright file="IDiskService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with disk API.
    /// </summary>
    public interface IDiskService
    {
        /// <summary>
        /// Gets volume labels of all disks.
        /// </summary>
        IEnumerable<string> GetVolumeLabels();
    }
}
