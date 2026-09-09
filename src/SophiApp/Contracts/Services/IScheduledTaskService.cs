// <copyright file="IScheduledTaskService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using Microsoft.Win32.TaskScheduler;

    /// <summary>
    /// A service for working with Scheduled Task API.
    /// </summary>
    public interface IScheduledTaskService
    {
        /// <summary>
        /// Get task or null by specified names.
        /// </summary>
        /// <param name="name">Task name to be searched.</param>
        /// <param name="searchAllFolders">If set to true search all sub folders.</param>
        Task? FindTaskOrDefault(string name, bool searchAllFolders = true);

        /// <summary>
        /// Gets the task or null with the specified path.
        /// </summary>
        /// <param name="taskPath">The task path.</param>
        Task? GetTaskOrDefault(string taskPath);

        /// <summary>
        /// Registers the "Windows Cleanup" task in the Task Scheduler.
        /// </summary>
        void RegisterCleanupTask();

        /// <summary>
        /// Unregisters the "Windows Cleanup" task in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterCleanupTask();

        /// <summary>
        /// Registers the "Windows Cleanup Notification" in the Task Scheduler.
        /// </summary>
        void RegisterCleanupNotificationTask();

        /// <summary>
        /// If task is not null set enabled or disabled state.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> instance.</param>
        /// <param name="enabled">A value that indicates the task state.</param>
        void SetState(Task? task, bool enabled);

        /// <summary>
        /// Try stop all instances of the task.
        /// </summary>
        /// <param name="task">The <see cref="Task"/> instance.</param>
        void TryStop(Task? task);

        /// <summary>
        /// Unregisters the "Windows Cleanup Notification" task in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterCleanupNotificationTask();

        /// <summary>
        /// Unregisters OneDrive tasks in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterOneDriveTasks();

        /// <summary>
        /// Registers the "Software Distribution" task in the Task Scheduler.
        /// </summary>
        void RegisterSoftwareDistributionTask();

        /// <summary>
        /// Unregisters the "Software Distribution" task in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterSoftwareDistributionTask();

        /// <summary>
        /// Registers the "Temp" task in the Task Scheduler.
        /// </summary>
        void RegisterTempTask();

        /// <summary>
        /// Unregisters the "Temp" task in the Task Scheduler.
        /// </summary>
        void UnregisterTempTask();

        /// <summary>
        /// Deletes a folder if there are no tasks in it.
        /// </summary>
        /// <param name="name">Name of the folder to delete.</param>
        void TryDeleteTaskFolder(string name);
    }
}
