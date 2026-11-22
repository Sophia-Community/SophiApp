// <copyright file="Mutators.TaskScheduler.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.Json.Nodes;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <content>
    /// The settings of the "TaskScheduler" page.
    /// </content>
    public static partial class Mutators
    {
        /// <summary>
        /// Set "Windows Cleanup" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void CleanupTask(bool enable)
        {
            var cleanupTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.vbs");
            var notificationTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.vbs");
            cleanupTaskVbs.TryDelete();
            notificationTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                RegistryService.RemoveVolumeCachesStateFlags();
                RegistryService.SetVolumeCachesStateFlags();
                AppNotificationService.RegisterAsToastSender();
                AppNotificationService.RegisterWindowsCleanupAsToastSender();
                ScheduledTaskService.RegisterCleanupTask();
                ScheduledTaskService.RegisterCleanupNotificationTask();
                return;
            }

            ScheduledTaskService.UnregisterCleanupTask();
            ScheduledTaskService.UnregisterCleanupNotificationTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
            RegistryService.RemoveVolumeCachesStateFlags();
            AppNotificationService.UnregisterCleanupProtocol();
        }

        /// <summary>
        /// Set "SoftwareDistribution" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void SoftwareDistributionTask(bool enable)
        {
            var distributionTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.vbs");
            distributionTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender();
                ScheduledTaskService.RegisterSoftwareDistributionTask();
                return;
            }

            ScheduledTaskService.UnregisterSoftwareDistributionTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }

        /// <summary>
        /// Set "Temp" scheduled task state.
        /// </summary>
        /// <param name="enable">Task state.</param>
        public static void TempTask(bool enable)
        {
            var tempTaskVbs = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.vbs");
            tempTaskVbs.TryDelete();

            if (enable)
            {
                AppNotificationService.EnableToastNotification();
                AppNotificationService.RegisterAsToastSender();
                ScheduledTaskService.RegisterTempTask();
                return;
            }

            ScheduledTaskService.UnregisterTempTask();
            ScheduledTaskService.TryDeleteTaskFolder("Sophia");
        }
    }
}
