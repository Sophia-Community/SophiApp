// <copyright file="IGroupPolicyService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with group policy API.
    /// </summary>
    public interface IGroupPolicyService
    {
        /// <summary>
        /// Clear the advertising id group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearAdvertisingIdCache();

        /// <summary>
        /// Clear the aero shaking group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearAeroShakingCache();

        /// <summary>
        /// Clear the apps silent installing group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearAppsInstallingCache();

        /// <summary>
        /// Clear the app suggestions group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearAppSuggestionsCache();

        /// <summary>
        /// Clear the control panel view group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearControlPanelViewCache();

        /// <summary>
        /// Clear the cortana button group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearCortanaButtonCache();

        /// <summary>
        /// Clear the error reporting group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearErrorReportingCache();

        /// <summary>
        /// Clear the feedback frequency group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearFeedbackFrequencyCache();

        /// <summary>
        /// Clear the file explorer ribbon group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearFileExplorerRibbonCache();

        /// <summary>
        /// Clear the first logon animation group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearFirstLogonAnimationCache();

        /// <summary>
        /// Clear the recommended section group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearStartRecommendedSectionCache();

        /// <summary>
        /// Clear the local security authority group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearLocalSecurityAuthorityCache();

        /// <summary>
        /// Clear the meet now group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearMeetNowCache();

        /// <summary>
        /// Clear the news and interests group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearNewsInterestsCache();

        /// <summary>
        /// Clear the notification area icons group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearNotificationAreaIconsCache();

        /// <summary>
        /// Clear the people taskbar group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearPeopleTaskbarCache();

        /// <summary>
        /// Clear the quick access recent files group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearQuickAccessRecentFilesCache();

        /// <summary>
        /// Clear the recently added apps group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearRecentlyAddedAppsCache();

        /// <summary>
        /// Clear the recycle bin delete confirmation group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearRecycleBinDeleteConfirmationCache();

        /// <summary>
        /// Clear the search highlights group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearSearchHighlightsCache();

        /// <summary>
        /// Clear the taskbar search group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearTaskbarSearchCache();

        /// <summary>
        /// Clear the task view button group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearTaskViewButtonCache();

        /// <summary>
        /// Clear the taskbar widgets group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearTaskbarWidgetsCache();

        /// <summary>
        /// Clear the tailored experiences group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearTailoredExperiencesCache();

        /// <summary>
        /// Clear the save zone information group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearSaveZoneInformationCache();

        /// <summary>
        /// Clear the signin info group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearSigninInfoCache();

        /// <summary>
        /// Clear the taskbar combine group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearTaskbarCombineCache();

        /// <summary>
        /// Clear the windows ink workspace group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearWindowsInkWorkspaceCache();

        /// <summary>
        /// Clear the windows tips group policy value cache to make changes visible in UI.
        /// </summary>
        void ClearWindowsTipsCache();
    }
}
