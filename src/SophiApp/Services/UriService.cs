// <copyright file="UriService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UriService : IUriService
    {
        /// <inheritdoc/>
        public Task OpenUri(string? uri)
            => string.IsNullOrWhiteSpace(uri) ? Task.CompletedTask : Task.FromResult(Process.Start(uri!));
    }
}
