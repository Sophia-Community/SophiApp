// <copyright file="SecurityPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="SecurityPage"/> class.
/// </summary>
public sealed partial class SecurityPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityPage"/> class.
    /// </summary>
    public SecurityPage()
    {
        ViewModel = App.GetService<SecurityViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="UwpViewModel"/>.
    /// </summary>
    public SecurityViewModel ViewModel
    {
        get;
    }
}
