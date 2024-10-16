// <copyright file="ProVersionPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="ProVersionPage"/> class.
/// </summary>
public sealed partial class ProVersionPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ProVersionPage"/> class.
    /// </summary>
    public ProVersionPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
    }

    /// <summary>
    /// Gets view model for pro version page.
    /// </summary>
    public ShellViewModel ViewModel { get; }
}
