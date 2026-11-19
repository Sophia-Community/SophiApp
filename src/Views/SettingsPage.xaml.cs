// <copyright file="SettingsPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
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

    // The list opens under the mouse as a separate window, like a menu
    private void ThemeSelector_DropDownOpened(object? sender, object e) => PopupCursor.Reset();

    private void SettingsExpander_Loaded(object sender, RoutedEventArgs e)
        => SettingsExpanderHelper.ApplyHeaderPadding((SettingsExpander)sender);
}
