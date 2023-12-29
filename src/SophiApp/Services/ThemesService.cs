// <copyright file="ThemesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;

/// <inheritdoc/>
public class ThemesService : IThemesService
{
    private const string SettingsKey = "AppTheme";
    private readonly ISettingsService localSettingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemesService"/> class.
    /// </summary>
    /// <param name="localSettingsService">Service for working with settings file.</param>
    public ThemesService(ISettingsService localSettingsService)
    {
        this.localSettingsService = localSettingsService;
    }

    /// <inheritdoc/>
    public ElementTheme Theme { get; private set; } = ElementTheme.Default;

    /// <inheritdoc/>
    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    /// <inheritdoc/>
    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;
        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
        App.Logger.LogChangeTheme(Theme);
    }

    /// <inheritdoc/>
    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;

            TitleBarHelper.UpdateTitleBar(Theme);
        }

        await Task.CompletedTask;
    }

    private async Task<ElementTheme> LoadThemeFromSettingsAsync()
    {
        var themeName = await localSettingsService.ReadSettingAsync<string>(SettingsKey);

        if (Enum.TryParse(themeName, out ElementTheme cacheTheme))
        {
            return cacheTheme;
        }

        return ElementTheme.Default;
    }

    private async Task SaveThemeInSettingsAsync(ElementTheme theme)
    {
        await localSettingsService.SaveSettingAsync(SettingsKey, theme.ToString());
    }
}
