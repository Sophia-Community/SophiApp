// <copyright file="LocalSettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.Extensions.Options;
using SophiApp.Contracts.Services;
using SophiApp.Core.Contracts.Services;
using SophiApp.Core.Helpers;
using SophiApp.Helpers;
using SophiApp.Models;
using Windows.Storage;

public class LocalSettingsService : ILocalSettingsService
{
    private const string defaultApplicationDataFolder = "SophiApp/ApplicationData";
    private const string defaultLocalSettingsFile = "LocalSettings.json";

    private readonly IFileService fileService;
    private readonly LocalSettingsOptions options;

    private readonly string localApplicationData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    private readonly string applicationDataFolder;
    private readonly string localsettingsFile;

    private IDictionary<string, object> settings;

    private bool isInitialized;

    /// <summary>
    /// Initializes a new instance of the <see cref="LocalSettingsService"/> class.
    /// </summary>
    /// <param name="fileService"><inheritdoc/></param>
    /// <param name="options">Used to retrieve configured <see cref="IOptions{TOptions}"/> instances.</param>
    public LocalSettingsService(IFileService fileService, IOptions<LocalSettingsOptions> options)
    {
        this.fileService = fileService;
        this.options = options.Value;

        applicationDataFolder = Path.Combine(localApplicationData, this.options.ApplicationDataFolder ?? defaultApplicationDataFolder);
        localsettingsFile = this.options.LocalSettingsFile ?? defaultLocalSettingsFile;

        settings = new Dictionary<string, object>();
    }

    private async Task InitializeAsync()
    {
        if (!isInitialized)
        {
            settings = await Task.Run(() => fileService.Read<IDictionary<string, object>>(applicationDataFolder, localsettingsFile)) ?? new Dictionary<string, object>();
            isInitialized = true;
        }
    }

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

            await Task.Run(() => fileService.Save(applicationDataFolder, localsettingsFile, settings));
        }
    }
}
