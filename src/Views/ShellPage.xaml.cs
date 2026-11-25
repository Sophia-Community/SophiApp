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
using SophiApp.Extensions;
using SophiApp.Helpers;
using SophiApp.ViewModels;
using System.ComponentModel;
using Windows.Foundation;
using Windows.System;

/// <summary>
/// Implements the <see cref="ShellPage"/> class.
/// </summary>
public sealed partial class ShellPage : Page
{
    // The room a menu item needs besides its name: the icon on the left, the counter of chosen settings and the margins on the right
    private const double PaneItemChromeWidth = 100;
    private const double PaneMinLength = 180;
    private const double PaneMaxLength = 320;

    private readonly Microsoft.UI.Dispatching.DispatcherQueueTimer searchTimer;
    private string announcedStatus = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellPage"/> class.
    /// </summary>
    /// <param name="viewModel"><see cref="ShellViewModel"/>.</param>
    /// <param name="choices"><see cref="ChosenSettingsViewModel"/>.</param>
    /// <param name="search"><see cref="SearchViewModel"/>.</param>
    /// <param name="commonDataService"><see cref="ICommonDataService"/>.</param>
    public ShellPage(ShellViewModel viewModel, ChosenSettingsViewModel choices, SearchViewModel search, ICommonDataService commonDataService)
    {
        InitializeComponent();
        SetSearchFlyoutBackdrop();
        ViewModel = viewModel;
        Choices = choices;
        Search = search;
        ViewModel.NavigationService.Frame = NavigationFrame;
        ViewModel.NavigationViewService.Initialize(NavigationViewControl);
        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        AppTitleName.Text = commonDataService.GetFullName();

        // Windows 11 LTSC 2024 has no UWP apps to uninstall, so the category has the gaming settings only
        if (commonDataService.OsProperties.IsLTSC)
        {
            UwpGamingItem.Content = "NavigationItem_Gaming".GetLocalized();
        }

        // The search starts when typing pauses, not on every key
        searchTimer = DispatcherQueue.CreateTimer();
        searchTimer.Interval = TimeSpan.FromMilliseconds(300);
        searchTimer.IsRepeating = false;
        searchTimer.Tick += (_, _) => Search.Search_Command.Execute(AutoSuggestBox.Text);
        ViewModel.PropertyChanged += OnViewModelPropertyChanged;
    }

    /// <summary>
    /// Gets <see cref="ShellViewModel"/>.
    /// </summary>
    public ShellViewModel ViewModel { get; }

    /// <summary>
    /// Gets <see cref="ChosenSettingsViewModel"/>.
    /// </summary>
    public ChosenSettingsViewModel Choices { get; }

    /// <summary>
    /// Gets <see cref="SearchViewModel"/>.
    /// </summary>
    public SearchViewModel Search { get; }

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
    private static Visibility GetApplyPanelVisibility(int chosenSettings, bool appIsBusy, bool restorePointIsAsked)
        => chosenSettings > 0 && !appIsBusy && !restorePointIsAsked ? Visibility.Visible : Visibility.Collapsed;

    // The question about a restore point takes the place of the question about applying
    private static Visibility GetQuestionVisibility(int chosenSettings, bool appIsBusy, bool restorePointIsAsked)
        => chosenSettings > 0 && !appIsBusy && restorePointIsAsked ? Visibility.Visible : Visibility.Collapsed;

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

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        TitleBarHelper.UpdateTitleBar(RequestedTheme);
        FitPaneToMenuItems();
        ShowSearchBoxOrButton();
        ShowPaneInNarrowWindow();
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));

        var searchAccelerator = new KeyboardAccelerator { Key = VirtualKey.F, Modifiers = VirtualKeyModifiers.Control };
        searchAccelerator.Invoked += OnSearchAcceleratorInvoked;
        KeyboardAccelerators.Add(searchAccelerator);
    }

    // Ctrl+F goes to the search box, or opens the one of the search button
    private void OnSearchAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        args.Handled = true;

        if (!ViewModel.NavigationViewHitTestVisible)
        {
            return;
        }

        if (AutoSuggestBox.Visibility == Visibility.Visible)
        {
            _ = AutoSuggestBox.Focus(FocusState.Keyboard);
            return;
        }

        SearchFlyout.ShowAt(SearchButton);
    }

    // The search box of the search button lies on the same acrylic as the menus and the lists.
    // In high contrast it keeps the system colors
    private void SetSearchFlyoutBackdrop()
    {
        if (new Windows.UI.ViewManagement.AccessibilitySettings().HighContrast)
        {
            return;
        }

        var presenterStyle = new Style(typeof(FlyoutPresenter)) { BasedOn = SearchFlyout.FlyoutPresenterStyle };
        presenterStyle.Setters.Add(new Setter(BackgroundProperty, new SolidColorBrush(Microsoft.UI.Colors.Transparent)));
        SearchFlyout.FlyoutPresenterStyle = presenterStyle;
        SearchFlyout.ShouldConstrainToRootBounds = false;
        SearchFlyoutLayers.Children.Insert(0, new SystemBackdropElement
        {
            SystemBackdrop = new DesktopAcrylicBackdrop(),
            CornerRadius = (CornerRadius)Application.Current.Resources["OverlayCornerRadius"],
        });
    }

    // In a window that is narrow from the start the menu keeps its place but is not drawn until the window is made wider.
    // Opening the menu and closing it at once makes it draw itself
    private void ShowPaneInNarrowWindow()
    {
        if (NavigationViewControl.DisplayMode != NavigationViewDisplayMode.Compact)
        {
            return;
        }

        // Both changes come in one frame, so nothing blinks
        NavigationViewControl.IsPaneOpen = true;
        NavigationViewControl.IsPaneOpen = false;
    }

    // The menu is as wide as its longest name needs, whatever the language is
    private void FitPaneToMenuItems()
    {
        var widestName = 0d;

        foreach (var item in NavigationViewControl.MenuItems.Concat(NavigationViewControl.FooterMenuItems).OfType<NavigationViewItem>())
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
        // The search box of the menu and the one of the search button show the same text
        var other = ReferenceEquals(sender, AutoSuggestBox) ? FlyoutSearchBox : AutoSuggestBox;

        if (other.Text != sender.Text)
        {
            other.Text = sender.Text;
        }

        searchTimer.Stop();
        searchTimer.Start();
    }

    private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
    {
        searchTimer.Stop();
        Search.Search_Command.Execute(args.QueryText);

        // The search box of the search button gives the room back to what is found
        SearchFlyout.Hide();
    }

    // The menu with the names has the search box. When only the icons are left, a button that opens a search box stands for it
    private void ShowSearchBoxOrButton()
    {
        var menuHasNames = NavigationViewControl.DisplayMode == NavigationViewDisplayMode.Expanded;
        AutoSuggestBox.Visibility = menuHasNames ? Visibility.Visible : Visibility.Collapsed;
        SearchButton.Visibility = menuHasNames ? Visibility.Collapsed : Visibility.Visible;

        if (menuHasNames)
        {
            SearchFlyout.Hide();
        }
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";
        AppTitleName.Foreground = (SolidColorBrush)App.Current.Resources[resource];
        App.AppTitlebar = AppTitleName;
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        ShowSearchBoxOrButton();

        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom,
        };
    }
}
