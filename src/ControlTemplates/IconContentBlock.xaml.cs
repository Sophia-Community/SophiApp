// <copyright file="IconContentBlock.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="IconContentBlock"/> element.
    /// </summary>
    public sealed partial class IconContentBlock : UserControl
    {
        /// <summary>
        /// <see cref="Content"/>.
        /// </summary>
        public static new readonly DependencyProperty ContentProperty =
            DependencyProperty.Register(nameof(Content), typeof(object), typeof(IconContentBlock), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="IconContentBlock"/> class.
        /// </summary>
        public IconContentBlock()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="IconContentBlock"/> content.
        /// </summary>
        public new object Content
        {
            get => GetValue(ContentProperty);
            set => SetValue(ContentProperty, value);
        }
    }
}
