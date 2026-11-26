// <copyright file="FatalErrorPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// The page of an error the app cannot get over: the notice, the way to the log and the links of the support.
    /// </summary>
    public sealed partial class FatalErrorPage : Page
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FatalErrorPage"/> class.
        /// </summary>
        public FatalErrorPage()
        {
            InitializeComponent();
            ViewModel = App.GetService<FatalErrorViewModel>();

            // The texts of the notice and of its buttons are the ones the page and its menu already have
            ErrorNotice.Title = "AppFatalError_1/Text".GetLocalized().TrimEnd(':', ' ');
            ErrorNotice.Message = "AppFatalError_2/Text".GetLocalized();
            OpenLogFileButton.Content = "Page_ContextMenu_OpenLogFile/Text".GetLocalized();
            OpenLogFolderButton.Content = "Page_ContextMenu_OpenLogFolder/Text".GetLocalized();
        }

        /// <summary>
        /// Gets view model for fatal error page.
        /// </summary>
        public FatalErrorViewModel ViewModel { get; }

        private void FatalErrorPageOpenLogFile_Clicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFile);

        private void FatalErrorPageOpenLogFolder_Clicked(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
            => ContextMenuHelper.OpenInExplorer(App.Logger.LogFolder);

        private void FatalErrorPage_ContextRequested(Microsoft.UI.Xaml.UIElement sender, Microsoft.UI.Xaml.Input.ContextRequestedEventArgs args)
            => ContextMenuHelper.ShowContextMenu(sender, FatalErrorPageCommandsFlyout, args);
    }
}
