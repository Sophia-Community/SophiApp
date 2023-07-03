// <copyright file="ContentBlock.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.UserControls
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="ContentBlock"/> element.
    /// </summary>
    public sealed partial class ContentBlock : UserControl
    {
        /// <summary>
        /// <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ContentBlock), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Content"/>.
        /// </summary>
        public static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(ContentBlock), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="ContentBlock"/> class.
        /// </summary>
        public ContentBlock() => InitializeComponent();

        /// <summary>
        /// Gets or sets <see cref="ContentBlock"/> title.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="ContentBlock"/> content.
        /// </summary>
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
