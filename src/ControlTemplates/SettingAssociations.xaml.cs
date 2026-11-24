// <copyright file="SettingAssociations.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.Windows.Storage.Pickers;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings card with the programs to open extensions with. Shows <see cref="UIAssociationsModel"/>.
    /// </summary>
    public sealed partial class SettingAssociations : UserControl
    {
        private static readonly string[] ProgramFiles = [".exe"];
        private static readonly string[] IconFiles = [".ico", ".exe", ".dll"];
        private static readonly string[] ExportedFiles = [".json"];

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingAssociations"/> class.
        /// </summary>
        public SettingAssociations()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // The button has an icon and a text inside, so a screen reader needs its name
            AutomationProperties.SetName(AddButton, "Association_Add/Text".GetLocalized());

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

        private static string GetDescription(object? model) => (model as UIAssociationsModel)?.Description ?? string.Empty;

        // The boxes of a block: the program, the extension and the icon
        private static (TextBox Program, TextBox Extension, TextBox Icon)? GetBoxes(FrameworkElement element)
        {
            // The boxes are known by their names to every element of the block
            for (var parent = element; parent is not null; parent = parent.Parent as FrameworkElement)
            {
                if (parent.FindName("ProgramBox") is TextBox program && parent.FindName("ExtensionBox") is TextBox extension && parent.FindName("IconBox") is TextBox icon)
                {
                    return (program, extension, icon);
                }
            }

            return null;
        }

        private static async Task<string> PickFileAsync(string[] extensions)
        {
            // This dialog works in an app that runs as administrator
            var picker = new FileOpenPicker(App.MainWindow.AppWindow.Id) { SuggestedStartLocation = PickerLocationId.ComputerFolder };

            foreach (var extension in extensions)
            {
                picker.FileTypeFilter.Add(extension);
            }

            return (await picker.PickSingleFileAsync())?.Path ?? string.Empty;
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var description = GetDescription(args.NewValue);
            ItemsList.ItemsSource = (args.NewValue as UIAssociationsModel)?.Items;

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

        // What stands in the boxes of a block is taken as the choice of the user
        private void Take(FrameworkElement? element)
        {
            if (element?.DataContext is not AssociationItem item || DataContext is not UIAssociationsModel model || GetBoxes(element) is not var (program, extension, icon))
            {
                return;
            }

            if (program.Text == item.Program && extension.Text == item.Extension && icon.Text == item.Icon)
            {
                return;
            }

            model.Update(item, program.Text, extension.Text, icon.Text);
            ChosenSettings.ChoiceChanged_Command.Execute(model);

            // The boxes show what is taken: without the spaces around, the extension in small letters
            (program.Text, extension.Text, icon.Text) = (item.Program, item.Extension, item.Icon);
        }

        private void Box_KeyDown(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                e.Handled = true;
                Take(sender as FrameworkElement);
            }
        }

        private void Box_LostFocus(object sender, RoutedEventArgs e) => Take(sender as FrameworkElement);

        private async void BrowseProgram_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement button && GetBoxes(button) is var (program, _, _) && await PickFileAsync(ProgramFiles) is { Length: > 0 } file)
            {
                program.Text = file;
                Take(button);
            }
        }

        private async void BrowseIcon_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement button && GetBoxes(button) is var (_, _, icon) && await PickFileAsync(IconFiles) is { Length: > 0 } file)
            {
                // The first icon of a program or a library
                icon.Text = file.EndsWith(".ico", StringComparison.OrdinalIgnoreCase) ? file : $"{file},0";
                Take(button);
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e) => (DataContext as UIAssociationsModel)?.Add();

        private void Remove_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as FrameworkElement)?.DataContext is AssociationItem item && DataContext is UIAssociationsModel model)
            {
                model.Remove(item);
                ChosenSettings.ChoiceChanged_Command.Execute(model);
            }
        }

        private async void Import_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UIAssociationsModel model && await PickFileAsync(ExportedFiles) is { Length: > 0 } file)
            {
                model.ChooseImport(file);
                ChosenSettings.ChoiceChanged_Command.Execute(model);
            }
        }

        private async void Export_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not UIAssociationsModel model)
            {
                return;
            }

            // The name is the one Sophia Script gives to its file: the files of the app and of the script are the same
            var picker = new FileSavePicker(App.MainWindow.AppWindow.Id)
            {
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "Application_Associations",
                DefaultFileExtension = ExportedFiles[0],
            };
            picker.FileTypeChoices.Add("JSON", ExportedFiles);

            if ((await picker.PickSaveFileAsync())?.Path is { Length: > 0 } file)
            {
                model.ChooseExport(file);
                ChosenSettings.ChoiceChanged_Command.Execute(model);
            }
        }

        private void CancelExport_Click(object sender, RoutedEventArgs e) => Cancel(model => model.ChooseExport(string.Empty));

        private void CancelImport_Click(object sender, RoutedEventArgs e) => Cancel(model => model.ChooseImport(string.Empty));

        private void Cancel(Action<UIAssociationsModel> cancel)
        {
            if (DataContext is UIAssociationsModel model)
            {
                cancel(model);
                ChosenSettings.ChoiceChanged_Command.Execute(model);
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
