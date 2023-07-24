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
    private readonly IUriService uriService;

    [ObservableProperty]
    private string build;

    [ObservableProperty]
    private ElementTheme elementTheme;

    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private string version;

    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
    /// </summary>
    /// <param name="themeSelectorService"><see cref="IThemeSelectorService"/>.</param>
    /// <param name="appContextService"><see cref="IAppContextService"/>.</param>
    /// <param name="uriService"><see cref="IUriService"/>.</param>
    public SettingsViewModel(
        IThemeSelectorService themeSelectorService, IAppContextService appContextService, IUriService uriService)
    {
        this.themeSelectorService = themeSelectorService;
        this.uriService = uriService;
        elementTheme = this.themeSelectorService.Theme;
        delimiter = appContextService.GetDelimiter();
        version = appContextService.GetFullName();
        build = appContextService.GetBuildName();
        InitializeCommand();
    }

    /// <summary>
    /// Gets app theme switch command.
    /// </summary>
    public ICommand? SwitchThemeCommand { get; private set; }

    /// <summary>
    /// Gets a resource using an identifier.
    /// </summary>
    public ICommand OpenUriCommand { get; private set; }

    private void InitializeCommand()
    {
        SwitchThemeCommand = new RelayCommand<ElementTheme>(async (param) =>
        {
            if (ElementTheme != param)
            {
                ElementTheme = param;
                await this.themeSelectorService.SetThemeAsync(param);
            }
        });

        OpenUriCommand = new AsyncRelayCommand<string?>((param) => uriService.OpenUri(param));
    }
}
