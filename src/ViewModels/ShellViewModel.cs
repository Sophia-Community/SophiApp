// <copyright file="ShellViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels;

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Contracts.Services;
using SophiApp.ControlTemplates;
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.Models;
using SophiApp.Views;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.ServiceProcess;

/// <summary>
/// Implements the <see cref="ShellViewModel"/> class.
/// </summary>
public partial class ShellViewModel : ObservableRecipient
{
    private const string AppAnimationDeveloperUrl = "https://www.linkedin.com/in/tmcrowmax";
    private const string AppDeveloperUrl = "https://github.com/Inestic";
    private const string AppDiscordUrl = "https://discord.gg/sSryhaEv79";
    private const string AppGitHubUrl = "https://github.com/Sophia-Community/SophiApp";
    private const string AppLatestReleaseUrl = "https://github.com/Sophia-Community/SophiApp/releases/latest";
    private const string AppProjectManagerUrl = "https://github.com/farag2";
    private const string AppTelegramUrl = "https://t.me/sophia_chat";
    private const string AppTesterUrl = "https://github.com/lowl1f3";
    private const string AppUiDeveloperUrl = "https://www.linkedin.com/in/artenjoyers";
    private const string AppUxDeveloperUrl = "https://www.linkedin.com/in/vladimir-palii-132745a1";
    private const string UpdateUEFICertificatesUrl = "https://techcommunity.microsoft.com/blog/windows-itpro-blog/updating-microsoft-secure-boot-keys/4055324";
    private const string SigningUEFICertificatesUrl = "https://techcommunity.microsoft.com/blog/hardware-dev-center/signing-with-the-new-2023-microsoft-uefi-certificates-what-submitters-need-to-kn/4455787";
    private const string EnableSecureBootUrl = "https://support.microsoft.com/en-us/windows/security/devicesecurity/windows-11-and-secure-boot";
    private readonly IAppxPackagesService packagesService;
    private readonly IGroupPolicyService groupPolicyService;
    private readonly IInstrumentationService instrumentationService;
    private readonly IModelService modelService;
    private readonly IOsService osService;
    private readonly IPowerShellService powerShellService;
    private readonly IProcessService processService;
    private readonly IRegistryService registryService;
    private readonly IRequirementsService requirementsService;
    private readonly IUpdateService updateService;
    private readonly StartupViewModel startupModel;
    private bool navigationViewHitTestVisible = false;
    private bool searchPageIsOpen;
    private int searchNumber;
    private List<UIModel> uwpAllUsersModels = [];
    private List<UIModel> uwpCurrentUserModels = [];
    private TaskCompletionSource<bool>? taskContinueSource;

    [ObservableProperty]
    private bool isBackEnabled;
    [ObservableProperty]
    private bool eventViewerButtonIsVisible = false;
    [ObservableProperty]
    private bool restartReminderIsVisible = false;
    [ObservableProperty]
    private bool setUpCustomizationsPanelIsVisible = false;
    [ObservableProperty]
    private bool uwpForAllUsersState = true;

