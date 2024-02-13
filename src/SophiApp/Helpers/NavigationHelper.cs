// <copyright file="NavigationHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// Helper class to set the navigation target for a NavigationViewItem.
/// </summary>
public static class NavigationHelper
{
    /// <summary>
    /// Registered "NavigateTo" dependency property.
    /// </summary>
    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(string), typeof(NavigationHelper), new PropertyMetadata(null));

    /// <summary>
    /// Get navigation target.
    /// </summary>
    /// <param name="item">Represents the item in a NavigationView control.</param>
    public static string GetNavigateTo(NavigationViewItem item) => (string)item.GetValue(NavigateToProperty);

    /// <summary>
    /// Set navigation target.
    /// </summary>
    /// <param name="item">Represents the item in a NavigationView control.</param>
    /// <param name="value">Navigation target.</param>
    public static void SetNavigateTo(NavigationViewItem item, string value) => item.SetValue(NavigateToProperty, value);
}
