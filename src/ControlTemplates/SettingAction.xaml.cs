// <copyright file="SettingAction.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using System.ComponentModel;
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings card with a button that chooses an action. Shows <see cref="UIActionModel"/>.
    /// </summary>
    public sealed partial class SettingAction : UserControl
    {
        private UIActionModel? followed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAction"/> class.
        /// </summary>
        public SettingAction()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // The action is followed only while the card is on a page, so a closed page does not keep its cards alive
            Loaded += (_, _) => Follow(DataContext as UIActionModel);
            Unloaded += (_, _) => Follow(null);

            // While the app is busy the mouse is blocked by the page, and the keyboard is blocked here
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

        private static string GetDescription(object? model) => (model as UIActionModel)?.Description ?? string.Empty;

        private void Follow(UIActionModel? action)
        {
            if (followed is not null)
            {
                followed.PropertyChanged -= OnActionPropertyChanged;
            }

            followed = action;

            if (followed is not null)
            {
                followed.PropertyChanged += OnActionPropertyChanged;
            }

            ShowChoice();
        }

        private void OnActionPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName is nameof(UIModel.IsSelected))
            {
                ShowChoice();
            }
        }

        // The button is pressed while the action is chosen
        private void ShowChoice() => ActionButton.IsChecked = followed is { IsSelected: true };

        private void ActionButton_Click(object sender, RoutedEventArgs e)
        {
            // The button pressed again takes the choice back
            if (DataContext is UIActionModel action)
            {
                action.IsRequested = !action.IsRequested;
                ChosenSettings.ChoiceChanged_Command.Execute(action);
            }

            // A toggle button switches by itself when it is pressed; what it shows comes from the choice only
            ShowChoice();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (IsLoaded)
            {
                Follow(args.NewValue as UIActionModel);
            }

            var description = GetDescription(args.NewValue);
            var hasDescription = !string.IsNullOrEmpty(description);

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            if (args.NewValue is UIActionModel action)
            {
                // An action that has an icon of its own shows it as the other cards show their logos
                if (Customizations.SettingLooks.GetGlyph(action.Name) is string glyph)
                {
                    Card.HeaderIcon = new FontIcon { Glyph = glyph };
                }
                else
                {
                    Card.ClearValue(SettingsCard.HeaderIconProperty);
                }

                // A screen reader says what the button does to what: "Unpin, Unpin all Start tiles"
                AutomationProperties.SetName(ActionButton, $"{action.ButtonText}, {action.Title}");
                AutomationProperties.SetHelpText(ActionButton, FormattedText.ToPlainText(description));
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
