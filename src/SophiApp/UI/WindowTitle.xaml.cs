// <copyright file="WindowTitle.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UI
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Логика взаимодействия для WindowTitle.xaml.
    /// </summary>
    public partial class WindowTitle : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WindowTitle), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowTitle"/> class.
        /// </summary>
        public WindowTitle()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets window title text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}