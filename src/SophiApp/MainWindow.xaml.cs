// <copyright file="MainWindow.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp;
using SophiApp.Helpers;

using Windows.UI.ViewManagement;

/// <summary>
/// Implements the <see cref="MainWindow"/> class.
/// </summary>
public sealed partial class MainWindow : WindowEx
{
    private Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue;

    private UISettings settings;

    /// <summary>
    /// Initializes a new instance of the <see cref="MainWindow"/> class.
    /// </summary>
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/SophiApp.ico"));
        Content = null;
        Title = "AppDisplayName".GetLocalized();
        dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged;
    }

    /// <summary>
    /// This handles updating the caption button colors correctly when indows system theme is changed while the app is open.
    /// </summary>
    /// <param name="sender">Contains a set of common app user interface settings and operations.</param>
    /// <param name="args">Arguments passed to the method.</param>
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        dispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
    }
}
