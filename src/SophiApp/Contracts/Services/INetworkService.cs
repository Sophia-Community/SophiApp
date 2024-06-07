// <copyright file="INetworkService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with networks API.
    /// </summary>
    public interface INetworkService
    {
        /// <summary>
        /// Determine if there has Internet access.
        /// </summary>
        bool IsOnline();
    }
}
