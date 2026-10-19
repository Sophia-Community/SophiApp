// <copyright file="INavigationService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using SophiApp.Helpers;

/// <summary>
/// A service for working with app <see cref="Page"/> navigation.
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
    /// Navigates to the most recent item in back navigation history.
    /// </summary>
    bool GoBack();

    /// <summary>
    /// Causes the <see cref="Frame"/> to load content represented by the specified <see cref="Page"/> derived data type.
    /// </summary>
    /// <param name="page">Page to navigate.</param>
    /// <param name="parameter">Parameter passed to the navigation page.</param>
    /// <param name="clearHistory">Clear navigation history.</param>
    /// <param name="disablePageAnimation">Disable page transition animation.</param>
    bool NavigateTo(string page, object? parameter = null, bool clearHistory = false, bool disablePageAnimation = false);
}
