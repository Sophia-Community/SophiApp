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
        private readonly IUpdateService updateService;
        private TaskCompletionSource<bool>? taskContinueSource;
        private TaskCompletionSource<bool>? termsSource;
        private StartupCheck? failedCheck;
        private StartupCheck? currentStep;

        [ObservableProperty]
        private RequirementsResult requirementsResult;
        [ObservableProperty]
        private bool supportIsVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="navigationService">Page navigation service.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        /// <param name="requirementsService">Service for working with OS requirements.</param>
        /// <param name="settingsService">A service for working with app settings.</param>
        /// <param name="updateService">Determines whether the Windows Update API is used to obtain updates for other Microsoft products.</param>
        public StartupViewModel(
            IHttpService httpService,
            IInstrumentationService instrumentationService,
            INavigationService navigationService,
            IProcessService processService,
            IRequirementsService requirementsService,
            ISettingsService settingsService,
            IUpdateService updateService)
        {
            this.instrumentationService = instrumentationService;
            this.navigationService = navigationService;
            this.requirementsService = requirementsService;
            this.settingsService = settingsService;
            this.updateService = updateService;
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
        }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to accept the terms: all their items are checked.
        /// </summary>
        public IRelayCommand AcceptTerms_Command { get; }

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
        /// Gets the checks that have been started, in the order they run. Each one gets its mark when it is over.
        /// </summary>
        public ObservableCollection<StartupCheck> Checks { get; } = [];

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
                var check = new StartupCheck(action.DisplayText ?? string.Empty);

                if (action.DisplayText is not null)
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
                SupportIsVisible = true;
                updateService.RunUpdateByReason(RequirementsResult);
                taskContinueSource = new();
                await taskContinueSource.Task;

                // The user chose to go on
                failedCheck = null;
                SupportIsVisible = false;
                check.Notice = string.Empty;
                check.Actions = [];
                check.State = StartupCheckState.Continued;
            }
        }

        /// <summary>
        /// Adds a line for what the app does after the checks. The line before it gets its mark.
        /// </summary>
        /// <param name="text">What the app is doing.</param>
        public void ShowStep(string text)
        {
            // It is called from the thread that reads the settings of Windows
            _ = App.MainWindow.DispatcherQueue.TryEnqueue(() =>
            {
                if (currentStep is not null)
                {
                    currentStep.State = StartupCheckState.Passed;
                }

                currentStep = new StartupCheck(text);
                Checks.Add(currentStep);
            });
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
                _ => [],
            };
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
