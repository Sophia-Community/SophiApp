// <copyright file="RequirementsFailurePage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="RequirementsFailurePage"/> class.
    /// </summary>
    public sealed partial class RequirementsFailurePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsFailurePage"/> class.
        /// </summary>
        public RequirementsFailurePage()
        {
            InitializeComponent();
            ViewModel = App.GetService<RequirementsFailureViewModel>();
        }

        /// <summary>
        /// Gets view model for requirements failure page.
        /// </summary>
        public RequirementsFailureViewModel ViewModel { get; }
    }
}
