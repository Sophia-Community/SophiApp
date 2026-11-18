// <copyright file="ISettingsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services;

using Microsoft.UI.Xaml;
using Windows.Graphics;

/// <summary>
/// A service for working with app settings.
/// </summary>
public interface ISettingsService
{
    /// <summary>
    /// Gets app <see cref="MainWindow"/> default width.
    /// </summary>
    double AppWindowDefaultWidth { get; }

    /// <summary>
    /// Gets app <see cref="MainWindow"/> default height.
    /// </summary>
    double AppWindowDefaultHeight { get; }

    /// <summary>
    /// Gets app <see cref="MainWindow"/> minimal height.
    /// </summary>
    double AppWindowMinHeight { get; }

    /// <summary>
    /// Gets app <see cref="MainWindow"/> minimal width.
    /// </summary>
    double AppWindowMinWidth { get; }

    /// <summary>
    /// Initialize <see cref="ISettingsService"/> data.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Read app <see cref="MainWindow"/> position from a settings file.
    /// </summary>
    Task <PointInt32> ReadAppWindowPositionAsync();

    /// <summary>
    /// Read app <see cref="MainWindow"/> height from a settings file.
    /// </summary>
    Task<double> ReadAppWindowHeightAsync();

    /// <summary>
    /// Read app <see cref="MainWindow"/> state from a settings file.
    /// </summary>
    Task<WindowState> ReadAppWindowStateAsync();

    /// <summary>
    /// Read app <see cref="MainWindow"/> width from a settings file.
    /// </summary>
    Task<double> ReadAppWindowWidthAsync();

    /// <summary>
    /// Reads app <see cref="ElementTheme"/> from a settings file.
    /// </summary>
    Task<ElementTheme> ReadThemeAsync();

    /// <summary>
    /// Write app <see cref="MainWindow"/> position to a settings file.
    /// </summary>
    /// <param name="point">Defines a point in a two-dimensional plane.</param>
    Task SaveAppWindowPositionAsync(PointInt32 point);

    /// <summary>
    /// Write app <see cref="MainWindow"/> size to a settings file.
    /// </summary>
    /// <param name="height">A <see cref="MainWindow"/> height.</param>
    /// <param name="width">A <see cref="MainWindow"/> width.</param>
    Task SaveAppWindowSizeAsync(double height, double width);

    /// <summary>
    /// Write app <see cref="MainWindow"/> state to a settings file.
    /// </summary>
    /// <param name="state">Specifies app window state.</param>
    Task SaveAppWindowStateAsync(WindowState state);

    /// <summary>
    /// Write app <see cref="ElementTheme"/> to a settings file.
    /// </summary>
    /// <param name="theme">Specifies a UI theme that should be used for UI elements.</param>
    Task SaveThemeAsync(ElementTheme theme);
}
