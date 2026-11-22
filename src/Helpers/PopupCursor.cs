// <copyright file="PopupCursor.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using System.Runtime.InteropServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;

/// <summary>
/// Keeps the mouse cursor right over a menu that has just opened.
/// A menu is a separate window. Until the mouse moves, Windows shows over it the cursor the app window had last:
/// the wait cursor left from the start of the app, or a resize arrow from the window border.
/// </summary>
public static class PopupCursor
{
    /// <summary>
    /// Shows the arrow cursor when the flyout opens.
    /// </summary>
    public static readonly DependencyProperty ResetOnOpenProperty =
        DependencyProperty.RegisterAttached("ResetOnOpen", typeof(bool), typeof(PopupCursor), new PropertyMetadata(false, OnResetOnOpenChanged));

    private const int ArrowCursorId = 32512;

    /// <summary>
    /// Gets a value indicating whether the arrow cursor is shown when the flyout opens.
    /// </summary>
    /// <param name="flyout">A flyout.</param>
    public static bool GetResetOnOpen(FlyoutBase flyout) => (bool)flyout.GetValue(ResetOnOpenProperty);

    /// <summary>
    /// Turns the arrow cursor on the opening of the flyout on or off.
    /// </summary>
    /// <param name="flyout">A flyout.</param>
    /// <param name="value">True to show the arrow cursor.</param>
    public static void SetResetOnOpen(FlyoutBase flyout, bool value) => flyout.SetValue(ResetOnOpenProperty, value);

    /// <summary>
    /// Shows the arrow cursor.
    /// </summary>
    public static void Reset() => SetCursor(LoadCursor(0, ArrowCursorId));

    private static void OnResetOnOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        if (sender is FlyoutBase flyout)
        {
            flyout.Opening -= OnFlyoutOpening;
            flyout.Opened -= OnFlyoutOpened;

            if ((bool)args.NewValue)
            {
                flyout.Opening += OnFlyoutOpening;
                flyout.Opened += OnFlyoutOpened;
            }
        }
    }

    private static void OnFlyoutOpening(object? sender, object args) => Reset();

    private static void OnFlyoutOpened(object? sender, object args) => Reset();

    [DllImport("user32.dll")]
    private static extern nint SetCursor(nint cursor);

    [DllImport("user32.dll", EntryPoint = "LoadCursorW")]
    private static extern nint LoadCursor(nint instance, nint cursorId);
}
