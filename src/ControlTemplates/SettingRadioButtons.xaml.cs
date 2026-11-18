// <copyright file="SettingRadioButtons.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
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
            OptionsExpander.Loaded += (_, _) => SettingsExpanderHelper.ApplyHeaderPadding(OptionsExpander);

            // Follows the model when its state is read again
            SetBinding(SelectedIdProperty, new Binding { Path = new PropertyPath("SelectedId") });

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
            var description = GetDescription(args.NewValue);
            var hasDescription = !string.IsNullOrEmpty(description);

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            if (hasDescription)
            {
                // A description can have line breaks, emphasized words and links
                FormattedTextBlock.SetText(DescriptionText, description);
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
                var text = new TextBlock { Text = options[i], TextWrapping = TextWrapping.Wrap, FlowDirection = FlowDirection.LeftToRight };

                // The tag is the number of the option, starting from 1
                // A mirrored radio button has its circle on the right, the text itself is not mirrored
                var option = new RadioButton
                {
                    Content = text,
                    Tag = (i + 1).ToString(),
                    FlowDirection = FlowDirection.RightToLeft,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    HorizontalContentAlignment = HorizontalAlignment.Stretch,
                };

                // A screen reader says the option and the setting it belongs to
                AutomationProperties.SetName(option, options[i]);
                AutomationProperties.SetHelpText(option, (args.NewValue as UIModel)?.Title ?? string.Empty);
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
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
