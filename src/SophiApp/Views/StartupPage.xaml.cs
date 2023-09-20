// <copyright file="StartupPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="StartupPage"/> class.
    /// </summary>
    public sealed partial class StartupPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartupPage"/> class.
        /// </summary>
        public StartupPage()
        {
            ViewModel = App.GetService<StartupViewModel>();
            InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="StartupViewModel"/>.
        /// </summary>
        public StartupViewModel ViewModel
        {
            get;
        }
    }
}
