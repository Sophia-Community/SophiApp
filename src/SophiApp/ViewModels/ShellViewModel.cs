// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CSharpFunctionalExtensions;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;

/// <summary>
/// Implements the <see cref="ShellViewModel"/> class.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    private readonly StartupViewModel startupVM;
    private readonly IRequirementsService requirementsService;
    private readonly ICommonDataService commonDataService;

    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    [ObservableProperty]
    private bool navigationViewHitTestVisible = false;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="navigationService">Page navigation service.</param>
    /// <param name="navigationViewService">A service for navigating to View.</param>
    /// <param name="commonDataService">A service for working with common app data.</param>
    /// <param name="startupVM">Implements the <see cref="StartupViewModel"/> class.</param>
    /// <param name="requirementsService">Service for working with OS requirements.</param>
    public ShellViewModel(
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        ICommonDataService commonDataService,
        StartupViewModel startupVM,
        IRequirementsService requirementsService)
    {
        NavigationService = navigationService;
        this.commonDataService = commonDataService;
        NavigationService.Navigated += OnNavigated;
        NavigationViewService = navigationViewService;
        delimiter = this.commonDataService.GetDelimiter();
        this.startupVM = startupVM;
        this.requirementsService = requirementsService;
    }

    /// <summary>
    /// Gets <see cref="INavigationService"/>.
    /// </summary>
    public INavigationService NavigationService
    {
        get;
    }

    /// <summary>
    /// Gets <see cref="INavigationViewService"/>.
    /// </summary>
    public INavigationViewService NavigationViewService
    {
        get;
    }

    /// <summary>
    /// Executes the ViewModel logic of the MVVM pattern.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var numberOfChecks = 20;

        await Task.Run(() =>
        {
            Result.Try(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupVM.StatusText = "OsRequirements_DetectWmiState".GetLocalized();
                NavigationService.NavigateTo(typeof(StartupViewModel).FullName!);
            }))
            .Bind(_ => requirementsService.GetWmiState())
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupVM.ProgressBarValue = startupVM.ProgressBarValue.PartialIncrease(numberOfChecks);
                startupVM.StatusText = "OsRequirements_DetectOsVersion".GetLocalized();
            }))
            .Bind(() => requirementsService.GetOsVersion())
            .Match(
                onSuccess: () => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    NavigationViewHitTestVisible = true;
                    NavigationService.NavigateTo(pageKey: typeof(PrivacyViewModel).FullName!, clearNavigation: true);
                }),
                onFailure: pageKey => App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(pageKey)));
        });
    }

    /// <summary>
    /// Handles the navigation event of a menu item.
    /// </summary>
    /// <param name="sender">An object is the source of an event.</param>
    /// <param name="e">Provides data for navigation methods and event handlers that cannot cancel the navigation request.</param>
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
