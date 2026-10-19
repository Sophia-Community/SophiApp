// <copyright file="INavigationAware.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.ViewModels;

/// <summary>
/// Service for working with API navigation in the app.
/// </summary>
public interface INavigationAware
{
    /// <summary>
    /// Implements navigation logic to object.
    /// </summary>
    /// <param name="parameter">The object to which navigation is performed.</param>
    void OnNavigatedTo(object parameter);

    /// <summary>
    /// Implements navigation logic from object.
    /// </summary>
    void OnNavigatedFrom();
}
