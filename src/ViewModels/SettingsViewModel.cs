// <copyright file="SettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using Views;

/// <summary>
/// Implements the <see cref="SettingsViewModel"/> class.
/// </summary>
public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemesService themesService;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private bool navigationViewHitTestVisible;

    [ObservableProperty]
    private ObservableCollection<AppTheme> themes =
    [
        new (ElementTheme.Default, "Settings_Themes_Default"), new (ElementTheme.Light, "Settings_Themes_Light"), new (ElementTheme.Dark, "Settings_Themes_Dark"),
    ];

    [ObservableProperty]
    private string version;
    private AppTheme selectedTheme;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themesService">A service for working with app themes.</param>
    /// <param name="dataService">A service for transferring app data between layers of DI.</param>
    /// <param name="shellViewModel">Implements the <see cref="ShellViewModel"/> class.</param>
    public SettingsViewModel(
        IThemesService themesService,
        ICommonDataService dataService,
        ShellViewModel shellViewModel)
    {
        FontOptions = shellViewModel.FontOptions;
        NavigationViewHitTestVisible = shellViewModel.NavigationViewHitTestVisible;
        LogPageVisibility = shellViewModel.LogPageVisibility;
        LogPageVisibilityCommand = shellViewModel.SetLogPageVisibility_Command;
        OpenAppAnimationDeveloperPage_Command = shellViewModel.OpenAppAnimationDeveloperPage_Command;
        OpenAppDeveloperPage_Command = shellViewModel.OpenAppDeveloperPage_Command;
        OpenAppDiscordPage_Command = shellViewModel.OpenAppDiscordPage_Command;
        OpenAppGitHubPage_Command = shellViewModel.OpenAppGitHubPage_Command;
        OpenAppProjectManagerPage_Command = shellViewModel.OpenAppProjectManagerPage_Command;
        OpenAppTelegramPage_Command = shellViewModel.OpenAppTelegramPage_Command;
        OpenAppTesterPage_Command = shellViewModel.OpenAppTesterPage_Command;
        OpenAppUiDeveloperPage_Command = shellViewModel.OpenAppUiDeveloperPage_Command;
        OpenAppUxDeveloperPage_Command = shellViewModel.OpenAppUxDeveloperPage_Command;
        OpenLatestAppRelease_Command = shellViewModel.OpenLatestAppRelease_Command;
        selectedTheme = themes.First(wrapper => wrapper.ElementTheme.Equals(themesService.Theme));
        this.themesService = themesService;
        version = dataService.GetFullName();
    }

    /// <summary>
    /// Gets or saves the app font sizes to a setting file.
    /// </summary>
    public FontOptions FontOptions { get; }

    /// <summary>
    /// Gets or sets app selected theme.
    /// </summary>
    public AppTheme SelectedTheme
    {
        get => selectedTheme;
        set
        {
            if (value != selectedTheme)
            {
                selectedTheme = value;
                _ = themesService.SetThemeAsync(selectedTheme.ElementTheme);
            }
        }
    }

    /// <summary>
    /// Gets or sets a value indicating whether <see cref="LogPage"/> is visible.
    /// </summary>
    public bool LogPageVisibility { get; set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app animation developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppAnimationDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app Discord page.
    /// </summary>
    public IRelayCommand OpenAppDiscordPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app GitHub page.
    /// </summary>
    public IRelayCommand OpenAppGitHubPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app project manager page.
    /// </summary>
    public IRelayCommand OpenAppProjectManagerPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app Telegram page.
    /// </summary>
    public IRelayCommand OpenAppTelegramPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app tester contacts page.
    /// </summary>
    public IRelayCommand OpenAppTesterPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app UI developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUiDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app UX developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUxDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open latest app release URL.
    /// </summary>
    public IRelayCommand OpenLatestAppRelease_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "Show log page in navigation menu" CheckBox in Settings page.
    /// </summary>
    public IRelayCommand<bool> LogPageVisibilityCommand { get; }
}
