// <copyright file="WmiStatePage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Implements the <see cref="WmiStatePage"/> class.
    /// </summary>
    public sealed partial class WmiStatePage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WmiStatePage"/> class.
        /// </summary>
        public WmiStatePage()
        {
            ViewModel = App.GetService<WmiStateViewModel>();
            InitializeComponent();
        }

        /// <summary>
        /// Gets <see cref="WmiStateViewModel"/>.
        /// </summary>
        public WmiStateViewModel ViewModel
        {
            get;
        }
    }
}
