// <copyright file="MsTerminalSettingsDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using Newtonsoft.Json;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Data transfer object for Microsoft Windows Terminal json settings file.
    /// </summary>
    public record MsTerminalSettingsDto([JsonProperty("profiles")] Profiles Profiles);

    /// <summary>
    /// Gets or set profile settings.
    /// </summary>
    public record Profiles([property: JsonProperty("defaults")] Defaults Defaults);

    /// <summary>
    /// Gets or set profile defaults settings.
    /// </summary>
    /// <param name="Elevate">Gets or sets elevate setting.</param>
    public record Defaults([property: JsonProperty("elevate")] bool? Elevate);

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
