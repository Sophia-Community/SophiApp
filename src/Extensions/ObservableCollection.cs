// <copyright file="ObservableCollection.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using SophiApp.Helpers;
    using SophiApp.Models;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Implements Observable collection extensions.
    /// </summary>
    public static class ObservableCollection
    {
        /// <summary>
        /// Filter collections by tag.
        /// </summary>
        /// <param name="models">Models collection.</param>
        /// <param name="tag">Models category tag.</param>
        public static IEnumerable<UIModel> FilterByTag(this ObservableCollection<UIModel> models, UICategoryTag tag)
            => models.Where(m => m.Tag == tag);
    }
}
