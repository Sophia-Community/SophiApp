// <copyright file="IUriService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with URIs.
    /// </summary>
    public interface IUriService
    {
        /// <summary>
        /// Opens a resource using an url.
        /// </summary>
        /// <param name="url">Discoverable url.</param>
        Task OpenUrl(string? url);
    }
}
