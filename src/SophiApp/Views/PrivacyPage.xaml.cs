// <copyright file="PrivacyPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;
using Microsoft.UI.Xaml.Controls;

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
        ViewModel = App.GetService<PrivacyViewModel>();
        InitializeComponent();
    }

    /// <summary>
    /// Gets <see cref="UwpViewModel"/>.
    /// </summary>
    public PrivacyViewModel ViewModel
    {
        get;
    }
}
