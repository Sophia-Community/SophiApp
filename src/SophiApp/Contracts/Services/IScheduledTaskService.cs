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
        /// <param name="taskFolders">Names of folders to delete.</param>
        void DeleteTaskFolders(string[] taskFolders);

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
        /// Registers the "Windows Cleanup" in the Task Scheduler.
        /// </summary>
        void RegisterWindowsCleanupTask();

        /// <summary>
        /// Registers the "Windows Cleanup Notification" in the Task Scheduler.
        /// </summary>
        void RegisterWindowsCleanupNotificationTask();

        /// <summary>
        /// Remove files with extensions from the "System32\Tasks" folder.
        /// </summary>
        /// <param name="taskFolder">Names of folders to remove files.</param>
        void RemoveExtensionsFilesFromTaskFolder(string taskFolder);
    }
}
