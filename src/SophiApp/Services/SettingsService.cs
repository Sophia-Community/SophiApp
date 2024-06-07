// <copyright file="SettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;
using Windows.Storage;

/// <inheritdoc/>
public class SettingsService : ISettingsService
{
    private readonly IFileService fileService;
    private readonly string settingsFolder = AppContext.BaseDirectory;
    private readonly string settingsFile = "Settings.json";
    private IDictionary<string, object>? settings;
    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="fileService"><inheritdoc/></param>
    public SettingsService(IFileService fileService)
    {
        this.fileService = fileService;
    }

    /// <inheritdoc/>
    public async Task<T?> ReadSettingAsync<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return await JsonExtensions.ToObjectAsync<T>((string)obj);
            }
        }
        else
        {
            await InitializeAsync();

            if (settings != null && settings.TryGetValue(key, out var obj))
            {
                return await JsonExtensions.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    /// <inheritdoc/>
    public async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await JsonExtensions.StringifyAsync(value!);
        }
        else
        {
            await InitializeAsync();
            settings![key] = await JsonExtensions.StringifyAsync(value!);
            await Task.Run(() => fileService.SaveToJson(settingsFolder, settingsFile, settings));
        }
    }

    private async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            settings = await Task.Run(() => fileService
            .ReadFromJson<IDictionary<string, object>>(settingsFolder, settingsFile)) ?? new Dictionary<string, object>();
            isInitialized = true;
        }
    }
}
