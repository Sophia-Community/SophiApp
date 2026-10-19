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
    /// Gets a minimum font size for UI elements description.
    /// </summary>
    int DescriptionTextMinSize { get; }

    /// <summary>
    /// Gets a maximum font size for UI elements description.
    /// </summary>
    int DescriptionTextMaxSize { get; }

    /// <summary>
    /// Gets a minimum font size for UI elements title.
    /// </summary>
    int TitleTextMinSize { get; }

    /// <summary>
    /// Gets a maximum font size for UI elements title.
    /// </summary>
    int TitleTextMaxSize { get; }

    /// <summary>
    /// Initialize <see cref="ISettingsService"/> data.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Delete requirement action name from a settings file.
    /// </summary>
    void DeleteDebugRequirementAction();

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
    /// Read navigation menu item log page visibility from a settings file.
    /// </summary>
    Task<bool> ReadLogPageVisibilityAsync();

    /// <summary>
    /// Read requirement action name from a settings file for debug.
    /// </summary>
    string ReadDebugRequirementAction();

    /// <summary>
    /// Read UI elements descriptions size from a settings file.
    /// </summary>
    Task<int> ReadTextDescriptionSizeAsync();

    /// <summary>
    /// Read UI elements title size from a settings file.
    /// </summary>
    Task<int> ReadTextTitleSizeAsync();

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
    /// Write navigation menu item log page visibility to a settings file.
    /// </summary>
    /// <param name="isVisible">Log page visibility.</param>
    void SaveLogPageVisibility(bool isVisible);

    /// <summary>
    /// Write requirement action name to a settings file for run to debug in next app launch.
    /// </summary>
    /// <param name="action">Requirement action name or empty string.</param>
    Task SaveDebugRequirementActionAsync(string action);

    /// <summary>
    /// Write UI elements description size to a settings file.
    /// </summary>
    /// <param name="size">UI elements description size.</param>
    Task SaveTextDescriptionSizeAsync(int size);

    /// <summary>
    /// Write UI elements title size to a settings file.
    /// </summary>
    /// <param name="size">UI elements title size.</param>
    Task SaveTextTitleSizeAsync(int size);

    /// <summary>
    /// Write app <see cref="ElementTheme"/> to a settings file.
    /// </summary>
    /// <param name="theme">Specifies a UI theme that should be used for UI elements.</param>
    Task SaveThemeAsync(ElementTheme theme);
}
