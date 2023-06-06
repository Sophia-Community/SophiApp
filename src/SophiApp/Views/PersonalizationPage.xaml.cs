// <copyright file="PersonalizationPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;

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
        ViewModel = App.GetService<PersonalizationViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="PersonalizationViewModel"/>.
    /// </summary>
    public PersonalizationViewModel ViewModel
    {
        get;
    }
}
