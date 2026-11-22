// <copyright file="DotNetReleaseIndex.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Data transfer object for the index of .NET releases: the latest release of every .NET version.
    /// </summary>
    public class DotNetReleaseIndex
    {
        /// <summary>
        /// Gets or sets the latest releases, one for a .NET version.
        /// </summary>
        [JsonPropertyName("releases-index")]
        public List<DotNetRelease> Releases { get; set; } = [];
    }
}
