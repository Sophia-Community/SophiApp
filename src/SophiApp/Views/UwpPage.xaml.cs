// <copyright file="UWPPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="UWPPage"/> class.
/// </summary>
public sealed partial class UWPPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UWPPage"/> class.
    /// </summary>
    public UWPPage()
    {
        ViewModel = App.GetService<UWPViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="UWPViewModel"/>.
    /// </summary>
    public UWPViewModel ViewModel
    {
        get;
    }
}
