// <copyright file="RequirementsFailurePage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Contracts.Services;
    using SophiApp.Helpers;
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
            var dataService = App.GetService<ICommonDataService>();
            ViewModel = App.GetService<ShellViewModel>();
            RequirementsResult_1 = dataService.RequirementsResult_1;
            RequirementsResult_2 = dataService.RequirementsResult_2;
        }

        /// <summary>
        /// Gets <see cref="ShellViewModel"/>.
        /// </summary>
        public ShellViewModel ViewModel { get; }

        /// <summary>
        /// Gets first localized description string of <see cref="ICommonDataService.RequirementsResult_1"/>.
        /// </summary>
        public string RequirementsResult_1 { get; }

        /// <summary>
        /// Gets second localized description string of <see cref="ICommonDataService.RequirementsResult_2"/>.
        /// </summary>
        public string RequirementsResult_2 { get; }

        private void RequirementsFailurePage_ContextRequested(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, RequirementsFailurePageCommandsFlyout, args);

        private void ContextMenuOpenLogFile_Clicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFile);

        private void ContextMenuOpenLogFolder_Clicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFolder);
    }
}
