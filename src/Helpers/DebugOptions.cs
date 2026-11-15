// <copyright file="DebugOptions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Gets or sets app debug mode options.
    /// </summary>
    public class DebugOptions : INotifyPropertyChanged
    {
        private bool showFunctionsInfo = false;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets or sets a value indicating whether functions detailed info.
        /// </summary>
        public bool ShowFunctionsInfo
        {
            get => showFunctionsInfo;
            set
            {
                if (showFunctionsInfo != value)
                {
                    showFunctionsInfo = value;
                    OnPropertyChanged(nameof(ShowFunctionsInfo));
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
