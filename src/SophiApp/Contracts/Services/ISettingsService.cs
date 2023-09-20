// <copyright file="ISettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// Service for working with settings file.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Reads the settings from a file.
    /// </summary>
    /// <typeparam name="T">Type of setting to read.</typeparam>
    /// <param name="key">Name of the setting to be read.</param>
    Task<T?> ReadSettingAsync<T>(string key);

    /// <summary>
    /// Writes the settings to a file.
    /// </summary>
    /// <typeparam name="T">Type of setting to write.</typeparam>
    /// <param name="key">Setting key to be write.</param>
    /// <param name="value">Setting value to be write.</param>
    Task SaveSettingAsync<T>(string key, T value);
}
