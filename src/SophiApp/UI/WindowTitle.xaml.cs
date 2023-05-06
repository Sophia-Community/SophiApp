// <copyright file="WindowTitle.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;

    /// <summary>
    /// Логика взаимодействия для WindowTitle.xaml.
    /// </summary>
    public partial class WindowTitle : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for CloseWindowProperty.
        /// </summary>
        public static readonly DependencyProperty CloseWindowProperty =
            DependencyProperty.Register("CloseWindowCommand", typeof(ICommand), typeof(WindowTitle), new PropertyMetadata(default));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WindowTitle), new PropertyMetadata(default));

        private static readonly RoutedEvent MinimizeButtonClickedEvent = EventManager
            .RegisterRoutedEvent("MinimizeButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WindowTitle));

        private static readonly RoutedEvent MinMaxButtonClickedEvent = EventManager
            .RegisterRoutedEvent("MinMaxButtonClicked", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(WindowTitle));

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTitle"/> class.
        /// </summary>
        public WindowTitle() => InitializeComponent();

        /// <summary>
        /// The event of minimizing a window to the tray.
        /// </summary>
        public event RoutedEventHandler MinimizeButtonClicked
        {
            add { AddHandler(MinimizeButtonClickedEvent, value); }
            remove { RemoveHandler(MinimizeButtonClickedEvent, value); }
        }

        /// <summary>
        /// The full screen window event.
        /// </summary>
        public event RoutedEventHandler MinMaxButtonClicked
        {
            add { AddHandler(MinMaxButtonClickedEvent, value); }
            remove { RemoveHandler(MinMaxButtonClickedEvent, value); }
        }

        /// <summary>
        /// Gets or sets window close command.
        /// </summary>
        public ICommand CloseWindowCommand
        {
            get => (ICommand)GetValue(CloseWindowProperty);
            set => SetValue(CloseWindowProperty, value);
        }

        /// <summary>
        /// Gets or sets window title text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private void OnMinimizeWindowButtonClicked(object sender, RoutedEventArgs e)
            => RaiseEvent(new RoutedEventArgs(MinimizeButtonClickedEvent));

        private void OnMinMaxWindowButtonClicked(object sender, RoutedEventArgs e)
            => RaiseEvent(new RoutedEventArgs(MinMaxButtonClickedEvent));
    }
}
