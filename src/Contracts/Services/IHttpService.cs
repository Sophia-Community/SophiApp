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
        /// Download OneDrive.
        /// </summary>
        /// <param name="saveTo">Full path to save the file.</param>
        void DownloadOneDrive(string saveTo);

        /// <summary>
        /// Sends a GET request to the specified url and returns deserialize value.
        /// </summary>
        /// <param name="url">A string that represents the request url.</param>
        /// <param name="timeout"><see cref="HttpClient"/> timeout in seconds.</param>
        /// <typeparam name="T">Type of return value.</typeparam>
        Task<T> GetFromJsonAsync<T>(string url, double timeout)
            where T : class;

        /// <summary>
        /// Open resource by url.
        /// </summary>
        /// <param name="url">Discoverable url.</param>
        void OpenUrl(string url);

        /// <summary>
        /// Determines whether the specified URL is available.
        /// </summary>
        /// <param name="url">Url to check.</param>
        bool UrlIsAvailable(string url);
    }
}
