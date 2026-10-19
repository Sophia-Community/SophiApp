// <copyright file="Resource.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;
using Microsoft.Windows.ApplicationModel.Resources;

/// <summary>
/// Implements app resources extensions.
/// </summary>
public static class Resource
{
    private static readonly ResourceLoader ResourceLoader = new ();

    /// <summary>
    /// Gets a localized string.
    /// </summary>
    /// <param name="resourceKey">Key to search for a localized string.</param>
    public static string GetLocalized(this string resourceKey) => ResourceLoader.GetString(resourceKey);
}
