// <copyright file="SettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
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
    private string buildName;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private string versionDescription;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themeSelectorService"><inheritdoc/></param>
    public SettingsViewModel(IThemeSelectorService themeSelectorService)
    {
        this.themeSelectorService = themeSelectorService;
        buildName = "Daria";
        elementTheme = this.themeSelectorService.Theme;
        versionDescription = "SophiApp |";

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
