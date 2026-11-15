// <copyright file="IAppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A service for working with toast notifications API.
/// </summary>
public interface IAppNotificationService
{
    /// <summary>
    /// Enables Windows notifications and the Notification Center by removing the settings and policies that turn them off.
    /// </summary>
    void EnableToastNotification();

    /// <summary>
    /// Register the SophiApp and Sophia Script for Windows as a toast sender.
    /// </summary>
    void RegisterAsToastSender();

    /// <summary>
    /// Register the "WindowsCleanup" protocol to be able to run the scheduled task by clicking the "Run" button in a toast.
    /// </summary>
    void RegisterWindowsCleanupAsToastSender();

    /// <summary>
    /// Unregister Windows cleanup protocol to run via toast notification.
    /// </summary>
    void UnregisterCleanupProtocol();
}
