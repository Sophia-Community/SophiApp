// <copyright file="IHttpService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with HTTP API.
    /// </summary>
    public interface IHttpService
    {
        /// <summary>
        /// Downloads HEVC appx.
        /// </summary>
        /// <param name="fileName">Full path to save the file.</param>
        Task DownloadHEVCAppxAsync(string fileName);
    }
}
