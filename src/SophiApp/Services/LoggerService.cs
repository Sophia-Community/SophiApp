// <copyright file="LoggerService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Diagnostics;
    using System.Globalization;
    using System.ServiceProcess;
    using Microsoft.UI.Xaml;
    using Serilog;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class LoggerService : ILoggerService
    {
        private readonly string logFile = $"{AppContext.BaseDirectory}\\Log\\SophiApp-{Environment.MachineName.ToUpper()}.log";

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggerService"/> class.
        /// </summary>
        public LoggerService()
        {
            logFile.TryDelete();
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Infinite,
                    outputTemplate: "[{Level:u3}] {Message}{NewLine}")
                .CreateLogger();
        }

        /// <inheritdoc/>
        public void LogOsProperties(OsProperties properties)
        {
            // TODO: Log switch here
            Log.Information("Windows version: {Caption:l}", properties.Caption);
            Log.Information("Windows edition: {Edition:l}", properties.Edition);
            Log.Information("Windows build: {Build}.{Ubr}", properties.BuildNumber, properties.UpdateBuildRevision);
            Log.Information("Computer name: {ComputerName:l}", properties.CSName);
            Log.Information("User name: {UserName:l}", Environment.UserName);
            Log.Information("User culture: {Culture:l}", CultureInfo.CurrentCulture.EnglishName);
            Log.Information("User region: {Region:l}", RegionInfo.CurrentRegion.EnglishName);
            Log.Information("User date and time: {DateTime:l}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            Log.Information("User time zone: {TimeZone:l}", TimeZoneInfo.Local.DisplayName);
        }

        /// <inheritdoc/>
        public void LogAppProperties(Version version, string directory)
        {
            Log.Information("App version: {Version:l}", version);
            Log.Information("App directory: {Directory}", directory);
        }

        /// <inheritdoc/>
        public void LogNavigateToPage(string name) => Log.Information("Navigate to page: {Name:l}", name);

        /// <inheritdoc/>
        public void LogChangeTheme(ElementTheme theme) => Log.Information("Change theme to: {Theme}", theme);

        /// <inheritdoc/>
        public void LogOpenedUrl(string url) => Log.Information("Opened url: {Url}", url);

        /// <inheritdoc/>
        public void LogOsBitness(bool is64BitOs) => Log.Information("Is x64: {Is64BitOs}", is64BitOs);

        /// <inheritdoc/>
        public void LogWMIState(ServiceControllerStatus serviceState, int repositoryExitCode, bool repositoryIsConsistent)
        {
            Log.Information("WMI service state: {ServiceState}", serviceState);
            Log.Information("WMI verify repository exit code: {ExitCode}", repositoryExitCode);
            Log.Information("WMI repository is consistent: {RepositoryIsConsistent}", repositoryIsConsistent);
        }

        /// <inheritdoc/>
        public void LogMalwareDetected(string name) => Log.Warning("Malware detected: {Malware:l} in the {Service:l}", name, nameof(IRequirementsService));

        /// <inheritdoc/>
        public void LogAppUpdate(Version version) => Log.Information("App version available in the repository: {Version:l}", version);

        /// <inheritdoc/>
        public void LogAllModelsBuilt(int count) => Log.Information("Service {Service:l} built models: {Count}", nameof(IModelService), count);

        /// <inheritdoc/>
        public void LogStartModelsGetState() => Log.Warning("Service {Service:l} has started get models state", nameof(IModelService));

        /// <inheritdoc/>
        public void LogStartApplicableModelsSetState() => Log.Warning("Service {Service:l} has started set models state in the applicable models collection", nameof(IModelService));

        /// <inheritdoc/>
        public void LogAllModelsGetState(Stopwatch timer, int count) => Log.Warning("Service {Service:l} took time to get {Count} models state: {TimeSpent}", nameof(IModelService), count, timer.Elapsed);

        /// <inheritdoc/>
        public void LogAllModelsSetState(Stopwatch timer, int count) => Log.Warning("Service {Service:l} took time to set {Count} model(s) state: {TimeSpent}", nameof(IModelService), count, timer.Elapsed);

        /// <inheritdoc/>
        public void LogAllModelsSetStateCanceled() => Log.Warning("Service {Service:l} has cancel set models state in the applicable models collection", nameof(IModelService));

        /// <inheritdoc/>
        public void LogModelGetState(string name, Stopwatch timer) => Log.Information("Model {Name:l} took time to get state: {TimeSpent}", name, timer.Elapsed);

        /// <inheritdoc/>
        public void LogModelSetState(string name, Stopwatch timer) => Log.Information("Model {Name:l} took time to set state: {TimeSpent}", name, timer.Elapsed);

        /// <inheritdoc/>
        public void LogModelState<T>(string name, T state)
               where T : struct => Log.Information("Model {Name:l} state: {State}", name, state);

        /// <inheritdoc/>
        public void LogApplicableModelsCanceled() => Log.Warning("The applying of customizations has been canceled");

        /// <inheritdoc/>
        public void LogApplicableModelsClear() => Log.Warning("Applicable models collection has been cleaned up");

        /// <inheritdoc/>
        public void LogApplicableModelChanged<T>(string name, T previous, T current)
            where T : struct
        {
            Log.Information("The value of the model {Name:l} parameter has been changed from {Previous} to {Current} in applicable models collection", name, previous, current);
        }

        /// <inheritdoc/>
        public void LogApplicableModelRemoved(string name) => Log.Information("Model {Name:l} has been removed from applicable models collection", name);

        /// <inheritdoc/>
        public void LogApplicableModelAdded(string name) => Log.Information("Model {Name:l} has been added to applicable models collection", name);

        /// <inheritdoc/>
        public void LogApplicableModelAdded<T>(string name, T parameter)
            where T : struct
        {
            Log.Information("Model {Name:l} with parameter: {Parameter} has been added to applicable models collection", name, parameter);
        }

        /// <inheritdoc/>
        public void LogUwpForAllUsersState(bool state) => Log.Information("The UWP For All Users checkbox state has been changed to: {State}", state);

        /// <inheritdoc/>
        public void LogDescriptionTextSizeChanged(int size) => Log.Information("The text size of UI element descriptions set to: {Size}", size);

        /// <inheritdoc/>
        public void LogTitleTextSizeChanged(int size) => Log.Information("The text size of UI element headers set to: {Size}", size);

        /// <inheritdoc/>
        public void LogStartTextSearch(string text) => Log.Information("A search for the text: {Text} has been launched", text);

        /// <inheritdoc/>
        public void LogStopTextSearch(Stopwatch timer, int count) => Log.Information("The text search took seconds: {Seconds} and return models: {Count}", timer.Elapsed.TotalSeconds, count);

        /// <inheritdoc/>
        public void LogNavigateToRequirementsFailure(RequirementsFailure failure) => Log.Information("Failure to meet {Service:l} requirements due to: {Name:l}", nameof(IRequirementsService), failure);

        /// <inheritdoc/>
        public void LogOsPropertiesException(Exception exception) => Log.Error(exception, "Failed to obtain the {Property:l} in the {Service:l}: {Message:l}", nameof(OsProperties), nameof(IInstrumentationService), exception.Message);

        /// <inheritdoc/>
        public void LogUwpAppsManagementException(Exception exception) => Log.Error(exception, "Failed to obtain UWP apps update API in the {Service:l}: {Message:l}", nameof(IInstrumentationService), exception.Message);

        /// <inheritdoc/>
        public void LogProcessOwnerException(Exception exception) => Log.Error(exception, "Failed to obtain process owner API in the {Service:l}: {Message:l}", nameof(IInstrumentationService), exception.Message);

        /// <inheritdoc/>
        public void LogAntivirusProductsException(Exception exception) => Log.Error(exception, "Failed to obtain antivirus product API in the {Service:l}: {Message:l}", nameof(IInstrumentationService), exception.Message);

        /// <inheritdoc/>
        public void LogUnhandledException(Exception exception) => Log.Fatal(exception, "AN UNHANDLED EXCEPTION OCCURED: {Message:l}", exception.Message);

        /// <inheritdoc/>
        public void LogRegisterNotificationSenderException(Exception exception) => Log.Error(exception, "Failed to obtain register as sender API in the {Service:l}: {Message:l}", nameof(IAppNotificationService), exception.Message);

        /// <inheritdoc/>
        public void LogOsUpdateException(Exception exception) => Log.Error(exception, "Failed to obtain os update API in the {Service:l}: {Message:l}", nameof(IUpdateService), exception.Message);

        /// <inheritdoc/>
        public void LogWMIStateException(Exception exception) => Log.Error(exception, "Failed to obtain WMI state requirements in the {Service:l}: {Message:l}", nameof(IRequirementsService), exception.Message);

        /// <inheritdoc/>
        public void LogEventLogException(Exception exception) => Log.Error(exception, "The EventLog broken or removed: {Message:l}", exception.Message);

        /// <inheritdoc/>
        public void LogAppUpdateException(Exception exception) => Log.Error(exception, "Failed to obtain app update requirements in the {Service:l}: {Message:l}", nameof(IRequirementsService), exception.Message);

        /// <inheritdoc/>
        public void LogMsDefenderFilesException(string file) => Log.Error("Microsoft Defender file missing: {File}", file);

        /// <inheritdoc/>
        public void LogMsDefenderServiceNotFound(string service) => Log.Error("Microsoft Defender service: {Service:l} not found", service);

        /// <inheritdoc/>
        public void LogModelGetStateException(string name, Exception exception) => Log.Error("An error occurred while get state in the model {Model:l}: {Message:l}", name, exception.Message);

        /// <inheritdoc/>
        public void LogModelSetStateException<T>(Exception exception, string name, T parameter)
            where T : struct => Log.Error(exception, "An error occurred while set state in the model {Model:l} with parameter {Parameter}: {Message:l}", name, parameter, exception.Message);
    }
}
