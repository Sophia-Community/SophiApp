// <copyright file="SecurityPage.xaml.cs" company="Team Sophia">
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
/// Implements the <see cref="SecurityPage"/> class.
/// </summary>
public sealed partial class SecurityPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SecurityPage"/> class.
    /// </summary>
    public SecurityPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Models = new(ViewModel.SettingModels.FilterByTag(UICategoryTag.Security));
    }

    /// <summary>
    /// Gets view model for security page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets the notes at the top of the page.
    /// </summary>
    public PageNotesViewModel Notes { get; } = App.GetService<PageNotesViewModel>();

    /// <summary>
    /// Gets the view model of the chosen settings.
    /// </summary>
    public ChosenSettingsViewModel Choices { get; } = App.GetService<ChosenSettingsViewModel>();

    /// <summary>
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public ObservableCollection<UIModel> Models { get; }
}
