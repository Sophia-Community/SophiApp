// <copyright file="SettingOption.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using Microsoft.UI.Xaml.Media;

    /// <summary>
    /// An option of a setting as the drop-down list shows it.
    /// </summary>
    public sealed class SettingOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingOption"/> class.
        /// </summary>
        /// <param name="id">The id of the option: its number starting from 1, or what the setting gets when the option is applied.</param>
        /// <param name="title">The title of the option.</param>
        /// <param name="icon">The icon of the option, if it has one.</param>
        public SettingOption(string id, string title, ImageSource? icon)
        {
            Id = id;
            Title = title;
            Icon = icon;
        }

        /// <summary>
        /// Gets the id of the option.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the title of the option.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the second line under the title.
        /// </summary>
        public string Subtitle { get; init; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the option can be chosen.
        /// </summary>
        public bool IsEnabled { get; init; } = true;

        /// <summary>
        /// Gets the icon of the option.
        /// </summary>
        public ImageSource? Icon { get; }

        /// <summary>
        /// Gets the symbol of the icon font that is shown as the icon, or an empty string. It takes the color of the text.
        /// </summary>
        public string Glyph { get; init; } = string.Empty;

        /// <summary>
        /// Gets the width of the room for the icon.
        /// </summary>
        public double IconWidth { get; init; }

        /// <summary>
        /// Gets a value indicating whether the room for an icon is kept although the option has none:
        /// other options of the list have icons, and the titles stand in one column.
        /// </summary>
        public bool KeepsIconRoom { get; init; }

        /// <summary>
        /// Gets a value indicating whether the option has an icon to show or keeps the room for it.
        /// </summary>
        public Visibility IconVisibility => Icon is null && Glyph.Length == 0 && !KeepsIconRoom ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Gets the address of a web page about the option, if it has one.
        /// </summary>
        public string Link { get; init; } = string.Empty;

        /// <summary>
        /// Gets the hint of the button that opens the web page.
        /// </summary>
        public string LinkHint { get; init; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether the option has a web page to open.
        /// </summary>
        public Visibility LinkVisibility => Link.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Gets a value indicating whether the option has a second line to show.
        /// </summary>
        public Visibility SubtitleVisibility => Subtitle.Length == 0 ? Visibility.Collapsed : Visibility.Visible;

        /// <summary>
        /// Returns the title: a screen reader and the search by typing take it from here.
        /// </summary>
        /// <returns>The title of the option.</returns>
        public override string ToString() => Title;
    }
}
