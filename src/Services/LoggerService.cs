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
        private readonly ShellViewModel shellViewModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerService"/> class.
        /// </summary>
        public LoggerService()
        {
            LogFolder = Path.Combine(AppContext.BaseDirectory, "Log");
            LogFile = Path.Combine(LogFolder, $"SophiApp-{Environment.MachineName.ToUpper()}.log");
            LogFile.TryDelete();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    LogFile,
                    rollingInterval: RollingInterval.Infinite,
                    outputTemplate: "{Message}{NewLine}")
                .CreateLogger();

            shellViewModel = App.GetService<ShellViewModel>();
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
            Log.Information("Windows build: {Build}.{Ubr}", properties.Build, properties.UBR);
            Log.Information("Computer name: {ComputerName:l}", properties.ComputerName);
            Log.Information("User name: {UserName:l}", Environment.UserName);
            Log.Information("User culture: {Culture:l}", CultureInfo.CurrentCulture.EnglishName);
            Log.Information("User region: {Region:l}", RegionInfo.CurrentRegion.EnglishName);
            Log.Information("User date and time: {DateTime:l}", dateTime);
            Log.Information("User time zone: {TimeZone:l}", TimeZoneInfo.Local.DisplayName);

            shellViewModel.LoggedActions =
            [
                $"Windows version: {properties.Caption}", $"Windows edition: {properties.Edition}", $"Windows build: {properties.Build}.{properties.UBR}",
                $"Computer name: {properties.ComputerName}", $"User name: {Environment.UserName}", $"User culture: {CultureInfo.CurrentCulture.EnglishName}",
                $"User region: {RegionInfo.CurrentRegion.EnglishName}", $"User date and time: {dateTime}", $"User time zone: {TimeZoneInfo.Local.DisplayName}",
            ];
        }

        /// <inheritdoc/>
        public void LogAppProperties(Version version, string directory)
        {
            Log.Information("App version: {Version:l}", version);
            Log.Information("App directory: {Directory}", directory);
            shellViewModel.LoggedActions.AddRange([$"App version: {version}", $"App directory: {directory}"]);
        }

        /// <inheritdoc/>
        public void LogNavigateToPage(string name)
        {
            Log.Information("Navigate to: {Name:l}", name);
            shellViewModel.LoggedActions.Add($"Navigate to: {name}");
        }

        /// <inheritdoc/>
        public void LogChangeTheme(ElementTheme theme)
        {
            Log.Information("Change theme to: {Theme}", theme);
            shellViewModel.LoggedActions.Add($"Change theme to: {theme}");
        }

        /// <inheritdoc/>
        public void LogDefenderFileMissing(string file)
        {
            Log.Error("[WRN] Microsoft Defender file missing: {File}", file);
            shellViewModel.LoggedActions.Add($"[WRN] Microsoft Defender file missing: {file}");
        }

        /// <inheritdoc/>
        public void LogOpenedUrl(string url)
        {
            Log.Information("Opened url: {Url}", url);
            shellViewModel.LoggedActions.Add($"Opened url: {url}");
        }

        /// <inheritdoc/>
        public void LogOneDriveSetupFile(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                Log.Information("OneDrive setup file not found in PC");
                shellViewModel.LoggedActions.Add("OneDrive setup file not found in PC");
                return;
            }

            Log.Information("OneDrive setup file found: {Path}", path);
            shellViewModel.LoggedActions.Add($"OneDrive setup file found: \"{path}\"");
        }

        /// <inheritdoc/>
        public void LogOneDriveUserFilesExist(string path, int count)
        {
            Log.Information("After uninstalling OneDrive, {Count:l} file(s) left in the {Path} folder", count, path);
            shellViewModel.LoggedActions.Add($"After uninstalling OneDrive, {count} file(s) left in the \"{path}\" folder");
        }

        /// <inheritdoc/>
        public void LogMalwareDetected(string name)
        {
            Log.Warning("[WRN] {Service:l} detect malware: {Malware:l}", nameof(IRequirementsService), name);
            shellViewModel.LoggedActions.Add($"[WRN] {nameof(IRequirementsService)} detect malware: {name}");
        }

        /// <inheritdoc/>
        public void LogAppNewVersionFound(Version version)
        {
            Log.Information("App version available in the repository: {Version:l}", version);
            shellViewModel.LoggedActions.Add($"App version available in the repository: {version}");
        }

        /// <inheritdoc/>
        public void LogStartModelsGetState()
        {
            Log.Information("{Service:l} has started model initialization", nameof(IModelService));
            shellViewModel.LoggedActions.Add($"{nameof(IModelService)} has started model initialization");
        }

        /// <inheritdoc/>
        public void LogStartApplicableModelsSetState()
        {
            Log.Information("{Service:l} has started set customizations in the applicable collection", nameof(IModelService));
            shellViewModel.LoggedActions.Add($"{nameof(IModelService)} has started set customizations state in the applicable collection");
        }

        /// <inheritdoc/>
        public void LogAllModelsGetState(Stopwatch timer, int count)
        {
            Log.Information("{Service:l} took time to get {Count} models state: {TimeSpent}", nameof(IModelService), count, timer.Elapsed);
            shellViewModel.LoggedActions.Add($"{nameof(IModelService)} took time to get {count} models state: {timer.Elapsed}");
        }

        /// <inheritdoc/>
        public void LogModelGetState(string name, Stopwatch timer)
        {
            Log.Information("{Name:l} took time to get state: {TimeSpent}", name, timer.Elapsed);
            shellViewModel.LoggedActions.Add($"{name} took time to get state: {timer.Elapsed}");
        }

        /// <inheritdoc/>
        public void LogModelSetState(string name, Stopwatch timer)
        {
            Log.Information("{Name:l} took time to set state: {TimeSpent}", name, timer.Elapsed);
            shellViewModel.LoggedActions.Add($"{name} took time to set state: {timer.Elapsed}");
        }

        /// <inheritdoc/>
        public void LogModelState<T>(string name, T state)
            where T : struct
        {
            Log.Information("{Name:l} has state: {State}", name, state);
            shellViewModel.LoggedActions.Add($"{name} has state: {state}");
        }

        /// <inheritdoc/>
        public void LogApplicableModelsCanceled()
        {
            Log.Information("The applying of customizations has been canceled by user");
            shellViewModel.LoggedActions.Add($"The applying of customizations has been canceled by user");
        }

        /// <inheritdoc/>
        public void LogApplicableModelsClear()
        {
            Log.Information("Applicable collection has been cleaned up");
            shellViewModel.LoggedActions.Add($"Applicable collection has been cleaned up");
        }

        /// <inheritdoc/>
        public void LogApplicableModelChanged<T>(string name, T value)
            where T : struct
        {
            Log.Information("The parameter value of customization {Name:l} has been changed to {Value} in applicable collection", name, value);
            shellViewModel.LoggedActions.Add($"The parameter value of the customization {name} has been changed to {value} in applicable collection");
        }

        /// <inheritdoc/>
        public void LogApplicableModelRemoved(string name)
        {
            Log.Information("{Name:l} has been removed from applicable collection", name);
            shellViewModel.LoggedActions.Add($"{name} has been removed from applicable collection");
        }

        /// <inheritdoc/>
        public void LogApplicableModelAdded(string name)
        {
            Log.Information("{Name:l} has been added to applicable customization collection", name);
            shellViewModel.LoggedActions.Add($"{name} has been added to applicable customization collection");
        }

        /// <inheritdoc/>
        public void LogApplicableModelAdded<T>(string name, T parameter)
            where T : struct
        {
            Log.Information("{Name:l} with parameter {Parameter} has been added to applicable customization collection", name, parameter);
            shellViewModel.LoggedActions.Add($"{name} with parameter {parameter} has been added to applicable customization collection");
        }

        /// <inheritdoc/>
        public void LogDefenderServiceExist(string name, bool isExist)
        {
            Log.Information("Microsoft Defender services {Name:l} is exist: {State:l}", name, isExist);
            shellViewModel.LoggedActions.Add($"Microsoft Defender services {name} is exist: {isExist}");
        }

        /// <inheritdoc/>
        public void LogUwpForAllUsersState(bool state)
        {
            Log.Information("The UWP For All Users checkbox state has been changed to: {State}", state);
            shellViewModel.LoggedActions.Add($"The UWP For All Users checkbox state has been changed to: {state}");
        }

        /// <inheritdoc/>
        public void LogDescriptionTextSizeChanged(int size)
        {
            Log.Information("The text size of UI element descriptions set to: {Size}", size);
            shellViewModel.LoggedActions.Add($"The text size of UI element descriptions set to: {size}");
        }

        /// <inheritdoc/>
        public void LogPageVisibility(bool visible)
        {
            Log.Information("Log page is visible: {IsVisible}", visible);
            shellViewModel.LoggedActions.Add($"Log page is visible: {visible}");
        }

        /// <inheritdoc/>
        public void LogTitleTextSizeChanged(int size)
        {
            Log.Information("The text size of UI element headers set to: {Size}", size);
            shellViewModel.LoggedActions.Add($"The text size of UI element headers set to: {size}");
        }

        /// <inheritdoc/>
        public void LogStopTextSearch(string text, Stopwatch timer, int count)
        {
            Log.Information("A search for the text {Text} took {Seconds} seconds and return {Count} customization(s)", text, timer.Elapsed.TotalSeconds, count);
            shellViewModel.LoggedActions.Add($"A search for the text \"{text}\" took {timer.Elapsed.TotalSeconds} seconds and return {count} customization(s)");
        }

        /// <inheritdoc/>
        public void LogUrlIsAvailable(string url, bool state)
        {
            Log.Information("The url {Url} is available: {State}", url, state);
            shellViewModel.LoggedActions.Add($"The url \"{url}\" is available: {state}");
        }

        /// <inheritdoc/>
        public void LogRequirementsActionExecute(string name, Stopwatch timer)
        {
            Log.Information("{Service:l} took {TimeSpent} to execute {Action:l} action", nameof(IRequirementsService), timer.Elapsed, name);
            shellViewModel.LoggedActions.Add($"{nameof(IRequirementsService)} took {timer.Elapsed} to execute {name} action");
        }

        /// <inheritdoc/>
        public void LogRequirementsFailureResult(RequirementsResult result)
        {
            Log.Information("[WRN] Failure to meet {Service:l} requirements due to {Name} action", nameof(IRequirementsService), result);
            shellViewModel.LoggedActions.Add($"[WRN] Failure to meet {nameof(IRequirementsService)} requirements due to {result} action");
        }

        /// <inheritdoc/>
        public void LogOsPropertiesException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain the {Property:l} in the {Service:l}: {Message}", nameof(OsProperties), nameof(IInstrumentationService), exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] Failed to obtain the {nameof(OsProperties)} in the {nameof(IInstrumentationService)}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogProcessOwnerException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain process owner API in the {Service:l}: {Message}", nameof(IInstrumentationService), exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] Failed to obtain process owner API in the {nameof(IInstrumentationService)}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogAntivirusProductsException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain AntiVirusProduct API in the {Service:l}: {Message}", nameof(IInstrumentationService), exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] Failed to obtain AntiVirusProduct API in the {nameof(IInstrumentationService)}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogUnhandledException(Exception exception) => Log.Fatal(exception, "[ERR] AN UNHANDLED EXCEPTION OCCURED: {Message}", exception.Message);

        /// <inheritdoc/>
        public void LogRegisterNotificationSenderException(Exception exception)
        {
            Log.Error(exception, "[WRN] Failed to obtain register as sender API in the {Service:l}: {Message}", nameof(IAppNotificationService), exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] Failed to obtain register as sender API in the {nameof(IAppNotificationService)}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogDefenderControlledFolderState(byte state)
        {
            Log.Information("Microsoft Defender controlled folder access state: {State}", state);
            shellViewModel.LoggedActions.Add($"Microsoft Defender controlled folder access state: {state}");
        }

        /// <inheritdoc/>
        public void LogDefenderIsDefault(bool isDefault)
        {
            Log.Information("Microsoft Defender is default AV: {IsDefault}", isDefault);
            shellViewModel.LoggedActions.Add($"Microsoft Defender is default AV: {isDefault}");
        }

        /// <inheritdoc/>
        public void LogDefenderControlledFolderException(Exception exception)
        {
            Log.Error("[WRN] Failed to obtain Microsoft Defender EnableControlledFolderAccess value in the {Service:l}: {Message}", nameof(IRequirementsService), exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] Failed to obtain Microsoft Defender EnableControlledFolderAccess value in the {nameof(IRequirementsService)}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogModelGetStateException(string name, Exception exception)
        {
            Log.Error("[WRN] An error occurred while getting state in {Model:l}: {Message}", name, exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] An error occurred while getting state in {name}: {exception.Message}");
        }

        /// <inheritdoc/>
        public void LogModelSetStateException<T>(Exception exception, string name, T parameter)
            where T : struct
        {
            Log.Error("[WRN] An error occurred while set state in {Model:l} with parameter {Parameter}: {Message}", name, parameter, exception.Message);
            shellViewModel.LoggedActions.Add($"[WRN] An error occurred while set state in {name} with parameter {parameter}: {exception.Message}");
        }
    }
}
