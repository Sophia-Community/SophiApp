// <copyright file="GroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class GroupPolicyService : IGroupPolicyService
    {
        private readonly ICommonDataService commonDataService = App.GetService<ICommonDataService>();

        /// <inheritdoc/>
        public void ClearAdvertisingIdCache()
        {
            var infoPath = "Software\\Policies\\Microsoft\\Windows\\AdvertisingInfo";
            Registry.LocalMachine.OpenSubKey(infoPath, true)?.DeleteValue("DisabledByGroupPolicy", false);
        }

        /// <inheritdoc/>
        public void ClearAeroShakingCache()
        {
            var policyPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var noShortcuts = "NoWindowMinimizingShortcuts";
            Registry.CurrentUser.OpenSubKey(policyPath, true)?.DeleteValue(noShortcuts, false);
            Registry.LocalMachine.OpenSubKey(policyPath, true)?.DeleteValue(noShortcuts, false);
        }

        /// <inheritdoc/>
        public void ClearAppsInstallingCache()
        {
            var contentPath = "Software\\Policies\\Microsoft\\Windows\\CloudContent";
            Registry.LocalMachine.OpenSubKey(contentPath, true)?.DeleteValue("DisableWindowsConsumerFeatures", false);
        }

        /// <inheritdoc/>
        public void ClearAppSuggestionsCache() => ClearAppsInstallingCache();

        /// <inheritdoc/>
        public void ClearControlPanelViewCache()
        {
            var panelPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            Registry.CurrentUser.OpenSubKey(panelPath, true)?.DeleteValue("ForceClassicControlPanel", false);
        }

        /// <inheritdoc/>
        public void ClearCortanaButtonCache()
        {
            var cortanaPath = "Software\\Policies\\Microsoft\\Windows\\Windows Search";
            Registry.LocalMachine.OpenSubKey(cortanaPath, true)?.DeleteValue("AllowCortana", false);
        }

        /// <inheritdoc/>
        public void ClearErrorReportingCache()
        {
            var reportingPoliciesPath = "Software\\Policies\\Microsoft\\Windows\\Windows Error Reporting";
            var disabled = "Disabled";
            Registry.LocalMachine.OpenSubKey(reportingPoliciesPath, true)?.DeleteValue(disabled, false);
            Registry.CurrentUser.OpenSubKey(reportingPoliciesPath, true)?.DeleteValue(disabled, false);
        }

        /// <inheritdoc/>
        public void ClearFeedbackFrequencyCache()
        {
            var dataPath = "Software\\Policies\\Microsoft\\Windows\\DataCollection";
            Registry.LocalMachine.OpenSubKey(dataPath, true)?.DeleteValue("DoNotShowFeedbackNotifications", false);
        }

        /// <inheritdoc/>
        public void ClearFileExplorerRibbonCache()
        {
            var explorerPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var minimized = "ExplorerRibbonStartsMinimized";
            Registry.LocalMachine.OpenSubKey(explorerPath, true)?.DeleteValue(minimized, false);
            Registry.CurrentUser.OpenSubKey(explorerPath, true)?.DeleteValue(minimized, false);
        }

        /// <inheritdoc/>
        public void ClearFirstLogonAnimationCache()
        {
            var animationPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            Registry.LocalMachine.OpenSubKey(animationPath, true)?.DeleteSubKey("EnableFirstLogonAnimation", false);
        }

        /// <inheritdoc/>
        public void ClearMeetNowCache()
        {
            var policiesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            var meetNow = "HideSCAMeetNow";
            Registry.CurrentUser.OpenSubKey(policiesPath, true)?.DeleteValue(meetNow, false);
            Registry.LocalMachine.OpenSubKey(policiesPath, true)?.DeleteValue(meetNow, false);
        }

        /// <inheritdoc/>
        public void ClearNewsInterestsCache()
        {
            var feedsPath = "Software\\Policies\\Microsoft\\Windows\\Windows Feeds";
            var newsPath = "Software\\Microsoft\\PolicyManager\\default\\NewsAndInterests\\AllowNewsAndInterests";
            Registry.LocalMachine.OpenSubKey(feedsPath, true)?.DeleteValue("EnableFeeds", false);
            Registry.LocalMachine.OpenSubKey(newsPath, true)?.DeleteValue("value", false);
        }

        /// <inheritdoc/>
        public void ClearNotificationAreaIconsCache()
        {
            var notificationPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            var trayNotify = "NoAutoTrayNotify";
            Registry.CurrentUser.OpenSubKey(notificationPath, true)?.DeleteValue(trayNotify, false);
            Registry.LocalMachine.OpenSubKey(notificationPath, true)?.DeleteValue(trayNotify, false);
        }

        /// <inheritdoc/>
        public void ClearPeopleTaskbarCache()
        {
            var policiesPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var peopleBar = "HidePeopleBar";
            Registry.CurrentUser.OpenSubKey(policiesPath, true)?.DeleteValue(peopleBar, false);
            Registry.LocalMachine.OpenSubKey(policiesPath, true)?.DeleteValue(peopleBar, false);
        }

        /// <inheritdoc/>
        public void ClearQuickAccessRecentFilesCache()
        {
            var machinePoliciesPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            var userPoliciesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            var historyValue = "NoRecentDocsHistory";
            Registry.LocalMachine.OpenSubKey(machinePoliciesPath, true)?.DeleteValue(historyValue, false);
            Registry.CurrentUser.OpenSubKey(userPoliciesPath, true)?.DeleteValue(historyValue, false);
        }

        /// <inheritdoc/>
        public void ClearRecentlyAddedAppsCache()
        {
            var appsPath = "Software\\Policies\\Microsoft\\Windows\\Explorer";
            Registry.LocalMachine.OpenSubKey(appsPath, true)?.DeleteValue("HideRecentlyAddedApps", false);
        }

        /// <inheritdoc/>
        public void ClearRecycleBinDeleteConfirmationCache()
        {
            var policiesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            var policiesValue = "ConfirmFileDelete";
            Registry.LocalMachine.OpenSubKey(policiesPath, true)?.DeleteValue(policiesValue, false);
            Registry.CurrentUser.OpenSubKey(policiesPath, true)?.DeleteValue(policiesValue, false);
        }

        /// <inheritdoc/>
        public void ClearSearchHighlightsCache()
        {
            var searchPath = "Software\\Policies\\Microsoft\\Windows\\Windows Search";
            Registry.LocalMachine.OpenSubKey(searchPath, true)?.DeleteValue("EnableDynamicContentInWSB", false);
        }

        /// <inheritdoc/>
        public void ClearTaskbarSearchCache()
        {
            if (commonDataService.IsWindows11)
            {
                var disablePath = "Software\\Microsoft\\PolicyManager\\default\\Search\\DisableSearch";
                Registry.LocalMachine.OpenSubKey(disablePath, true)?.SetValue("value", 0, RegistryValueKind.DWord);
            }

            var windowsPath = "Software\\Policies\\Microsoft\\Windows\\Windows Search";
            Registry.LocalMachine.OpenSubKey(windowsPath, true)?.DeleteValue("DisableSearch", false);
            Registry.LocalMachine.OpenSubKey(windowsPath, true)?.DeleteValue("SearchOnTaskbarMode", false);
        }

        /// <inheritdoc/>
        public void ClearTaskbarWidgetsCache()
        {
            var newsPath = "Software\\Microsoft\\PolicyManager\\default\\NewsAndInterests\\AllowNewsAndInterests";
            var dshPath = "Software\\Policies\\Microsoft\\Dsh";
            Registry.LocalMachine.OpenSubKey(newsPath, true)?.DeleteValue("value", false);
            Registry.LocalMachine.OpenSubKey(dshPath, true)?.DeleteValue("AllowNewsAndInterests", false);
        }

        /// <inheritdoc/>
        public void ClearTailoredExperiencesCache()
        {
            var contentPath = "Software\\Policies\\Microsoft\\Windows\\CloudContent";
            Registry.CurrentUser.OpenSubKey(contentPath, true)?.DeleteValue("DisableTailoredExperiencesWithDiagnosticData", false);
        }

        /// <inheritdoc/>
        public void ClearSaveZoneInformationCache()
        {
            var attachmentsPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Attachments";
            Registry.LocalMachine.OpenSubKey(attachmentsPath, true)?.DeleteValue("SaveZoneInformation", false);
        }

        /// <inheritdoc/>
        public void ClearSigninInfoCache()
        {
            var policiesPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\System";
            Registry.LocalMachine.OpenSubKey(policiesPath, true)?.DeleteValue("DisableAutomaticRestartSignOn", false);
        }

        /// <inheritdoc/>
        public void ClearTaskbarCombineCache()
        {
            var taskbarPath = "Software\\Microsoft\\Windows\\CurrentVersion\\Policies\\Explorer";
            var taskGrouping = "NoTaskGrouping";
            Registry.LocalMachine.OpenSubKey(taskbarPath, true)?.DeleteValue(taskGrouping, false);
            Registry.CurrentUser.OpenSubKey(taskbarPath, true)?.DeleteValue(taskGrouping, false);
        }

        /// <inheritdoc/>
        public void ClearWindowsInkWorkspaceCache()
        {
            var workspacePath = "Software\\Policies\\Microsoft\\WindowsInkWorkspace";
            Registry.LocalMachine.OpenSubKey(workspacePath, true)?.DeleteValue("AllowWindowsInkWorkspace", false);
        }

        /// <inheritdoc/>
        public void ClearWindowsTipsCache()
        {
            var contentPath = "Software\\Policies\\Microsoft\\Windows\\CloudContent";
            Registry.LocalMachine.OpenSubKey(contentPath, true)?.DeleteValue("DisableSoftLanding", false);
        }
    }
}
