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

        /// <summary>
        /// Puts a subtitle before the first model of every group.
        /// </summary>
        /// <param name="models">The models of a page in the order they are shown.</param>
        public static List<object> WithGroupHeaders(this IEnumerable<UIModel> models)
        {
            var items = new List<object>();
            var group = string.Empty;

            foreach (var model in models)
            {
                if (model.Group.Length > 0 && model.Group != group)
                {
                    items.Add(new SettingsGroupHeader($"SettingsGroup_{model.Group}".GetLocalized(), isFirst: items.Count == 0));
                }

                group = model.Group;
                items.Add(model);
            }

            return items;
        }
    }
}
