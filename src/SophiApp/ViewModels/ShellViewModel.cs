// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CSharpFunctionalExtensions;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Diagnostics;

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
    private ObservableCollection<UIModel> applicableModels = new ();

    [ObservableProperty]
    private string delimiter;

    [ObservableProperty]
    private bool isBackEnabled;

    [ObservableProperty]
    private object? selected;

    [ObservableProperty]
    private bool navigationViewHitTestVisible = false;

    [ObservableProperty]
    private bool setUpCustomizationsPanelIsVisible = false;

    [ObservableProperty]
    private bool setUpCustomizationsPanelCancelButtonIsVisible = false;

    [ObservableProperty]
    private string setUpCustomizationsPanelText = string.Empty;

    [ObservableProperty]
    private int progressBarValue = 0;

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

        ApplicableModelsClear_Command = new AsyncRelayCommand(ApplicableModelsClearAsync);
        UIModelClicked_Command = new RelayCommand<UIModel>(model => UIModelClicked(model!));
    }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "Cancel" button in the "Apply Customizations" panel.
    /// </summary>
    public IAsyncRelayCommand ApplicableModelsClear_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an element in the interface.
    /// </summary>
    public IRelayCommand<UIModel> UIModelClicked_Command { get; }

    /// <summary>
    /// Gets <see cref="INavigationService"/>.
    /// </summary>
    public INavigationService NavigationService { get; }

    /// <summary>
    /// Gets <see cref="INavigationViewService"/>.
    /// </summary>
    public INavigationViewService NavigationViewService { get; }

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
            _ = Result.Try(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.StatusText = "OsRequirements_GetOsBitness".GetLocalized();
                _ = NavigationService.NavigateTo(typeof(StartupViewModel).FullName!);
            }))
            .Bind(_ => requirementsService.GetOsBitness())
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetWmiState".GetLocalized();
            }))
            .Bind(requirementsService.GetWmiState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetOsVersion".GetLocalized();
            }))
            .Bind(requirementsService.GetOsVersion)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_AppRunFromLoggedUser".GetLocalized();
            }))
            .Bind(requirementsService.AppRunFromLoggedUser)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_MalwareDetection".GetLocalized();
            }))
            .Bind(requirementsService.MalwareDetection)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetFeatureExperiencePackState".GetLocalized();
            }))
            .Bind(requirementsService.GetFeatureExperiencePackState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetPendingRebootState".GetLocalized();
            }))
            .Bind(requirementsService.GetPendingRebootState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_UpdateDetection".GetLocalized();
            }))
            .Bind(requirementsService.AppUpdateDetection)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderFilesExist".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderFilesExist)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderServicesState".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderServicesState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMsDefenderState".GetLocalized();
            }))
            .Bind(requirementsService.GetMsDefenderState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
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
                    _ = NavigationService.NavigateTo(pageKey: typeof(PrivacyViewModel).FullName!, clearNavigation: true);
                }),
                onFailure: failure =>
                {
                    var failureReason = failure.ToEnum<RequirementsFailure>();
                    App.Logger.LogNavigateToRequirementsFailure(failureReason);
                    failureViewModel.PrepareForNavigation(failureReason);
                    App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(typeof(RequirementsFailureViewModel).FullName!));
                });
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

    private async Task ApplicableModelsClearAsync()
    {
        await Task.Run(() =>
        {
            _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                NavigationViewHitTestVisible = false;
                ProgressBarValue = 0;
                SetUpCustomizationsPanelText = "OsRequirements_ReadWindowsSettings".GetLocalized();
                SetUpCustomizationsPanelCancelButtonIsVisible = false;
                SetUpCustomizationsPanelIsVisible = true;
            });

            ApplicableModels.ForEach(model =>
            {
                var timer = Stopwatch.StartNew();
                model.GetState();
                timer.Stop();
                App.Logger.LogModelRefreshState(model.Name, timer);
                _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => ProgressBarValue = ProgressBarValue.Increase(ApplicableModels.Count));
            });

            _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ApplicableModels.Clear();
                App.Logger.LogApplicableModelsClear();
                SetUpCustomizationsPanelIsVisible = false;
                NavigationService.NavigateTo(pageKey: NavigationService.LastVmUsed, ignorePageType: true);
                NavigationViewHitTestVisible = true;
            });
        });
    }

    private void UIModelClicked(UIModel model)
    {
        if (applicableModels.Contains(model))
        {
            applicableModels.Remove(model);
            App.Logger.LogApplicableModelRemoved(model.Name);
            return;
        }

        applicableModels.Add(model);
        App.Logger.LogApplicableModelAdded(model.Name);
    }
}
