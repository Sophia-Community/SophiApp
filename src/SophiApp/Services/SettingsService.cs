// <copyright file="SettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using SophiApp.Contracts.Services;
using SophiApp.Core.Helpers;
using SophiApp.Helpers;
using Windows.Storage;

/// <summary>
/// <inheritdoc/>
/// </summary>
public class SettingsService : ISettingsService
{
    private readonly IFileService fileService;
    private readonly string optionsFolder = AppContext.BaseDirectory;
    private readonly string optionsFile = "Settings.json";
    private IDictionary<string, object> settings;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="fileService"><inheritdoc/></param>
    public SettingsService(IFileService fileService) => this.fileService = fileService;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    /// <param name="key"><inheritdoc/></param>
    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (settings != null && settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <typeparam name="T"><inheritdoc/></typeparam>
    /// <param name="key"><inheritdoc/></param>
    /// <param name="value"><inheritdoc/></param>
    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value);
        }
        else
        {
            await InitializeAsync();
            settings[key] = await Json.StringifyAsync(value);
            await Task.Run(() => fileService.Save(optionsFolder, optionsFile, settings));
        }
    }

    private async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            settings = await Task.Run(() => fileService
            .Read<IDictionary<string, object>>(optionsFolder, optionsFile)) ?? new Dictionary<string, object>();
            isInitialized = true;
        }
    }
}
