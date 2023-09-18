// <copyright file="FrameExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// Implements <see cref="Frame"/> extensions.
/// </summary>
public static class FrameExtensions
{
    /// <summary>
    /// Returns the <see cref="Frame"/> ViewModel.
    /// </summary>
    /// <param name="frame">Frame for which need to get a ViewModel.</param>
    public static object? GetPageViewModel(this Frame frame)
        => frame?.Content?.GetType().GetProperty("ViewModel")?.GetValue(frame.Content, null);
}
