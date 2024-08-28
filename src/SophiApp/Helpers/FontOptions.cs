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
        private const string TitleTextSizeSettingKey = "TextHeaderSize";
        private const string DescriptionTextSizeSettingKey = "TextDescriptionSize";
        private readonly ISettingsService settingsService = App.GetService<ISettingsService>();
        private int descriptionTextSize;
        private int titleTextSize;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets a minimum font size for UI elements description.
        /// </summary>
        public int DescriptionTextMinSize { get; } = 12;

        /// <summary>
        /// Gets a maximum font size for UI elements description.
        /// </summary>
        public int DescriptionTextMaxSize { get; } = 22;

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
                    SaveTextSizeSetting(DescriptionTextSizeSettingKey, value);
                    OnPropertyChanged(nameof(DescriptionTextSize));
                }
            }
        }

        /// <summary>
        /// Gets a minimum font size for UI elements title.
        /// </summary>
        public int TitleTextMinSize { get; } = 14;

        /// <summary>
        /// Gets a maximum font size for UI elements header.
        /// </summary>
        public int TitleTextMaxSize { get; } = 26;

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
                    SaveTextSizeSetting(TitleTextSizeSettingKey, value);
                    OnPropertyChanged(nameof(TitleTextSize));
                }
            }
        }

        /// <summary>
        /// Initializes <see cref="FontOptions"/> data.
        /// </summary>
        public async Task InitializeAsync()
        {
            TitleTextSize = await ReadTextSizeSettingAsync(TitleTextSizeSettingKey, TitleTextMinSize, TitleTextMaxSize);
            DescriptionTextSize = await ReadTextSizeSettingAsync(DescriptionTextSizeSettingKey, DescriptionTextMinSize, DescriptionTextMaxSize);
        }

        private async Task<int> ReadTextSizeSettingAsync(string settingKey, int settingMinValue, int settingMaxValue)
        {
            var textSize = await settingsService.ReadSettingAsync<int>(settingKey);
            return textSize > 0 && textSize <= settingMaxValue ? textSize : settingMinValue;
        }

        private void SaveTextSizeSetting(string settingKey, int settingValue)
            => Task.Run(async () => await settingsService.SaveSettingAsync(settingKey, settingValue));

        private void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
