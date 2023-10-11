// <copyright file="Win11Build22KPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="Win11Build22KPage"/> class.
    /// </summary>
    public sealed partial class Win11Build22KPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Win11Build22KPage"/> class.
        /// </summary>
        public Win11Build22KPage()
        {
            ViewModel = App.GetService<Win11Build22KViewModel>();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="Win11Build22KViewModel"/>.
        /// </summary>
        public Win11Build22KViewModel ViewModel
        {
            get;
        }
    }
}
