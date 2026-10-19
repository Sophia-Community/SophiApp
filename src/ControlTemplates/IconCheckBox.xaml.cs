// <copyright file="IconCheckBox.xaml.cs" company="Team Sophia">
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
    /// Implements the logic and appearance of the <see cref="IconCheckBox"/> element.
    /// </summary>
    public sealed partial class IconCheckBox : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(IconCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="IconIndex"/>.
        /// </summary>
        public static readonly DependencyProperty IconIndexProperty = DependencyProperty.Register("IconIndex", typeof(int), typeof(IconCheckBox), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="IconCheckBox"/> class.
        /// </summary>
        public IconCheckBox()
        {
            InitializeComponent();
            var shellViewModel = App.GetService<ShellViewModel>();
            FontOptions = shellViewModel.FontOptions;
            DebugOptions = shellViewModel.DebugOptions;
        }

        /// <summary>
        /// Gets app debug mode options.
        /// </summary>
        public DebugOptions DebugOptions { get; }

        /// <summary>
        /// Gets the app font sizes.
        /// </summary>
        public FontOptions FontOptions { get; }

        /// <summary>
        /// Gets or sets <see cref="IconCheckBox"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets icon index.
        /// </summary>
        public int IconIndex
        {
            get => (int)GetValue(IconIndexProperty);
            set => SetValue(IconIndexProperty, value);
        }

        private void TextCommandsFlyoutCopyDescription_Click(object sender, RoutedEventArgs e)
           => ContextMenuHelper.CopyToClipboard(DescriptionTextBlock.Text);

        private void TextCommandsFlyoutCopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(TitleTextBlock.Text);

        private void IconCheckBox_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, TextCommandsFlyout, args);
    }
}
