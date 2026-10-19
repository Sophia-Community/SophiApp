// <copyright file="IThemesService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;
using Microsoft.UI.Xaml;

/// <summary>
/// A service for working with app themes.
/// </summary>
public interface IThemesService
{
    /// <summary>
    /// Gets app theme.
    /// </summary>
    ElementTheme Theme
    {
        get;
    }

    /// <summary>
    /// Service initialization.
    /// </summary>
    Task InitializeAsync();

    /// <summary>
    /// Apply the selected application theme and save to a settings file.
    /// </summary>
    /// <param name="theme">Specifies a UI theme that should be used for UIElements.</param>
    Task SetThemeAsync(ElementTheme theme);

    /// <summary>
    /// Apply the selected theme.
    /// </summary>
    Task SetRequestedThemeAsync();
}
