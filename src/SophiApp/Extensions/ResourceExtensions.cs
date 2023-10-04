// <copyright file="ResourceExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;
using Microsoft.Windows.ApplicationModel.Resources;

public static class ResourceExtensions
{
    private static readonly ResourceLoader ResourceLoader = new ();

    public static string GetLocalized(this string resourceKey) => ResourceLoader.GetString(resourceKey);
}
