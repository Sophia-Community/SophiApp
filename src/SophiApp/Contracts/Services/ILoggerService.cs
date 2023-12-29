// <copyright file="ILoggerService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System;
    using System.Diagnostics;
    using System.ServiceProcess;
    using Microsoft.UI.Xaml;
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with app log.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Resets logger to the default and disposes the original if possible.
        /// </summary>
        void CloseAndFlush();

        /// <summary>
        /// Write <see cref="OsProperties"/> data to log.
        /// </summary>
        /// <param name="properties">Encapsulates OS properties.</param>
        void LogOsProperties(OsProperties properties);

        /// <summary>
        /// Write app properties to log.
        /// </summary>
        /// <param name="version">App version.</param>
        /// <param name="directory">App work directory.</param>
        void LogAppProperties(Version version, string directory);

        /// <summary>
        /// Write result of Internet access check to log.
        /// </summary>
        /// <param name="isOnline">Result of Internet access check.</param>
        void LogIsOnline(bool isOnline);

        /// <summary>
        /// Write the page navigation to log.
        /// </summary>
        /// <param name="name">The name of the page are navigating to.</param>
        void LogNavigateToPage(string name);

        /// <summary>
        /// Write the app theme change to log.
        /// </summary>
        /// <param name="theme">Specifies a app UI theme.</param>
        void LogChangeTheme(ElementTheme theme);

        /// <summary>
        /// Write the opened url to log.
        /// </summary>
        /// <param name="url">Openable url.</param>
        void LogOpenedUrl(string url);

        /// <summary>
        /// Write the bitness of the os to log.
        /// </summary>
        /// <param name="is64BitOs">Indicates whether the os is 64 bit.</param>
        void LogOsBitness(bool is64BitOs);

        /// <summary>
        /// Write the WMI state to log.
        /// </summary>
        /// <param name="serviceState">The WMI service state.</param>
        /// <param name="repositoryState">The WMI repository state.</param>
        /// <param name="repositoryIsConsistent">The WMI repository is consistent.</param>
        void LogWmiState(ServiceControllerStatus serviceState, string repositoryState, bool repositoryIsConsistent);

        /// <summary>
        /// Write the detected malware to log.
        /// </summary>
        /// <param name="name">Malware name.</param>
        void LogMalwareDetected(string name);

        /// <summary>
        /// Write the available version of the app to log.
        /// </summary>
        /// <param name="version">Available app version.</param>
        void LogAppUpdate(Version version);

        /// <summary>
        /// Write number of models built to log.
        /// </summary>
        /// <param name="count">Number of models built.</param>
        void LogBuildModels(int count);

        /// <summary>
        /// Write the start of all models state to log.
        /// </summary>
        void LogStartAllModelGetState();

        /// <summary>
        /// Write the completion of all models state to log.
        /// </summary>
        /// <param name="timer">Get models state spent time.</param>
        /// <param name="count">Number of models.</param>
        void LogAllModelGetStateCompleted(Stopwatch timer, int count);

        /// <summary>
        /// Write the completion of one model state to log.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="timer">Get model state spent time.</param>
        void LogModelGetStateCompleted(string name, Stopwatch timer);

        /// <summary>
        /// Write <see cref="RequirementsFailure"/> reason in the <see cref="IRequirementsService"/> to log.
        /// </summary>
        /// <param name="failure">A failure reason.</param>
        void LogNavigateToRequirementsFailure(RequirementsFailure failure);

        /// <summary>
        /// Handles an exception when accessing to <see cref="OsProperties"/> in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during <see cref="OsProperties"/> are retrieved.</param>
        void LogOsPropertiesException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to UWP apps update API in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogUwpAppsManagementException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to process owner in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogProcessOwnerException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to antivirus product API in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogAntivirusProductsException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to user SID API in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogUserSidException(Exception exception);

        /// <summary>
        /// Handles an unhandled exception occur during app executing.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogUnhandledException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to Internet check in the <see cref="INetworkService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogIsOnlineException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to register as sender API in the <see cref="IAppNotificationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogRegisterAsSenderException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to os update in the <see cref="IUpdateService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogOsUpdateException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to WMI API in the <see cref="IRequirementsService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogWmiStateException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to app update in the <see cref="IRequirementsService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogAppUpdateException(Exception exception);

        /// <summary>
        /// Handles occur during the Microsoft Defender files is missing exception.
        /// </summary>
        /// <param name="file">A missing file.</param>
        void LogMsDefenderFilesException(string file);

        /// <summary>
        /// Handles occur during the Microsoft Defender services not running exception.
        /// </summary>
        /// <param name="service">Not running service.</param>
        /// <param name="status">A service status.</param>
        void LogMsDefenderServicesStatusException(string service, ServiceControllerStatus status);

        /// <summary>
        /// Handles an exception when accessing to Microsoft Defender service API in the <see cref="IRequirementsService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogMsDefenderServicesException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to get UI model state.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogModelGetStateException(string name, Exception exception);
    }
}
