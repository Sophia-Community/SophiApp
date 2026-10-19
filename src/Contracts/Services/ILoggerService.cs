// <copyright file="ILoggerService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using Microsoft.UI.Xaml;
    using SophiApp.Helpers;
    using SophiApp.Views;
    using System;
    using System.Diagnostics;

    /// <summary>
    /// A service for working with app log.
    /// </summary>
    public interface ILoggerService
    {
        /// <summary>
        /// Gets log folder path.
        /// </summary>
        string LogFolder { get; init; }

        /// <summary>
        /// Gets log file path.
        /// </summary>
        string LogFile { get; init; }

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
        /// Handles occur during the Microsoft Defender files is missing exception.
        /// </summary>
        /// <param name="file">A missing file.</param>
        void LogDefenderFileMissing(string file);

        /// <summary>
        /// Write the opened url in the log.
        /// </summary>
        /// <param name="url">Openable url.</param>
        void LogOpenedUrl(string url);

        /// <summary>
        /// Write OneDrive setup file path in the log.
        /// </summary>
        /// <param name="path">OneDrive setup file path.</param>
        void LogOneDriveSetupFile(string path);

        /// <summary>
        /// Write the number of user files remaining after uninstall OneDrive in the log.
        /// </summary>
        /// <param name="path">OneDrive user folder path.</param>
        /// <param name="count">Number of files remaining after uninstall OneDrive.</param>
        void LogOneDriveUserFilesExist(string path, int count);

        /// <summary>
        /// Write the detected malware in the log.
        /// </summary>
        /// <param name="name">Malware name.</param>
        void LogMalwareDetected(string name);

        /// <summary>
        /// Write if new app version found in the log.
        /// </summary>
        /// <param name="version">Available app version.</param>
        void LogAppNewVersionFound(Version version);

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
        /// Write information about changes applicable model parameter in the log.
        /// </summary>
        /// <typeparam name="T">A parameters type.</typeparam>
        /// <param name="name">Model name.</param>
        /// <param name="value">Parameter value.</param>
        void LogApplicableModelChanged<T>(string name, T value)
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
        /// Write Microsoft Defender services state in the log.
        /// </summary>
        /// <param name="name">Microsoft Defender services name.</param>
        /// <param name="isExist">Microsoft Defender services state.</param>
        void LogDefenderServiceExist(string name, bool isExist);

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
        /// Write <see cref="LogPage"/> visibility in the log.
        /// </summary>
        /// <param name="visible">Log page visibility state.</param>
        void LogPageVisibility(bool visible);

        /// <summary>
        /// Write information about the resizing of UI elements title text.
        /// </summary>
        /// <param name="size">A title text size.</param>
        void LogTitleTextSizeChanged(int size);

        /// <summary>
        /// Write the time spent searching for text in models.
        /// </summary>
        /// <param name="text">A searched text.</param>
        /// <param name="timer">Time spent on search.</param>
        /// <param name="count">Number of found models.</param>
        void LogStopTextSearch(string text, Stopwatch timer, int count);

        /// <summary>
        /// Write url available state in the log.
        /// </summary>
        /// <param name="url">Checked url.</param>
        /// <param name="state">Url available state.</param>
        void LogUrlIsAvailable(string url, bool state);

        /// <summary>
        /// Write the spent time taken by <see cref="RequirementAction"/> execute in the log.
        /// </summary>
        /// <param name="name">Requirements action name.</param>
        /// <param name="timer">Time spent on execute.</param>
        void LogRequirementsActionExecute(string name, Stopwatch timer);

        /// <summary>
        /// Write requirements failure result in the log.
        /// </summary>
        /// <param name="result">Result of requirements action execution.</param>
        void LogRequirementsFailureResult(RequirementsResult result);

        /// <summary>
        /// Handles an exception when accessing to <see cref="OsProperties"/> in the <see cref="IInstrumentationService"/>.
        /// </summary>
        /// <param name="exception">Represents errors that occur during <see cref="OsProperties"/> are retrieved.</param>
        void LogOsPropertiesException(Exception exception);

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
        /// Write Microsoft Defender control folder state in the log.
        /// </summary>
        /// <param name="state">Microsoft Defender control folder state.</param>
        void LogDefenderControlledFolderState(byte state);

        /// <summary>
        /// Write Microsoft Defender is default AV in the log.
        /// </summary>
        /// <param name="isDefault">Microsoft Defender is default AV.</param>
        void LogDefenderIsDefault(bool isDefault);

        /// <summary>
        /// Handles occur exception of the Get-MpPreference cmdlet execution.
        /// </summary>
        /// <param name="exception">Represents errors that occur during app executing.</param>
        void LogDefenderControlledFolderException(Exception exception);

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
