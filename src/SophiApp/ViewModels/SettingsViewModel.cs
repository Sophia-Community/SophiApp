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

/// <summary>
/// Implements the <see cref="SettingsViewModel"/> class.
/// </summary>
public partial class SettingsViewModel : ObservableRecipient
{
    private readonly IThemesService themeSelectorService;

    [ObservableProperty]
    private string build;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private bool navigationViewHitTestVisible;

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
    /// <param name="themeSelectorService">A service for working with app themes.</param>
    /// <param name="commonDataService">A service for transferring app data between layers of DI.</param>
    /// <param name="uriService">A service for working with URI.</param>
    /// <param name="shellViewModel">Implements the <see cref="ShellViewModel"/> class.</param>
    public SettingsViewModel(IThemesService themeSelectorService, ICommonDataService commonDataService, IUriService uriService, ShellViewModel shellViewModel)
    {
        build = commonDataService.GetBuildName();
        delimiter = commonDataService.GetDelimiter();
        FontOptions = shellViewModel.FontOptions;
        NavigationViewHitTestVisible = shellViewModel.NavigationViewHitTestVisible;
        OpenLinkCommand = new AsyncRelayCommand<string>(url => uriService.OpenUrlAsync(url!));
        selectedTheme = themes.First(wrapper => wrapper.ElementTheme.Equals(themeSelectorService.Theme));
        this.themeSelectorService = themeSelectorService;
        version = commonDataService.GetFullName();
    }

    /// <summary>
    /// Gets or saves the app font sizes to a setting file.
    /// </summary>
    public FontOptions FontOptions { get; }

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
    public IAsyncRelayCommand OpenLinkCommand { get; }
}
