// <copyright file="SearchPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SearchPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchPage"/> class.
        /// </summary>
        public SearchPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<ShellViewModel>();
        }

        /// <summary>
        /// Gets view model for search page.
        /// </summary>
        public ShellViewModel ViewModel { get; }
    }
}
