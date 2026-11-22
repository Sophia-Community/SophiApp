// <copyright file="StartupViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Views;
    using System.Diagnostics;

    /// <summary>
    /// The start of the app: the checks of Windows that run before the settings are shown, and what the user answers when a check fails.
    /// </summary>
    public partial class StartupViewModel : ObservableRecipient
    {
        private const string AppLatestReleaseUrl = "https://github.com/Sophia-Community/SophiApp/releases/latest";
        private const string UpdateUEFICertificatesUrl = "https://techcommunity.microsoft.com/blog/windows-itpro-blog/updating-microsoft-secure-boot-keys/4055324";
        private const string SigningUEFICertificatesUrl = "https://techcommunity.microsoft.com/blog/hardware-dev-center/signing-with-the-new-2023-microsoft-uefi-certificates-what-submitters-need-to-kn/4455787";
        private const string EnableSecureBootUrl = "https://support.microsoft.com/en-us/windows/security/devicesecurity/windows-11-and-secure-boot";

        private readonly IInstrumentationService instrumentationService;
        private readonly INavigationService navigationService;
        private readonly IRequirementsService requirementsService;
        private readonly IUpdateService updateService;
        private string? statusText;
        private TaskCompletionSource<bool>? taskContinueSource;

        [ObservableProperty]
        private RequirementsResult requirementsResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
        /// </summary>
        /// <param name="httpService">A service for working with HTTP API.</param>
        /// <param name="instrumentationService">A service for working with WMI API.</param>
        /// <param name="navigationService">Page navigation service.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        /// <param name="requirementsService">Service for working with OS requirements.</param>
        /// <param name="updateService">Determines whether the Windows Update API is used to obtain updates for other Microsoft products.</param>
        public StartupViewModel(
            IHttpService httpService,
            IInstrumentationService instrumentationService,
            INavigationService navigationService,
            IProcessService processService,
            IRequirementsService requirementsService,
            IUpdateService updateService)
        {
            this.instrumentationService = instrumentationService;
            this.navigationService = navigationService;
            this.requirementsService = requirementsService;
            this.updateService = updateService;
            AppUpdate_Command = new RelayCommand(() => httpService.OpenUrl(AppLatestReleaseUrl));
            ContinueRequirementActionsExecute_Command = new RelayCommand(ContinueRequirementActionsExecute);
            DecryptSystemDrive_Command = new AsyncRelayCommand(DecryptSystemDriveAsync);
            OpenBitLockerSettings_Command = new RelayCommand(() => processService.StartProcessByName("control.exe", "/name Microsoft.BitLockerDriveEncryption"));
            OpenDefenderControlledFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", "windowsdefender://RansomwareProtection"));
            OpenHostsFolder_Command = new RelayCommand(() => processService.StartProcessByName("explorer.exe", Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "drivers\\etc")));
            UpdateUEFICertificates_Command = new RelayCommand(() =>
            {
                httpService.OpenUrl(UpdateUEFICertificatesUrl);
                httpService.OpenUrl(SigningUEFICertificatesUrl);
            });
            EnableSecureBoot_Command = new RelayCommand(() => httpService.OpenUrl(EnableSecureBootUrl));
        }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to click an "Update" button in the RequirementsFailurePage.
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
        /// Gets or sets the text that tells what the app is doing while it starts.
        /// </summary>
        public string StatusText
        {
            get
            {
                return statusText!;
            }

            set
            {
                if (value is not null && statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        /// <summary>
        /// Runs the checks of Windows one by one. When a check fails, its page is shown and the checks wait for the user.
        /// </summary>
        public async Task RunRequirementActionsAsync()
        {
            foreach (var action in requirementsService.GetActions())
            {
                _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => StatusText = action.DisplayText!);
                await Task.Delay(100);
                var timer = Stopwatch.StartNew();

                // A check can wait for the network or for Windows, so it runs away from the window thread
                RequirementsResult = await Task.Run(action.Execute);
                timer.Stop();
                App.Logger.LogRequirementsActionExecute(action.Execute.Method.Name, timer);

                if (RequirementsResult != RequirementsResult.AllCorrect)
                {
                    App.Logger.LogRequirementsFailureResult(RequirementsResult);
                    LogRequirementsFailureTexts(RequirementsResult);
                    _ = App.MainWindow.DispatcherQueue.TryEnqueue(() => navigationService.NavigateTo<RequirementsFailurePage>(clearHistory: true, disablePageAnimation: true));
                    updateService.RunUpdateByReason(RequirementsResult);
                    taskContinueSource = new();
                    await taskContinueSource.Task;
                }
            }
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

        private async Task DecryptSystemDriveAsync()
        {
            // Start decrypting the system drive and continue. The decryption goes on in the background
            await Task.Run(instrumentationService.TryDecryptSystemDrive);
            ContinueRequirementActionsExecute();
        }

        private void ContinueRequirementActionsExecute()
        {
            // The button pressed twice continues once
            if (taskContinueSource?.TrySetResult(true) == false)
            {
                return;
            }

            App.MainWindow.DispatcherQueue.TryEnqueue(() => navigationService.NavigateTo<StartupPage>(clearHistory: true, disablePageAnimation: true));
        }
    }
}
