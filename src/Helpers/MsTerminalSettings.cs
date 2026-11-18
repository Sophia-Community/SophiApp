// <copyright file="MsTerminalSettings.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.Text.Json.Serialization;

    /// <summary>
    /// Data transfer object for Windows Terminal settings.
    /// </summary>
    public class MsTerminalSettings
    {
        /// <summary>
        /// Gets or sets profile settings.
        /// </summary>
        [JsonPropertyName("profiles")]
        public Profiles? Profiles { get; set; }
    }

    #pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// Data transfer object for Windows Terminal profile settings.
    /// </summary>
    public class Profiles
    {
        /// <summary>
        /// Gets or sets profile defaults.
        /// </summary>
        [JsonPropertyName("defaults")]
        public Defaults? Defaults { get; set; }
    }

    /// <summary>
    /// Gets or set defaults profile setting value.
    /// </summary>
    public class Defaults
    {
        /// <summary>
        /// Gets or sets a value indicating whether elevate setting.
        /// </summary>
        [JsonPropertyName("elevate")]
        public bool Elevate { get; set; }
    }

    #pragma warning restore SA1402 // File may only contain a single type
}
