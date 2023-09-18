// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Services;
using SophiApp.Views;

/// <summary>
/// Implements the <see cref="ShellViewModel"/> class.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="navigationService"><see cref="INavigationService"/>.</param>
    /// <param name="navigationViewService"><see cref="INavigationViewService"/>.</param>
    /// <param name="commonDataService"><see cref="ICommonDataService"/>.</param>
    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService, ICommonDataService commonDataService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        delimiter = commonDataService.GetDelimiter();
    }

    public INavigationService NavigationService
    {
        get;
    }

    public INavigationViewService NavigationViewService
    {
        get;
    }

    private void OnNavigated(object sender, NavigationEventArgs e)
    {
        IsBackEnabled = NavigationService.CanGoBack;

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
