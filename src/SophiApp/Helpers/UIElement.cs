// <copyright file="UIElement.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// Implements the basic logic of the interface element.
    /// </summary>
    public class UIElement
    {
        /// <summary>
        /// Gets <see cref="UIElement"/> unique name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets <see cref="UIElement"/> unique tag.
        /// </summary>
        public string Tag { get; init; }

        /// <summary>
        /// Gets a value indicating whether <see cref="UIElement"/> supported windows 10.
        /// </summary>
        public bool Win10Supported { get; init; }

        /// <summary>
        /// Gets a value indicating whether <see cref="UIElement"/> supported windows 11.
        /// </summary>
        public bool Win11Supported { get; init; }

        /// <summary>
        /// Gets <see cref="UIElement"/> localized title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets <see cref="UIElement"/> localized description.
        /// </summary>
        public string Description { get; init; }
    }
}
