// <copyright file="SettingUserFolders.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Controls.Primitives;
    using Microsoft.Windows.Storage.Pickers;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings card with the locations of the user folders. Shows <see cref="UIUserFoldersModel"/>.
    /// </summary>
    public sealed partial class SettingUserFolders : UserControl
    {
        // The column of a row that has the name of a folder
        private const int NameColumn = 1;

        // The folder whose menu is open
        private UserFolderItem? menuFolder;

        // The width of the longest name of a folder
        private double nameColumnWidth;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingUserFolders"/> class.
        /// </summary>
        public SettingUserFolders()
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

        /// <summary>
        /// Gets the view model of the chosen settings.
        /// </summary>
        public ChosenSettingsViewModel ChosenSettings { get; } = App.GetService<ChosenSettingsViewModel>();

        private static string GetDescription(object? model) => (model as UIUserFoldersModel)?.Description ?? string.Empty;

        private static double MeasureName(string name)
        {
            var block = new TextBlock { Text = name };
            block.Measure(new Windows.Foundation.Size(double.PositiveInfinity, double.PositiveInfinity));
            return Math.Ceiling(block.DesiredSize.Width);
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var description = GetDescription(args.NewValue);
            var folders = (args.NewValue as UIUserFoldersModel)?.Folders;
            nameColumnWidth = folders is null ? 0 : folders.Max(folder => MeasureName(folder.Title));
            FoldersList.ItemsSource = folders;

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

        // The names of all rows get the same room, so the locations stand in a column
        private void FolderRow_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is not Grid row)
            {
                return;
            }

            if (nameColumnWidth > 0)
            {
                row.ColumnDefinitions[NameColumn].Width = new GridLength(nameColumnWidth);
            }

            // A screen reader tells the buttons of one row from the buttons of another: "Change: Desktop"
            if (row.DataContext is UserFolderItem folder)
            {
                // Some buttons stand in a grid of their own, under their mark
                var buttons = row.Children.OfType<ButtonBase>().Concat(row.Children.OfType<Grid>().SelectMany(cell => cell.Children.OfType<ButtonBase>()));

                foreach (var button in buttons)
                {
                    if (button.Content is string text)
                    {
                        AutomationProperties.SetName(button, $"{text}: {folder.Title}");
                    }
                }
            }
        }

        private void Choose(UserFolderItem folder, string path)
        {
            if (DataContext is UIUserFoldersModel model)
            {
                model.Choose(folder, path);
                ChosenSettings.ChoiceChanged_Command.Execute(model);
            }
        }

        // The menu offers a folder in the root of every other drive, and a dialog to choose any folder
        private void ChangeMenu_Opening(object? sender, object e)
        {
            if (sender is not MenuFlyout menu || menu.Target is not FrameworkElement { Tag: UserFolderItem folder })
            {
                return;
            }

            menuFolder = folder;

            // The drives are read anew every time: one can be plugged in while the app runs
            foreach (var old in menu.Items.Where(item => item.Tag is string).ToList())
            {
                _ = menu.Items.Remove(old);
            }

            var position = 0;

            // A location that is shown or already offered is not offered once more
            bool IsNew(string path) => !UserFolderHelper.IsSamePath(path, folder.Path) && !UserFolderHelper.IsSamePath(path, folder.CurrentPath);

            // A chosen location can be taken back for this folder alone
            if (folder.IsChanged)
            {
                var current = new MenuFlyoutItem
                {
                    Text = string.Format("UserFolder_Current".GetLocalized(), folder.CurrentPath),
                    Tag = folder.CurrentPath,
                    Icon = new FontIcon { Glyph = "" },
                };
                current.Click += (_, _) => Choose(folder, folder.CurrentPath);
                menu.Items.Insert(position++, current);
            }

            // The screenshots are often kept on the desktop
            if (folder.Folder == UserFolder.Screenshots)
            {
                var desktop = UserFolderHelper.GetPath(UserFolder.Desktop);

                if (IsNew(desktop))
                {
                    var item = new MenuFlyoutItem { Text = desktop, Tag = desktop };
                    item.Click += (_, _) => Choose(folder, desktop);
                    menu.Items.Insert(position++, item);
                }
            }

            foreach (var path in UserFolderHelper.GetOtherDrives().Select(drive => Path.Combine(drive, folder.Folder.ToString())).Where(IsNew))
            {
                var item = new MenuFlyoutItem { Text = path, Tag = path };
                item.Click += (_, _) => Choose(folder, path);
                menu.Items.Insert(position++, item);
            }

            if (position > 0)
            {
                menu.Items.Insert(position, new MenuFlyoutSeparator { Tag = string.Empty });
            }
        }

        private async void Browse_Click(object sender, RoutedEventArgs e)
        {
            if (menuFolder is not UserFolderItem folder)
            {
                return;
            }

            // This dialog works in an app that runs as administrator
            var picker = new FolderPicker(App.MainWindow.AppWindow.Id) { SuggestedStartLocation = PickerLocationId.ComputerFolder };
            var picked = await picker.PickSingleFolderAsync();

            if (picked is not null && picked.Path.Length > 0)
            {
                Choose(folder, picked.Path);
            }
        }

        private void PathBox_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                TakeTypedPath(sender as TextBox);
            }
        }

        private void PathBox_LostFocus(object sender, RoutedEventArgs e) => TakeTypedPath(sender as TextBox);

        // A typed location is chosen like one from the menu. One that cannot be used is replaced by the location that stays chosen
        private void TakeTypedPath(TextBox? box)
        {
            if (box?.Tag is not UserFolderItem folder || DataContext is not UIUserFoldersModel model || box.Text == folder.Path)
            {
                return;
            }

            model.ChooseTyped(folder, box.Text);
            ChosenSettings.ChoiceChanged_Command.Execute(model);
            box.Text = folder.Path;
        }

        private void Default_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not ToggleButton { Tag: UserFolderItem folder } button)
            {
                return;
            }

            // Pressed again, the button takes the choice back
            Choose(folder, folder.DefaultIsChosen ? folder.CurrentPath : folder.DefaultPath);

            // A toggle button switches by itself when it is pressed; what it shows comes from the choice only
            button.IsChecked = folder.DefaultIsChosen;
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
