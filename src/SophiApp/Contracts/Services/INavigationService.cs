// <copyright file="INavigationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;

/// <summary>
/// App page navigation service.
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Represents the method that will handle the Navigated event.
    /// </summary>
    event NavigatedEventHandler Navigated;

    /// <summary>
    /// Gets or sets <see cref="Page"/> instances.
    /// </summary>
    Frame? Frame
    {
        get; set;
    }

    /// <summary>
    /// Gets a value indicating whether there is at least one entry in back navigation history.
    /// </summary>
    bool CanGoBack
    {
        get;
    }

    /// <summary>
    /// Gets name of last VM used in <see cref="NavigateTo(string, object?, bool, bool)"/>.
    /// </summary>
    string LastVmUsed
    {
        get;
    }

    /// <summary>
    /// Navigates to the most recent item in back navigation history.
    /// </summary>
    bool GoBack();

    /// <summary>
    /// Causes the <see cref="Frame"/> to load content represented by the specified <see cref="Page"/> derived data type.
    /// </summary>
    /// <param name="pageKey">Page to navigate.</param>
    /// <param name="parameter">Parameter passed to the navigation page.</param>
    /// <param name="clearNavigation">Clears the navigation history.</param>
    /// <param name="ignorePageType">Allow to refresh active page.</param>
    bool NavigateTo(string pageKey, object? parameter = null, bool clearNavigation = false, bool ignorePageType = false);
}
