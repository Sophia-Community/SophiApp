// <copyright file="SettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using SophiApp.Contracts.Services;

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
    private string version;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/>.</param>
    /// <param name="appContextService"><see cref="IAppContextService"/>.</param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService, IAppContextService appContextService)
    {
        this.themeSelectorService = themeSelectorService;

        elementTheme = this.themeSelectorService.Theme;
        version = appContextService.GetFullName();
        build = appContextService.GetBuildName();

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await this.themeSelectorService.SetThemeAsync(param);
                }
            });
    }

    /// <summary>
    /// Gets app theme switch command.
    /// </summary>
    public ICommand SwitchThemeCommand
    {
        get;
    }
}
