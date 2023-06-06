// <copyright file="ResourceExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;
using Microsoft.Windows.ApplicationModel.Resources;

public static class ResourceExtensions
{
    private static readonly ResourceLoader resourceLoader = new ();

    public static string GetLocalized(this string resourceKey) => resourceLoader.GetString(resourceKey);
}
