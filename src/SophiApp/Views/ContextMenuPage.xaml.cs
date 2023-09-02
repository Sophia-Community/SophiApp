// <copyright file="ContextMenuPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="ContextMenuPage"/> class.
/// </summary>
public sealed partial class ContextMenuPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ContextMenuPage"/> class.
    /// </summary>
    public ContextMenuPage()
    {
        ViewModel = App.GetService<ContextMenuViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="ContextMenuViewModel"/>.
    /// </summary>
    public ContextMenuViewModel ViewModel
    {
        get;
    }
}
