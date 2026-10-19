// <copyright file="SettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;

using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;
using Windows.Graphics;
using Windows.Storage;

/// <inheritdoc/>
public class SettingsService : ISettingsService
{
    private const string AppTheme = "AppTheme";
    private const string AppWindowHeight = "AppWindowHeight";
    private const string AppWindowPositionX = "AppWindowPositionX";
    private const string AppWindowPositionY = "AppWindowPositionY";
    private const string AppWindowState = "AppWindowState";
    private const string AppWindowWidth = "AppWindowWidth";
    private const string LogPageVisibility = "ShowLogPage";
    private const string DebugRequirementAction = "RequirementActionForDebug";
    private const string SettingsFile = "Settings.json";
    private const string TextDescriptionSize = "TextDescriptionSize";
    private const string TextTitleSize = "TextTitleSize";

    private readonly IFileService fileService;
    private readonly string settingsFolder = AppContext.BaseDirectory;
    private IDictionary<string, object>? settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsService"/> class.
    /// </summary>
    /// <param name="fileService"><inheritdoc/></param>
    public SettingsService(IFileService fileService)
    {
        this.fileService = fileService;
    }

    /// <inheritdoc/>
    public double AppWindowDefaultWidth => 1115;

    /// <inheritdoc/>
    public double AppWindowDefaultHeight => 850;

    /// <inheritdoc/>
    public double AppWindowMinHeight => 600;

    /// <inheritdoc/>
    public double AppWindowMinWidth => 830;

    /// <inheritdoc/>
    public int DescriptionTextMinSize => 14;

    /// <inheritdoc/>
    public int DescriptionTextMaxSize => 24;

    /// <inheritdoc/>
    public int TitleTextMinSize => 16;

    /// <inheritdoc/>
    public int TitleTextMaxSize => 26;

    /// <inheritdoc/>
    public void Initialize() => settings = fileService.ReadFromJson<IDictionary<string, object>>(settingsFolder, SettingsFile) ?? new Dictionary<string, object>();

    /// <inheritdoc/>
    public void DeleteDebugRequirementAction() => DeleteSetting(DebugRequirementAction);

    /// <inheritdoc/>
    public async Task<PointInt32> ReadAppWindowPositionAsync()
    {
        var x = await ReadSettingAsync<int>(AppWindowPositionX);
        var y = await ReadSettingAsync<int>(AppWindowPositionY);
        return new PointInt32(x, y);
    }

    /// <inheritdoc/>
    public async Task<double> ReadAppWindowHeightAsync()
    {
        var height = await ReadSettingAsync<double>(AppWindowHeight);
        return height > AppWindowMinHeight ? height : AppWindowMinHeight;
    }

    /// <inheritdoc/>
    public async Task<WindowState> ReadAppWindowStateAsync() => await ReadSettingAsync<WindowState>(AppWindowState);

    /// <inheritdoc/>
    public async Task<double> ReadAppWindowWidthAsync()
    {
        var width = await ReadSettingAsync<double>(AppWindowWidth);
        return width > AppWindowMinWidth ? width : AppWindowMinWidth;
    }

    /// <inheritdoc/>
    public async Task<bool> ReadLogPageVisibilityAsync() => await ReadSettingAsync<bool>(LogPageVisibility);

    /// <inheritdoc/>
    public string ReadDebugRequirementAction() => ReadSetting<string>(DebugRequirementAction) ?? string.Empty;

    /// <inheritdoc/>
    public async Task<int> ReadTextDescriptionSizeAsync()
    {
        var descriptionTextSize = await ReadSettingAsync<int>(TextDescriptionSize);
        return descriptionTextSize > DescriptionTextMinSize && descriptionTextSize <= DescriptionTextMaxSize ? descriptionTextSize : DescriptionTextMinSize;
    }

    /// <inheritdoc/>
    public async Task<int> ReadTextTitleSizeAsync()
    {
        var titleTextSize = await ReadSettingAsync<int>(TextTitleSize);
        return titleTextSize > TitleTextMinSize && titleTextSize <= TitleTextMaxSize ? titleTextSize : TitleTextMinSize;
    }

    /// <inheritdoc/>
    public async Task<ElementTheme> ReadThemeAsync() => await ReadSettingAsync<ElementTheme>(AppTheme);

    /// <inheritdoc/>
    public async Task SaveAppWindowPositionAsync(PointInt32 point)
    {
        await SaveSettingAsync(AppWindowPositionX, point.X);
        await SaveSettingAsync(AppWindowPositionY, point.Y);
    }

    /// <inheritdoc/>
    public async Task SaveAppWindowSizeAsync(double height, double width)
    {
        await SaveSettingAsync(AppWindowHeight, height);
        await SaveSettingAsync(AppWindowWidth, width);
    }

    /// <inheritdoc/>
    public async Task SaveAppWindowStateAsync(WindowState state) => await SaveSettingAsync(AppWindowState, state);

    /// <inheritdoc/>
    public void SaveLogPageVisibility(bool isVisible) => Task.Run(async () => await SaveSettingAsync(LogPageVisibility, isVisible));

    /// <inheritdoc/>
    public async Task SaveDebugRequirementActionAsync(string action) => await SaveSettingAsync(DebugRequirementAction, action);

    /// <inheritdoc/>
    public async Task SaveTextDescriptionSizeAsync(int size) => await SaveSettingAsync(TextDescriptionSize, size);

    /// <inheritdoc/>
    public async Task SaveTextTitleSizeAsync(int size) => await SaveSettingAsync(TextTitleSize, size);

    /// <inheritdoc/>
    public async Task SaveThemeAsync(ElementTheme theme) => await SaveSettingAsync(AppTheme, theme);

    private void DeleteSetting(string name)
    {
        if (settings?.ContainsKey(name) ?? false)
        {
            settings.Remove(name);
            fileService.SaveToJson(settingsFolder, SettingsFile, settings);
        }
    }

    private async Task<T?> ReadSettingAsync<T>(string key)
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
            if (settings != null && settings.TryGetValue(key, out var obj))
            {
                return await Json.ToObjectAsync<T>((string)obj);
            }
        }

        return default;
    }

    private T? ReadSetting<T>(string key)
    {
        if (RuntimeHelper.IsMSIX)
        {
            if (ApplicationData.Current.LocalSettings.Values.TryGetValue(key, out var obj))
            {
                return Json.ToObject<T>((string)obj);
            }
        }
        else
        {
            if (settings != null && settings.TryGetValue(key, out var obj))
            {
                return Json.ToObject<T>((string)obj);
            }
        }

        return default;
    }

    private async Task SaveSettingAsync<T>(string key, T value)
    {
        if (RuntimeHelper.IsMSIX)
        {
            ApplicationData.Current.LocalSettings.Values[key] = await Json.StringifyAsync(value!);
        }
        else
        {
            settings![key] = await Json.StringifyAsync(value!);
            fileService.SaveToJson(settingsFolder, SettingsFile, settings);
        }
    }
}
