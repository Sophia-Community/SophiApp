// <copyright file="ElementThemeWrapper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Microsoft.UI.Xaml;
    using SophiApp.Extensions;

    /// <summary>
    /// <see cref="ElementTheme"/> wrapper.
    /// </summary>
    public record ElementThemeWrapper
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ElementThemeWrapper"/> class.
        /// </summary>
        /// <param name="elementTheme">App elements theme.</param>
        /// <param name="key">Theme description localized key.</param>
        public ElementThemeWrapper(ElementTheme elementTheme, string key)
        {
            ElementTheme = elementTheme;
            Description = key.GetLocalized();
        }

        /// <summary>
        /// Gets app element theme description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets app elements theme.
        /// </summary>
        public ElementTheme ElementTheme { get; init; }
    }
}
