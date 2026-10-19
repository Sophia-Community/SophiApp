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
        private string? statusText;

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupViewModel"/> class.
        /// </summary>
        public StartupViewModel()
        {
        }

        /// <summary>
        /// Gets or sets <see cref="StartupViewModel"/> status text.
        /// </summary>
        public string StatusText
        {
            get => statusText!;
            set
            {
                if (value is not null && statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }
    }
}
