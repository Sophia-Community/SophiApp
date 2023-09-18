// <copyright file="IconHyperlink.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using System.Windows.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media;
    using SophiApp.Services;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="IconHyperlink"/> element.
    /// </summary>
    public sealed partial class IconHyperlink : UserControl
    {
        /// <summary>
        /// <see cref="CommandParameter"/>.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(string), typeof(IconHyperlink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(IconHyperlink), new PropertyMetadata(default));

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
        /// Gets or sets <see cref="IconHyperlink"/> command parameter.
        /// </summary>
        public string CommandParameter
        {
            get => (string)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="IconHyperlink"/> command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

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

        private void TextBlock_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            CommonDataService.UserCursor = ProtectedCursor;
            ProtectedCursor = CommonDataService.UrlCursor;
        }

        private void TextBlock_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ProtectedCursor = CommonDataService.UserCursor;
        }
    }
}
