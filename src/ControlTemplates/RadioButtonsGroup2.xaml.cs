// <copyright file="RadioButtonsGroup2.xaml.cs" company="Team Sophia">
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
    /// Implements the logic and appearance of the <see cref="RadioButtonsGroup2"/> element.
    /// </summary>
    public sealed partial class RadioButtonsGroup2 : UserControl
    {
        /// <summary>
        /// <see cref="Command"/>.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(IRelayCommand), typeof(RadioButtonsGroup2), new PropertyMetadata(default));

        /// <summary>
        /// <see cref="UserControlActualWidth"/>.
        /// </summary>
        public static readonly DependencyProperty UserControlActualWidthProperty =
            DependencyProperty.Register("UserControlActualWidth", typeof(double), typeof(RadioButtonsGroup2), new PropertyMetadata(default));

        /// <summary>
        /// Initializes a new instance of the <see cref="RadioButtonsGroup2"/> class.
        /// </summary>
        public RadioButtonsGroup2()
        {
            InitializeComponent();
            var shellViewModel = App.GetService<ShellViewModel>();
            DebugOptions = shellViewModel.DebugOptions;
            FontOptions = shellViewModel.FontOptions;
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
        /// Gets or sets <see cref="RadioButtonsGroup2"/> command.
        /// </summary>
        public IRelayCommand Command
        {
            get => (IRelayCommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets user control actual width"/>.
        /// </summary>
        public double UserControlActualWidth
        {
            get => (double)GetValue(UserControlActualWidthProperty);
            set => SetValue(UserControlActualWidthProperty, value);
        }

        private void TextCommandsFlyoutCopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(DescriptionTextBlock.Text);

        private void TextCommandsFlyoutCopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(TitleTextBlock.Text);

        private void ExpandingRadioGroup_ContextRequested(UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, TextCommandsFlyout, args);

        private void Border_SizeChanged(object sender, SizeChangedEventArgs e)
            => UserControlActualWidth = e.NewSize.Width;
    }
}
