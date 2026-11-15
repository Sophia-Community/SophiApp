// <copyright file="SettingCheckBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Media.Imaging;
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
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Gets the text sizes chosen on the Settings page.
        /// </summary>
        public FontOptions FontOptions => ViewModel.FontOptions;

        private static string GetDescription(object? model) => model switch
        {
            UICheckBoxModel checkBox => checkBox.Description,
            UIIconCheckBoxModel iconCheckBox => iconCheckBox.Description,
            _ => string.Empty,
        };

        private static ImageIcon CreateIcon(int iconIndex)
        {
            var file = iconIndex == 0 ? "Windows11.svg" : "Folder.svg";
            return new ImageIcon { Source = new SvgImageSource(new Uri($"ms-appx:///Assets/{file}")) };
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var hasDescription = !string.IsNullOrEmpty(GetDescription(args.NewValue));

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            if (hasDescription)
            {
                Card.Description = DescriptionText;
            }
            else
            {
                // An empty description would leave a blank line in the card
                Card.ClearValue(SettingsCard.DescriptionProperty);
            }

            if (args.NewValue is UIIconCheckBoxModel iconCheckBox)
            {
                Card.HeaderIcon = CreateIcon(iconCheckBox.IconIndex);
            }
            else
            {
                Card.ClearValue(SettingsCard.HeaderIconProperty);
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(GetDescription(DataContext));
    }
}
