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
using System.Threading;

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
    private readonly IProcessService processService;
    private readonly IAppxPackagesService packagesService;

    private List<UIModel> uwpAllUsersModels = new ();
    private List<UIModel> uwpCurrentUserModels = new ();

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

    [ObservableProperty]
    private bool uwpForAllUsersState = true;

    [ObservableProperty]
    private ObservableCollection<UIModel> uwpAppsModels = new ();

    private CancellationTokenSource? cancellationTokenSource;

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
    /// <param name="processService">A service for working with Windows process API.</param>
    /// <param name="packagesService">A service for working with appx packages API.</param>
    public ShellViewModel(
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        ICommonDataService commonDataService,
        IRequirementsService requirementsService,
        StartupViewModel startupViewModel,
        RequirementsFailureViewModel requirementsFailureViewModel,
        IModelService modelService,
        IProcessService processService,
        IAppxPackagesService packagesService)
    {
        this.requirementsService = requirementsService;
        this.processService = processService;
        this.packagesService = packagesService ?? throw new ArgumentNullException(nameof(packagesService));
        this.modelService = modelService;
        this.commonDataService = commonDataService;
        startupModel = startupViewModel;
        NavigationViewService = navigationViewService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        failureViewModel = requirementsFailureViewModel;
        delimiter = this.commonDataService.GetDelimiter();

        ApplicableModelsApply_Command = new AsyncRelayCommand(ApplicableModelsApplyAsync);
        ApplicableModelsCancel_Command = new RelayCommand(ApplicableModelsCancel);
        ApplicableModelsClear_Command = new AsyncRelayCommand(ApplicableModelsClearAsync);
        UIModelClicked_Command = new RelayCommand<UIModel>(model => UIModelClicked(model!));
        UIUwpAppModelClicked_Command = new RelayCommand<UIUwpAppModel>(model => UIUwpAppModelClicked(model!));
        UwpForAllUsersClicked_Command = new RelayCommand(UwpForAllUsersClicked);
    }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to click an "Cancel" button in the Apply Customizations Panel.
    /// </summary>
    public IAsyncRelayCommand ApplicableModelsClear_Command { get; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to click an "Apply" button in the Apply Customizations Panel.
    /// </summary>
    public IAsyncRelayCommand ApplicableModelsApply_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "Cancel" button in the Setup Customizations Panel.
    /// </summary>
    public IRelayCommand ApplicableModelsCancel_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an element in the interface.
    /// </summary>
    public IRelayCommand<UIModel> UIModelClicked_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "For all users" checkbox in the UWP page.
    /// </summary>
    public IRelayCommand UwpForAllUsersClicked_Command { get; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an <see cref="UIUwpAppModel"/> in the interface.
    /// </summary>
    public IRelayCommand<UIUwpAppModel> UIUwpAppModelClicked_Command { get; }

    /// <summary>
    /// Gets <see cref="INavigationService"/>.
    /// </summary>
    public INavigationService NavigationService { get; }

    /// <summary>
    /// Gets <see cref="INavigationViewService"/>.
    /// </summary>
    public INavigationViewService NavigationViewService { get; }

    /// <summary>
    /// Gets <see cref="UIModel"/> collection from "UIMarkup.json" file.
    /// </summary>
    public ConcurrentBag<UIModel> JsonModels { get; private set; } = [];

    /// <summary>
    /// Executes the ViewModel logic of the MVVM pattern.
    /// </summary>
    public async Task ExecuteAsync()
    {
        var numberOfRequirements = 16;
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
                startupModel.StatusText = "OsRequirements_GetEventLogState".GetLocalized();
            }))
            .Bind(requirementsService.GetEventLogState)
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GetMicrosoftStoreState".GetLocalized();
            }))
            .Bind(requirementsService.GetMicrosoftStoreState)
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
                startupModel.StatusText = "OsRequirements_GetSecuritySettingsPageState".GetLocalized();
            }))
            .Bind(requirementsService.GetWindowsSecurityState)
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
                var jsonModels = await modelService.BuildJsonModelsAsync();
                JsonModels = new (jsonModels);
                await modelService.GetStateAsync(JsonModels);
            })
            .Tap(() => App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                startupModel.ProgressBarValue = startupModel.ProgressBarValue.Increase(numberOfRequirements);
                startupModel.StatusText = "OsRequirements_GeneratingUserInterface".GetLocalized();
            }))
            .Tap(async () =>
            {
                uwpAllUsersModels = await modelService.BuildUwpAppModelsAsync(forAllUsers: true);
                uwpCurrentUserModels = await modelService.BuildUwpAppModelsAsync(forAllUsers: false);
                UwpAppsModels = new (uwpAllUsersModels);
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
        NavigationViewHitTestVisible = false;
        App.Logger.LogApplicableModelsCanceled();
        ProgressBarValue = 0;
        SetUpCustomizationsPanelText = "OsRequirements_ReadWindowsSettings".GetLocalized();
        SetUpCustomizationsPanelCancelButtonIsVisible = false;
        SetUpCustomizationsPanelIsVisible = true;
        var callback = new Action(() => App.MainWindow.DispatcherQueue.TryEnqueue(() => ProgressBarValue = ProgressBarValue.Increase(ApplicableModels.Count)));
        await modelService.GetStateAsync(ApplicableModels, callback);
        ApplicableModels.Clear();
        App.Logger.LogApplicableModelsClear();
        SetUpCustomizationsPanelIsVisible = false;
        NavigationViewHitTestVisible = true;
    }

    private async Task ApplicableModelsApplyAsync()
    {
        NavigationViewHitTestVisible = false;
        ProgressBarValue = 0;
        SetUpCustomizationsPanelText = "Panel_SetupCustomizations_Applying".GetLocalized();
        SetUpCustomizationsPanelCancelButtonIsVisible = true;
        SetUpCustomizationsPanelIsVisible = true;
        cancellationTokenSource = new CancellationTokenSource();
        var callback = new Action(() => App.MainWindow.DispatcherQueue.TryEnqueue(() => ProgressBarValue = ProgressBarValue.Increase(ApplicableModels.Count)));
        await modelService.SetStateAsync(ApplicableModels, callback, cancellationTokenSource.Token);
        cancellationTokenSource.Dispose();
        ProgressBarValue = 0;
        SetUpCustomizationsPanelText = "OsRequirements_ReadWindowsSettings".GetLocalized();
        SetUpCustomizationsPanelCancelButtonIsVisible = false;
        await modelService.GetStateAsync(ApplicableModels, callback);
        ApplicableModels.Clear();
        App.Logger.LogApplicableModelsClear();
        EnvironmentHelper.RefreshUserDesktop();
        EnvironmentHelper.ForcedRefresh();
        processService.KillAllProcesses("StartMenuExperienceHost");
        processService.KillAllProcesses("explorer");
        SetUpCustomizationsPanelIsVisible = false;
        NavigationViewHitTestVisible = true;
    }

    private void ApplicableModelsCancel()
    {
        SetUpCustomizationsPanelCancelButtonIsVisible = false;
        cancellationTokenSource?.Cancel();
    }

    private void UIModelClicked(UIModel model)
    {
        if (ApplicableModels.Contains(model))
        {
            ApplicableModels.Remove(model);
            App.Logger.LogApplicableModelRemoved(model.Name);
            return;
        }

        ApplicableModels.Add(model);
        App.Logger.LogApplicableModelAdded(model.Name);
    }

    private void UwpForAllUsersClicked()
    {
        NavigationViewHitTestVisible = false;
        App.Logger.LogUwpForAllUsersState(UwpForAllUsersState);
        UwpForAllUsersState = !UwpForAllUsersState;
        UwpAppsModels = new ObservableCollection<UIModel>(UwpForAllUsersState ? uwpAllUsersModels : uwpCurrentUserModels);
        NavigationViewHitTestVisible = true;
    }

    private void UIUwpAppModelClicked(UIUwpAppModel model)
    {
        model.ForAllUsers = UwpForAllUsersState;
        model.Mutator = (packageFullName, removeForAll) => packagesService.RemovePackage(packageName: model.Title, forAllUsers: model.ForAllUsers);
        UIModelClicked(model);
    }
}
