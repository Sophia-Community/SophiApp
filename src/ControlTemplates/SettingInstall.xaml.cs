// <copyright file="SettingInstall.xaml.cs" company="Team Sophia">
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
    /// A settings card with the buttons that install, update and, for some packages, uninstall a package. Shows <see cref="UIInstallModel"/>.
    /// </summary>
    public sealed partial class SettingInstall : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingInstall"/> class.
        /// </summary>
        public SettingInstall()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // While the app is busy the mouse is blocked by the page, and the keyboard is blocked here
            PreviewKeyDown += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
            PreviewKeyUp += (_, args) => args.Handled = !ViewModel.NavigationViewHitTestVisible;
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        private static string GetDescription(object? model) => (model as UIInstallModel)?.Description ?? string.Empty;

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var description = GetDescription(args.NewValue);
            var hasDescription = !string.IsNullOrEmpty(description);

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;
            UninstallPanel.Visibility = args.NewValue is UIInstallModel { CanUninstall: true } ? Visibility.Visible : Visibility.Collapsed;
            AllUsersPanel.Visibility = args.NewValue is UIInstallModel { HasAllUsersOption: true } ? Visibility.Visible : Visibility.Collapsed;

            if (args.NewValue is UIInstallModel model)
            {
                Card.HeaderIcon = SettingIcon.Create(model.IconIndex);

                // A screen reader says what a button does to which package: "Install, OneDrive"
                foreach (var button in new[] { InstallButton, UpdateButton, UninstallButton })
                {
                    AutomationProperties.SetName(button, $"{button.Content}, {model.Title}");
                    AutomationProperties.SetHelpText(button, FormattedText.ToPlainText(description));
                }

                AutomationProperties.SetName(AllUsersCheckBox, $"{AllUsersCheckBox.Content}, {model.Title}");
            }

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
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
