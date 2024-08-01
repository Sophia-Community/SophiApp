// <copyright file="IInitializeService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

/// <summary>
/// A service for working with app services data.
/// </summary>
public interface IInitializeService
{
    /// <summary>
    /// Initializes the app services data.
    /// </summary>
    /// <param name="args">App launch arguments.</param>
    Task InitializeAsync(object args);
}
