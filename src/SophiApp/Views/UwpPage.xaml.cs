// <copyright file="UwpPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml.Controls;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;

/// <summary>
/// Implements the <see cref="UwpPage"/> class.
/// </summary>
public sealed partial class UwpPage : Page, INotifyPropertyChanged
{
    private double currentWidth = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="UwpPage"/> class.
    /// </summary>
    public UwpPage()
    {
        InitializeComponent();
        ViewModel = App.GetService<ShellViewModel>();
        GamingModels = ViewModel.JsonModels.FilterByTag(UICategoryTag.Gaming);
        UWPModels = ViewModel.JsonModels.FilterByTag(UICategoryTag.UWP);
    }

    /// <summary>
    /// Property change event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets view model for UWP page.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets a value indicating current width.
    /// </summary>
    public double CurrentWidth
    {
        get => currentWidth;
        private set
        {
            currentWidth = value;
            OnPropertyChanged(nameof(CurrentWidth));
        }
    }

    /// <summary>
    /// Gets a gaming <see cref="UIModel"/> collection.
    /// </summary>
    public List<UIModel> GamingModels { get; }

    /// <summary>
    /// Gets a uwp miscellaneous <see cref="UIModel"/> collection.
    /// </summary>
    public List<UIModel> UWPModels { get; }

    private void PageUwp_SizeChanged(object sender, Microsoft.UI.Xaml.SizeChangedEventArgs e)
    {
        CurrentWidth = ActualWidth;
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
