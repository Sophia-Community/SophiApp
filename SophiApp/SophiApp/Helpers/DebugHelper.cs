using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace SophiApp.Helpers
{
    internal class DebugHelper
    {
        private const string APP_FOLDER = "Application folder";
        private const string APP_LOC = "Application localization";
        private const string APP_THEME = "Application theme";
        private const string APP_VER = "Application version";
        private const string PC_NAME = "Computer name";
        private const string REG_ORG = "Registered organization";
        private const string REG_OWNER = "Registered owner";
        private const string USER_CULTURE = "User culture";
        private const string USER_DOMAIN = "User domain";
        private const string USER_NAME = "Current user";
        private const string USER_REGION = "User region";
        private static List<string> ErrorsLog = new List<string>();

        private static List<string> InfoLog = new List<string>
        {
            $"{OsHelper.GetProductName()} {OsHelper.GetDisplayVersion()} build: {OsHelper.GetVersion()}",
            $"{PC_NAME}: {Environment.MachineName}",
            $"{REG_ORG}: {OsHelper.GetRegisteredOrganization()}",
            $"{REG_OWNER}: {OsHelper.GetRegisteredOwner()}",
            $"{USER_NAME}: {Environment.UserName}",
            $"{USER_DOMAIN}: {Environment.GetEnvironmentVariable("userdnsdomain") ?? Environment.UserDomainName}",
            $"{USER_CULTURE}: {OsHelper.GetCurrentCultureName()}",
            $"{USER_REGION}: {OsHelper.GetRegionName()}",
            $"{APP_VER}: {AppHelper.Version}",
            $"{APP_FOLDER}: \"{AppHelper.StartupFolder}\""
        };

        internal static void UwpForAllUsersState(ElementStatus value) => WriteStatusLog($"Uwp for all users switch state is: {value}");

        private static List<string> InitLog = new List<string>();
        private static List<string> StatusLog = new List<string>();

        private static string DateTime { get => System.DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"); }

        private static void WriteInfoLog(string record) => InfoLog.Add(record);

        private static void WriteInfoLog(List<string> list) => InfoLog.AddRange(list);

        private static void WriteInitLog(string record) => InitLog.Add($"{DateTime} {record}");

        private static void WriteStatusLog(string record) => StatusLog.Add($"{DateTime} {record}");

        internal static void ActionPerformed(uint actionID, bool actionParameter, double totalSeconds) => WriteStatusLog($"Customization action {actionID} with parameter {actionParameter} completed successfully in {totalSeconds:N0} second(s)");

        internal static void AdvancedSettinsVisibility(bool value) => WriteStatusLog($"Advanced settings is visible: {value}");

        internal static void AppLanguage(string language) => WriteInfoLog($"{APP_LOC}: {language}");

        internal static void AppTheme(string theme) => WriteInfoLog($"{APP_THEME}: {theme}");

        internal static void DebugMode(bool value) => WriteStatusLog($"Debug mode is: {value}");

        internal static void HasException(string message, Exception e)
        {
            ErrorsLog.AddRange(new List<string>()
            {
                $"{DateTime} {message}",
                $"{DateTime} Error information: {e.Message}",
                $"{DateTime} The method that caused the error: {e.TargetSite.Name}"
            });
        }

        internal static void HasUpdateRelease(ReleaseDto release) => WriteInfoLog(new List<string>()
        {
            $"New version is available: {release.tag_name}",
            $"Is prerelease: {release.prerelease}",
            $"Is draft: {release.draft}"
        });

        internal static void HasUpdateResponse() => WriteInfoLog("When checking for an update, a response was received from the update server");

        internal static void IsNewRelease() => WriteInfoLog("The update can be done");

        internal static void LinkClicked(string link) => WriteStatusLog($"Clicked link: \"{link}\"");

        internal static void OsConditionChanged(ICondition condition) => WriteStatusLog($"{condition.Tag} is: {condition.Result}");

        internal static void Save(string path) => File.WriteAllLines(path, InfoLog.Split(string.Empty).Merge(ErrorsLog).Split(string.Empty).Merge(InitLog).Split(string.Empty).Merge(StatusLog));

        internal static void SelectedLocalization(string localization) => WriteStatusLog($"Localization selected: {localization}");

        internal static void SelectedTheme(string value) => WriteStatusLog($"Theme selected: {value}");

        internal static void StartApplyingSettings(int actionsCount) => WriteStatusLog($"Started applying {actionsCount} setting(s)");

        internal static void StartInitOsConditions() => WriteStatusLog("Starting the initial OS conditions");

        internal static void StartInitTextedElements() => WriteStatusLog("Started initialization of texted elements");

        internal static void StartInitUwpApps() => WriteStatusLog("Started initialization of uwp elements");

        internal static void StartResetTextedElements() => WriteStatusLog("Started reset texted elements status");

        internal static void StopApplyingSettings(double totalSeconds) => WriteStatusLog($"Applying the setting(s) took {totalSeconds:N0} second(s)");

        internal static void StopInitOsConditions(double totalSeconds) => WriteStatusLog($"It took {totalSeconds:N0} second(s) to initialize Os conditions");

        internal static void StopInitTextedElements(double totalSeconds) => WriteStatusLog($"It took {totalSeconds:N0} second(s) to initialize texted elements");

        internal static void StopInitUwpApps(double totalSeconds) => WriteStatusLog($"It took {totalSeconds:N0} second(s) to initialize uwp elements");

        internal static void StopResetTextedElements(double totalSeconds) => WriteStatusLog($"It took {totalSeconds:N0} second(s) to reset texted elements");

        internal static void TextedElementChanged(uint elementID, ElementStatus elementStatus) => WriteStatusLog($"The element {elementID} has changed status to: {elementStatus}");

        internal static void TextedElementInit(uint elementID, double totalSeconds) => WriteInitLog($"The element {elementID} was initialized in {totalSeconds:N3} second(s)");

        internal static void UpdateNotNecessary() => WriteInfoLog("No update required");

        internal static void VisibleViewChanged(string value) => WriteStatusLog($"Active view is: {value}");
    }
}