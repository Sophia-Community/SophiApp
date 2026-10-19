// <copyright file="MainWindow.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;

using Microsoft.UI.Windowing;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using System;
using System.Runtime.InteropServices;
using Windows.Graphics;
using Windows.UI.ViewManagement;
using Windows.UI.WindowManagement;
using WinRT.Interop;

/// <summary>
/// Implements the <see cref="MainWindow"/> class.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
    private readonly Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;
    private readonly UISettings settings;
    private readonly ISettingsService settingsService;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();
        App.SetSingleInstance();
        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/SophiApp.ico"));
        Content = null;
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged;
        settingsService = App.GetService<ISettingsService>();
        ExtendsContentIntoTitleBar = true;
    }

    [DllImport("dwmapi")]
    private static extern IntPtr DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMarInset);

    /// <summary>
    /// This handles updating the caption button colors correctly when windows system theme is changed while the app is open.
    /// </summary>
    /// <param name="sender">Contains a set of common app user interface settings and operations.</param>
    /// <param name="args">Arguments passed to the method.</param>
    private void Settings_ColorValuesChanged(UISettings sender, object args)
        => dispatcherQueue.TryEnqueue(TitleBarHelper.ApplySystemThemeToCaptionButtons);

    private void MainWindow_StateChanged(object sender, WindowState e)
        => Task.Run(async () => await settingsService.SaveAppWindowStateAsync(WindowState));

    private void MainWindow_PositionChanged(object sender, PointInt32 e)
    {
        if (Visible && e.X > 0 && e.Y > 0)
        {
            _ = Task.Run(async () => await settingsService.SaveAppWindowPositionAsync(e));
        }
    }

    private void MainWindow_SizeChanged(object sender, Microsoft.UI.Xaml.WindowSizeChangedEventArgs args)
    {
        if (Visible && WindowState == WindowState.Normal)
        {
            var height = Height;
            var width = Width;
            _ = Task.Run(async () => await settingsService.SaveAppWindowSizeAsync(height, width));
        }
    }

    private void MainWindow_Activated(object sender, Microsoft.UI.Xaml.WindowActivatedEventArgs args)
    {
        var handle = WindowNative.GetWindowHandle(this);
        var margins = new MARGINS { cxLeftWidth = 0, cxRightWidth = 0, cyBottomHeight = 0, cyTopHeight = 2 };
        DwmExtendFrameIntoClientArea(handle, ref margins);
    }
}
