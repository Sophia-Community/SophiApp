// <copyright file="NavigationHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// Tells which page a menu item opens. In XAML the page is written as its type: "views:PrivacyPage".
/// </summary>
public static class NavigationHelper
{
    /// <summary>
    /// Registered "NavigateTo" dependency property.
    /// </summary>
    public static readonly DependencyProperty NavigateToProperty =
        DependencyProperty.RegisterAttached("NavigateTo", typeof(Type), typeof(NavigationHelper), new PropertyMetadata(null));

    /// <summary>
    /// Gets the type of the page that a menu item opens.
    /// </summary>
    /// <param name="item">Represents the item in a NavigationView control.</param>
    public static Type? GetNavigateTo(NavigationViewItem item) => item.GetValue(NavigateToProperty) as Type;

    /// <summary>
    /// Sets the type of the page that a menu item opens.
    /// </summary>
    /// <param name="item">Represents the item in a NavigationView control.</param>
    /// <param name="value">The type of the page.</param>
    public static void SetNavigateTo(NavigationViewItem item, Type value) => item.SetValue(NavigateToProperty, value);
}
