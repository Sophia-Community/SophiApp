// <copyright file="Accessors.TaskScheduler.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <content>
    /// The settings of the "TaskScheduler" page.
    /// </content>
    public static partial class Accessors
    {
        /// <summary>
        /// Get Windows Cleanup scheduled task state.
        /// </summary>
        public static bool CleanupTask()
        {
            var cleanupTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup") is not null;
            var notificationTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Windows Cleanup Notification") is not null;
            var cleanupPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1"));
            var notificationPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1"));
            var cleanupXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows Cleanup"));
            var notificationXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows Cleanup Notification"));
            var cleanupArgumentsPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup.ps1")}";
            var notificationTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Windows_Cleanup_Notification.ps1")}";

            // Letter case is ignored: the same path can be written as "System32" or "system32"
            return cleanupTaskExist && notificationTaskExist && cleanupPsExist && notificationPsExist && (cleanupXmlArguments?.InnerText.Equals(cleanupArgumentsPattern, StringComparison.OrdinalIgnoreCase) ?? false) && (notificationXmlArguments?.InnerText.Equals(notificationTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Get SoftwareDistribution scheduled task state.
        /// </summary>
        public static bool SoftwareDistributionTask()
        {
            var distributionTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\SoftwareDistribution") is not null;
            var distributionPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1"));
            var distributionXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistribution"));
            var distributionTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\SoftwareDistributionTask.ps1")}";
            return distributionTaskExist && distributionPsExist && (distributionXmlArguments?.InnerText.Equals(distributionTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }

        /// <summary>
        /// Get "Temp" scheduled task state.
        /// </summary>
        public static bool TempTask()
        {
            var tempTaskExist = ScheduledTaskService.GetTaskOrDefault("Sophia\\Temp") is not null;
            var tempPsExist = File.Exists(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1"));
            var tempXmlArguments = XmlService.GetScheduledTaskArguments(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\Temp"));
            var tempTaskPattern = $"--headless powershell.exe -NoProfile -ExecutionPolicy Bypass -File {Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.System), "Tasks\\Sophia\\TempTask.ps1")}";
            return tempTaskExist && tempPsExist && (tempXmlArguments?.InnerText.Equals(tempTaskPattern, StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }
}
