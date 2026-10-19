// <copyright file="VCRelease.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Newtonsoft.Json;

    /// <summary>
    /// Data transfer object for Visual C++ release settings.
    /// </summary>
    public class VCRelease
    {
        #pragma warning disable CS8618 // Non nullable field name is not initialized. Consider declare the field as nullable type.

        /// <summary>
        /// Gets or sets latest release version.
        /// </summary>
        [JsonProperty("version")]
        public Version? Version { get; set; }

        #pragma warning restore CS8618 // Non nullable field name is not initialized. Consider declare the field as nullable type.
    }
}
