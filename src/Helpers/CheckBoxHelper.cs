// <copyright file="CheckBoxHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

/// <summary>
/// A helper for work with <see cref="CheckBox"/>.
/// </summary>
public static class CheckBoxHelper
{
    /// <summary>
    /// The state of a check box that comes from a setting. A card that is scrolled out of view loses its setting
    /// until it is used again; a check box bound directly shows a dash then, and the dash can stay on the reused card.
    /// Bound through this property, the check box keeps its last state instead.
    /// </summary>
    public static readonly DependencyProperty IsCheckedProperty =
        DependencyProperty.RegisterAttached("IsChecked", typeof(object), typeof(CheckBoxHelper), new PropertyMetadata(null, OnIsCheckedChanged));

    // Whether the clicks of the check box are already followed
    private static readonly DependencyProperty IsFollowedProperty =
        DependencyProperty.RegisterAttached("IsFollowed", typeof(bool), typeof(CheckBoxHelper), new PropertyMetadata(false));

    /// <summary>
    /// Gets the state of a check box that comes from a setting.
    /// </summary>
    /// <param name="checkBox">A check box.</param>
    public static object? GetIsChecked(CheckBox checkBox) => checkBox.GetValue(IsCheckedProperty);

    /// <summary>
    /// Sets the state of a check box that comes from a setting.
    /// </summary>
    /// <param name="checkBox">A check box.</param>
    /// <param name="value">True if the check box is checked. Null if there is no setting.</param>
    public static void SetIsChecked(CheckBox checkBox, object? value) => checkBox.SetValue(IsCheckedProperty, value);

    private static void OnIsCheckedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is not CheckBox checkBox)
        {
            return;
        }

        if (!(bool)checkBox.GetValue(IsFollowedProperty))
        {
            checkBox.SetValue(IsFollowedProperty, true);
            checkBox.Checked += OnToggled;
            checkBox.Unchecked += OnToggled;
        }

        // Without a setting the check box stays as it is
        if (args.NewValue is bool isChecked && checkBox.IsChecked != isChecked)
        {
            checkBox.IsChecked = isChecked;
        }
    }

    // A click of the user goes back to the setting
    private static void OnToggled(object sender, RoutedEventArgs args)
    {
        var checkBox = (CheckBox)sender;

        if (checkBox.IsChecked is bool isChecked && GetIsChecked(checkBox) is bool current && current != isChecked)
        {
            SetIsChecked(checkBox, isChecked);
        }
    }
}
