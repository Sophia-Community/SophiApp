// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CSharpFunctionalExtensions;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;
using System;

/// <summary>
/// Implements the <see cref="ShellViewModel"/> class.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    private readonly StartupViewModel startupModel;
    private readonly RequirementsFailureViewModel requirementsFailureModel;
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
    /// <param name="requirementsService">Service for working with OS requirements.</param>
    /// <param name="startupViewModel">Implements the <see cref="StartupViewModel"/> class.</param>
    /// <param name="requirementsFailureViewModel">Implements the <see cref="RequirementsFailureViewModel"/> class.</param>
    public ShellViewModel(
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        ICommonDataService commonDataService,
        IRequirementsService requirementsService,
        StartupViewModel startupViewModel,
        RequirementsFailureViewModel requirementsFailureViewModel)
    {
        NavigationService = navigationService;
        this.commonDataService = commonDataService;
        this.requirementsService = requirementsService;
        NavigationViewService = navigationViewService;
        NavigationService.Navigated += OnNavigated;
        delimiter = this.commonDataService.GetDelimiter();
        this.startupModel = startupViewModel;
        this.requirementsFailureModel = requirementsFailureViewModel;
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
        var numberOfRequirements = 20;

        await Task.Run(() =>
        {
            Result.Try(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.StatusText = "OsRequirements_GetBitness".GetLocalized();
                NavigationService.NavigateTo(typeof(StartupViewModel).FullName!);
            }))
            .Bind(_ => requirementsService.GetOsBitness())
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetWmiState".GetLocalized();
            }))
            .Bind(requirementsService.GetWmiState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetVersion".GetLocalized();
            }))
            .Bind(requirementsService.GetOsVersion)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetUserRights".GetLocalized();
            }))
            .Bind(requirementsService.HasAdminRights)
            .Match(
                onSuccess: () => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    NavigationViewHitTestVisible = true;
                    NavigationService.NavigateTo(pageKey: typeof(PrivacyViewModel).FullName!, clearNavigation: true);
                }),
                onFailure: failure =>
                {
                    var reason = failure.ToEnum<RequirementsFailure>();
                    var reasonText = GetLocalizedString(reason);
                    var needUpdate = NeedUpdate(reason);
                    requirementsFailureModel.PrepareForNavigate(reasonText, needUpdate);
                    App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(typeof(RequirementsFailureViewModel).FullName!));
                });
        });
    }

    private string GetLocalizedString(RequirementsFailure reason)
    {
        return reason switch
        {
            RequirementsFailure.Is32BitOs => "OsRequirements_Failure_IsX32".GetLocalized(),
            RequirementsFailure.WMIBroken => "OsRequirements_Failure_WmiBroken".GetLocalized(),
            RequirementsFailure.Win11BuildLess22k => "OsRequirements_Failure_Win11UnsupportedBuild".GetLocalized(),
            RequirementsFailure.Win11BuildEqual22k => string.Format("OsRequirements_Failure_Win11UnsupportedVersion".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win11UBRLess2283 => string.Format("OsRequirements_Failure_Win11UnsupportedVersion".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win10UBRLess3448 => string.Format("OsRequirements_Failure_Win10UnsupportedVersion".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win10WrongBuild => "OsRequirements_Failure_Win10UnsupportedBuild".GetLocalized(),
            RequirementsFailure.Win10LTSC2k19 => "OsRequirements_Failure_Win10Ltsc2k19".GetLocalized(),
            RequirementsFailure.Win10LTSC2k21 => "OsRequirements_Failure_Win10Ltsc2k21".GetLocalized(),
            RequirementsFailure.Win10BuildEquals19044 => string.Format("OsRequirements_Failure_Win10UnsupportedVersion".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.UserIsNotAdmin => "OsRequirements_Failure_UserIsNotAdmin".GetLocalized(),
            _ => throw new ArgumentOutOfRangeException(paramName: reason.ToString(), message: $"Value: {reason} is not found in {typeof(RequirementsFailure).Name} enumeration.")
        };
    }

    private bool NeedUpdate(RequirementsFailure reason)
    {
        switch (reason)
        {
            case RequirementsFailure.Is32BitOs:
            case RequirementsFailure.WMIBroken:
            case RequirementsFailure.Win11BuildLess22k:
            case RequirementsFailure.Win10WrongBuild:
            case RequirementsFailure.Win10LTSC2k19:
            case RequirementsFailure.Win10LTSC2k21:
            case RequirementsFailure.UserIsNotAdmin:
                return false;

            case RequirementsFailure.Win11BuildEqual22k:
            case RequirementsFailure.Win11UBRLess2283:
            case RequirementsFailure.Win10UBRLess3448:
            case RequirementsFailure.Win10BuildEquals19044:
                return true;

            default:
                throw new ArgumentOutOfRangeException(paramName: nameof(reason), message: $"Value: {reason} is not found in {typeof(RequirementsFailure).Name} enumeration.");
        }
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
