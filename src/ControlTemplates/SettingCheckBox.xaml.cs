// <copyright file="SettingCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings card with a check box. Shows <see cref="UICheckBoxModel"/> and <see cref="UIIconCheckBoxModel"/>.
    /// </summary>
    public sealed partial class SettingCheckBox : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingCheckBox"/> class.
        /// </summary>
        public SettingCheckBox()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // While the app is busy the mouse is blocked by the page, and the keyboard is blocked here.
            // The control is not disabled for that: a check box that was disabled and enabled again keeps a stale check mark
            PreviewKeyDown += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
            PreviewKeyUp += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Gets the view model of the chosen settings.
        /// </summary>
        public ChosenSettingsViewModel ChosenSettings { get; } = App.GetService<ChosenSettingsViewModel>();

        private static string GetDescription(object? model) => model switch
        {
            UICheckBoxModel checkBox => checkBox.Description,
            UIIconCheckBoxModel iconCheckBox => iconCheckBox.Description,
            _ => string.Empty,
        };

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var description = GetDescription(args.NewValue);
            var hasDescription = !string.IsNullOrEmpty(description);

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            // A screen reader reads the description after the title of the setting
            AutomationProperties.SetHelpText(StateCheckBox, FormattedText.ToPlainText(description));

            if (hasDescription)
            {
                // A description can have line breaks, emphasized words and links
                FormattedTextBlock.SetText(DescriptionText, description);
                Card.Description = DescriptionText;
            }
            else
            {
                // An empty description would leave a blank line in the card
                Card.ClearValue(SettingsCard.DescriptionProperty);
            }

            if (args.NewValue is UIIconCheckBoxModel iconCheckBox)
            {
                Card.HeaderIcon = SettingIcon.Create(iconCheckBox.IconIndex);
            }
            else
            {
                Card.ClearValue(SettingsCard.HeaderIconProperty);
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
