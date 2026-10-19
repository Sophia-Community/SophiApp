// <copyright file="DebugViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;

    /// <summary>
    /// Implements the <see cref="DebugViewModel"/> class.
    /// </summary>
    public partial class DebugViewModel : ObservableRecipient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DebugViewModel"/> class.
        /// </summary>
        /// <param name="settingsService">A service for working with app settings.</param>
        /// <param name="shellViewModel">Implements the <see cref="ShellViewModel"/> class.</param>
        /// <param name="requirementsService">A service for working with app requirements.</param>
        public DebugViewModel(ISettingsService settingsService, ShellViewModel shellViewModel, IRequirementsService requirementsService)
        {
            DebugOptions = shellViewModel.DebugOptions;
            DeleteLGPOFileCommand = shellViewModel.DeleteLGPOFile_Command;
            RequirementActions = requirementsService.GetActions();
            SaveDebugRequirementActionCommand = new AsyncRelayCommand<string>(s => settingsService.SaveDebugRequirementActionAsync(s!));
            SetShowFunctionsInfoCommand = shellViewModel.SetShowFunctionsInfo_Command;
        }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to click an "Delete LGPO.txt file" CheckBox in Settings page.
        /// </summary>
        public IRelayCommand DeleteLGPOFileCommand { get; }

        /// <summary>
        /// Gets app debug mode options.
        /// </summary>
        public DebugOptions DebugOptions { get; }

        /// <summary>
        /// Gets <see cref="RequirementAction"/> collections.
        /// </summary>
        public List<RequirementAction> RequirementActions { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to write requirement action name a settings file for debug.
        /// </summary>
        public IRelayCommand<string> SaveDebugRequirementActionCommand { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to click an "Show functions name and ID" CheckBox in Settings page.
        /// </summary>
        public IRelayCommand SetShowFunctionsInfoCommand { get; }
    }
}
