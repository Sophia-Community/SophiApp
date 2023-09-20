// <copyright file="ThemesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;

/// <summary>
/// <inheritdoc/>
/// </summary>
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

    /// <summary>
    /// Gets or sets app theme.
    /// </summary>
    public ElementTheme Theme { get; set; } = ElementTheme.Default;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public async Task InitializeAsync()
    {
        Theme = await LoadThemeFromSettingsAsync();
        await Task.CompletedTask;
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    /// <param name="theme">Specifies a UI theme that should be used for UIElements.</param>
    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;
        await SetRequestedThemeAsync();
        await SaveThemeInSettingsAsync(Theme);
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
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
