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
        private readonly string logFile = $"{AppContext.BaseDirectory}\\Logs\\SophiApp-{Environment.MachineName.ToUpper()}.log";

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
                    outputTemplate: "[{Level:u3}] {Message}{NewLine}{Exception}")
                .CreateLogger();
        }

        /// <inheritdoc/>
        public void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }

        /// <inheritdoc/>
        public void LogOsProperties(OsProperties properties)
        {
            Log.Information("Windows version: {Caption}", properties.Caption);
            Log.Information("Windows edition: {Edition}", properties.Edition);
            Log.Information("Windows build: {Build}.{Ubr}", properties.BuildNumber, properties.UpdateBuildRevision);
            Log.Information("Computer name: {Name}", properties.CSName);
            Log.Information("User name: {Name}", Environment.UserName);
            Log.Information("User culture: {Culture}", CultureInfo.CurrentCulture.EnglishName);
            Log.Information("User region: {Region}", RegionInfo.CurrentRegion.EnglishName);
            Log.Information("User data: {DateTime}", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
            Log.Information("User time zone: {TimeZone}", TimeZoneInfo.Local.DisplayName);
        }

        /// <inheritdoc/>
        public void LogAppProperties(Version version, string directory)
        {
            Log.Information("App version: {Version}", version);
            Log.Information("App directory: {Directory}", directory);
        }

        /// <inheritdoc/>
        public void LogIsOnline(bool isOnline)
        {
            Log.Information("Has Internet access: {IsOnline}", isOnline);
        }

        /// <inheritdoc/>
        public void LogNavigateToPage(string name)
        {
            Log.Information("Navigate to page: {Name}", name);
        }

        /// <inheritdoc/>
        public void LogChangeTheme(ElementTheme theme)
        {
            Log.Information("Change theme to: \"{Theme}\"", theme);
        }

        /// <inheritdoc/>
        public void LogOpenedUrl(string url)
        {
            Log.Information("Opened url: {Url}", url);
        }

        /// <inheritdoc/>
        public void LogOsBitness(bool is64BitOs)
        {
            Log.Information("Is x64: {Is64BitOs}", is64BitOs);
        }

        /// <inheritdoc/>
        public void LogWmiState(ServiceControllerStatus serviceState, string repositoryState, bool repositoryIsConsistent)
        {
            Log.Information("WMI service state: {ServiceState}", serviceState);
            Log.Information("WMI repository state: {RepositoryState}", repositoryState);
            Log.Information("WMI repository is consistent: {RepositoryIsConsistent}", repositoryIsConsistent);
        }

        /// <inheritdoc/>
        public void LogMalwareDetected(string name)
        {
            Log.Warning("Malware detected: {Malware} in the {Service}", name, nameof(IRequirementsService));
        }

        /// <inheritdoc/>
        public void LogAppUpdate(Version version)
        {
            Log.Information("Available app version: {Version}", version);
        }

        /// <inheritdoc/>
        public void LogBuildModels(int count)
        {
            Log.Information("Service {Service} built models: {Count}", nameof(IModelService), count);
        }

        /// <inheritdoc/>
        public void LogStartAllModelGetState()
        {
            Log.Information("Service {Service} has started get models state", nameof(IModelService));
        }

        /// <inheritdoc/>
        public void LogAllModelGetStateCompleted(Stopwatch timer, int count)
        {
            Log.Information("Service {Service} took time to get {Count} models state: {TimeSpent}", nameof(IModelService), count, timer.Elapsed);
        }

        /// <inheritdoc/>
        public void LogModelGetStateCompleted(string name, Stopwatch timer)
        {
            Log.Information("Model {Name} took time to get state: {TimeSpent}", name, timer.Elapsed);
        }

        /// <inheritdoc/>
        public void LogNavigateToRequirementsFailure(RequirementsFailure failure)
        {
            Log.Information("Failure to meet {Service} requirements due to: {Name}", nameof(IRequirementsService), failure);
        }

        /// <inheritdoc/>
        public void LogOsPropertiesException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain the {Property} in the {Service}", nameof(OsProperties), nameof(IInstrumentationService));
        }

        /// <inheritdoc/>
        public void LogUwpAppsManagementException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain UWP apps update API in the {Service}", nameof(IInstrumentationService));
        }

        /// <inheritdoc/>
        public void LogProcessOwnerException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain process owner API in the {Service}", nameof(IInstrumentationService));
        }

        /// <inheritdoc/>
        public void LogAntivirusProductsException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain antivirus product API in the {Service}", nameof(IInstrumentationService));
        }

        /// <inheritdoc/>
        public void LogUserSidException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain user SID API in the {Service}", nameof(IInstrumentationService));
        }

        /// <inheritdoc/>
        public void LogUnhandledException(Exception exception)
        {
            Log.Fatal(exception, "APP UNHANDLED EXCEPTION");
        }

        /// <inheritdoc/>
        public void LogIsOnlineException(Exception exception)
        {
            Log.Error(exception, "Failed Internet access check in the {Service}", nameof(INetworkService));
        }

        /// <inheritdoc/>
        public void LogRegisterAsSenderException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain register as sender API in the {Service}", nameof(IAppNotificationService));
        }

        /// <inheritdoc/>
        public void LogOsUpdateException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain os update API in the {Service}", nameof(IUpdateService));
        }

        /// <inheritdoc/>
        public void LogWmiStateException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain WMI state requirements in the {Service}", nameof(IRequirementsService));
        }

        /// <inheritdoc/>
        public void LogAppUpdateException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain app update requirements in the {Service}", nameof(IRequirementsService));
        }

        /// <inheritdoc/>
        public void LogMsDefenderFilesException(string file)
        {
            Log.Error("Microsoft Defender file missing: {File}", file);
        }

        /// <inheritdoc/>
        public void LogMsDefenderServicesStatusException(string service, ServiceControllerStatus status)
        {
            Log.Error("Microsoft Defender service: {Service} has status: {Status}", service, status);
        }

        /// <inheritdoc/>
        public void LogMsDefenderServicesException(Exception exception)
        {
            Log.Error(exception, "Failed to obtain Microsoft Defender service requirements in the {Service}", nameof(IRequirementsService));
        }

        /// <inheritdoc/>
        public void LogModelGetStateException(string name, Exception exception)
        {
            Log.Error(exception, "An error occurred while get state in the model: {Model}", name);
        }
    }
}
