// <copyright file="ShellPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views;

using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using SophiApp.Contracts.Services;
using SophiApp.Helpers;
using SophiApp.ViewModels;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.Foundation;
using Windows.System;

/// <summary>
/// Implements the <see cref="ShellPage"/> class.
/// </summary>
public sealed partial class ShellPage : Page, INotifyPropertyChanged
{
    // The room a menu item needs besides its name: the icon on the left, the counter of chosen settings and the margins on the right
    private const double PaneItemChromeWidth = 100;
    private const double PaneMinLength = 180;
    private const double PaneMaxLength = 320;

    // The search box is as wide as the menu without its margins
    private const double SearchBoxMargins = 15;

    private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer searchTimer;
    private double currentWidth = default;
    private string announcedStatus = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellPage"/> class.
    /// </summary>
    /// <param name="viewModel"><see cref="ShellViewModel"/>.</param>
    /// <param name="commonDataService"><see cref="ICommonDataService"/>.</param>
    public ShellPage(ShellViewModel viewModel, ICommonDataService commonDataService)
    {
        InitializeComponent();
        CurrentWidth = ActualWidth;
        ViewModel = viewModel;
        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleName.Text = commonDataService.GetFullName();

        // The search starts when typing pauses, not on every key
        searchTimer = DispatcherQueue.CreateTimer();
        searchTimer.Interval = TimeSpan.FromMilliseconds(300);
        searchTimer.IsRepeating = false;
        searchTimer.Tick += (_, _) => ViewModel.Search_Command.Execute(AutoSuggestBox.Text);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <summary>
    /// Property change event.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets <see cref="ShellViewModel"/>.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets a value indicating current width.
    /// </summary>
    public double CurrentWidth
    {
        get
        {
            return currentWidth;
        }

        private set
        {
            currentWidth = value;
            OnPropertyChanged(nameof(CurrentWidth));
        }
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        // There is nowhere to go back to while the app is starting or applying settings
        args.Handled = App.GetService<ShellViewModel>().NavigationViewHitTestVisible && App.GetService<INavigationService>().GoBack();
    }

    private static bool IsBackAllowed(bool canGoBack, bool navigationIsAllowed) => canGoBack && navigationIsAllowed;

    // The question about applying is hidden while the app is busy: the progress line takes its place
    private static Visibility GetApplyPanelVisibility(int chosenSettings, bool appIsBusy) => chosenSettings > 0 && !appIsBusy ? Visibility.Visible : Visibility.Collapsed;

    private static Visibility GetReminderVisibility(bool appIsBusy) => appIsBusy ? Visibility.Collapsed : Visibility.Visible;

    private static TextBox? FindTextBox(DependencyObject parent)
    {
        for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
        {
            var child = VisualTreeHelper.GetChild(parent, i);

            if (child is TextBox textBox)
            {
                return textBox;
            }

            if (FindTextBox(child) is TextBox found)
            {
                return found;
            }
        }

        return null;
    }

    private void PageShell_SizeChanged(object sender, SizeChangedEventArgs e) => CurrentWidth = ActualWidth;

    private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);
        FitPaneToMenuItems();
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    // The menu is as wide as its longest name needs, whatever the language is
    private void FitPaneToMenuItems()
    {
        var widestName = 0d;

        foreach (var item in NavigationViewControl.MenuItems.OfType<NavigationViewItem>())
        {
            if (item.Visibility == Visibility.Visible && item.Content is string name)
            {
                var block = new TextBlock { Text = name, FontSize = item.FontSize, FontFamily = item.FontFamily };
                block.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                widestName = Math.Max(widestName, block.DesiredSize.Width);
            }
        }

        var paneLength = Math.Clamp(Math.Ceiling(widestName) + PaneItemChromeWidth, PaneMinLength, PaneMaxLength);
        NavigationViewControl.OpenPaneLength = paneLength;
        AutoSuggestBox.Width = paneLength - SearchBoxMargins;
    }

    private void AutoSuggestBox_Loaded(object sender, RoutedEventArgs e) => SetSearchBoxTabStop();

    // While the app is busy the search box looks as usual, but it cannot be reached with the Tab key
    private void SetSearchBoxTabStop()
    {
        if (FindTextBox(AutoSuggestBox) is TextBox textBox)
        {
            textBox.IsTabStop = ViewModel.NavigationViewHitTestVisible;
        }
    }

    private void OnViewModelPropertyChanged(object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName == nameof(ViewModel.NavigationViewHitTestVisible))
        {
            SetSearchBoxTabStop();
            return;
        }

        if (args.PropertyName != nameof(ViewModel.SetUpCustomizationsPanelText))
        {
            return;
        }

        // A screen reader announces what the app is doing. Download percents are skipped: they change too often to be read aloud
        var status = new string(ViewModel.SetUpCustomizationsPanelText.Where(symbol => !char.IsDigit(symbol)).ToArray());

        if (status.Length > 0 && status != announcedStatus)
        {
            announcedStatus = status;
            FrameworkElementAutomationPeer.FromElement(StatusText)?.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
        }
    }

    private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
    {
        searchTimer.Stop();
        searchTimer.Start();
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        searchTimer.Stop();
        ViewModel.Search_Command.Execute(args.QueryText);
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";
        AppTitleName.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        App.AppTitlebar = AppTitleName;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom,
        };
    }
}
