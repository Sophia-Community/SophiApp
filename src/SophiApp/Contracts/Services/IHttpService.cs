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
        /// Downloads and saves file. If the file exists, it will be overwritten.
        /// </summary>
        /// <param name="url">File download link.</param>
        /// <param name="saveTo">File save path.</param>
        void DownloadFile(string url, string saveTo);

        /// <summary>
        /// Downloads HEVC appx.
        /// </summary>
        /// <param name="fileName">Full path to save the file.</param>
        Task DownloadHEVCAppxAsync(string fileName);

        /// <summary>
        /// Throws exception if url unavailable.
        /// </summary>
        /// <param name="url">Url to check availability.</param>
        void ThrowIfOffline(string url = "https://google.com");
    }
}
