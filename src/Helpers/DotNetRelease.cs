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
        /// Gets or sets latest release version.
        /// </summary>
        [JsonProperty("latest-release")]
        public Version? Version { get; set; }
    }
}
