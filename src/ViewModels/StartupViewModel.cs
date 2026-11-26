// <copyright file="StartupViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.Views;
    using System.Collections.ObjectModel;
    using System.Diagnostics;

    /// <summary>
    /// The start of the app: the terms the user accepts, the checks of Windows that run before the settings are shown,
    /// and what the user answers when a check fails.
    /// </summary>
    public partial class StartupViewModel : ObservableRecipient
    {
        // Increased when the terms or the privacy policy change: the user is asked to accept them again
        private const int TermsVersion = 1;

        private readonly IInstrumentationService instrumentationService;
        private readonly INavigationService navigationService;
        private readonly IRequirementsService requirementsService;
        private readonly ISettingsService settingsService;
        private readonly IThemesService themesService;
        private readonly IUpdateService updateService;
        private TaskCompletionSource<bool>? taskContinueSource;
        private TaskCompletionSource<bool>? termsSource;
        private StartupCheck? failedCheck;
        private RequirementAction? failedAction;
        private StartupCheck? currentStep;

        [ObservableProperty]
        private RequirementsResult requirementsResult;
        [ObservableProperty]
        private bool supportIsVisible;
        [ObservableProperty]
        private bool languageCanBeChanged = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="navigationService">Page navigation service.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        /// <param name="requirementsService">Service for working with OS requirements.</param>
        /// <param name="settingsService">A service for working with app settings.</param>
        /// <param name="themesService">A service for working with app themes.</param>
        /// <param name="updateService">Determines whether the Windows Update API is used to obtain updates for other Microsoft products.</param>
        public StartupViewModel(
            IHttpService httpService,
            IInstrumentationService instrumentationService,
            INavigationService navigationService,
            IProcessService processService,
            IRequirementsService requirementsService,
            ISettingsService settingsService,
            IThemesService themesService,
            IUpdateService updateService)
        {
            this.instrumentationService = instrumentationService;
            this.navigationService = navigationService;
            this.requirementsService = requirementsService;
            this.settingsService = settingsService;
            this.themesService = themesService;
            this.updateService = updateService;
            ChangeLanguage_Command = new AsyncRelayCommand<string>(ChangeLanguageAsync);
            AcceptTerms_Command = new RelayCommand(() => termsSource?.TrySetResult(true));
            AppUpdate_Command = new RelayCommand(() => httpService.OpenUrl(AppLinks.LatestRelease));
            ContinueRequirementActionsExecute_Command = new RelayCommand(ContinueRequirementActionsExecute);
            DecryptSystemDrive_Command = new AsyncRelayCommand(DecryptSystemDriveAsync);
            OpenBitLockerSettings_Command = new RelayCommand(() => processService.StartProcessByName("control.exe", "/name Microsoft.BitLockerDriveEncryption"));
            OpenDefenderControlledFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", "windowsdefender://RansomwareProtection"));
            OpenHostsFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc")));
            UpdateUEFICertificates_Command = new RelayCommand(() =>
            {
                httpService.OpenUrl(AppLinks.UpdateUefiCertificates);
                httpService.OpenUrl(AppLinks.SigningUefiCertificates);
            });
            EnableSecureBoot_Command = new RelayCommand(() => httpService.OpenUrl(AppLinks.EnableSecureBoot));
            OpenLogFile_Command = new RelayCommand(() => ContextMenuHelper.OpenInExplorer(App.Logger.LogFile));
        }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to accept the terms: all their items are checked.
        /// </summary>
        public IRelayCommand AcceptTerms_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to change the language of the app while it starts. Its parameter is the language tag.
        /// </summary>
        public IAsyncRelayCommand<string> ChangeLanguage_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open the page of the latest release of the app.
        /// </summary>
        public IRelayCommand AppUpdate_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to continue the checks after one of them failed.
        /// </summary>
        public IRelayCommand ContinueRequirementActionsExecute_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to decrypt the system drive and continue the checks.
        /// </summary>
        public IAsyncRelayCommand DecryptSystemDrive_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open BitLocker settings.
        /// </summary>
        public IRelayCommand OpenBitLockerSettings_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open Microsoft Defender controlled folder settings.
        /// </summary>
        public IRelayCommand OpenDefenderControlledFolder_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open hosts file folder in explorer.
        /// </summary>
        public IRelayCommand OpenHostsFolder_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open update UEFI certificates manual url.
        /// </summary>
        public IRelayCommand UpdateUEFICertificates_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open enable Secure Boot manual url.
        /// </summary>
        public IRelayCommand EnableSecureBoot_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open the log file: it tells why a check has stopped the start.
        /// </summary>
        public IRelayCommand OpenLogFile_Command { get; }

        /// <summary>
        /// Gets the checks that have been started, in the order they run. Each one gets its mark when it is over.
        /// </summary>
        public ObservableCollection<StartupCheck> Checks { get; } = [];

        /// <summary>
        /// Gets the terms that are checked, by their numbers. They are kept while the page of the terms is created anew.
        /// </summary>
        public HashSet<int> CheckedTerms { get; } = [];

        /// <summary>
        /// Gets or sets a value indicating whether the privacy policy is opened on the page of the terms.
        /// </summary>
        public bool PolicyIsOpen { get; set; }

        /// <summary>
        /// Shows the terms and waits until the user accepts them. Accepted terms are not shown again until they change.
        /// Nothing is checked and nothing is requested from the Internet before that.
        /// </summary>
        public async Task ShowTermsAsync()
        {
            if (settingsService.ReadAcceptedTerms() >= TermsVersion)
            {
                return;
            }

            termsSource = new();
            _ = navigationService.NavigateTo<WelcomePage>(clearHistory: true, disablePageAnimation: true);
            await termsSource.Task;
            await settingsService.SaveAcceptedTermsAsync(TermsVersion);
            App.Logger.LogTermsAccepted(TermsVersion);
        }

        /// <summary>
        /// Runs the checks of Windows one by one. When a check fails, its notice is shown and the checks wait for the user.
        /// </summary>
        public async Task RunRequirementActionsAsync()
        {
            foreach (var action in requirementsService.GetActions())
            {
                // An action without a text only starts something in the background, so it has no line
                var check = new StartupCheck(action.DisplayKey ?? string.Empty, action.Details);

                if (action.DisplayKey is not null)
                {
                    Checks.Add(check);
                }

                // The line is drawn before the check takes the processor
                await Task.Delay(100);
                var timer = Stopwatch.StartNew();

                // A check can wait for the network or for Windows, so it runs away from the window thread
                RequirementsResult = await Task.Run(action.Execute);
                timer.Stop();
                App.Logger.LogRequirementsActionExecute(action.Execute.Method.Name, timer);

                if (RequirementsResult == RequirementsResult.AllCorrect)
                {
                    // What the check has found is shown under its line
                    check.ShowDetails();
                    check.State = StartupCheckState.Passed;
                    continue;
                }

                var texts = GetFailureTexts(RequirementsResult);
                App.Logger.LogRequirementsFailureResult(RequirementsResult);
                App.Logger.LogUserNotice(string.Join(" ", texts));

                // A question can be passed by and has the mark of a warning, anything else stops the start
                var isQuestion = CanBeContinued(RequirementsResult);
                check.Actions = GetFailureActions(RequirementsResult);
                check.NoticeSeverity = isQuestion ? InfoBarSeverity.Warning : InfoBarSeverity.Error;
                check.Notice = string.Join(Environment.NewLine, texts);
                check.State = isQuestion ? StartupCheckState.Continued : StartupCheckState.Failed;
                failedCheck = check;
                failedAction = action;
                SupportIsVisible = true;
                updateService.RunUpdateByReason(RequirementsResult);
                taskContinueSource = new();
                await taskContinueSource.Task;

                // The user chose to go on
                failedCheck = null;
                failedAction = null;
                SupportIsVisible = false;
                check.Notice = string.Empty;
                check.Actions = [];
                check.State = StartupCheckState.Continued;
            }
        }

        /// <summary>
        /// Adds a line for what the app does after the checks. The line before it gets its mark.
        /// </summary>
        /// <param name="textKey">The resource key of the text that tells what the app is doing.</param>
        public void ShowStep(string textKey)
        {
            // It is called from the thread that reads the settings of Windows
            _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                // The settings are read with their names, so from here on the language stays
                LanguageCanBeChanged = false;

                if (currentStep is not null)
                {
                    currentStep.State = StartupCheckState.Passed;
                }

                currentStep = new StartupCheck(textKey);
                Checks.Add(currentStep);
            });
        }

        /// <summary>
        /// Fills the window anew, with the menu and the panels in the language of the app. A page reads its texts when it is created.
        /// </summary>
        public async Task ReloadWindowAsync()
        {
            // The selected menu item belongs to the menu that is going away. Given to the new menu, it crashes the app
            App.GetService<ShellViewModel>().SelectedNavigationViewItem = null;
            var shell = App.GetService<ShellPage>();
            var loaded = new TaskCompletionSource<bool>();
            shell.Loaded += (_, _) => loaded.TrySetResult(true);
            App.MainWindow.Content = shell;
            await themesService.SetRequestedThemeAsync();

            // A page is opened only when the menu is on the screen: a menu that gets its selected item earlier mixes its items up
            await loaded.Task;
        }

        private static bool CanBeContinued(RequirementsResult result) => result
            is RequirementsResult.ExternalDataUnavailable
            or RequirementsResult.HostsEntriesFound
            or RequirementsResult.SystemDriveEncryptedBitLockerDisabled
            or RequirementsResult.UpdateUEFICertificates
            or RequirementsResult.EnableSecureBoot;

        // What the user is told when a check fails. The log gets the same texts
        private static string[] GetFailureTexts(RequirementsResult result)
        {
            // A notice is a plain text, so the markup is dropped
            static string Text(string key) => FormattedText.ToPlainText($"{key}/Text".GetLocalized());
            var data = App.GetService<ICommonDataService>();

            string[] texts = result switch
            {
                RequirementsResult.WindowsComponentStabilityDisrupted or RequirementsResult.HarmfulTweakerFound or RequirementsResult.UWPComponentsMissing or RequirementsResult.DefenderComponentsMissing
                    => [data.RequirementsResult_1, Text("OsRequirements_ReinstallWindows")],
                RequirementsResult.PowerShellBlocked => [Text("OsRequirements_PowerShellBlocked_1"), data.RequirementsResult_1, Text("OsRequirements_PowerShellBlocked_2")],
                RequirementsResult.NewAppVersionFound => [data.RequirementsResult_1, Text("OsRequirements_NewAppVersionFound_2")],
                RequirementsResult.LoggedUserNotAdmin => [data.RequirementsResult_1, data.RequirementsResult_2, Text("OsRequirements_LoggedUserNotAdmin_3")],
                RequirementsResult.ExternalDataUnavailable => [Text("OsRequirements_ExternalDataUnavailable_1"), data.RequirementsResult_1, Text("OsRequirements_ExternalDataUnavailable_2"), Text("OsRequirements_DoYouWantToContinue")],
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

            return [.. texts.Where(text => text.Length > 0)];
        }

        // The buttons of the notice of a failed check. The answer the app suggests goes first
        private StartupCheckAction[] GetFailureActions(RequirementsResult result)
        {
            static string Text(string key) => $"{key}/Content".GetLocalized();
            var goOn = new StartupCheckAction(Text("Button_Continue"), ContinueRequirementActionsExecute_Command, IsPrimary: true);

            return result switch
            {
                RequirementsResult.NewAppVersionFound => [new(Text("Button_Update"), AppUpdate_Command, IsPrimary: true)],
                RequirementsResult.ExternalDataUnavailable => [goOn],
                RequirementsResult.HostsEntriesFound => [goOn, new(Text("Button_OpenHostFileFolder"), OpenHostsFolder_Command)],
                RequirementsResult.DisableControlledFolderAccess => [new(Text("Button_OpenControlledFolderSettings"), OpenDefenderControlledFolder_Command, IsPrimary: true)],
                RequirementsResult.SystemDriveEncryptedBitLockerDisabled =>
                [
                    new(Text("Button_Yes"), DecryptSystemDrive_Command, IsPrimary: true),
                    new(Text("Button_No"), ContinueRequirementActionsExecute_Command),
                    new(Text("Button_OpenBitLockerSettings"), OpenBitLockerSettings_Command),
                ],
                RequirementsResult.UpdateUEFICertificates => [goOn, new(Text("Button_UpdateUEFICertificates"), UpdateUEFICertificates_Command)],
                RequirementsResult.EnableSecureBoot => [goOn, new(Text("Button_EnableSecureBoot"), EnableSecureBoot_Command)],

                // A check that cannot be passed by and has nothing to open: the log tells the details
                _ => [new("Page_ContextMenu_OpenLogFile/Text".GetLocalized(), OpenLogFile_Command)],
            };
        }

        private async Task ChangeLanguageAsync(string? tag)
        {
            if (tag is null || !LanguageCanBeChanged || !AppLanguage.IsSupported(tag) || tag.Equals(AppLanguage.Current, StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            AppLanguage.Apply(tag);
            await settingsService.SaveLanguageAsync(tag);
            App.Logger.LogChangeLanguage(tag);

            foreach (var check in Checks)
            {
                check.Relocalize();
            }

            // The check that waits for an answer is run again, so its notice comes in the new language
            if (failedCheck is StartupCheck waiting && failedAction is RequirementAction action)
            {
                var result = await Task.Run(action.Execute);

                if (result == RequirementsResult && ReferenceEquals(waiting, failedCheck))
                {
                    waiting.Actions = GetFailureActions(result);
                    waiting.Notice = string.Join(Environment.NewLine, GetFailureTexts(result));
                }
            }

            // A page reads its texts when it is created, so the window is filled anew: the menu, and the page that was shown
            var page = termsSource is { Task.IsCompleted: false } ? typeof(WelcomePage) : typeof(StartupPage);
            await ReloadWindowAsync();
            _ = navigationService.NavigateTo(page, clearHistory: true, disablePageAnimation: true);
        }

        private async Task DecryptSystemDriveAsync()
        {
            // Windows can refuse to decrypt the drive. The user is told, and decides what to do next
            if (!await Task.Run(instrumentationService.TryDecryptSystemDrive))
            {
                var refusal = "OsRequirements_SystemDriveDecryptionFailed/Message".GetLocalized();
                App.Logger.LogUserNotice(refusal);

                if (failedCheck is not null && !failedCheck.Notice.Contains(refusal))
                {
                    failedCheck.NoticeSeverity = InfoBarSeverity.Error;
                    failedCheck.Notice = string.Join(Environment.NewLine, failedCheck.Notice, refusal);
                    failedCheck.State = StartupCheckState.Failed;
                }

                return;
            }

            // The decryption goes on in the background
            ContinueRequirementActionsExecute();
        }

        // The button pressed twice continues once
        private void ContinueRequirementActionsExecute() => taskContinueSource?.TrySetResult(true);
    }
}
