// <copyright file="ScrollViewExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// Implements <see cref="ScrollView"/> extensions.
/// </summary>
public static class ScrollViewExtensions
{
    /// <summary>
    /// Correct the vertical offset so that the last <see cref="FrameworkElement"/> in the sequence fits on the UI.
    /// </summary>
    /// <param name="scrollView">Displayed content in scrollable area.</param>
    public static void VerticalOffsetCorrection(this ScrollView scrollView)
    {
        if (scrollView.VerticalOffset == scrollView.ScrollableHeight)
        {
            scrollView.UpdateLayout();
            scrollView.ScrollTo(0, scrollView.ScrollableHeight, new ScrollingScrollOptions(ScrollingAnimationMode.Disabled));
        }
    }
}
