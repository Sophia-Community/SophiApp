// <copyright file="WelcomePage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Automation;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// The first page of a new copy of the app: the terms the user accepts before anything is checked, and the privacy policy.
    /// </summary>
    public sealed partial class WelcomePage : Page
    {
        // The sections of the privacy policy in the strings file
        private const int PolicySections = 10;

        private readonly List<CheckBox> terms = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="WelcomePage"/> class.
        /// </summary>
        public WelcomePage()
        {
            ViewModel = App.GetService<StartupViewModel>();
            InitializeComponent();
            FindTerms(TermsPanel);
            FillPolicy();
            RestoreChoice();

            // The keyboard starts from the first item, not from the search box of the menu
            Loaded += (_, _) => terms.FirstOrDefault()?.Focus(FocusState.Pointer);
        }

        /// <summary>
        /// Gets <see cref="StartupViewModel"/>.
        /// </summary>
        public StartupViewModel ViewModel { get; }

        // A screen reader says the text of an item, which is a text block inside the check box
        private void FindTerms(Panel panel)
        {
            foreach (var child in panel.Children)
            {
                if (child is CheckBox box)
                {
                    AutomationProperties.SetName(box, (box.Content as TextBlock)?.Text ?? string.Empty);
                    terms.Add(box);
                }
                else if (child is Panel inner)
                {
                    FindTerms(inner);
                }
            }
        }

        private void FillPolicy()
        {
            PolicyTitle.Text = "PrivacyPolicy_Title".GetLocalized();
            PolicyUpdated.Text = "PrivacyPolicy_Updated".GetLocalized();

            for (var section = 1; section <= PolicySections; section++)
            {
                PolicyPanel.Children.Add(new TextBlock
                {
                    Text = $"PrivacyPolicy_{section}_Title".GetLocalized(),
                    Style = (Style)Application.Current.Resources["BodyStrongTextBlockStyle"],
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(0, 8, 0, 0),
                });

                var text = new TextBlock
                {
                    Style = (Style)Application.Current.Resources["BodyTextBlockStyle"],
                    TextWrapping = TextWrapping.Wrap,
                    IsTextSelectionEnabled = true,
                };

                // The last section has the addresses to write to
                FormattedTextBlock.SetText(text, string.Format($"PrivacyPolicy_{section}_Text".GetLocalized(), AppLinks.GitHub, AppLinks.Telegram));
                PolicyPanel.Children.Add(text);
            }
        }

        // When the language is changed the page is created anew: what was checked and what was opened stays
        private void RestoreChoice()
        {
            for (var number = 0; number < terms.Count; number++)
            {
                terms[number].IsChecked = ViewModel.CheckedTerms.Contains(number);
            }

            ShowContinue();
            ShowPolicy(ViewModel.PolicyIsOpen);
        }

        // The app goes on only when every item is checked
        private void ShowContinue()
            => ContinueButton.IsEnabled = terms.Count > 0 && terms.TrueForAll(box => box.IsChecked == true);

        private void ShowPolicy(bool show)
        {
            ViewModel.PolicyIsOpen = show;
            TermsView.Visibility = show ? Visibility.Collapsed : Visibility.Visible;
            PolicyView.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        private void Term_Changed(object sender, RoutedEventArgs args)
        {
            if (sender is CheckBox box && terms.IndexOf(box) is >= 0 and var number)
            {
                _ = box.IsChecked == true ? ViewModel.CheckedTerms.Add(number) : ViewModel.CheckedTerms.Remove(number);
            }

            ShowContinue();
        }

        private void ReadPolicy_Click(object sender, RoutedEventArgs args)
        {
            ShowPolicy(true);
            _ = BackButton.Focus(FocusState.Programmatic);
        }

        private void Back_Click(object sender, RoutedEventArgs args)
        {
            ShowPolicy(false);
            _ = ReadPolicyButton.Focus(FocusState.Programmatic);
        }
    }
}
