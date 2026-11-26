// <copyright file="SettingsPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Helpers;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="SettingsPage"/> class.
/// </summary>
public sealed partial class SettingsPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SettingsPage"/> class.
    /// </summary>
    public SettingsPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<SettingsViewModel>();
    }

    /// <summary>
    /// Gets view model for settings page.
    /// </summary>
    public SettingsViewModel ViewModel { get; }

    /// <summary>
    /// Gets the view model of the chosen settings: it creates restore points.
    /// </summary>
    public ChosenSettingsViewModel Choices { get; } = App.GetService<ChosenSettingsViewModel>();

    // The list opens under the mouse as a separate window, like a menu
    private void ThemeSelector_DropDownOpened(object? sender, object e) => PopupCursor.Reset();
}
