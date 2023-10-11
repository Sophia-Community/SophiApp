// <copyright file="Win11BuildLess22KPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="Win11BuildLess22KPage"/> class.
    /// </summary>
    public sealed partial class Win11BuildLess22KPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Win11BuildLess22KPage"/> class.
        /// </summary>
        public Win11BuildLess22KPage()
        {
            ViewModel = App.GetService<Win11BuildLess22KViewModel>();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="Win11BuildLess22KViewModel"/>.
        /// </summary>
        public Win11BuildLess22KViewModel ViewModel
        {
            get;
        }
    }
}
