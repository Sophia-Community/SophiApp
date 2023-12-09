// <copyright file="ExpandingCheckBoxGroup.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="ExpandingCheckBoxGroup"/> element.
    /// </summary>
    public sealed partial class ExpandingCheckBoxGroup : UserControl
    {
        /// <summary>
        /// <see cref="Title"/>.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ExpandingCheckBoxGroup), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Description"/>.
        /// </summary>
        public static readonly DependencyProperty DescriptionProperty =
            DependencyProperty.Register("Description", typeof(string), typeof(ExpandingCheckBoxGroup), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpandingCheckBoxGroup"/> class.
        /// </summary>
        public ExpandingCheckBoxGroup()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="ExpandingCheckBoxGroup"/> title.
        /// </summary>
        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="ExpandingCheckBoxGroup"/> description.
        /// </summary>
        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }
    }
}
