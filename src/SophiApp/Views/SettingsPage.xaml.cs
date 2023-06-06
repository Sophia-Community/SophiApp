// <copyright file="SettingsPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
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
        ViewModel = App.GetService<SettingsViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="SettingsViewModel"/>.
    /// </summary>
    public SettingsViewModel ViewModel
    {
        get;
    }
}
