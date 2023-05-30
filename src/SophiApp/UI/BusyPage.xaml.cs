// <copyright file="BusyPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UI
{
    using System.Windows;
    using System.Windows.Controls;
    using SophiApp.Helpers;

    /// <summary>
    /// Логика взаимодействия для BusyPage.xaml.
    /// </summary>
    public partial class BusyPage : UserControl
    {
        /// <summary>
        /// Using a DependencyProperty as the backing store for Tag.
        /// </summary>
        public static new readonly DependencyProperty TagProperty =
            DependencyProperty.Register("Tag", typeof(PageTag), typeof(BusyPage), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="BusyPage"/> class.
        /// </summary>
        public BusyPage() => InitializeComponent();

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
