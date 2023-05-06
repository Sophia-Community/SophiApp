// <copyright file="CategoryButton.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UI
{
    using System.Windows;
    using System.Windows.Controls;

    /// <summary>
    /// Логика взаимодействия для CategoryButton.xaml.
    /// </summary>
    public partial class CategoryButton : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Icon.
        /// </summary>
        public static readonly DependencyProperty IconProperty =
            DependencyProperty.Register("Icon", typeof(object), typeof(CategoryButton), new PropertyMetadata(default));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Text.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(CategoryButton), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryButton"/> class.
        /// </summary>
        public CategoryButton() => InitializeComponent();

        /// <summary>
        /// Gets or sets button icon.
        /// </summary>
        public object Icon
        {
            get => GetValue(IconProperty);
            set => SetValue(IconProperty, value);
        }

        /// <summary>
        /// Gets or sets button text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
