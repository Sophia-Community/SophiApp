// <copyright file="SearchViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using Microsoft.UI.Xaml.Navigation;
    using SophiApp.Contracts.Services;
    using SophiApp.Models;
    using SophiApp.Views;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The search for settings by a text, and its results.
    /// </summary>
    public partial class SearchViewModel : ObservableRecipient
    {
        private readonly AppActivity activity;
        private readonly IModelService modelService;
        private readonly INavigationService navigationService;
        private readonly ShellViewModel shell;
        private bool searchPageIsOpen;
        private int searchNumber;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <param name="activity">What the app is doing now.</param>
        /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
        /// <param name="navigationService">Page navigation service.</param>
        /// <param name="shell">The view model that has all settings.</param>
        public SearchViewModel(AppActivity activity, IModelService modelService, INavigationService navigationService, ShellViewModel shell)
        {
            this.activity = activity;
            this.modelService = modelService;
            this.navigationService = navigationService;
            this.shell = shell;
            navigationService.Navigated += OnNavigated;
            Search_Command = new AsyncRelayCommand<string>(text => SearchAsync(text ?? string.Empty), AsyncRelayCommandOptions.AllowConcurrentExecutions);
        }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to show the settings that contain a text.
        /// </summary>
        public IAsyncRelayCommand<string> Search_Command { get; }

        /// <summary>
        /// Gets the settings found by the last search.
        /// </summary>
        public ObservableCollection<UIModel> FoundModels { get; } = [];

        private void OnNavigated(object sender, NavigationEventArgs e) => searchPageIsOpen = e.SourcePageType == typeof(SearchPage);

        private async Task SearchAsync(string text)
        {
            if (!activity.IsIdle)
            {
                return;
            }

            var number = ++searchNumber;

            if (string.IsNullOrWhiteSpace(text))
            {
                // An empty query closes the results
                if (searchPageIsOpen)
                {
                    _ = navigationService.GoBack();
                }

                return;
            }

            var found = await modelService.GetModelsContainsTextAsync(shell.SettingModels, text);

            // The text has been changed while the search was running
            if (number != searchNumber)
            {
                return;
            }

            ShowFoundModels(found);

            if (!searchPageIsOpen)
            {
                _ = navigationService.NavigateTo<SearchPage>(disablePageAnimation: true);
            }
        }

        // The list is changed in place, so the page redraws only the cards that appear or disappear while the text is typed
        private void ShowFoundModels(IReadOnlyList<UIModel> found)
        {
            var wanted = found.ToHashSet();

            for (var i = FoundModels.Count - 1; i >= 0; i--)
            {
                if (!wanted.Contains(FoundModels[i]))
                {
                    FoundModels.RemoveAt(i);
                }
            }

            for (var i = 0; i < found.Count; i++)
            {
                if (i >= FoundModels.Count || FoundModels[i] != found[i])
                {
                    FoundModels.Insert(i, found[i]);
                }
            }
        }
    }
}
