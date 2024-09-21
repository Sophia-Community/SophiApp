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
    /// Getting Windows Action Center and Windows Script Host status and enabling notifications.
    /// </summary>
    void EnableToastNotification();

    /// <summary>
    /// Register the app by <paramref name="name"/> as a toast sender.
    /// </summary>
    /// <param name="name">Name of the app to be registered.</param>
    void RegisterAsToastSender(string name);

    /// <summary>
    /// Register Windows cleanup protocol to run via toast notification.
    /// </summary>
    void RegisterCleanupProtocolAsToastSender();

    /// <summary>
    /// Show the toast notification.
    /// </summary>
    /// <param name="payload">Toast payload.</param>
    void Show(string payload);

    /// <summary>
    /// Unregister Windows cleanup protocol to run via toast notification.
    /// </summary>
    void UnregisterCleanupProtocol();
}
