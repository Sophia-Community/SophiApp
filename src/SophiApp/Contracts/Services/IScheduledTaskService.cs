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
        /// Deletes tasks and folder in the Task Scheduler.
        /// </summary>
        /// <param name="folders">Names of folders to delete.</param>
        void DeleteTaskFolders(string[] folders);

        /// <summary>
        /// Get task or null by specified names.
        /// </summary>
        /// <param name="names">Task names to be searched.</param>
        /// <param name="searchAllFolders">if set to true search all sub folders.</param>
        IEnumerable<Task?> FindTaskOrDefault(string[] names, bool searchAllFolders = true);

        /// <summary>
        /// Gets the task or null with the specified path.
        /// </summary>
        /// <param name="taskPath">The task path.</param>
        Task GetTaskOrDefault(string taskPath);

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
        /// Unregisters the "Windows Cleanup Notification" task in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterCleanupNotificationTask();

        /// <summary>
        /// Registers the "Software Distribution" task in the Task Scheduler.
        /// </summary>
        void RegisterSoftwareDistributionTask();

        /// <summary>
        /// Unregisters the "Software Distribution" task in the Task Scheduler and remove task files.
        /// </summary>
        void UnregisterSoftwareDistributionTask();

        /// <summary>
        /// Deletes a folder if there are no tasks in it.
        /// </summary>
        /// <param name="name">Name of the folder to delete.</param>
        void TryDeleteTaskFolder(string name);
    }
}
