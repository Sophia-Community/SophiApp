// <copyright file="IconHyperlink.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="IconHyperlink"/> element.
    /// </summary>
    public sealed partial class IconHyperlink : UserControl
    {
        /// <summary>
        /// <see cref="ImageSource"/>.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(IconHyperlink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Text"/>.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(IconHyperlink), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="IconHyperlink"/> class.
        /// </summary>
        public IconHyperlink() => InitializeComponent();

        /// <summary>
        /// Gets or sets <see cref="IconHyperlink"/> icon source.
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="IconHyperlink"/> Text.
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
    }
}
