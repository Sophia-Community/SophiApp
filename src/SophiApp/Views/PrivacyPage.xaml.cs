﻿// <copyright file="PrivacyPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml;
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
    private readonly int modelMaxViewId;

    /// <summary>
    /// Initializes a new instance of the <see cref="PrivacyPage"/> class.
    /// </summary>
    public PrivacyPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        Models = ViewModel.JsonModels.FilterByTag(UICategoryTag.Privacy);
        modelMaxViewId = Models.Max(m => m.ViewId);
    }

    /// <summary>
    /// Gets view model for privacy page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public List<UIModel> Models { get; }

    /// <summary>
    /// Correct the vertical offset so that the last <see cref="FrameworkElement"/> in the sequence fits on the UI.
    /// </summary>
    public void CorrectScrollViewPosition()
    {
        if (ViewModel.ApplicableModels[0].ViewId == modelMaxViewId)
        {
            this.FindName<ScrollView>("PrivacyScrollView")?.VerticalOffsetCorrection();
        }
    }
}