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
    /// Register the <see cref="IAppNotificationService"/> as a toast notifications sender.
    /// </summary>
    void RegisterAsSender();

    /// <summary>
    /// Show the toast notification.
    /// </summary>
    /// <param name="payload">Toast payload.</param>
    void Show(string payload);
}
