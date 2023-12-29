// <copyright file="ConcurrentBagExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.Collections.Concurrent;
    using SophiApp.Helpers;
    using SophiApp.Models;

    /// <summary>
    /// Implements ConcurrentBag extensions.
    /// </summary>
    public static class ConcurrentBagExtensions
    {
        /// <summary>
        /// Filter collections by <see cref="UICategoryTag"/> tag.
        /// </summary>
        /// <param name="models"><see cref="UIModel"/> collection.</param>
        /// <param name="tag"><see cref="UICategoryTag"/> tag.</param>
        public static List<UIModel> FilterByTag(this ConcurrentBag<UIModel> models, UICategoryTag tag)
        {
            return models.Where(m => m.Tag == tag).ToList();
        }
    }
}
