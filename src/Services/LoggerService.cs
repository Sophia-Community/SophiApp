// <copyright file="LoggerService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using Microsoft.UI.Xaml;
    using Serilog;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;
    using System.Diagnostics;
    using System.Globalization;

    /// <inheritdoc/>
    public class LoggerService : ILoggerService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerService"/> class.
        /// </summary>
        public LoggerService()
        {
            LogFolder = Path.Combine(EnvironmentHelper.AppFolder, "Log");
            LogFile = Path.Combine(LogFolder, $"SophiApp2-{Environment.MachineName.ToUpper()}.log");
            LogFile.TryDelete();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    LogFile,
                    rollingInterval: RollingInterval.Infinite,
                    outputTemplate: "{Message}{NewLine}")
                .CreateLogger();
        }

        /// <inheritdoc/>
        public string LogFolder { get; init; }

        /// <inheritdoc/>
        public string LogFile { get; init; }

        /// <inheritdoc/>
        public void LogOsProperties(OsProperties properties)
        {
            var dateTime = DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss");

            Log.Information("Windows version: {Caption:l}", properties.Caption);
            Log.Information("Windows edition: {Edition:l}", properties.Edition);
            Log.Information("Windows build: {Build}.{Ubr}", properties.MajorBuild, properties.MinorBuild);
            Log.Information("Computer name: {ComputerName:l}", properties.ComputerName);
            Log.Information("User name: {UserName:l}", Environment.UserName);
            Log.Information("User culture: {Culture:l}", CultureInfo.CurrentCulture.EnglishName);
            Log.Information("User region: {Region:l}", RegionInfo.CurrentRegion.EnglishName);
            Log.Information("User date and time: {DateTime:l}", dateTime);
            Log.Information("User time zone: {TimeZone:l}", TimeZoneInfo.Local.DisplayName);
        }

        /// <inheritdoc/>
        public void LogAppProperties(Version version, string directory)
        {
            Log.Information("App version: {Version:l}", version);
            Log.Information("App directory: {Directory}", directory);
        }

        /// <inheritdoc/>
        public void LogNavigateToPage(string name)
        {
            Log.Information("Navigate to: {Name:l}", name);
        }

        /// <inheritdoc/>
        public void LogChangeTheme(ElementTheme theme)
        {
            Log.Information("Change theme to: {Theme}", theme);
        }

        /// <inheritdoc/>
        public void LogChangeLanguage(string language)
        {
            Log.Information("Change language to: {Language:l}. It is used after the app is restarted", language.Length > 0 ? language : "the language of Windows");
        }

        /// <inheritdoc/>
        public void LogDefenderFileMissing(string file)
        {
            Log.Error("[WRN] Microsoft Defender file missing: {File}", file);
        }

        /// <inheritdoc/>
        public void LogOpenedUrl(string url)
        {
            Log.Information("Opened url: {Url}", url);
        }

        /// <inheritdoc/>
        public void LogOneDriveSetupFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Information("OneDrive setup file not found in PC");
                return;
            }

            Log.Information("OneDrive setup file found: {Path}", path);
        }

        /// <inheritdoc/>
        public void LogOneDriveUserFilesExist(string path, int count)
        {
            Log.Information("After uninstalling OneDrive, {Count:l} file(s) left in the {Path} folder", count, path);
        }

        /// <inheritdoc/>
        public void LogMalwareDetected(string name)
        {
            Log.Warning("[WRN] {Service:l} detect malware: {Malware:l}", nameof(IRequirementsService), name);
        }

        /// <inheritdoc/>
        public void LogAppNewVersionFound(Version version)
        {
            Log.Information("App version available in the repository: {Version:l}", version);
        }

        /// <inheritdoc/>
        public void LogStartModelsGetState()
        {
            Log.Information("{Service:l} has started model initialization", nameof(IModelService));
        }

        /// <inheritdoc/>
        public void LogStartApplicableModelsSetState()
        {
            Log.Information("{Service:l} has started set customizations in the applicable collection", nameof(IModelService));
        }

        /// <inheritdoc/>
        public void LogAllModelsGetState(Stopwatch timer, int count)
        {
            Log.Information("{Service:l} took time to get {Count} models state: {TimeSpent}", nameof(IModelService), count, timer.Elapsed);
        }

        /// <inheritdoc/>
        public void LogModelGetState(string name, Stopwatch timer)
        {
            Log.Information("{Name:l} took time to get state: {TimeSpent}", name, timer.Elapsed);
        }

        /// <inheritdoc/>
        public void LogModelSetState(string name, Stopwatch timer)
        {
            Log.Information("{Name:l} took time to set state: {TimeSpent}", name, timer.Elapsed);
        }

        /// <inheritdoc/>
        public void LogModelState<T>(string name, T state)
            where T : struct
        {
            Log.Information("{Name:l} has state: {State}", name, state);
        }

        /// <inheritdoc/>
        public void LogModelState(string name, bool? state)
        {
            Log.Information("{Name:l} has state: {State}", name, state);
        }

        /// <inheritdoc/>
        public void LogApplicableModelsCanceled()
        {
            Log.Information("The applying of customizations has been canceled by user");
        }

        /// <inheritdoc/>
        public void LogApplicableModelsClear()
        {
            Log.Information("Applicable collection has been cleaned up");
        }

        /// <inheritdoc/>
        public void LogApplicableModelChanged<T>(string name, T value)
            where T : struct
        {
            Log.Information("The parameter value of customization {Name:l} has been changed to {Value} in applicable collection", name, value);
        }

        /// <inheritdoc/>
        public void LogApplicableModelChanged(string name, string value)
        {
            Log.Information("The parameter value of customization {Name:l} has been changed to {Value} in applicable collection", name, value);
        }

        /// <inheritdoc/>
        public void LogApplicableModelRemoved(string name)
        {
            Log.Information("{Name:l} has been removed from applicable collection", name);
        }

        /// <inheritdoc/>
        public void LogApplicableModelAdded(string name)
        {
            Log.Information("{Name:l} has been added to applicable customization collection", name);
        }

        /// <inheritdoc/>
        public void LogApplicableModelAdded<T>(string name, T parameter)
            where T : struct
        {
            Log.Information("{Name:l} with parameter {Parameter} has been added to applicable customization collection", name, parameter);
        }

        /// <inheritdoc/>
        public void LogApplicableModelAdded(string name, string parameter)
        {
            Log.Information("{Name:l} with parameter {Parameter} has been added to applicable customization collection", name, parameter);
        }

        /// <inheritdoc/>
        public void LogUwpForAllUsersState(bool state)
        {
            Log.Information("The UWP For All Users checkbox state has been changed to: {State}", state);
        }

        /// <inheritdoc/>
        public void LogStopTextSearch(string text, Stopwatch timer, int count)
        {
            Log.Information("A search for the text {Text} took {Seconds} seconds and return {Count} customization(s)", text, timer.Elapsed.TotalSeconds, count);
        }

        /// <inheritdoc/>
        public void LogUrlIsAvailable(string url, bool state)
        {
            Log.Information("The url {Url} is available: {State}", url, state);
        }

        /// <inheritdoc/>
        public void LogRequirementsActionExecute(string name, Stopwatch timer)
        {
            Log.Information("{Service:l} took {TimeSpent} to execute {Action:l} action", nameof(IRequirementsService), timer.Elapsed, name);
        }

        /// <inheritdoc/>
        public void LogRequirementsFailureResult(RequirementsResult result)
        {
            Log.Information("[WRN] Failure to meet {Service:l} requirements due to {Name} action", nameof(IRequirementsService), result);
        }

        /// <inheritdoc/>
        public void LogOsPropertiesException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain the {Property:l} in the {Service:l}: {Message}", nameof(OsProperties), nameof(IInstrumentationService), exception.Message);
        }

        /// <inheritdoc/>
        public void LogProcessOwnerException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain process owner API in the {Service:l}: {Message}", nameof(IInstrumentationService), exception.Message);
        }

        /// <inheritdoc/>
        public void LogAntivirusProductsException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain AntiVirusProduct API in the {Service:l}: {Message}", nameof(IInstrumentationService), exception.Message);
        }

        /// <inheritdoc/>
        public void LogUnhandledException(Exception exception) => Log.Fatal(exception, "[ERR] AN UNHANDLED EXCEPTION OCCURED: {Message}", exception.Message);

        /// <inheritdoc/>
        public void LogRegisterNotificationSenderException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain register as sender API in the {Service:l}: {Message}", nameof(IAppNotificationService), exception.Message);
        }

        /// <inheritdoc/>
        public void LogDefenderControlledFolderState(byte state)
        {
            Log.Information("Microsoft Defender controlled folder access state: {State}", state);
        }

        /// <inheritdoc/>
        public void LogDefenderIsDefault(bool isDefault)
        {
            Log.Information("Microsoft Defender is default AV: {IsDefault}", isDefault);
        }

        /// <inheritdoc/>
        public void LogPolicyUpdateException(Exception exception)
        {
            Log.Error("[WRN] Failed to write the policies to Registry.pol: {Message}", exception.Message);
        }

        /// <inheritdoc/>
        public void LogRegistryPolicyFound(string key, string name, string value)
        {
            Log.Information("Registry policy found: {Key:l} | {Name:l} | {Value:l}", key, name, value);
        }

        /// <inheritdoc/>
        public void LogComponentStabilityException(string component, Exception exception)
        {
            Log.Error("[WRN] Windows component check failed on {Component}: {Message}", component, exception.Message);
        }

        /// <inheritdoc/>
        public void LogControlledFolderException(Exception exception)
        {
            Log.Error("[WRN] Failed to obtain Microsoft Defender EnableControlledFolderAccess value in the {Service:l}: {Message}", nameof(IRequirementsService), exception.Message);
        }

        /// <inheritdoc/>
        public void LogModelGetStateException(string name, Exception exception)
        {
            Log.Error("[WRN] An error occurred while getting state in {Model:l}: {Message}", name, exception.Message);
        }

        /// <inheritdoc/>
        public void LogModelSetStateException<T>(Exception exception, string name, T parameter)
            where T : struct
        {
            Log.Error("[WRN] An error occurred while set state in {Model:l} with parameter {Parameter}: {Message}", name, parameter, exception.Message);
        }

        /// <inheritdoc/>
        public void LogModelSetStateException(Exception exception, string name, string parameter)
        {
            Log.Error("[WRN] An error occurred while set state in {Model:l} with parameter {Parameter}: {Message}", name, parameter, exception.Message);
        }

        /// <inheritdoc/>
        public void LogModelNotice(string name, string text)
        {
            Log.Information("{Model:l} shows to the user: {Text}", name, text);
        }

        /// <inheritdoc/>
        public void LogSettingWithoutTitle(string name)
        {
            Log.Error("[ERR] {Model:l} has no title: the string \"UIModel_{Name:l}_Title\" is not found", name, name);
        }

        /// <inheritdoc/>
        public void LogUserNotice(string text)
        {
            Log.Information("Shown to the user: {Text}", text);
        }

        /// <inheritdoc/>
        public void LogRestorePointCreated(Stopwatch timer)
        {
            Log.Information("A restore point for the system drive was created in {Elapsed:l}", timer.Elapsed.ToString());
        }

        /// <inheritdoc/>
        public void LogTermsAccepted(int version)
        {
            Log.Information("The user has accepted the terms and the privacy policy, version {Version}", version);
        }

        /// <inheritdoc/>
        public void LogSearchWindowWatch(bool closed, string seen)
        {
            Log.Information("Windows Search window after File Explorer restart was closed: {Closed}. Foreground: {Seen}", closed, seen);
        }

        /// <inheritdoc/>
        public void LogSkippedException(string action, Exception exception)
        {
            Log.Error("[WRN] {Action:l} failed: {Type:l}: {Message}", action, exception.GetType().Name, exception.Message);
        }
    }
}
