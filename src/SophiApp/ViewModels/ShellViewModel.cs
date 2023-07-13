// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;

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
    /// <param name="appContextService"><see cref="IAppContextService"/>.</param>
    public ShellViewModel(INavigationService navigationService, INavigationViewService navigationViewService, IAppContextService appContextService)
    {
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        delimiter = appContextService.GetDelimiter();
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

        // TODO: Returns null, so the page is not in the history of pages viewed.

        //// if (e.SourcePageType == typeof(SettingsPage))
        //// {
        ////    Selected = NavigationViewService.SettingsItem;
        ////    return;
        //// }

        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);
        if (selectedItem != null)
        {
            Selected = selectedItem;
        }
    }
}
