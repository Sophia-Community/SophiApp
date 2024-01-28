// <copyright file="UriService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class UriService : IUriService
    {
        /// <inheritdoc/>
        public async Task OpenUrlAsync(string url)
        {
            await Task.Run(() =>
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    Process.Start("explorer.exe", url);
                    App.Logger.LogOpenedUrl(url);
                }
            });
        }
    }
}
