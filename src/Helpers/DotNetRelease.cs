// <copyright file="DotNetRelease.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data transfer object for .NET release settings.
    /// </summary>
    public class DotNetRelease
    {
        /// <summary>
        /// Gets or sets latest release version. It is a part of the installer file name and may have a suffix, e.g. "11.0.0-rc.2.25502.107".
        /// </summary>
        [JsonProperty("latest-release")]
        public string? Version { get; set; }
    }
}
