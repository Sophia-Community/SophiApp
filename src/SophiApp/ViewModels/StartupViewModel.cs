// <copyright file="StartupViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;

    /// <summary>
    /// Implements the <see cref="StartupViewModel"/> class.
    /// </summary>
    public partial class StartupViewModel : ObservableRecipient
    {
        [ObservableProperty]
        private string statusText = string.Empty;

        [ObservableProperty]
        private int progressBarValue = 0;
    }
}
