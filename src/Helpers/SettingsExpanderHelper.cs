// <copyright file="SettingsExpanderHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using CommunityToolkit.WinUI.Controls;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;

/// <summary>
/// A helper for work with <see cref="SettingsExpander"/>.
/// </summary>
internal static class SettingsExpanderHelper
{
    private const string HeaderPaddingKey = "SettingsExpanderHeaderPadding";
    private const string MinHeightKey = "SettingsCardMinHeight";

    /// <summary>
    /// Makes the header of an expander as high as a settings card.
    /// </summary>
    /// <param name="expander">A loaded expander.</param>
    public static void ApplyHeaderPadding(SettingsExpander expander)
    {
        // The template of the expander does not take the sizes from the app resources, so they are set on the header itself
        var resources = Application.Current.Resources;

        if (FindCard(expander) is SettingsCard header && resources[HeaderPaddingKey] is Thickness padding && resources[MinHeightKey] is double minHeight)
        {
            header.Padding = padding;
            header.MinHeight = minHeight;
        }
    }

    // The header is the first card of the expander
    private static SettingsCard? FindCard(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is SettingsCard card)
            {
                return card;
            }

            if (FindCard(child) is SettingsCard found)
            {
                return found;
            }
        }

        return null;
    }
}
