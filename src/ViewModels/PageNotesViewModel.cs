// <copyright file="PageNotesViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// The notes at the top of the pages with settings. A note that the user has closed stays closed on every page and after a restart.
    /// </summary>
    public partial class PageNotesViewModel : ObservableObject
    {
        private const string CurrentStateNote = "CurrentState";
        private const string PoliciesNote = "Policies";
        private const string TasksNote = "Tasks";
        private const string UwpAppsNote = "UwpApps";

        private readonly ISettingsService settingsService;

        [ObservableProperty]
        private bool currentStateIsOpen;
        [ObservableProperty]
        private bool policiesIsOpen;
        [ObservableProperty]
        private bool tasksIsOpen;
        [ObservableProperty]
        private bool uwpAppsIsOpen;

        /// <summary>
        /// Initializes a new instance of the <see cref="PageNotesViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">A service for working with app settings.</param>
        public PageNotesViewModel(ISettingsService settingsService)
        {
            this.settingsService = settingsService;
            currentStateIsOpen = !settingsService.ReadNoteIsClosed(CurrentStateNote);
            policiesIsOpen = !settingsService.ReadNoteIsClosed(PoliciesNote);
            tasksIsOpen = !settingsService.ReadNoteIsClosed(TasksNote);
            uwpAppsIsOpen = !settingsService.ReadNoteIsClosed(UwpAppsNote);
        }

        partial void OnCurrentStateIsOpenChanged(bool value) => RememberClosed(CurrentStateNote, value);

        partial void OnPoliciesIsOpenChanged(bool value) => RememberClosed(PoliciesNote, value);

        partial void OnTasksIsOpenChanged(bool value) => RememberClosed(TasksNote, value);

        partial void OnUwpAppsIsOpenChanged(bool value) => RememberClosed(UwpAppsNote, value);

        private void RememberClosed(string note, bool isOpen)
        {
            if (!isOpen)
            {
                _ = settingsService.SaveNoteIsClosedAsync(note);
            }
        }
    }
}
