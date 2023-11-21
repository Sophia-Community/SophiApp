// <copyright file="IUpdateService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for dealing with OS updates.
    /// </summary>
    public interface IUpdateService
    {
        /// <summary>
        /// Start receiving os updates.
        /// </summary>
        void RunOsUpdate();
    }
}
