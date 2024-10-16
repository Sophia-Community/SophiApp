// <copyright file="PrivacyPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="PrivacyPage"/> class.
/// </summary>
public sealed partial class PrivacyPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivacyPage"/> class.
    /// </summary>
    public PrivacyPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Models = ViewModel.JsonModels.FilterByTag(UICategoryTag.Privacy);
    }

    /// <summary>
    /// Gets view model for privacy page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public List<UIModel> Models { get; }
}
