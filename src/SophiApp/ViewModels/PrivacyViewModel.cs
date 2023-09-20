// <copyright file="PrivacyViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.ObjectModel;

/// <summary>
/// Implements the <see cref="PrivacyViewModel"/> class.
/// </summary>
public partial class PrivacyViewModel : ObservableRecipient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PrivacyViewModel"/> class.
    /// </summary>
    public PrivacyViewModel()
    {
        Models = new ObservableCollection<UIModel>(App.GetService<IModelBuilderService>()
            .GetModels(UICategoryTag.Privacy));
    }

    /// <summary>
    /// Gets <see cref="UIModel"/> collections.
    /// </summary>
    public ObservableCollection<UIModel> Models { get; }
}
