// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.Extensions;
using SophiApp.Models;
using SophiApp.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// The view model of the app window: the menu, the settings read from Windows and the UWP apps.
/// The start of the app is in <see cref="StartupViewModel"/>, the chosen settings are in <see cref="ChosenSettingsViewModel"/>,
/// and the search is in <see cref="SearchViewModel"/>.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    private const string AppDeveloperUrl = "https://github.com/Inestic";
    private const string AppSupporterUrl = "https://github.com/BenchTweakGaming";
    private const string AppDiscordUrl = "https://discord.gg/sSryhaEv79";
    private const string AppGitHubUrl = "https://github.com/Sophia-Community/SophiApp";
    private const string AppLatestReleaseUrl = "https://github.com/Sophia-Community/SophiApp/releases/latest";
    private const string AppProjectManagerUrl = "https://github.com/farag2";
    private const string AppTelegramUrl = "https://t.me/sophia_chat";
    private const string AppTesterUrl = "https://github.com/lowl1f3";
    private const string AppUxDeveloperUrl = "https://www.linkedin.com/in/vladimir-palii-132745a1";
    private readonly AppActivity activity;
    private readonly ChosenSettingsViewModel chosenSettings;
    private readonly IAppxPackagesService packagesService;
    private readonly IGroupPolicyService groupPolicyService;
    private readonly IModelService modelService;
    private readonly IRegistryService registryService;
    private readonly StartupViewModel startup;
    private List<UIModel> uwpAllUsersModels = [];
    private List<UIModel> uwpCurrentUserModels = [];

    [ObservableProperty]
    private bool isBackEnabled;
    [ObservableProperty]
    private bool uwpForAllUsersState = true;

    [ObservableProperty]
    private object? selectedNavigationViewItem;
    [ObservableProperty]
    private ObservableCollection<UIModel> uwpAppsModels = [];

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="activity">What the app is doing now.</param>
    /// <param name="chosenSettings">The settings the user chose to change.</param>
    /// <param name="groupPolicyService">A service for working with group policy API.</param>
    /// <param name="httpService">A service for working with HTTP API.</param>
    /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
    /// <param name="navigationService">Page navigation service.</param>
    /// <param name="navigationViewService">A service for navigating to View.</param>
    /// <param name="packagesService">A service for working with appx packages API.</param>
    /// <param name="processService">A service for working with Windows process API.</param>
    /// <param name="registryService">A service for working with Windows registry API.</param>
    /// <param name="startup">The start of the app.</param>
    public ShellViewModel(
        AppActivity activity,
        ChosenSettingsViewModel chosenSettings,
        IAppxPackagesService packagesService,
        IGroupPolicyService groupPolicyService,
        IHttpService httpService,
        IModelService modelService,
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        IProcessService processService,
        IRegistryService registryService,
        StartupViewModel startup)
    {
        this.activity = activity;
        this.chosenSettings = chosenSettings;
        this.groupPolicyService = groupPolicyService;
        this.modelService = modelService;
        this.packagesService = packagesService;
        this.registryService = registryService;
        this.startup = startup;
        NavigationViewService = navigationViewService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;
        activity.PropertyChanged += OnActivityChanged;
        OpenAppDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppDeveloperUrl));
        OpenAppSupporterPage_Command = new RelayCommand(() => httpService.OpenUrl(AppSupporterUrl));
        OpenAppDiscordPage_Command = new RelayCommand(() => httpService.OpenUrl(AppDiscordUrl));
        OpenAppGitHubPage_Command = new RelayCommand(() => httpService.OpenUrl(AppGitHubUrl));
        OpenAppProjectManagerPage_Command = new RelayCommand(() => httpService.OpenUrl(AppProjectManagerUrl));
        OpenAppTelegramPage_Command = new RelayCommand(() => httpService.OpenUrl(AppTelegramUrl));
        OpenAppTesterPage_Command = new RelayCommand(() => httpService.OpenUrl(AppTesterUrl));
        OpenAppUxDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppUxDeveloperUrl));
        OpenEventViewer_Command = new AsyncRelayCommand(() => Task.Run(() => processService.StartProcessByName(name: "mmc.exe", arguments: "eventvwr.msc")));
        OpenTaskScheduler_Command = new AsyncRelayCommand(() => Task.Run(() => processService.StartProcessByName(name: "control.exe", arguments: "schedtasks")));
        OpenLatestAppRelease_Command = new RelayCommand(() => httpService.OpenUrl(AppLatestReleaseUrl));
        UIUwpAppModelClicked_Command = new RelayCommand<UIUwpAppModel>(model => UIUwpAppModelClicked(model!));
        UwpForAllUsersClicked_Command = new RelayCommand(UwpForAllUsersClicked);
        UwpAppsUninstall_Command = new AsyncRelayCommand(UwpAppsUninstallAsync);
    }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppDeveloperPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open the page of the person who supports the app.
    /// </summary>
    public IRelayCommand OpenAppSupporterPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app Discord page.
    /// </summary>
    public IRelayCommand OpenAppDiscordPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app GitHub page.
    /// </summary>
    public IRelayCommand OpenAppGitHubPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app project manager contacts page.
    /// </summary>
    public IRelayCommand OpenAppProjectManagerPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app Telegram page.
    /// </summary>
    public IRelayCommand OpenAppTelegramPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app tester contacts page.
    /// </summary>
    public IRelayCommand OpenAppTesterPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app UX developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUxDeveloperPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to click an "Open" button in the Task Scheduler page.
    /// </summary>
    public IAsyncRelayCommand OpenTaskScheduler_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to open Event Viewer.
    /// </summary>
    public IAsyncRelayCommand OpenEventViewer_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open latest app release URL.
    /// </summary>
    public IRelayCommand OpenLatestAppRelease_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an <see cref="UIUwpAppModel"/> in the interface.
    /// </summary>
    public IRelayCommand<UIUwpAppModel> UIUwpAppModelClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "For all users" checkbox in the UWP page.
    /// </summary>
    public IRelayCommand UwpForAllUsersClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to uninstall the UWP apps chosen in the UWP page.
    /// </summary>
    public IAsyncRelayCommand UwpAppsUninstall_Command { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the user can use the menu and the settings: the app is not starting, reading or applying anything.
    /// </summary>
    public bool NavigationViewHitTestVisible => activity.IsIdle;

    /// <summary>
    /// Gets a value indicating whether the line that tells what the app is doing is shown.
    /// </summary>
    public bool SetUpCustomizationsPanelIsVisible => activity.ProgressIsVisible;

    /// <summary>
    /// Gets the text that tells what the app is doing.
    /// </summary>
    public string SetUpCustomizationsPanelText => activity.ProgressText;

    /// <summary>
    /// Gets <see cref="INavigationService"/>.
    /// </summary>
    public INavigationService NavigationService { get; }

    /// <summary>
    /// Gets <see cref="INavigationViewService"/>.
    /// </summary>
    public INavigationViewService NavigationViewService { get; }

    /// <summary>
    /// Gets <see cref="UIModel"/> collection of the settings.
    /// </summary>
    public ObservableCollection<UIModel> SettingModels { get; private set; } = [];

    /// <summary>
    /// Gets the UWP apps chosen to be uninstalled. They are not applied with the settings: they are uninstalled right from their page.
    /// </summary>
    public ObservableCollection<UIUwpAppModel> UwpAppsToUninstall { get; } = [];

    /// <summary>
    /// Executes the ViewModel logic of the MVVM pattern.
    /// </summary>
    public async Task ExecuteAsync()
    {
        await startup.RunRequirementActionsAsync();
        await RunPostActionsAsync();
        await GenerateUIAsync();
    }

    /// <summary>
    /// Show a page with a message about a fatal error.
    /// </summary>
    public void FatalErrorHandling()
    {
        activity.IsIdle = false;
        _ = NavigationService.NavigateTo<FatalErrorPage>();
    }

    // The pages are bound to the properties of this view model
    private void OnActivityChanged(object? sender, PropertyChangedEventArgs args)
    {
        var name = args.PropertyName switch
        {
            nameof(AppActivity.IsIdle) => nameof(NavigationViewHitTestVisible),
            nameof(AppActivity.ProgressIsVisible) => nameof(SetUpCustomizationsPanelIsVisible),
            nameof(AppActivity.ProgressText) => nameof(SetUpCustomizationsPanelText),
            _ => null,
        };

        if (name is not null)
        {
            OnPropertyChanged(name);
        }
    }

    private async Task GenerateUIAsync()
    {
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => startup.StatusText = "OsRequirements_ReadWindowsSettings".GetLocalized());
        SettingModels = await modelService.BuildSettingModelsAsync();
        await modelService.GetModelsState(SettingModels);
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => startup.StatusText = "OsRequirements_GeneratingUserInterface".GetLocalized());
        await ReadUwpAppsAsync();
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            activity.IsIdle = true;
            NavigationService.NavigateTo<PrivacyPage>(clearHistory: true);
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

        // The search results belong to no category, so no menu item is highlighted there
        if (selectedItem != null || e.SourcePageType == typeof(SearchPage))
        {
            SelectedNavigationViewItem = selectedItem;
        }
    }

    private async Task ReadUwpAppsAsync()
    {
        uwpAllUsersModels = await modelService.BuildUwpAppModelsAsync(forAllUsers: true);
        uwpCurrentUserModels = await modelService.BuildUwpAppModelsAsync(forAllUsers: false);
        UwpAppsModels = new(UwpForAllUsersState ? uwpAllUsersModels : uwpCurrentUserModels);
    }

    private async Task RunPostActionsAsync()
    {
        await Task.Run(() =>
        {
            registryService.UseLatestCLR();
            groupPolicyService.DeleteLGPOConfig();
        });
    }

    private void UwpForAllUsersClicked()
    {
        activity.IsIdle = false;
        App.Logger.LogUwpForAllUsersState(UwpForAllUsersState);

        // The other list has other apps, so the choice made in this one is dropped
        foreach (var app in UwpAppsToUninstall)
        {
            app.IsSelected = false;
            app.IsChecked = false;
        }

        UwpAppsToUninstall.Clear();
        chosenSettings.InfoBadgeCounters.ResetUwp();
        UwpForAllUsersState = !UwpForAllUsersState;
        UwpAppsModels = new ObservableCollection<UIModel>(UwpForAllUsersState ? uwpAllUsersModels : uwpCurrentUserModels);
        activity.IsIdle = true;
    }

    private void UIUwpAppModelClicked(UIUwpAppModel model)
    {
        model.ForAllUsers = UwpForAllUsersState;
        model.Mutator = (_, _) => packagesService.RemovePackages(model.PackageFullNames, model.ForAllUsers);

        // The apps have a queue of their own
        var isChosen = UwpAppsToUninstall.Remove(model);
        model.IsSelected = !isChosen;

        if (isChosen)
        {
            chosenSettings.InfoBadgeCounters.DecrementCategory(model.Tag);
            return;
        }

        UwpAppsToUninstall.Add(model);

        // The menu shows that there is something to uninstall
        chosenSettings.InfoBadgeCounters.IncrementCategory(model.Tag);
    }

    private async Task UwpAppsUninstallAsync()
    {
        // Settings are already being applied or read
        if (!activity.IsIdle || UwpAppsToUninstall.Count == 0)
        {
            return;
        }

        activity.IsIdle = false;
        activity.ProgressText = string.Empty;
        activity.ProgressIsVisible = true;
        var apps = UwpAppsToUninstall.ToList();

        await Task.Run(() =>
        {
            for (var i = 0; i < apps.Count; i++)
            {
                // The user sees which app is being uninstalled and how many are left
                App.Status.SetOperation(apps.Count == 1
                    ? string.Format("Panel_Install_Uninstalling".GetLocalized(), apps[i].Title)
                    : string.Format("Panel_Status_Uninstalling".GetLocalized(), i + 1, apps.Count, apps[i].Title));
                apps[i].IsSelected = false;
                var timer = Stopwatch.StartNew();
                apps[i].SetState();
                timer.Stop();
                App.Logger.LogModelSetState(apps[i].Name, timer);
            }
        });

        // The uninstalled apps disappear from the list
        App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
        UwpAppsToUninstall.Clear();
        chosenSettings.InfoBadgeCounters.ResetUwp();
        await ReadUwpAppsAsync();
        activity.ProgressIsVisible = false;
        activity.IsIdle = true;
    }
}
