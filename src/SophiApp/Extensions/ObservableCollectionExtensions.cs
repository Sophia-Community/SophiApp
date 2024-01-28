// <copyright file="ObservableCollectionExtensions.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.Collections.ObjectModel;
    using SophiApp.Models;

    /// <summary>
    /// Implements ObservableCollection extensions.
    /// </summary>
    public static class ObservableCollectionExtensions
    {
        /// <summary>
        /// Performs the specified action on each element of the collection.
        /// </summary>
        /// <param name="collection"><see cref="UIModel"/> collection.</param>
        /// <param name="action">Encapsulates a method that has a single parameter and does not return a value.</param>
        public static void ForEach(this ObservableCollection<UIModel> collection, Action<UIModel> action)
        {
            foreach (var item in collection)
            {
                action(item);
            }
        }
    }
}
