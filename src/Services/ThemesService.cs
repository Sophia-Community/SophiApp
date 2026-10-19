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
    private readonly ISettingsService settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ThemesService"/> class.
    /// </summary>
    /// <param name="settingsService">Service for working with app settings.</param>
    public ThemesService(ISettingsService settingsService)
    {
        this.settingsService = settingsService;
    }

    /// <inheritdoc/>
    public ElementTheme Theme { get; private set; }

    /// <inheritdoc/>
    public async Task InitializeAsync() => Theme = await settingsService.ReadThemeAsync();

    /// <inheritdoc/>
    public async Task SetThemeAsync(ElementTheme theme)
    {
        Theme = theme;
        await SetRequestedThemeAsync();
        await settingsService.SaveThemeAsync(theme);
        App.Logger.LogChangeTheme(Theme);
    }

    /// <inheritdoc/>
    public async Task SetRequestedThemeAsync()
    {
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            rootElement.RequestedTheme = Theme;
            TitleBarHelper.UpdateTitleBar(Theme);
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        }

        await Task.CompletedTask;
    }
}
