// <copyright file="TextCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="TextCheckBox"/> element.
    /// </summary>
    public sealed partial class TextCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(TextCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCheckBox"/> class.
        /// </summary>
        public TextCheckBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> title.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }
    }
}
