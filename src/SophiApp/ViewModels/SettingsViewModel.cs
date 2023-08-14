// <copyright file="SettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;

/// <summary>
/// Implements the <see cref="SettingsViewModel"/> class.
/// </summary>
public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService themeSelectorService;

    [ObservableProperty]
    private string build;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private ObservableCollection<ElementThemeWrapper> themes = new ()
    {
        new (ElementTheme.Default, "Settings_Themes_Default"), new (ElementTheme.Dark, "Settings_Themes_Dark"), new (ElementTheme.Light, "Settings_Themes_Light"),
    };

    [ObservableProperty]
    private string version;
    private ElementThemeWrapper selectedTheme;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/>.</param>
    /// <param name="appContextService"><see cref="IAppContextService"/>.</param>
    /// <param name="uriService"><see cref="IUriService"/>.</param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService, IAppContextService appContextService, IUriService uriService)
    {
        this.themeSelectorService = themeSelectorService;
        delimiter = appContextService.GetDelimiter();
        version = appContextService.GetFullName();
        build = appContextService.GetBuildName();
        selectedTheme = themes.First(wrapper => wrapper.ElementTheme.Equals(themeSelectorService.Theme));
        OpenLinkCommand = new AsyncRelayCommand<string>((param) => uriService.OpenUrl(param));
    }

    /// <summary>
    /// Gets or sets app selected theme.
    /// </summary>
    public ElementThemeWrapper SelectedTheme
    {
        get => selectedTheme;
        set
        {
            if (value != selectedTheme)
            {
                selectedTheme = value;
                _ = themeSelectorService.SetThemeAsync(selectedTheme.ElementTheme);
            }
        }
    }

    /// <summary>
    /// Gets a resource using an identifier.
    /// </summary>
    public ICommand OpenLinkCommand { get; }
}
