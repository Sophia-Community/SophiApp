// <copyright file="SystemPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="SystemPage"/> class.
/// </summary>
public sealed partial class SystemPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SystemPage"/> class.
    /// </summary>
    public SystemPage()
    {
        ViewModel = App.GetService<SystemViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="SystemViewModel"/>.
    /// </summary>
    public SystemViewModel ViewModel
    {
        get;
    }
}
