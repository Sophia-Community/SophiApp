// <copyright file="TextCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the logic and appearance of the <see cref="TextCheckBox"/> element.
    /// </summary>
    public sealed partial class TextCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(TextCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="TextCheckBox"/> class.
        /// </summary>
        public TextCheckBox()
        {
            this.InitializeComponent();
            FontOptions = App.GetService<ShellViewModel>().FontOptions;
        }

        /// <summary>
        /// Gets the app font sizes.
        /// </summary>
        public FontOptions FontOptions { get; }

        /// <summary>
        /// Gets or sets <see cref="TextCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        private void TextCommandsFlyoutCopyDescription_Click(object sender, RoutedEventArgs e)
           => ContextMenuHelper.CopyTextToClipboard(DescriptionTextBlock.Text);

        private void TextCommandsFlyoutCopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyTextToClipboard(TitleTextBlock.Text);

        private void TextCheckBox_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, TextCommandsFlyout, args);
    }
}
