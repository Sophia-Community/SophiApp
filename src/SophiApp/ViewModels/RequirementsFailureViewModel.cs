// <copyright file="RequirementsFailureViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// Implements the <see cref="RequirementsFailureViewModel"/> class.
    /// </summary>
    public partial class RequirementsFailureViewModel : ObservableRecipient
    {
        private readonly IUpdateService updateService;

        [ObservableProperty]
        private string statusText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsFailureViewModel"/> class.
        /// </summary>
        /// <param name="updateService">A service for dealing with OS updates.</param>
        public RequirementsFailureViewModel(IUpdateService updateService)
        {
            this.updateService = updateService;
        }

        /// <summary>
        /// Prepares the ViewModel for display in interface.
        /// </summary>
        /// <param name="text">Set message text.</param>
        /// <param name="runUpdate">Start the system update.</param>
        public void PrepareForNavigation(string text, bool runUpdate)
        {
            StatusText = text;

            if (runUpdate)
            {
                updateService.RunOsUpdate();
            }
        }
    }
}
