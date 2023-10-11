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
        public Task OpenUrlAsync(string url)
            => string.IsNullOrWhiteSpace(url) ? Task.CompletedTask : Task.FromResult(Process.Start("explorer.exe", url));
    }
}
