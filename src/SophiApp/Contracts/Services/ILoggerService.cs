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
        /// Write <see cref="OsProperties"/> data in the log.
        /// </summary>
        /// <param name="properties">Encapsulates OS properties.</param>
        void LogOsProperties(OsProperties properties);

        /// <summary>
        /// Write app properties in the log.
        /// </summary>
        /// <param name="version">App version.</param>
        /// <param name="directory">App work directory.</param>
        void LogAppProperties(Version version, string directory);

        /// <summary>
        /// Write result of Internet access check in the log.
        /// </summary>
        /// <param name="isOnline">Result of Internet access check.</param>
        void LogIsOnline(bool isOnline);

        /// <summary>
        /// Write the page navigation in the log.
        /// </summary>
        /// <param name="name">The name of the page are navigating to.</param>
        void LogNavigateToPage(string name);

        /// <summary>
        /// Write the app theme change in the log.
        /// </summary>
        /// <param name="theme">Specifies a app UI theme.</param>
        void LogChangeTheme(ElementTheme theme);

        /// <summary>
        /// Write the opened url in the log.
        /// </summary>
        /// <param name="url">Openable url.</param>
        void LogOpenedUrl(string url);

        /// <summary>
        /// Write the bitness of the os in the log.
        /// </summary>
        /// <param name="is64BitOs">Indicates whether the os is 64 bit.</param>
        void LogOsBitness(bool is64BitOs);

        /// <summary>
        /// Write the WMI state in the log.
        /// </summary>
        /// <param name="serviceState">The WMI service state.</param>
        /// <param name="repositoryExitCode">The verify WMI repository exit code.</param>
        /// <param name="repositoryIsConsistent">The WMI repository is consistent.</param>
        void LogWMIState(ServiceControllerStatus serviceState, int repositoryExitCode, bool repositoryIsConsistent);

        /// <summary>
        /// Write the detected malware in the log.
        /// </summary>
        /// <param name="name">Malware name.</param>
        void LogMalwareDetected(string name);

        /// <summary>
        /// Write the available version of the app in the log.
        /// </summary>
        /// <param name="version">Available app version.</param>
        void LogAppUpdate(Version version);

        /// <summary>
        /// Write the start of all models build in the log.
        /// </summary>
        void LogStartModelsBuild();

        /// <summary>
        /// Write number of models built in the log.
        /// </summary>
        /// <param name="count">Number of models built.</param>
        void LogAllModelsBuilt(int count);

        /// <summary>
        /// Write the start of all models state in the log.
        /// </summary>
        void LogStartModelsGetState();

        /// <summary>
        /// Write the start all models set state in applicable models collection in the log.
        /// </summary>
        void LogStartApplicableModelsSetState();

        /// <summary>
        /// Write the spent time taken by all models to get the state in the log.
        /// </summary>
        /// <param name="timer">Models get state spent time.</param>
        /// <param name="count">Number of models.</param>
        void LogAllModelsGetState(Stopwatch timer, int count);

        /// <summary>
        /// Write the spent time taken by all models to set the state in the log.
        /// </summary>
        /// <param name="timer">Models set state spent time.</param>
        /// <param name="count">Number of models.</param>
        void LogAllModelsSetState(Stopwatch timer, int count);

        /// <summary>
        /// Write the cancel models set state in applicable models collection in the log.
        /// </summary>
        void LogAllModelsSetStateCanceled();

        /// <summary>
        /// Write the spent time taken by one model to get the state in the log.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="timer">Model get state spent time.</param>
        void LogModelGetState(string name, Stopwatch timer);

        /// <summary>
        /// Write the spent time taken by one model to set the state in the log.
        /// </summary>
        /// <param name="name">The model name.</param>
        /// <param name="timer">Model set state spent time.</param>
        void LogModelSetState(string name, Stopwatch timer);

        /// <summary>
        /// Write the model state changed to the log.
        /// </summary>
        /// <typeparam name="T">Type of model state.</typeparam>
        /// <param name="name">The model name.</param>
        /// <param name="state">The model state.</param>
        void LogModelState<T>(string name, T state)
             where T : struct;

        /// <summary>
        /// Write information about the canceling of a applicable models collection in the log.
        /// </summary>
        void LogApplicableModelsCanceled();

        /// <summary>
        /// Write information about the deletion of all models in the applied collection in the log.
        /// </summary>
        void LogApplicableModelsClear();

        /// <summary>
        /// Write information about deleting a model from the applied collection in the log.
        /// </summary>
        /// <param name="name">Deleted model name.</param>
        void LogApplicableModelRemoved(string name);

        /// <summary>
        /// Write information about changes model parameters from the applied collection in the log.
        /// </summary>
        /// <typeparam name="T">A parameters type.</typeparam>
        /// <param name="name">Changed model name.</param>
        /// <param name="previous">Previous parameter value.</param>
        /// <param name="current">Current parameter value.</param>
        void LogApplicableModelChanged<T>(string name, T previous, T current)
            where T : struct;

        /// <summary>
        /// Write information about adding a model to the applied collection in the log.
        /// </summary>
        /// <param name="name">Added model name.</param>
        void LogApplicableModelAdded(string name);

        /// <summary>
        /// Write information about adding a model with parameter to the applied collection in the log.
        /// </summary>
        /// <typeparam name="T">A <paramref name="parameter"/> type.</typeparam>
        /// <param name="name">Added model name.</param>
        /// <param name="parameter">Parameter used by the model.</param>
        void LogApplicableModelAdded<T>(string name, T parameter)
            where T : struct;

        /// <summary>
        /// Write information about state a "For all users" checkbox in the UWP page.
        /// </summary>
        /// <param name="state">A "For all users" checkbox state.</param>
        void LogUwpForAllUsersState(bool state);

        /// <summary>
        /// Write information about the resizing of UI elements description text.
        /// </summary>
        /// <param name="size">A description text size.</param>
        void LogDescriptionTextSizeChanged(int size);

        /// <summary>
        /// Write information about the resizing of UI elements title text.
        /// </summary>
        /// <param name="size">A title text size.</param>
        void LogTitleTextSizeChanged(int size);

        /// <summary>
        /// Write information about text search in UI elements title or description.
        /// </summary>
        /// /// <param name="text">A searched text.</param>
        void LogStartTextSearch(string text);

        /// <summary>
        /// Write the time spent searching for text in models.
        /// </summary>
        /// <param name="timer">Time spent on search.</param>
        /// <param name="count">Number of found models.</param>
        void LogStopTextSearch(Stopwatch timer, int count);

        /// <summary>
        /// Write <see cref="RequirementsFailure"/> reason in the <see cref="IRequirementsService"/> in the log.
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
        /// Handles an unhandled exception occur during app executing.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogUnhandledException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to register as sender API in the <see cref="IAppNotificationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogRegisterNotificationSenderException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to os update in the <see cref="IUpdateService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogOsUpdateException(Exception exception);

        /// <summary>
        /// Handles an exception when accessing to WMI API in the <see cref="IRequirementsService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogWMIStateException(Exception exception);

        /// <summary>
        /// Handles occur during the EventLog service is broken.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogEventLogException(Exception exception);

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
        /// Handles occur during the Microsoft Defender services not found.
        /// </summary>
        /// <param name="service">Microsoft Defender service name.</param>
        void LogMsDefenderServiceNotFound(string service);

        /// <summary>
        /// Handles an exception when accessing to get UI model state.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogModelGetStateException(string name, Exception exception);

        /// <summary>
        /// Handles an exception when accessing to set UI model state.
        /// </summary>
        /// <typeparam name="T">Type of model parameter.</typeparam>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        /// <param name="name">Model name.</param>
        /// <param name="parameter">Model parameter.</param>
        void LogModelSetStateException<T>(Exception exception, string name, T parameter)
            where T : struct;
    }
}
