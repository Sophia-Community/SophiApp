// <copyright file="FrameNavigation.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;

/// <summary>
/// Implements <see cref="Microsoft.UI.Xaml.Controls.Frame"/> extensions.
/// </summary>
public static class FrameNavigation
{
    /// <summary>
    /// Returns the <see cref="Microsoft.UI.Xaml.Controls.Frame"/> ViewModel.
    /// </summary>
    /// <param name="frame">Frame for which need to get a ViewModel.</param>
    public static object? GetPageViewModel(this Microsoft.UI.Xaml.Controls.Frame frame)
        => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
}
