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
            ViewModel = App.GetService<RequirementsFailureViewModel>();
            InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="RequirementsFailureViewModel"/>.
        /// </summary>
        public RequirementsFailureViewModel ViewModel
        {
            get;
        }
    }
}
