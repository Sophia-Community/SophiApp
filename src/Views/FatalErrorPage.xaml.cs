// <copyright file="FatalErrorPage.xaml.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Views
{
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Helpers;
    using SophiApp.ViewModels;

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
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
