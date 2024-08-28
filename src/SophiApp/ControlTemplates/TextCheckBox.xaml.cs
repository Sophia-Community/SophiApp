// <copyright file="TextCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="TextCheckBox"/> element.
    /// </summary>
    public sealed partial class TextCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="DescriptionSize"/>.
        /// </summary>
        public static readonly DependencyProperty DescriptionSizeProperty =
            DependencyProperty.Register("DescriptionSize", typeof(int), typeof(TextCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(TextCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="TitleSize"/>.
        /// </summary>
        public static readonly DependencyProperty TitleSizeProperty =
            DependencyProperty.Register("TitleSize", typeof(int), typeof(TextCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCheckBox"/> class.
        /// </summary>
        public TextCheckBox()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> description size.
        /// </summary>
        public int DescriptionSize
        {
            get => (int)GetValue(DescriptionSizeProperty);
            set => SetValue(DescriptionSizeProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> title size.
        /// </summary>
        public int TitleSize
        {
            get => (int)GetValue(TitleSizeProperty);
            set => SetValue(TitleSizeProperty, value);
        }
    }
}
