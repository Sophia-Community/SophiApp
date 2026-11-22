// <copyright file="SettingChoice.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// An item of a list that is known only when the app runs, such as a Linux distribution to install.
    /// </summary>
    /// <param name="Id">What the setting gets when the item is applied.</param>
    /// <param name="Title">The title of the item.</param>
    /// <param name="Subtitle">The second line under the title, or an empty string.</param>
    /// <param name="IsEnabled">False if the item is shown but cannot be chosen.</param>
    /// <param name="Icon">The address of the icon of the item, if it has one.</param>
    public sealed record SettingChoice(string Id, string Title, string Subtitle, bool IsEnabled, Uri? Icon = null)
    {
        /// <summary>
        /// Gets a value indicating whether the item is the one that is set in Windows now.
        /// A list with such an item always has an item selected; a list without one starts with "nothing is chosen".
        /// </summary>
        public bool IsCurrent { get; init; }
    }
}
