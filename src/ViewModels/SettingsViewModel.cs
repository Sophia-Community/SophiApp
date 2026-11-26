// <copyright file="SettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;

using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;

/// <summary>
/// Implements the <see cref="SettingsViewModel"/> class.
/// </summary>
public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemesService themesService;
    private readonly ISettingsService settingsService;
    private readonly ShellViewModel shellViewModel;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private bool navigationViewHitTestVisible;

    [ObservableProperty]
    private ObservableCollection<AppTheme> themes =
    [
        new(ElementTheme.Default, "Settings_Themes_Default"), new(ElementTheme.Light, "Settings_Themes_Light"), new(ElementTheme.Dark, "Settings_Themes_Dark"),
    ];

    // The names of the languages are not translated: everyone finds their own language
    [ObservableProperty]
    private ObservableCollection<AppLanguage> languages =
    [
        new(AppLanguage.WindowsLanguageTag, "Settings_Languages_Default".GetLocalized()), new("en-US", "English"), new("ru-RU", "Русский"),
    ];

    [ObservableProperty]
    private string version;
    private AppTheme selectedTheme;
    private AppLanguage selectedLanguage;
    private bool rememberTheme;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themesService">A service for working with app themes.</param>
    /// <param name="settingsService">A service for working with app settings.</param>
    /// <param name="dataService">A service for transferring app data between layers of DI.</param>
    /// <param name="shellViewModel">Implements the <see cref="ShellViewModel"/> class.</param>
    public SettingsViewModel(
        IThemesService themesService,
        ISettingsService settingsService,
        ICommonDataService dataService,
        ShellViewModel shellViewModel)
    {
        NavigationViewHitTestVisible = shellViewModel.NavigationViewHitTestVisible;
        OpenAppDeveloperPage_Command = shellViewModel.OpenAppDeveloperPage_Command;
        OpenAppSupporterPage_Command = shellViewModel.OpenAppSupporterPage_Command;
        OpenAppPromoterPage_Command = shellViewModel.OpenAppPromoterPage_Command;
        OpenAppDiscordPage_Command = shellViewModel.OpenAppDiscordPage_Command;
        OpenAppGitHubPage_Command = shellViewModel.OpenAppGitHubPage_Command;
        OpenAppProjectManagerPage_Command = shellViewModel.OpenAppProjectManagerPage_Command;
        OpenAppTelegramPage_Command = shellViewModel.OpenAppTelegramPage_Command;
        OpenAppTesterPage_Command = shellViewModel.OpenAppTesterPage_Command;
        OpenAppUxDeveloperPage_Command = shellViewModel.OpenAppUxDeveloperPage_Command;
        OpenLatestAppRelease_Command = shellViewModel.OpenLatestAppRelease_Command;
        selectedTheme = themes.First(wrapper => wrapper.ElementTheme.Equals(themesService.Theme));
        selectedLanguage = languages.First(language => language.Tag.Equals(AppLanguage.Chosen, StringComparison.OrdinalIgnoreCase));
        this.themesService = themesService;
        this.settingsService = settingsService;
        this.shellViewModel = shellViewModel;
        rememberTheme = settingsService.ReadRememberTheme();
        version = dataService.GetFullName();
    }

    /// <summary>
    /// Gets or sets app selected theme.
    /// </summary>
    public AppTheme SelectedTheme
    {
        get
        {
            return selectedTheme;
        }

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
    /// Gets or sets a value indicating whether the chosen theme is kept for the next start of the app.
    /// Without it the app starts with the theme of Windows.
    /// </summary>
    public bool RememberTheme
    {
        get
        {
            return rememberTheme;
        }

        set
        {
            if (SetProperty(ref rememberTheme, value))
            {
                _ = SaveRememberThemeAsync(value);
            }
        }
    }

    /// <summary>
    /// Gets or sets the app language. A new one is used at once: the app reads the settings of Windows again.
    /// </summary>
    public AppLanguage SelectedLanguage
    {
        get
        {
            return selectedLanguage;
        }

        set
        {
            if (value is not null && value != selectedLanguage)
            {
                selectedLanguage = value;
                AppLanguage.Chosen = value.Tag;
                App.Logger.LogChangeLanguage(value.Tag);

                // The window is filled anew, so the list is let to close first
                _ = App.MainWindow.DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => _ = ChangeLanguageAsync(value.Tag));
            }
        }
    }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open the page of the person who supports the app.
    /// </summary>
    public IRelayCommand OpenAppSupporterPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open the page of the person who promotes the app.
    /// </summary>
    public IRelayCommand OpenAppPromoterPage_Command { get; }

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
    /// Gets <see cref="IRelayCommand"/> to open app UX developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUxDeveloperPage_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open latest app release URL.
    /// </summary>
    public IRelayCommand OpenLatestAppRelease_Command { get; }

    private async Task ChangeLanguageAsync(string tag)
    {
        await settingsService.SaveLanguageAsync(tag);

        try
        {
            await shellViewModel.ChangeLanguageAsync(tag);
        }
        catch (Exception ex)
        {
            // The language is saved, so it is used when the app starts next time
            App.Logger.LogSkippedException("Changing the language while the app runs", ex);
        }
    }

    // The file keeps the theme only while it is to be remembered
    private async Task SaveRememberThemeAsync(bool remember)
    {
        await settingsService.SaveRememberThemeAsync(remember);
        await settingsService.SaveThemeAsync(remember ? themesService.Theme : ElementTheme.Default);
    }
}
