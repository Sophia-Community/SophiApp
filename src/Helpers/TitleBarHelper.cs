// <copyright file="TitleBarHelper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System.Runtime.InteropServices;
using Windows.UI;
using Windows.UI.ViewManagement;
using WinRT.Interop;

/// <summary>
/// A helper for work with window titlebar.
/// </summary>
internal static class TitleBarHelper
{
    private const int WAINACTIVE = 0x00;
    private const int WAACTIVE = 0x01;
    private const int WMACTIVATE = 0x0006;

    /// <summary>
    /// Apply app theme to window titlebar.
    /// </summary>
    public static void ApplySystemThemeToCaptionButtons()
    {
        var res = Application.Current.Resources;
        var frame = App.AppTitlebar as FrameworkElement;

        if (frame != null)
        {
            var handler = WindowNative.GetWindowHandle(App.MainWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(handler);
            var appWindow = AppWindow.GetFromWindowId(windowId);

            if (frame.ActualTheme == ElementTheme.Dark)
            {
                res["WindowCaptionForeground"] = Colors.White;
                appWindow.TitleBar.ButtonForegroundColor = Colors.White;
            }
            else
            {
                res["WindowCaptionForeground"] = Colors.Black;
                appWindow.TitleBar.ButtonForegroundColor = Colors.Black;
            }

            UpdateTitleBar(frame.ActualTheme);
        }
    }

    /// <summary>
    /// Update window titlebar to app theme.
    /// </summary>
    /// <param name="theme">Specifies app UI theme.</param>
    public static void UpdateTitleBar(ElementTheme theme)
    {
        if (!App.MainWindow.ExtendsContentIntoTitleBar)
        {
            return;
        }

        // "Follow Windows theme" is dark unless the system background is white
        var isDark = theme == ElementTheme.Default
            ? new UISettings().GetColorValue(UIColorType.Background) != Colors.White
            : theme == ElementTheme.Dark;

        // The caption is white on a dark window and black on a light one
        var color = isDark ? Colors.White : Colors.Black;
        var resources = Application.Current.Resources;
        resources["WindowCaptionForeground"] = CreateBrush(color, 0xFF);
        resources["WindowCaptionForegroundDisabled"] = CreateBrush(color, 0x66);
        resources["WindowCaptionButtonBackgroundPointerOver"] = CreateBrush(color, 0x33);
        resources["WindowCaptionButtonBackgroundPressed"] = CreateBrush(color, 0x66);
        resources["WindowCaptionButtonStrokePointerOver"] = CreateBrush(color, 0xFF);
        resources["WindowCaptionButtonStrokePressed"] = CreateBrush(color, 0xFF);
        resources["WindowCaptionBackground"] = new SolidColorBrush(Colors.Transparent);
        resources["WindowCaptionBackgroundDisabled"] = new SolidColorBrush(Colors.Transparent);

        // The caption is redrawn when the window activation changes, so it is switched there and back
        var hwnd = WindowNative.GetWindowHandle(App.MainWindow);
        var isActive = hwnd == GetActiveWindow();
        SendMessage(hwnd, WMACTIVATE, isActive ? WAINACTIVE : WAACTIVE, nint.Zero);
        SendMessage(hwnd, WMACTIVATE, isActive ? WAACTIVE : WAINACTIVE, nint.Zero);
    }

    private static SolidColorBrush CreateBrush(Color color, byte opacity) => new(Color.FromArgb(opacity, color.R, color.G, color.B));

    [DllImport("user32.dll")]
    private static extern nint GetActiveWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    private static extern nint SendMessage(nint hWnd, int msg, int wParam, nint lParam);
}
