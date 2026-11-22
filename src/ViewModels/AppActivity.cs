// <copyright file="AppActivity.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// What the app is doing now. It is shared by the view models, so one of them can make the app busy and the others see it.
    /// </summary>
    public partial class AppActivity : ObservableObject
    {
        [ObservableProperty]
        private bool isIdle;
        [ObservableProperty]
        private bool progressIsVisible;
        [ObservableProperty]
        private string progressText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppActivity"/> class.
        /// </summary>
        /// <param name="statusService">A service that reports what is being done.</param>
        public AppActivity(IStatusService statusService)
        {
            // The status is reported from background threads
            statusService.Changed += text => App.MainWindow.DispatcherQueue.TryEnqueue(() => ProgressText = text);
        }
    }
}
