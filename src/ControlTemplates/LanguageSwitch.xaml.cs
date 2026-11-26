// <copyright file="LanguageSwitch.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Dispatching;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// The button of the startup pages that changes the language of the app at once.
    /// </summary>
    public sealed partial class LanguageSwitch : UserControl
    {
        private const string MenuGroup = "Languages";

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageSwitch"/> class.
        /// </summary>
        public LanguageSwitch()
        {
            ViewModel = App.GetService<StartupViewModel>();
            InitializeComponent();
            var current = AppLanguage.Current;

            foreach (var language in AppLanguage.Translations)
            {
                var isCurrent = language.Tag.Equals(current, StringComparison.OrdinalIgnoreCase);
                var item = new RadioMenuFlyoutItem { Text = language.Name, GroupName = MenuGroup, IsChecked = isCurrent, Tag = language.Tag };
                item.Click += Language_Click;
                LanguagesMenu.Items.Add(item);

                if (isCurrent)
                {
                    CurrentLanguage.Text = language.Name;
                }
            }
        }

        /// <summary>
        /// Gets <see cref="StartupViewModel"/>.
        /// </summary>
        public StartupViewModel ViewModel { get; }

        // The window is filled anew in the new language, so the menu is let to close first
        private void Language_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs args)
        {
            if (sender is RadioMenuFlyoutItem { Tag: string tag })
            {
                _ = DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Low, () => ViewModel.ChangeLanguage_Command.Execute(tag));
            }
        }
    }
}
