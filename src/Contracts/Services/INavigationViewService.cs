// <copyright file="INavigationViewService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// A service for working with app <see cref="Views"/> navigation.
/// </summary>
public interface INavigationViewService
{
    /// <summary>
    /// Gets collection of menu items.
    /// </summary>
    IList<object>? MenuItems
    {
        get;
    }

    /// <summary>
    /// Gets the "Settings" menu item.
    /// </summary>
    object? SettingsItem
    {
        get;
    }

    /// <summary>
    /// Initializes <see cref="INavigationService"/> data.
    /// </summary>
    /// <param name="navigationView">Represents a container that enables navigation of app content.</param>
    void Initialize(NavigationView navigationView);

    /// <summary>
    /// Unregister menu item events.
    /// </summary>
    void UnregisterEvents();

    /// <summary>
    /// Get selected menu item.
    /// </summary>
    /// <param name="pageType">Page type.</param>
    NavigationViewItem? GetSelectedItem(Type pageType);
}
