// <copyright file="IInitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// Service for initialization of data and services.
/// </summary>
public interface IInitializeService
{
    /// <summary>
    /// Initializes the application's data and services.
    /// </summary>
    /// <param name="args">Application launch arguments.</param>
    Task InitializeAsync(object args);
}
