// <copyright file="MsTerminalSettingsDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using Newtonsoft.Json;

    #pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    /// Data transfer object for Windows Terminal settings.
    /// </summary>
    public class MsTerminalSettingsDto
    {
        /// <summary>
        /// Gets or sets profile settings.
        /// </summary>
        [JsonProperty("profiles")]
        public Profiles? Profiles { get; set; }
    }

    /// <summary>
    /// Data transfer object for Windows Terminal profile settings.
    /// </summary>
    public class Profiles
    {
        /// <summary>
        /// Gets or sets profile defaults.
        /// </summary>
        [JsonProperty("defaults")]
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
        [JsonProperty("elevate")]
        public bool Elevate { get; set; }
    }

    #pragma warning restore SA1402 // File may only contain a single type
}
