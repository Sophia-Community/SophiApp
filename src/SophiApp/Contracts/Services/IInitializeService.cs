// <copyright file="IInitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A service for initializing data and other services.
/// </summary>
public interface IInitializeService
{
    /// <summary>
    /// Initializes the app data for services.
    /// </summary>
    /// <param name="args">Application launch arguments.</param>
    Task InitializeAsync(object args);
}
