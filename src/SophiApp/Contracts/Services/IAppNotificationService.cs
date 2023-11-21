// <copyright file="IAppNotificationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A service for working with toast notifications.
/// </summary>
public interface IAppNotificationService
{
    /// <summary>
    /// Registers the app as a toast notifications sender.
    /// </summary>
    void Register();

    /// <summary>
    /// Show the toast notification.
    /// </summary>
    /// <param name="payload">Toast payload.</param>
    void Show(string payload);
}