    [ObservableProperty]
    private object? selectedNavigationViewItem;
    [ObservableProperty]
    private ObservableCollection<UIModel> applicableModels = [];
    [ObservableProperty]
    private ObservableCollection<UIModel> uwpAppsModels = [];
    [ObservableProperty]
    private RequirementsResult requirementsResult;
    [ObservableProperty]
    private string setUpCustomizationsPanelText = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
    /// </summary>
    /// <param name="groupPolicyService">A service for working with group policy API.</param>
    /// <param name="httpService">A service for working with HTTP API.</param>
    /// <param name="instrumentationService">A service for working with WMI API.</param>
    /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
    /// <param name="navigationService">Page navigation service.</param>
    /// <param name="navigationViewService">A service for navigating to View.</param>
    /// <param name="osService">A service for working with Windows services API.</param>
    /// <param name="packagesService">A service for working with appx packages API.</param>
    /// <param name="powerShellService">A service for working with Windows PowerShell API.</param>
    /// <param name="processService">A service for working with Windows process API.</param>
    /// <param name="registryService">A service for working with Windows registry API.</param>
    /// <param name="requirementsService">Service for working with OS requirements.</param>
    /// <param name="updateService">Determines whether the Windows Update API is used to obtain updates for other Microsoft products.</param>
    /// <param name="startupViewModel">Implements the <see cref="StartupViewModel"/> class.</param>
    public ShellViewModel(
        IAppxPackagesService packagesService,
        IGroupPolicyService groupPolicyService,
        IHttpService httpService,
        IInstrumentationService instrumentationService,
        IModelService modelService,
        INavigationService navigationService,
        INavigationViewService navigationViewService,
        IOsService osService,
        IPowerShellService powerShellService,
        IProcessService processService,
        IRegistryService registryService,
        IRequirementsService requirementsService,
        IUpdateService updateService,
        StartupViewModel startupViewModel)
    {
        this.groupPolicyService = groupPolicyService;
        this.instrumentationService = instrumentationService;
        this.modelService = modelService;
        this.osService = osService;
        this.packagesService = packagesService;
        this.powerShellService = powerShellService;
        this.processService = processService;
        this.registryService = registryService;
        this.requirementsService = requirementsService;
        this.updateService = updateService;
        startupModel = startupViewModel;
        NavigationViewService = navigationViewService;
        NavigationService = navigationService;
        NavigationService.Navigated += OnNavigated;

        // The status is reported from background threads
        App.Status.Changed += text => App.MainWindow.DispatcherQueue.TryEnqueue(() => SetUpCustomizationsPanelText = text);
        ApplicableModelsApply_Command = new AsyncRelayCommand(ApplicableModelsApplyAsync);
        ApplicableModelsClear_Command = new AsyncRelayCommand(ApplicableModelsClearAsync);
        AppUpdate_Command = new RelayCommand(() => httpService.OpenUrl(AppLatestReleaseUrl));
        ContinueRequirementActionsExecute_Command = new RelayCommand(ContinueRequirementActionsExecute);
        DecryptSystemDrive_Command = new AsyncRelayCommand(DecryptSystemDriveAsync);
        OpenAppAnimationDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppAnimationDeveloperUrl));
        OpenAppDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppDeveloperUrl));
        OpenAppDiscordPage_Command = new RelayCommand(() => httpService.OpenUrl(AppDiscordUrl));
        OpenAppGitHubPage_Command = new RelayCommand(() => httpService.OpenUrl(AppGitHubUrl));
        OpenAppProjectManagerPage_Command = new RelayCommand(() => httpService.OpenUrl(AppProjectManagerUrl));
        OpenAppTelegramPage_Command = new RelayCommand(() => httpService.OpenUrl(AppTelegramUrl));
        OpenAppTesterPage_Command = new RelayCommand(() => httpService.OpenUrl(AppTesterUrl));
        OpenAppUiDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppUiDeveloperUrl));
        OpenAppUxDeveloperPage_Command = new RelayCommand(() => httpService.OpenUrl(AppUxDeveloperUrl));
        OpenBitLockerSettings_Command = new RelayCommand(() => processService.StartProcessByName("control.exe", "/name Microsoft.BitLockerDriveEncryption"));
        OpenDefenderControlledFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", "windowsdefender://RansomwareProtection"));
        OpenHostsFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc")));
        OpenEventViewer_Command = new AsyncRelayCommand(() => Task.Run(() => processService.StartProcessByName(name: "mmc.exe", arguments: "eventvwr.msc")));
        OpenTaskScheduler_Command = new AsyncRelayCommand(() => Task.Run(() => processService.StartProcessByName(name: "control.exe", arguments: "schedtasks")));
        OpenLatestAppRelease_Command = new RelayCommand(() => httpService.OpenUrl(AppLatestReleaseUrl));
        RadioButtonsGroup2Clicked_Command = new RelayCommand<UIRadioButtonsGroup2Model>(group => RadioButtonsGroup2Clicked(group!));
        RadioButtonsGroup3Clicked_Command = new RelayCommand<UIRadioButtonsGroup3Model>(group => RadioButtonsGroup3Clicked(group!));
        RadioButtonsGroup4Clicked_Command = new RelayCommand<UIRadioButtonsGroup4Model>(group => RadioButtonsGroup4Clicked(group!));
        IconRadioButtonsClicked_Command = new RelayCommand<UIIconRadioButtonsModel>(group => IconRadioButtonsClicked(group!));
        ChoiceClicked_Command = new RelayCommand<UIChoiceModel>(choice => ChoiceClicked(choice!));
        Search_Command = new AsyncRelayCommand<string>(text => SearchAsync(text ?? string.Empty), AsyncRelayCommandOptions.AllowConcurrentExecutions);
        UIModelClicked_Command = new RelayCommand<UIModel>(model => UIModelClicked(model!));
        UIUwpAppModelClicked_Command = new RelayCommand<UIUwpAppModel>(model => UIUwpAppModelClicked(model!));
        UpdateUEFICertificates_Command = new RelayCommand(() =>
        {
            httpService.OpenUrl(UpdateUEFICertificatesUrl);
            httpService.OpenUrl(SigningUEFICertificatesUrl);
        });
        EnableSecureBoot_Command = new RelayCommand(() => httpService.OpenUrl(EnableSecureBootUrl));
        InstallModelInstall_Command = new RelayCommand<UIInstallModel>(model => InstallModelClicked(model!, install: true));
        InstallModelUninstall_Command = new RelayCommand<UIInstallModel>(model => InstallModelClicked(model!, install: false));
        UwpForAllUsersClicked_Command = new RelayCommand(UwpForAllUsersClicked);
        UwpAppsUninstall_Command = new AsyncRelayCommand(UwpAppsUninstallAsync);
    }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to click an "Apply" button in the Apply Customizations Panel.
    /// </summary>
    public IAsyncRelayCommand ApplicableModelsApply_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to click an "Cancel" button in the Apply Customizations Panel.
    /// </summary>
    public IAsyncRelayCommand ApplicableModelsClear_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "Update" button in the RequirementsFailurePage.
    /// </summary>
    public IRelayCommand AppUpdate_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to continue performing <see cref="RequirementAction"/> after fail result.
    /// </summary>
    public IRelayCommand ContinueRequirementActionsExecute_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to decrypt the system drive and continue execution of requirement actions.
    /// </summary>
    public IAsyncRelayCommand DecryptSystemDrive_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app animation developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppAnimationDeveloperPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppDeveloperPage_Command { get; private set; }

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
    /// Gets <see cref="IRelayCommand"/> to open app UI developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUiDeveloperPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open app UX developer contacts page.
    /// </summary>
    public IRelayCommand OpenAppUxDeveloperPage_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open BitLocker settings.
    /// </summary>
    public IRelayCommand OpenBitLockerSettings_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open Microsoft Defender controlled folder settings.
    /// </summary>
    public IRelayCommand OpenDefenderControlledFolder_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open hosts file folder in explorer.
    /// </summary>
    public IRelayCommand OpenHostsFolder_Command { get; private set; }

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
    /// Gets <see cref="IRelayCommand"/> to choose an option of <see cref="UIRadioButtonsGroup2Model"/>.
    /// </summary>
    public IRelayCommand<UIRadioButtonsGroup2Model> RadioButtonsGroup2Clicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose an option of <see cref="UIRadioButtonsGroup3Model"/>.
    /// </summary>
    public IRelayCommand<UIRadioButtonsGroup3Model> RadioButtonsGroup3Clicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose an option of <see cref="UIRadioButtonsGroup4Model"/>.
    /// </summary>
    public IRelayCommand<UIRadioButtonsGroup4Model> RadioButtonsGroup4Clicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose an option of <see cref="UIIconRadioButtonsModel"/> or to reset the choice.
    /// </summary>
    public IRelayCommand<UIIconRadioButtonsModel> IconRadioButtonsClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose an item of <see cref="UIChoiceModel"/>.
    /// </summary>
    public IRelayCommand<UIChoiceModel> ChoiceClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to show the settings that contain a text.
    /// </summary>
    public IAsyncRelayCommand<string> Search_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose the installation or the update of a package of <see cref="UIInstallModel"/>.
    /// </summary>
    public IRelayCommand<UIInstallModel> InstallModelInstall_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to choose the uninstallation of a package of <see cref="UIInstallModel"/>.
    /// </summary>
    public IRelayCommand<UIInstallModel> InstallModelUninstall_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an element in the interface.
    /// </summary>
    public IRelayCommand<UIModel> UIModelClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an <see cref="UIUwpAppModel"/> in the interface.
    /// </summary>
    public IRelayCommand<UIUwpAppModel> UIUwpAppModelClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open update UEFI certificates manual url.
    /// </summary>
    public IRelayCommand UpdateUEFICertificates_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to open enable Secure Boot manual url.
    /// </summary>
    public IRelayCommand EnableSecureBoot_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IRelayCommand"/> to click an "For all users" checkbox in the UWP page.
    /// </summary>
    public IRelayCommand UwpForAllUsersClicked_Command { get; private set; }

    /// <summary>
    /// Gets <see cref="IAsyncRelayCommand"/> to uninstall the UWP apps chosen in the UWP page.
    /// </summary>
    public IAsyncRelayCommand UwpAppsUninstall_Command { get; private set; }

    /// <summary>
    /// Gets the info badges counters state by <see cref="UICategoryTag"/>.
    /// </summary>
    public InfoBadgeCounters InfoBadgeCounters { get; } = new();

    /// <summary>
    /// Gets a value indicating whether NavigationView items hit test visible property.
    /// </summary>
    public bool NavigationViewHitTestVisible
    {
        get => navigationViewHitTestVisible;
        private set => SetProperty(ref navigationViewHitTestVisible, value, nameof(NavigationViewHitTestVisible));
    }

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
    public ObservableCollection<UIModel> JsonModels { get; private set; } = [];

    /// <summary>
    /// Gets <see cref="UIModel"/> collection founded by AutoSuggestBox query.
    /// </summary>
    public ObservableCollection<UIModel> FoundModels { get; } = [];

    /// <summary>
    /// Gets the UWP apps chosen to be uninstalled. They are not applied with the settings: they are uninstalled right from their page.
    /// </summary>
    public ObservableCollection<UIUwpAppModel> UwpAppsToUninstall { get; } = [];

    /// <summary>
    /// Executes the ViewModel logic of the MVVM pattern.
    /// </summary>
    public async Task ExecuteAsync()
    {
        await RunRequirementActionsAsync();
        await RunPostActionsAsync();
        await GenerateUIAsync();
    }

    /// <summary>
    /// Show a page with a message about a fatal error.
    /// </summary>
    public void FatalErrorHandling()
    {
        NavigationViewHitTestVisible = false;
        _ = NavigationService.NavigateTo(typeof(FatalErrorViewModel).FullName!);
    }

    // The log gets the same texts as the page of a failed startup check shows
    private static void LogRequirementsFailureTexts(RequirementsResult result)
    {
        // The log gets the texts without the markup
        static string Text(string key) => FormattedText.ToPlainText($"{key}/Text".GetLocalized());
        var data = App.GetService<ICommonDataService>();

        string[] texts = result switch
        {
            RequirementsResult.WindowsComponentStabilityDisrupted or RequirementsResult.HarmfulTweakerFound or RequirementsResult.UWPComponentsMissing or RequirementsResult.DefenderComponentsMissing
                => [data.RequirementsResult_1, Text("OsRequirements_ReinstallWindows")],
            RequirementsResult.NewAppVersionFound => [data.RequirementsResult_1, Text("OsRequirements_NewAppVersionFound_2")],
            RequirementsResult.LoggedUserNotAdmin => [data.RequirementsResult_1, data.RequirementsResult_2, Text("OsRequirements_LoggedUserNotAdmin_3")],
            RequirementsResult.HostsEntriesFound => [Text("OsRequirements_HostsEntriesFound_1"), Text("OsRequirements_HostsEntriesFound_2"), Text("OsRequirements_DoYouWantToContinue")],
            RequirementsResult.DisableControlledFolderAccess => [Text("OsRequirements_DisableControlledFolderAccessPanel_1"), Text("OsRequirements_DisableControlledFolderAccessPanel_2")],
            RequirementsResult.RebootPending => [Text("OsRequirements_RebootPending")],
            RequirementsResult.SystemDriveEncryptedBitLockerDisabled => [Text("OsRequirements_SystemDriveEncryptedBitLockerDisabled_1"), Text("OsRequirements_SystemDriveEncryptedBitLockerDisabled_2")],
            RequirementsResult.UpdateUEFICertificates => [Text("OsRequirements_UpdateUEFICertificates_1"), Text("OsRequirements_UpdateUEFICertificates_2"), Text("OsRequirements_UpdateUEFICertificates_3")],
            RequirementsResult.EnableSecureBoot => [Text("OsRequirements_EnableSecureBoot")],
            RequirementsResult.UnsupportedArchitecture => [data.RequirementsResult_1],
            RequirementsResult.WrongWindowsVersion => [data.RequirementsResult_1, data.RequirementsResult_2],
            RequirementsResult.UpdateWindowsBuild => [data.RequirementsResult_1, data.RequirementsResult_2, Text("OsRequirements_RunWinUpdate")],
            _ => [],
        };

        App.Logger.LogUserNotice(string.Join(" ", texts.Where(text => text.Length > 0)));
    }

    private static async Task CloseSearchWindowAsync()
    {
        var closed = await ForegroundHelper.CloseSearchAsync(WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow));
        App.Logger.LogSearchWindowWatch(closed, ForegroundHelper.LastWatch);
    }

    private async Task ApplicableModelsApplyAsync()
    {
        // Settings are already being applied or read
        if (!NavigationViewHitTestVisible)
        {
            return;
        }

        NavigationViewHitTestVisible = false;
        SetUpCustomizationsPanelText = string.Empty;
        SetUpCustomizationsPanelIsVisible = true;
        App.Logger.LogStartApplicableModelsSetState();
        StartMenuHelper.Reset();

        // The button that opens Event Viewer appears only after the custom view was created
        var eventViewCreated = ApplicableModels.OfType<UICheckBoxModel>().Any(model => model.IsChecked && model.Name == nameof(Customizations.Mutators.EventViewerCustomView));

        // The registry is scanned for policies last, after the other settings have written theirs
        if (ApplicableModels.FirstOrDefault(model => model.Name == nameof(Customizations.Mutators.ScanRegistryPolicies)) is UIModel scan)
        {
            ApplicableModels.Move(ApplicableModels.IndexOf(scan), ApplicableModels.Count - 1);
        }

        await modelService.SetModelsStateAsync(ApplicableModels);
        App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
        await modelService.GetModelsStateAsync(ApplicableModels);
        ApplicableModels.Clear();
        InfoBadgeCounters.ResetAll();
        App.Logger.LogApplicableModelsClear();

        // Open consoles would show outdated data, and an open policy editor could overwrite the changes
        App.Status.SetOperation("Panel_Status_UpdatingPolicies".GetLocalized());
        await Task.Run(() => processService.CloseManagementConsoles("taskschd.msc", "eventvwr.msc", "gpedit.msc"));
        groupPolicyService.UpdatePolicy();

        // Save all opened folders in order to restore them after File Explorer restart
        App.Status.SetOperation("Panel_Status_RestartingExplorer".GetLocalized());
        var openedFolders = ExplorerHelper.GetOpenedFolders();
        EnvironmentHelper.RefreshUserDesktop();
        EnvironmentHelper.ForcedRefresh();
        processService.KillProcessByName("StartMenuExperienceHost");
        processService.KillProcessByName("explorer");
        await ExplorerHelper.RestoreAsync(openedFolders);

        // Windows Search is restarted too, a little later, so it is watched for after the settings are applied
        _ = CloseSearchWindowAsync();
        SetUpCustomizationsPanelIsVisible = false;
        RestartReminderIsVisible = true;
        App.Logger.LogUserNotice("Panel_RestartReminder/Message".GetLocalized());
        EventViewerButtonIsVisible |= eventViewCreated;
        NavigationViewHitTestVisible = true;
    }

    private async Task ApplicableModelsClearAsync()
    {
        // Settings are already being applied or read
        if (!NavigationViewHitTestVisible)
        {
            return;
        }

        NavigationViewHitTestVisible = false;
        App.Logger.LogApplicableModelsCanceled();
        App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
        SetUpCustomizationsPanelIsVisible = true;
        await modelService.GetModelsStateAsync(ApplicableModels);
        ApplicableModels.Clear();
        InfoBadgeCounters.ResetAll();
        App.Logger.LogApplicableModelsClear();
        SetUpCustomizationsPanelIsVisible = false;
        NavigationViewHitTestVisible = true;
    }

    // A package is installed, updated or uninstalled together with the other chosen settings
    private void InstallModelClicked(UIInstallModel model, bool install)
    {
        // Settings are already being applied or read
        if (!NavigationViewHitTestVisible)
        {
            return;
        }

        var isChosen = ApplicableModels.Contains(model);

        // The same button pressed again takes the choice back
        if (isChosen && model.InstallRequested == install)
        {
            UIModelClicked(model);
            return;
        }

        model.InstallRequested = install;

        if (!isChosen)
        {
            UIModelClicked(model);
        }
    }

    private async Task DecryptSystemDriveAsync()
    {
        // Start decrypting the system drive and continue. The decryption goes on in the background
        await Task.Run(() => powerShellService.TryInvoke("Disable-BitLocker -MountPoint $env:SystemDrive", out _));
        ContinueRequirementActionsExecute();
    }

    private void ContinueRequirementActionsExecute()
    {
        Task.Delay(80).Wait();

        // The button pressed twice continues once
        if (taskContinueSource?.TrySetResult(true) == false)
        {
            return;
        }

        App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(page: typeof(StartupViewModel).FullName!, clearHistory: true, disablePageAnimation: true));
    }

    private async Task GenerateUIAsync()
    {
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => startupModel.StatusText = "OsRequirements_ReadWindowsSettings".GetLocalized());
        JsonModels = await modelService.BuildJsonModelsAsync();
        await modelService.GetModelsState(JsonModels);
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => startupModel.StatusText = "OsRequirements_GeneratingUserInterface".GetLocalized());
        await ReadUwpAppsAsync();
        _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
        {
            NavigationViewHitTestVisible = true;
            NavigationService.NavigateTo(page: typeof(PrivacyViewModel).FullName!, clearHistory: true);
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
        searchPageIsOpen = e.SourcePageType == typeof(SearchPage);
        var selectedItem = NavigationViewService.GetSelectedItem(e.SourcePageType);

        // The search results belong to no category, so no menu item is highlighted there
        if (selectedItem != null || searchPageIsOpen)
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

    private void RadioButtonsGroup2Clicked(UIRadioButtonsGroup2Model group)
    {
        var selectedId = int.Parse(group.SelectedId);
        var groupExist = ApplicableModels.Contains(group) && group.DefaultId == selectedId;
        group.IsSelected = !groupExist;

        if (groupExist)
        {
            ApplicableModels.Remove(group);
            InfoBadgeCounters.DecrementCategory(group.Tag);
            App.Logger.LogApplicableModelRemoved(group.Name);
            return;
        }

        ApplicableModels.Add(group);
        InfoBadgeCounters.IncrementCategory(group.Tag);
        App.Logger.LogApplicableModelAdded(group.Name, selectedId);
    }

    private void RadioButtonsGroup3Clicked(UIRadioButtonsGroup3Model group)
    {
        var selectedId = int.Parse(group.SelectedId);
        group.IsSelected = group.DefaultId != selectedId;

        if (ApplicableModels.Contains(group))
        {
            if (group.DefaultId == selectedId)
            {
                ApplicableModels.Remove(group);
                InfoBadgeCounters.DecrementCategory(group.Tag);
                App.Logger.LogApplicableModelRemoved(group.Name);
                return;
            }
            else
            {
                App.Logger.LogApplicableModelChanged(group.Name, selectedId);
                return;
            }
        }

        ApplicableModels.Add(group);
        InfoBadgeCounters.IncrementCategory(group.Tag);
        App.Logger.LogApplicableModelAdded(group.Name, selectedId);
    }

    private void RadioButtonsGroup4Clicked(UIRadioButtonsGroup4Model group)
    {
        var selectedId = int.Parse(group.SelectedId);
        group.IsSelected = group.DefaultId != selectedId;

        if (ApplicableModels.Contains(group))
        {
            if (group.DefaultId == selectedId)
            {
                ApplicableModels.Remove(group);
                InfoBadgeCounters.DecrementCategory(group.Tag);
                App.Logger.LogApplicableModelRemoved(group.Name);
                return;
            }
            else
            {
                App.Logger.LogApplicableModelChanged(group.Name, selectedId);
                return;
            }
        }

        ApplicableModels.Add(group);
        InfoBadgeCounters.IncrementCategory(group.Tag);
        App.Logger.LogApplicableModelAdded(group.Name, selectedId);
    }

    private void IconRadioButtonsClicked(UIIconRadioButtonsModel group)
    {
        var selectedId = int.Parse(group.SelectedId);

        // The reset is a choice too, even when no option is selected in Windows
        var isChanged = group.IsChanged;
        group.IsSelected = isChanged;

        if (ApplicableModels.Contains(group))
        {
            if (isChanged)
            {
                App.Logger.LogApplicableModelChanged(group.Name, selectedId);
            }
            else
            {
                ApplicableModels.Remove(group);
                InfoBadgeCounters.DecrementCategory(group.Tag);
                App.Logger.LogApplicableModelRemoved(group.Name);
            }

            return;
        }

        if (!isChanged)
        {
            return;
        }

        ApplicableModels.Add(group);
        InfoBadgeCounters.IncrementCategory(group.Tag);
        App.Logger.LogApplicableModelAdded(group.Name, selectedId);
    }

    private void ChoiceClicked(UIChoiceModel choice)
    {
        var isChanged = choice.IsChanged;
        choice.IsSelected = isChanged;

        if (ApplicableModels.Contains(choice))
        {
            if (isChanged)
            {
                App.Logger.LogApplicableModelChanged(choice.Name, choice.SelectedId);
            }
            else
            {
                ApplicableModels.Remove(choice);
                InfoBadgeCounters.DecrementCategory(choice.Tag);
                App.Logger.LogApplicableModelRemoved(choice.Name);
            }

            return;
        }

        if (!isChanged)
        {
            return;
        }

        ApplicableModels.Add(choice);
        InfoBadgeCounters.IncrementCategory(choice.Tag);
        App.Logger.LogApplicableModelAdded(choice.Name, choice.SelectedId);
    }

    private async Task RunPostActionsAsync()
    {
        await Task.Run(() =>
        {
            if (osService.Exist("SysMain") && osService.GetStatus("SysMain") == ServiceControllerStatus.Stopped)
            {
                osService.TrySetStartMode("SysMain", ServiceStartMode.Automatic);
                osService.TryStart("SysMain");
            }

            // Windows Security Service must be running
            osService.TryStart("SecurityHealthService");
            instrumentationService.SetPageFileAutoSize();
            registryService.UseLatestCLR();
            groupPolicyService.DeleteLGPOConfig();
        });
    }

    private async Task RunRequirementActionsAsync()
    {
        foreach (var action in requirementsService.GetActions())
        {
            _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => startupModel.StatusText = action.DisplayText!);
            await Task.Delay(100);
            var timer = Stopwatch.StartNew();
            RequirementsResult = action.Execute();
            timer.Stop();
            App.Logger.LogRequirementsActionExecute(action.Execute.Method.Name, timer);

            if (RequirementsResult != RequirementsResult.AllCorrect)
            {
                App.Logger.LogRequirementsFailureResult(RequirementsResult);
                LogRequirementsFailureTexts(RequirementsResult);
                _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => NavigationService.NavigateTo(page: typeof(RequirementsFailureModel).FullName!, clearHistory: true, disablePageAnimation: true));
                updateService.RunUpdateByReason(RequirementsResult);
                taskContinueSource = new();
                await taskContinueSource.Task;
            }
        }
    }

    private async Task SearchAsync(string text)
    {
        if (!NavigationViewHitTestVisible)
        {
            return;
        }

        var number = ++searchNumber;

        if (string.IsNullOrWhiteSpace(text))
        {
            // An empty query closes the results
            if (searchPageIsOpen)
            {
                _ = NavigationService.GoBack();
            }

            return;
        }

        var found = await modelService.GetModelsContainsTextAsync(JsonModels, text);

        // The text has been changed while the search was running
        if (number != searchNumber)
        {
            return;
        }

        ShowFoundModels(found);

        if (!searchPageIsOpen)
        {
            _ = NavigationService.NavigateTo(page: typeof(SearchViewModel).FullName!, disablePageAnimation: true);
        }
    }

    // The list is changed in place, so the page redraws only the cards that appear or disappear while the text is typed
    private void ShowFoundModels(IReadOnlyList<UIModel> found)
    {
        var wanted = found.ToHashSet();

        for (var i = FoundModels.Count - 1; i >= 0; i--)
        {
            if (!wanted.Contains(FoundModels[i]))
            {
                FoundModels.RemoveAt(i);
            }
        }

        for (var i = 0; i < found.Count; i++)
        {
            if (i >= FoundModels.Count || FoundModels[i] != found[i])
            {
                FoundModels.Insert(i, found[i]);
            }
        }
    }

    private void UIModelClicked(UIModel model)
    {
        var modelExist = ApplicableModels.Contains(model);
        model.IsSelected = !modelExist;

        if (modelExist)
        {
            ApplicableModels.Remove(model);
            InfoBadgeCounters.DecrementCategory(model.Tag);
            App.Logger.LogApplicableModelRemoved(model.Name);
            return;
        }

        ApplicableModels.Add(model);
        InfoBadgeCounters.IncrementCategory(model.Tag);
        App.Logger.LogApplicableModelAdded(model.Name);
    }

    private void UwpForAllUsersClicked()
    {
        NavigationViewHitTestVisible = false;
        App.Logger.LogUwpForAllUsersState(UwpForAllUsersState);

        // The other list has other apps, so the choice made in this one is dropped
        foreach (var app in UwpAppsToUninstall)
        {
            app.IsSelected = false;
            app.IsChecked = false;
        }

        UwpAppsToUninstall.Clear();
        InfoBadgeCounters.ResetUwp();
        UwpForAllUsersState = !UwpForAllUsersState;
        UwpAppsModels = new ObservableCollection<UIModel>(UwpForAllUsersState ? uwpAllUsersModels : uwpCurrentUserModels);
        NavigationViewHitTestVisible = true;
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
            InfoBadgeCounters.DecrementCategory(model.Tag);
            return;
        }

        UwpAppsToUninstall.Add(model);

        // The menu shows that there is something to uninstall
        InfoBadgeCounters.IncrementCategory(model.Tag);
    }

    private async Task UwpAppsUninstallAsync()
    {
        // Settings are already being applied or read
        if (!NavigationViewHitTestVisible || UwpAppsToUninstall.Count == 0)
        {
            return;
        }

        NavigationViewHitTestVisible = false;
        SetUpCustomizationsPanelText = string.Empty;
        SetUpCustomizationsPanelIsVisible = true;
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
        InfoBadgeCounters.ResetUwp();
        await ReadUwpAppsAsync();
        SetUpCustomizationsPanelIsVisible = false;
        NavigationViewHitTestVisible = true;
    }
}
