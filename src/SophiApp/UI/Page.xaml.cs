// <copyright file="Page.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using SophiApp.Helpers;

    /// <summary>
    /// Логика взаимодействия для Page.xaml.
    /// </summary>
    public partial class Page : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Tag.
        /// </summary>
        public static new readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(PageTag), typeof(Page), new PropertyMetadata(default));

        /// <summary>
        /// Using a DependencyProperty as the backing store for Title.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(Page), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="Page"/> class.
        /// </summary>
        public Page() => InitializeComponent();

        /// <summary>
        /// Gets or sets page title.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="Page"/> tag.
        /// </summary>
        public new PageTag Tag
        {
            get => (PageTag)GetValue(TagProperty);
            set => SetValue(TagProperty, value);
        }
    }
}
