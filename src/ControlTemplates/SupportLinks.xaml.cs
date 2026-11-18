// <copyright file="SupportLinks.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ControlTemplates
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.ViewModels;

    /// <summary>
    /// Links to the places where the app team can be asked for help.
    /// </summary>
    public sealed partial class SupportLinks : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SupportLinks"/> class.
        /// </summary>
        public SupportLinks()
        {
            ViewModel = App.GetService<ShellViewModel>();
            InitializeComponent();
        }

        /// <summary>
        /// Gets the shell view model.
        /// </summary>
        public ShellViewModel ViewModel { get; }
    }
}
