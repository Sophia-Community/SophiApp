// <copyright file="SettingComboBox.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using CommunityToolkit.WinUI.Controls;
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using Microsoft.UI.Xaml.Data;
    using Microsoft.UI.Xaml.Media.Imaging;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using SophiApp.ViewModels;

    /// <summary>
    /// A settings card with a drop-down list of options. Shows the choices of one option out of any number.
    /// </summary>
    public sealed partial class SettingComboBox : UserControl
    {
        /// <summary>
        /// Identifies the <see cref="SelectedId"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedIdProperty = DependencyProperty.Register(
            nameof(SelectedId),
            typeof(string),
            typeof(SettingComboBox),
            new PropertyMetadata(string.Empty, (sender, _) => ((SettingComboBox)sender).SelectOption()));

        /// <summary>
        /// Identifies the <see cref="ResetRequested"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ResetRequestedProperty = DependencyProperty.Register(
            nameof(ResetRequested),
            typeof(bool),
            typeof(SettingComboBox),
            new PropertyMetadata(false, (sender, args) => ((SettingComboBox)sender).ResetButton.IsChecked = (bool)args.NewValue));

        /// <summary>
        /// Identifies the <see cref="Choices"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChoicesProperty = DependencyProperty.Register(
            nameof(Choices),
            typeof(object),
            typeof(SettingComboBox),
            new PropertyMetadata(null, (sender, args) => ((SettingComboBox)sender).OnChoicesChanged(args.NewValue)));

        /// <summary>
        /// Identifies the <see cref="ThirdParty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ThirdPartyProperty = DependencyProperty.Register(
            nameof(ThirdParty),
            typeof(object),
            typeof(SettingComboBox),
            new PropertyMetadata(null, (sender, args) => ((SettingComboBox)sender).OnThirdPartyChanged(args.NewValue)));

        // The room for an option icon: wide for the logos that are wordmarks, and a square of the same height for the logos that are signs,
        // so they all look the same size. The icon is drawn larger than it is shown, so it stays sharp at any display scale
        private const double OptionIconWidth = 44;
        private const double OptionIconHeight = 26;
        private const double OptionIconSharpness = 4;

        // The id of the state in which no option is selected
        private static readonly string NoOptionId = UIIconRadioButtonsModel.NoOptionId.ToString();

        private readonly List<ComboBoxItem> items = [];

        // The items the list is built from, if they are known only when the app runs
        private object? shownChoices;

        // The state that is none of the options, if the list has an item for it
        private ThirdPartyState? shownThirdParty;

        // True while the list only repeats the model
        private bool isFollowingModel;

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingComboBox"/> class.
        /// </summary>
        public SettingComboBox()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;

            // Follows the model when its state is read again
            SetBinding(SelectedIdProperty, new Binding { Path = new PropertyPath("SelectedId") });
            SetBinding(ResetRequestedProperty, new Binding { Path = new PropertyPath("ResetRequested") });

            // A list that is known only when the app runs can change when the state is read again
            SetBinding(ChoicesProperty, new Binding { Path = new PropertyPath("Choices") });

            // So can the item of a state that is none of the options: it is there only while Windows is in that state
            SetBinding(ThirdPartyProperty, new Binding { Path = new PropertyPath("ThirdParty") });

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

        /// <summary>
        /// Gets or sets the id of the option selected in the model: its number starting from 1, or the id of a list item.
        /// </summary>
        public string SelectedId
        {
            get => (string)GetValue(SelectedIdProperty);
            set => SetValue(SelectedIdProperty, value);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the reset to the Windows default is chosen in the model.
        /// </summary>
        public bool ResetRequested
        {
            get => (bool)GetValue(ResetRequestedProperty);
            set => SetValue(ResetRequestedProperty, value);
        }

        /// <summary>
        /// Gets or sets the items of a list that is known only when the app runs.
        /// </summary>
        public object? Choices
        {
            get => GetValue(ChoicesProperty);
            set => SetValue(ChoicesProperty, value);
        }

        /// <summary>
        /// Gets or sets what Windows is set to when it is none of the options of the model.
        /// </summary>
        public object? ThirdParty
        {
            get => GetValue(ThirdPartyProperty);
            set => SetValue(ThirdPartyProperty, value);
        }

        private static string GetDescription(object? model) => model switch
        {
            UIChoiceModel choice => choice.Description,
            UIRadioButtonsGroupModel group => group.Description,
            UIIconRadioButtonsModel group => group.Description,
            _ => string.Empty,
        };

        private static string[] GetOptionTitles(object? model) => model switch
        {
            UIRadioButtonsGroupModel group => [.. group.Options],
            UIIconRadioButtonsModel group => [.. group.Options],
            _ => [],
        };

        private static SvgImageSource? GetOptionIcon(Uri? icon, double width) => icon is null
            ? null
            : new SvgImageSource(icon) { RasterizePixelWidth = width * OptionIconSharpness, RasterizePixelHeight = OptionIconHeight * OptionIconSharpness };

        private static (string Title, ThirdPartyState? State) GetThirdParty(object? model) => model switch
        {
            UIRadioButtonsGroupModel group => (group.ThirdPartyTitle, group.ThirdParty),
            UIIconRadioButtonsModel group => (group.ThirdPartyTitle, group.ThirdParty),
            _ => (string.Empty, null),
        };

        private static IEnumerable<SettingOption> GetOptions(object? model, double scale)
        {
            if (model is UIChoiceModel choice)
            {
                var hasIcons = choice.Choices.Any(item => item.Icon is not null);

                // The item that stands for "nothing is chosen" is shown in the closed list, where an empty room would look like an indent
                return choice.Choices.Select(item => new SettingOption(item.Id, item.Title, GetOptionIcon(item.Icon, OptionIconHeight))
                {
                    IconWidth = OptionIconHeight,
                    Subtitle = item.Subtitle,
                    IsEnabled = item.IsEnabled,
                    KeepsIconRoom = hasIcons && item.Id != UIChoiceModel.NoChoiceId,
                });
            }

            // The id is the number of the option, starting from 1
            var iconGroup = model as UIIconRadioButtonsModel;
            var pictureGroup = model as UIRadioButtonsGroupModel;
            var linkHint = "SettingOption_OpenWebPage".GetLocalized();

            // The pictures of a plain choice are signs and get a square room, the logos of an icon choice can be wordmarks and get a wide one
            var iconWidth = iconGroup is null ? OptionIconHeight : OptionIconWidth;

            var options = GetOptionTitles(model).Select((title, index) => new SettingOption((index + 1).ToString(), title, GetOptionIcon(iconGroup?.GetOptionIcon(index + 1) ?? pictureGroup?.GetOptionIcon(index + 1), iconWidth))
            {
                IconWidth = iconWidth,
                Glyph = pictureGroup?.GetOptionGlyph(index + 1) ?? string.Empty,
                Link = iconGroup?.Links.ElementAtOrDefault(index) ?? string.Empty,
                LinkHint = linkHint,
            });

            var (thirdPartyTitle, thirdParty) = GetThirdParty(model);

            if (thirdParty is null)
            {
                return options;
            }

            // A cursor takes about a half of its square, and the margins are cut off, so it is drawn twice as large as it is shown
            var cursor = thirdParty.IconFile.Length > 0 ? CursorImage.Load(thirdParty.IconFile, (int)Math.Ceiling(OptionIconHeight * scale * 2)) : null;

            // What is set by another app is the last item: it is there only to be seen and to return to
            return options.Append(new SettingOption(ThirdPartyState.Id.ToString(), thirdPartyTitle, cursor)
            {
                IconWidth = iconWidth,
                Subtitle = thirdParty.Details,
                KeepsIconRoom = iconGroup is not null || pictureGroup?.HasOptionIcons == true,
            });
        }

        private void OnDataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            var description = GetDescription(args.NewValue);
            var hasDescription = !string.IsNullOrEmpty(description);

            CopyDescriptionButton.Visibility = hasDescription ? Visibility.Visible : Visibility.Collapsed;

            // A screen reader reads the description after the title of the setting
            AutomationProperties.SetHelpText(OptionsBox, FormattedText.ToPlainText(description));

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

            if (args.NewValue is UIChoiceModel { IconIndex: >= 0 } choice)
            {
                Card.HeaderIcon = SettingIcon.Create(choice.IconIndex);
            }
            else
            {
                Card.ClearValue(SettingsCard.HeaderIconProperty);
            }

            ResetButton.Visibility = args.NewValue is UIIconRadioButtonsModel ? Visibility.Visible : Visibility.Collapsed;
            ShowOptions();
        }

        private void OnChoicesChanged(object? choices)
        {
            // A setting that has just been shown already has its list built from these items
            if (!ReferenceEquals(choices, shownChoices))
            {
                ShowOptions();
            }
        }

        private void OnThirdPartyChanged(object? state)
        {
            // A setting that has just been shown already has the item in its list
            if (!Equals(state, shownThirdParty))
            {
                ShowOptions();
            }
        }

        private void ShowOptions()
        {
            var template = (DataTemplate)Resources["OptionTemplate"];
            shownChoices = (DataContext as UIChoiceModel)?.Choices;
            shownThirdParty = GetThirdParty(DataContext).State;

            // The window knows the display scale even when the card is not on the page yet
            var scale = (XamlRoot ?? App.MainWindow.Content?.XamlRoot)?.RasterizationScale ?? 1;

            isFollowingModel = true;
            OptionsBox.Items.Clear();
            items.Clear();

            // The options are list items of their own, so one that cannot be chosen is shown disabled
            foreach (var option in GetOptions(DataContext, scale))
            {
                // The option is kept in the tag too: the closed list takes the content of the selected item for itself
                // The content takes the whole width of the item, so what stands at its right edge is in one column
                var item = new ComboBoxItem { Content = option, ContentTemplate = template, IsEnabled = option.IsEnabled, Tag = option, HorizontalContentAlignment = HorizontalAlignment.Stretch };
                items.Add(item);
                OptionsBox.Items.Add(item);
            }

            isFollowingModel = false;
            SelectOption();

            // The closed list can stay blank for an item that was selected while the list was being built,
            // so the selection is set once more when the list is ready
            _ = DispatcherQueue.TryEnqueue(Microsoft.UI.Dispatching.DispatcherQueuePriority.Low, SelectOption);
        }

        private void SelectOption()
        {
            // No option with such an id leaves the list empty
            isFollowingModel = true;
            OptionsBox.SelectedItem = null;
            OptionsBox.SelectedItem = items.Find(item => (item.Content as SettingOption ?? item.Tag as SettingOption)?.Id == SelectedId);
            isFollowingModel = false;
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is UIIconRadioButtonsModel group)
            {
                // The button pressed again takes the reset back, and the option that is set in Windows is shown again
                group.ResetRequested = !group.ResetRequested;
                group.SelectedId = group.ResetRequested ? NoOptionId : group.DefaultId.ToString();
                ChosenSettings.ChoiceChanged_Command.Execute(group);
            }

            // A toggle button switches by itself when it is pressed; what it shows comes from the choice only
            ResetButton.IsChecked = (DataContext as UIIconRadioButtonsModel)?.ResetRequested ?? false;
        }

        private void OptionsBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Nothing to do if the selection only repeats the model
            if (isFollowingModel || (OptionsBox.SelectedItem as ComboBoxItem)?.Tag is not SettingOption option || option.Id == SelectedId)
            {
                return;
            }

            switch (DataContext)
            {
                case UIRadioButtonsGroupModel group:
                    group.SelectedId = option.Id;
                    ChosenSettings.ChoiceChanged_Command.Execute(group);
                    break;
                case UIChoiceModel choice:
                    choice.SelectedId = option.Id;
                    ChosenSettings.ChoiceChanged_Command.Execute(choice);
                    break;
                case UIIconRadioButtonsModel group:
                    // An option replaces the reset
                    group.ResetRequested = false;
                    group.SelectedId = option.Id;
                    ChosenSettings.ChoiceChanged_Command.Execute(group);
                    break;
            }
        }

        private void OptionsBox_DropDownOpened(object? sender, object e) => PopupCursor.Reset();

        private void OptionLink_Click(object sender, RoutedEventArgs e)
        {
            // The address is opened by the app itself: it runs elevated, and the browser must not
            if ((sender as FrameworkElement)?.Tag is string { Length: > 0 } link)
            {
                OptionsBox.IsDropDownOpen = false;
                App.GetService<IHttpService>().OpenUrl(link);
            }
        }

        private void CopyTitle_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard((DataContext as UIModel)?.Title);

        private void CopyDescription_Click(object sender, RoutedEventArgs e)
            => ContextMenuHelper.CopyToClipboard(FormattedText.ToPlainText(GetDescription(DataContext)));
    }
}
