// <copyright file="TeamMateLink.xaml.cs" company="Team Sophia">
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
    /// Implements the logic and appearance of the <see cref="TeamMateLink"/> element.
    /// </summary>
    public sealed partial class TeamMateLink : UserControl
    {
        /// <summary>
        /// <see cref="ImageSource"/>.
        /// </summary>
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="WorkedOn"/>.
        /// </summary>
        public static readonly DependencyProperty WorkedOnProperty =
            DependencyProperty.Register("WorkedOn", typeof(string), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="NickName"/>.
        /// </summary>
        public static readonly DependencyProperty NickNameProperty =
            DependencyProperty.Register("NickName", typeof(string), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("CommandProperty", typeof(ICommand), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="CommandParameter"/>.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(string), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="Delimiter"/>.
        /// </summary>
        public static readonly DependencyProperty DelimiterProperty =
            DependencyProperty.Register("Delimiter", typeof(string), typeof(TeamMateLink), new PropertyMetadata(":"));

        /// <summary>
        /// <see cref="IconMargin"/>.
        /// </summary>
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register("IconMargin", typeof(Thickness), typeof(TeamMateLink), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamMateLink"/> class.
        /// </summary>
        public TeamMateLink() => InitializeComponent();

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> icon source.
        /// </summary>
        public ImageSource ImageSource
        {
            get => (ImageSource)GetValue(ImageSourceProperty);
            set => SetValue(ImageSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> mate worked on.
        /// </summary>
        public string WorkedOn
        {
            get => (string)GetValue(WorkedOnProperty);
            set => SetValue(WorkedOnProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> mate nickname.
        /// </summary>
        public string NickName
        {
            get => (string)GetValue(NickNameProperty);
            set => SetValue(NickNameProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> command.
        /// </summary>
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> command parameter.
        /// </summary>
        public string CommandParameter
        {
            get => (string)GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> string delimiter.
        /// </summary>
        public string Delimiter
        {
            get => (string)GetValue(DelimiterProperty);
            set => SetValue(DelimiterProperty, value);
        }

        /// <summary>
        /// Gets or sets <see cref="TeamMateLink"/> icon margin.
        /// </summary>
        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }

        private void TextBlock_PointerEntered(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            AppContextService.UserCursor = ProtectedCursor;
            ProtectedCursor = AppContextService.UrlCursor;
        }

        private void TextBlock_PointerExited(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            e.Handled = true;
            ProtectedCursor = AppContextService.UserCursor;
        }
    }
}
