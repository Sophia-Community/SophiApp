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
using SophiApp.Models;
using System;
using System.Collections.Concurrent;

/// <summary>
/// Implements the <see cref="ShellViewModel"/> class.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    private readonly StartupViewModel startupModel;
    private readonly RequirementsFailureViewModel failureViewModel;
    private readonly IRequirementsService requirementsService;
    private readonly ICommonDataService commonDataService;
    private readonly IModelService modelService;

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
    /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
    public ShellViewModel(
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        ICommonDataService commonDataService,
        IRequirementsService requirementsService,
        StartupViewModel startupViewModel,
        RequirementsFailureViewModel requirementsFailureViewModel,
        IModelService modelService)
    {
        NavigationService = navigationService;
        this.commonDataService = commonDataService;
        this.requirementsService = requirementsService;
        NavigationViewService = navigationViewService;
        NavigationService.Navigated += OnNavigated;
        delimiter = this.commonDataService.GetDelimiter();
        startupModel = startupViewModel;
        failureViewModel = requirementsFailureViewModel;
        this.modelService = modelService;
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
    /// Gets <see cref="UIModel"/> collection.
    /// </summary>
    public ConcurrentBag<UIModel> Models { get; private set; } = new ();

    /// <summary>
    /// Executes the ViewModel logic of the MVVM pattern.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var numberOfRequirements = 12;

        await Task.Run(() =>
        {
            Result.Try(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.StatusText = "OsRequirements_GetOsBitness".GetLocalized();
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
                startupModel.StatusText = "OsRequirements_GetOsVersion".GetLocalized();
            }))
            .Bind(requirementsService.GetOsVersion)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_AppRunFromLoggedUser".GetLocalized();
            }))
            .Bind(requirementsService.AppRunFromLoggedUser)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_MalwareDetection".GetLocalized();
            }))
            .Bind(requirementsService.MalwareDetection)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetFeatureExperiencePackState".GetLocalized();
            }))
            .Bind(requirementsService.GetFeatureExperiencePackState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetPendingRebootState".GetLocalized();
            }))
            .Bind(requirementsService.GetPendingRebootState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_UpdateDetection".GetLocalized();
            }))
            .Bind(requirementsService.AppUpdateDetection)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderFilesExist".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderFilesExist)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderServicesState".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderServicesState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderState".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.PartialIncrease(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_ReadWindowsSettings".GetLocalized();
            }))
            .Tap(async () =>
            {
                var models = modelService.BuildModels();
                Models = new (models);
                await modelService.GetStateAsync(Models);
            })
            .Match(
                onSuccess: () => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
                {
                    NavigationViewHitTestVisible = true;
                    NavigationService.NavigateTo(pageKey: typeof(PrivacyViewModel).FullName!, clearNavigation: true);
                }),
                onFailure: failure =>
                {
                    var failureReason = failure.ToEnum<RequirementsFailure>();
                    var reason = GetFailureReason(failureReason);
                    var needUpdate = IsNeedUpdate(failureReason);
                    App.Logger.LogNavigateToRequirementsFailure(failureReason);
                    failureViewModel.PrepareForNavigation(reason, needUpdate);
                    App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(typeof(RequirementsFailureViewModel).FullName!));
                });
        });
    }

    private string GetFailureReason(RequirementsFailure reason)
    {
        return reason switch
        {
            RequirementsFailure.Is32BitOs => "OsRequirementsFailure_Is32BitOs".GetLocalized(),
            RequirementsFailure.WMIBroken => "OsRequirementsFailure_WmiBroken".GetLocalized(),
            RequirementsFailure.Win11BuildLess22631 => string.Format("OsRequirementsFailure_Win11UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win11UbrLess2283 => string.Format("OsRequirementsFailure_Win11UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win10EnterpriseSVersion => "OsRequirementsFailure_Win10EnterpriseSVersion".GetLocalized(),
            RequirementsFailure.Win10UnsupportedBuild => string.Format("OsRequirementsFailure_Win10UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.Win10UpdateBuildRevisionLess3448 => string.Format("OsRequirementsFailure_Win10UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
            RequirementsFailure.RunByNotLoggedUser => "OsRequirementsFailure_RunByNotLoggedUser".GetLocalized(),
            RequirementsFailure.MalwareDetected => string.Format("OsRequirementsFailure_MalwareDetected".GetLocalized(), commonDataService.DetectedMalware),
            RequirementsFailure.FeatureExperiencePackRemoved => "OsRequirementsFailure_FeatureExperiencePackRemoved".GetLocalized(),
            RequirementsFailure.RebootRequired => "OsRequirementsFailure_RebootRequired".GetLocalized(),
            RequirementsFailure.MsDefenderFilesMissing => string.Format("OsRequirementsFailure_MsDefenderFilesMissing".GetLocalized(), commonDataService.MsDefenderFileMissing),
            RequirementsFailure.MsDefenderServiceStopped => commonDataService.MsDefenderServiceStopped.GetLocalized(),
            RequirementsFailure.MsDefenderIsBroken => "OsRequirementsFailure_MsDefenderIsBroken".GetLocalized(),
            _ => throw new ArgumentOutOfRangeException(paramName: nameof(reason), message: $"Value: {reason} is not found in {typeof(RequirementsFailure).FullName} enumeration.")
        };
    }

    private bool IsNeedUpdate(RequirementsFailure reason)
    {
        switch (reason)
        {
            case RequirementsFailure.Win11BuildLess22631:
            case RequirementsFailure.Win11UbrLess2283:
            case RequirementsFailure.Win10UnsupportedBuild:
                return true;

            default:
                return false;
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
