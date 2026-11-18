// <copyright file="TintedAcrylicBackdrop.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers;

using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

/// <summary>
/// An acrylic window background in the colors of the app. It lets more of the desktop through than the standard one,
/// so the menu on it is lighter than the page with settings.
/// </summary>
public sealed partial class TintedAcrylicBackdrop : SystemBackdrop
{
    // The window is taken from the mockup as is: a luminosity layer at 85% and no color layer.
    // Its hue comes from what is behind the window: sea blue over the blue Windows wallpaper, grey over a white window
    private const float DarkTintOpacity = 0f;
    private const float DarkLuminosityOpacity = 0.85f;
    private const float LightTintOpacity = 0f;
    private const float LightLuminosityOpacity = 0.85f;

    // The luminosity colors of the mockup styles "00 bg [d]" and "00 bg [l]"
    private static readonly Color DarkTint = Color.FromArgb(0xFF, 0x23, 0x27, 0x35);
    private static readonly Color LightTint = Color.FromArgb(0xFF, 0xFC, 0xFC, 0xFC);

    // Shown instead of acrylic when the window is inactive or transparency is turned off in Windows.
    // The dark one is the mockup style "01 bg [d]": a little darker than the page with settings, so the menu still stands out
    private static readonly Color DarkFallback = Color.FromArgb(0xFF, 0x1A, 0x20, 0x33);
    private static readonly Color LightFallback = Color.FromArgb(0xFF, 0xEE, 0xEF, 0xF2);
    private DesktopAcrylicController? controller;
    private bool isDark = Application.Current.RequestedTheme == ApplicationTheme.Dark;

    /// <summary>
    /// Sets the colors of the dark or the light theme.
    /// </summary>
    /// <param name="dark">Whether the app is shown in the dark theme.</param>
    public void SetTheme(bool dark)
    {
        isDark = dark;
        ApplyColors();
    }

    /// <inheritdoc/>
    protected override void OnTargetConnected(ICompositionSupportsSystemBackdrop connectedTarget, XamlRoot xamlRoot)
    {
        base.OnTargetConnected(connectedTarget, xamlRoot);

        controller = new DesktopAcrylicController();
        ApplyColors();

        // The configuration tells the controller when the window is inactive or high contrast is on
        controller.SetSystemBackdropConfiguration(GetDefaultSystemBackdropConfiguration(connectedTarget, xamlRoot));
        controller.AddSystemBackdropTarget(connectedTarget);
    }

    /// <inheritdoc/>
    protected override void OnTargetDisconnected(ICompositionSupportsSystemBackdrop disconnectedTarget)
    {
        base.OnTargetDisconnected(disconnectedTarget);

        controller?.RemoveSystemBackdropTarget(disconnectedTarget);
        controller?.Dispose();
        controller = null;
    }

    /// <inheritdoc/>
    protected override void OnDefaultSystemBackdropConfigurationChanged(ICompositionSupportsSystemBackdrop target, XamlRoot xamlRoot)
    {
        // The base method is not called on purpose: it fails with an invalid argument when the app theme is changed.
        // Nothing has to be done here, the colors are switched by SetTheme.
    }

    private void ApplyColors()
    {
        if (controller is null)
        {
            return;
        }

        controller.TintColor = isDark ? DarkTint : LightTint;
        controller.TintOpacity = isDark ? DarkTintOpacity : LightTintOpacity;
        controller.LuminosityOpacity = isDark ? DarkLuminosityOpacity : LightLuminosityOpacity;
        controller.FallbackColor = isDark ? DarkFallback : LightFallback;
    }
}
