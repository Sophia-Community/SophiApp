// <copyright file="SettingCheckList.xaml.cs" company="Team Sophia">
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
    /// A settings card with a list of things that are turned on or off one by one. Shows <see cref="UICheckListModel"/>.
    /// </summary>
    public sealed partial class SettingCheckList : UserControl
    {
        private const int ShortListLength = 3;

        private UICheckListModel? followed;
        private bool isShown;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingCheckList"/> class.
        /// </summary>
        public SettingCheckList()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // The list is followed only while the card is on a page, so a closed page does not keep its cards alive
            Loaded += (_, _) => Follow(DataContext as UICheckListModel);
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

        private static string GetDescription(object? model) => (model as UICheckListModel)?.Description ?? string.Empty;

        private void Follow(UICheckListModel? list)
        {
            if (followed is not null)
            {
                followed.PropertyChanged -= OnListPropertyChanged;
            }

            followed = list;

            if (followed is not null)
            {
                followed.PropertyChanged += OnListPropertyChanged;

                // A short list is shown at once; a long one waits until the card is opened
                if (!isShown)
                {
                    isShown = true;
                    Expander.IsExpanded = followed.Items.Count is > 0 and <= ShortListLength;
                }
            }

            ShowAllState();
        }

        // The items are read anew when the settings are applied or the choice is canceled
        private void OnListPropertyChanged(object? sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName is nameof(UICheckListModel.Items))
            {
                ShowAllState();
            }
        }

        // The check box of the card is checked if all items are, and shows a dash if only some of them are
        private void ShowAllState()
        {
            var items = followed?.Items;

            if (items is null || items.Count == 0)
            {
                AllCheckBox.IsChecked = false;
                return;
            }

            var checkedItems = items.Count(item => item.IsChecked);

            if (checkedItems == 0 || checkedItems == items.Count)
            {
                AllCheckBox.IsChecked = checkedItems > 0;
                return;
            }

            AllCheckBox.IsChecked = null;
        }

        private void ChoiceChanged()
        {
            if (DataContext is UICheckListModel list)
            {
                ChosenSettings.ChoiceChanged_Command.Execute(list);
            }

            // A check box switches by itself when it is pressed; what the check box of the card shows comes from the items only
            ShowAllState();
        }

        // All items are turned on, or all are turned off if they all were on
        private void AllCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UICheckListModel list)
            {
                var check = !list.Items.All(item => item.IsChecked);

                foreach (var item in list.Items)
                {
                    item.IsChecked = check;
                }
            }

            ChoiceChanged();
        }

        private void ItemCheckBox_Click(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox { Tag: CheckListItem item } checkBox)
            {
                item.IsChecked = checkBox.IsChecked == true;
            }

            ChoiceChanged();
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (IsLoaded)
            {
                Follow(args.NewValue as UICheckListModel);
            }

            var description = GetDescription(args.NewValue);

            // A screen reader reads the description after the title of the setting
            AutomationProperties.SetHelpText(AllCheckBox, FormattedText.ToPlainText(description));

            if (description.Length > 0)
            {
                // A description can have line breaks, emphasized words and links
                FormattedTextBlock.SetText(DescriptionText, description);
                Expander.Description = DescriptionText;
            }
            else
            {
                // An empty description would leave a blank line in the card
                Expander.ClearValue(SettingsExpander.DescriptionProperty);
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
