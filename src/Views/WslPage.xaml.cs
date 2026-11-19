// <copyright file="WslPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.ViewModels;
using System.Collections.ObjectModel;

/// <summary>
/// Implements the <see cref="WslPage"/> class.
/// </summary>
public sealed partial class WslPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WslPage"/> class.
    /// </summary>
    public WslPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Models = new(ViewModel.JsonModels.FilterByTag(UICategoryTag.WSL));
    }

    /// <summary>
    /// Gets view model for WSL page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public ObservableCollection<UIModel> Models { get; }
}
