// <copyright file="FontOptions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// Modifies and saves the app font sizes to a setting file.
    /// </summary>
    public class FontOptions : INotifyPropertyChanged
    {
        private readonly ISettingsService settingsService = App.GetService<ISettingsService>();
        private int descriptionTextSize;
        private int titleTextSize;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets a minimum font size for UI elements description.
        /// </summary>
        public int DescriptionTextMinSize { get; private set; }

        /// <summary>
        /// Gets a maximum font size for UI elements description.
        /// </summary>
        public int DescriptionTextMaxSize { get; private set; }

        /// <summary>
        /// Gets or sets the font size for UI elements description.
        /// </summary>
        public int DescriptionTextSize
        {
            get => descriptionTextSize;
            set
            {
                if (descriptionTextSize != value)
                {
                    descriptionTextSize = value;
                    App.Logger.LogDescriptionTextSizeChanged(value);
                    Task.Run(async () => await settingsService.SaveTextDescriptionSizeAsync(value));
                    OnPropertyChanged(nameof(DescriptionTextSize));
                }
            }
        }

        /// <summary>
        /// Gets a minimum font size for UI elements title.
        /// </summary>
        public int TitleTextMinSize { get; private set; }

        /// <summary>
        /// Gets a maximum font size for UI elements title.
        /// </summary>
        public int TitleTextMaxSize { get; private set; }

        /// <summary>
        /// Gets or sets the font size for UI elements title.
        /// </summary>
        public int TitleTextSize
        {
            get => titleTextSize;
            set
            {
                if (titleTextSize != value)
                {
                    titleTextSize = value;
                    App.Logger.LogTitleTextSizeChanged(value);
                    Task.Run(async () => await settingsService.SaveTextTitleSizeAsync(value));
                    OnPropertyChanged(nameof(TitleTextSize));
                }
            }
        }

        /// <summary>
        /// Initializes <see cref="FontOptions"/> data.
        /// </summary>
        public async Task InitializeAsync()
        {
            DescriptionTextMinSize = settingsService.DescriptionTextMinSize;
            DescriptionTextMaxSize = settingsService.DescriptionTextMaxSize;
            descriptionTextSize = await settingsService.ReadTextDescriptionSizeAsync();
            TitleTextMinSize = settingsService.TitleTextMinSize;
            TitleTextMaxSize = settingsService.TitleTextMaxSize;
            titleTextSize = await settingsService.ReadTextTitleSizeAsync();
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
