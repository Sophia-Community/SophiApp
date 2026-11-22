// <copyright file="SettingsGroupHeader.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// The subtitle that stands above a group of settings on a page.
    /// </summary>
    public sealed class SettingsGroupHeader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsGroupHeader"/> class.
        /// </summary>
        /// <param name="title">The localized name of the group.</param>
        /// <param name="isFirst">The subtitle is the first item on the page.</param>
        public SettingsGroupHeader(string title, bool isFirst)
        {
            Title = title;
            IsFirst = isFirst;
        }

        /// <summary>
        /// Gets the localized name of the group.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets a value indicating whether the subtitle is the first item on the page.
        /// </summary>
        public bool IsFirst { get; }
    }
}
