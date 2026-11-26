// <copyright file="StartupPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using System.Collections.Specialized;
    using System.ComponentModel;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// The page of the startup checks: every check gets a line, and a failed one gets a notice under its line.
    /// </summary>
    public sealed partial class StartupPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupPage"/> class.
        /// </summary>
        public StartupPage()
        {
            ViewModel = App.GetService<StartupViewModel>();
            InitializeComponent();
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Gets <see cref="StartupViewModel"/>.
        /// </summary>
        public StartupViewModel ViewModel { get; }

        // The answer the app suggests has the accent color
        private static Style GetActionStyle(bool isPrimary)
            => (Style)Application.Current.Resources[isPrimary ? "AccentButtonStyle" : "DefaultButtonStyle"];

        private static Button? FindNoticeButton(DependencyObject parent)
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                // The buttons of a notice are bound to the commands of the view model
                if (child is Button { Command: not null, IsEnabled: true } button && button.Visibility == Visibility.Visible)
                {
                    return button;
                }

                if (child is UIElement { Visibility: Visibility.Visible } && FindNoticeButton(child) is Button found)
                {
                    return found;
                }
            }

            return null;
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            ViewModel.Checks.CollectionChanged += OnChecksChanged;

            foreach (var check in ViewModel.Checks)
            {
                check.PropertyChanged += OnCheckChanged;
            }

            ShowLastLine();

            // The page is created anew when the language is changed: a question that waits for an answer keeps the keyboard
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => FindNoticeButton(ChecksScroll)?.Focus(FocusState.Pointer));
        }

        private void OnUnloaded(object sender, RoutedEventArgs args)
        {
            ViewModel.Checks.CollectionChanged -= OnChecksChanged;

            foreach (var check in ViewModel.Checks)
            {
                check.PropertyChanged -= OnCheckChanged;
            }
        }

        private void OnChecksChanged(object? sender, NotifyCollectionChangedEventArgs args)
        {
            foreach (var check in args.NewItems?.OfType<StartupCheck>() ?? [])
            {
                check.PropertyChanged += OnCheckChanged;
            }

            ShowLastLine();
        }

        // A notice makes the list longer. Its first button gets the keyboard, so Enter answers the question
        private void OnCheckChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName != nameof(StartupCheck.NoticeIsOpen))
            {
                return;
            }

            ShowLastLine();
            DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => FindNoticeButton(ChecksScroll)?.Focus(FocusState.Pointer));
        }

        // In a small window the list is longer than the page: the line in progress stays in view
        private void ShowLastLine()
            => DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, () => ChecksScroll.ChangeView(null, double.MaxValue, null, true));

        private void StartupPage_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, StartupPageCommandsFlyout, args);

        private void ContextMenuOpenLogFile_Clicked(object sender, RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFile);

        private void ContextMenuOpenLogFolder_Clicked(object sender, RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFolder);
    }
}
