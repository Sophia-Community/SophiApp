// <copyright file="MsTerminalSettingsDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using Newtonsoft.Json;

#pragma warning disable SA1402 // File may only contain a single type
#pragma warning disable CS1591 // There is no XML comment for an open visible type or member
#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    /// Data transfer object for Windows Terminal settings.
    /// </summary>
    public class MsTerminalSettingsDto
    {
        [JsonProperty("profiles")]
        public Profiles? Profiles { get; set; }
    }

    /// <summary>
    /// Gets or set profile settings value.
    /// </summary>
    public class Profiles
    {
        [JsonProperty("defaults")]
        public Defaults? Defaults { get; set; }
    }

    /// <summary>
    /// Gets or set defaults profile setting value.
    /// </summary>
    public class Defaults
    {
        [JsonProperty("elevate")]
        public bool Elevate { get; set; }
    }

#pragma warning restore SA1402 // File may only contain a single type
#pragma warning restore CS1591 // There is no XML comment for an open visible type or member
#pragma warning restore SA1600 // Elements should be documented

}
