// <copyright file="PersonalizationPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.ViewModels;

/// <summary>
/// Implements the <see cref="PersonalizationPage"/> class.
/// </summary>
public sealed partial class PersonalizationPage : Page
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PersonalizationPage"/> class.
    /// </summary>
    public PersonalizationPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Items = ViewModel.SettingModels.FilterByTag(UICategoryTag.Personalization).WithGroupHeaders();
    }

    /// <summary>
    /// Gets view model for personalization page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets the notes at the top of the page.
    /// </summary>
    public PageNotesViewModel Notes { get; } = App.GetService<PageNotesViewModel>();

    /// <summary>
    /// Gets the settings of the page and the subtitles of their groups.
    /// </summary>
    public List<object> Items { get; }
}
