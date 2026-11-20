// <copyright file="UwpPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
        GamingModels = new(ViewModel.JsonModels.FilterByTag(UICategoryTag.Gaming));

        // Windows 11 LTSC 2024 has no UWP apps to uninstall
        UwpAppsPanel.Visibility = App.GetService<ICommonDataService>().OsProperties.IsLTSC ? Visibility.Collapsed : Visibility.Visible;
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
        get
        {
            return currentWidth;
        }

        private set
        {
            currentWidth = value;
            OnPropertyChanged(nameof(CurrentWidth));
        }
    }

    /// <summary>
    /// Gets a gaming <see cref="UIModel"/> collection.
    /// </summary>
    public ObservableCollection<UIModel> GamingModels { get; }

    private static string GetChosenAppsText(int count) => string.Format(CultureInfo.CurrentCulture, "UwpPage_ChosenApps".GetLocalized(), count);

    private static bool HasChosenApps(int count) => count > 0;

    private void PageUwp_SizeChanged(object sender, SizeChangedEventArgs e) => CurrentWidth = ActualWidth;

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
