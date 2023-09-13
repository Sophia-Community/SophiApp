// <copyright file="UwpPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="UwpPage"/> class.
/// </summary>
public sealed partial class UwpPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UwpPage"/> class.
    /// </summary>
    public UwpPage()
    {
        ViewModel = App.GetService<UwpViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="UwpViewModel"/>.
    /// </summary>
    public UwpViewModel ViewModel
    {
        get;
    }
}
