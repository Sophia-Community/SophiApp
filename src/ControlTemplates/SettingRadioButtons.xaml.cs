// <copyright file="SettingRadioButtons.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings expander with a list of options. Shows radio button groups of any size.
    /// </summary>
    public sealed partial class SettingRadioButtons : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="SelectedId"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIdProperty = DependencyProperty.Register(
            nameof(SelectedId),
            typeof(string),
            typeof(SettingRadioButtons),
            new PropertyMetadata(string.Empty, (sender, _) => ((SettingRadioButtons)sender).SelectOption()));

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingRadioButtons"/> class.
        /// </summary>
        public SettingRadioButtons()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // Follows the model when its state is read again
            SetBinding(SelectedIdProperty, new Binding { Path = new PropertyPath("SelectedId") });
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Gets the text sizes chosen on the Settings page.
        /// </summary>
        public FontOptions FontOptions => ViewModel.FontOptions;

        /// <summary>
        /// Gets or sets the number of the option selected in the model, starting from 1.
        /// </summary>
        public string SelectedId
        {
            get => (string)GetValue(SelectedIdProperty);
            set => SetValue(SelectedIdProperty, value);
        }

        private static string GetDescription(object? model) => model switch
        {
            UIRadioButtonsGroup2Model group => group.Description,
            UIRadioButtonsGroup3Model group => group.Description,
            UIRadioButtonsGroup4Model group => group.Description,
            _ => string.Empty,
        };

        private static string[] GetOptions(object? model) => model switch
        {
            UIRadioButtonsGroup2Model group => [group.Title_1, group.Title_2],
            UIRadioButtonsGroup3Model group => [group.Title_1, group.Title_2, group.Title_3],
            UIRadioButtonsGroup4Model group => [group.Title_1, group.Title_2, group.Title_3, group.Title_4],
            _ => [],
        };

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var hasDescription = !string.IsNullOrEmpty(GetDescription(args.NewValue));

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            if (hasDescription)
            {
                OptionsExpander.Description = DescriptionText;
            }
            else
            {
                // An empty description would leave a blank line in the header
                OptionsExpander.ClearValue(SettingsExpander.DescriptionProperty);
            }

            Options.Children.Clear();
            var options = GetOptions(args.NewValue);

            for (var i = 0; i < options.Length; i++)
            {
                var text = new TextBlock { Text = options[i], TextWrapping = TextWrapping.Wrap };
                text.SetBinding(TextBlock.FontSizeProperty, new Binding { Source = FontOptions, Path = new PropertyPath(nameof(FontOptions.TitleTextSize)) });

                // The tag is the number of the option, starting from 1
                var option = new RadioButton { Content = text, Tag = (i + 1).ToString() };
                option.Checked += Option_Checked;
                Options.Children.Add(option);
            }

            SelectOption();
        }

        private void SelectOption()
        {
            foreach (var option in Options.Children.OfType<RadioButton>())
            {
                option.IsChecked = (string)option.Tag == SelectedId;
            }
        }

        private void Option_Checked(object sender, RoutedEventArgs e)
        {
            var id = (string)((RadioButton)sender).Tag;

            // The selection only repeats the model
            if (id == SelectedId)
            {
                return;
            }

            switch (DataContext)
            {
                case UIRadioButtonsGroup2Model group:
                    group.SelectedId = id;
                    ViewModel.RadioButtonsGroup2Clicked_Command.Execute(group);
                    break;
                case UIRadioButtonsGroup3Model group:
                    group.SelectedId = id;
                    ViewModel.RadioButtonsGroup3Clicked_Command.Execute(group);
                    break;
                case UIRadioButtonsGroup4Model group:
                    group.SelectedId = id;
                    ViewModel.RadioButtonsGroup4Clicked_Command.Execute(group);
                    break;
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(GetDescription(DataContext));
    }
}
