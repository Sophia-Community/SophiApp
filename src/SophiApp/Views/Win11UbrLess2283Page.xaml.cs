// <copyright file="Win11UbrLess2283Page.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="Win11UbrLess2283Page"/> class.
    /// </summary>
    public sealed partial class Win11UbrLess2283Page : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Win11UbrLess2283Page"/> class.
        /// </summary>
        public Win11UbrLess2283Page()
        {
            ViewModel = App.GetService<Win11UbrLess2283ViewModel>();
            this.InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="Win11UbrLess2283ViewModel"/>.
        /// </summary>
        internal Win11UbrLess2283ViewModel ViewModel
        {
            get;
        }
    }
}
