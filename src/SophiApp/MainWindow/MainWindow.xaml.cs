// <copyright file="MainWindow.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp
{
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Extensions.DependencyInjection;
    using SophiApp.UI;
    using SophiApp.ViewModel;

    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        public MainWindow() => InitializeComponent();

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="vm"><see cref="MainVM"/>.</param>
        public MainWindow(MainVM vm)
            : this() => DataContext = vm;

        private void OnMinimizeButtonClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            WindowState = WindowState.Minimized;
        }

        private void OnMinMaxButtonClicked(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void OnTitleMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is WindowTitle && e.ButtonState == MouseButtonState.Pressed)
            {
                e.Handled = true;
                DragMove();
            }
        }

        private void OnWindowLoaded(object sender, RoutedEventArgs e) => App.Host!.Services.GetRequiredService<MainVM>()
            .InitializeComponents();
    }
}
