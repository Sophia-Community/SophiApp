// <copyright file="Resource.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;

using Microsoft.Windows.ApplicationModel.Resources;
using SophiApp.Helpers;

/// <summary>
/// Implements app resources extensions.
/// </summary>
public static class Resource
{
    /// <summary>
    /// Gets a localized string in the language of the app.
    /// </summary>
    /// <param name="resourceKey">Key to search for a localized string.</param>
    public static string GetLocalized(this string resourceKey)
        => AppResources.Instance.Strings.TryGetValue(resourceKey, AppResources.Instance.Context)?.ValueAsString ?? string.Empty;
}
