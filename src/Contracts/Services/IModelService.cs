// <copyright file="IModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Collections.ObjectModel;
    using SophiApp.Models;

    /// <summary>
    /// A service for working with <see cref="UIModel"/> using MVVM pattern.
    /// </summary>
    public interface IModelService
    {
        /// <summary>
        /// Using the file "UIMarkup.json" creates a collection of <see cref="UIModel"/> types.
        /// </summary>
        Task<ObservableCollection<UIModel>> BuildJsonModelsAsync();

        /// <summary>
        /// Using the <see cref="IAppxPackagesService"/> creates a UWP <see cref="UIModel"/> collection.
        /// </summary>
        /// <param name="forAllUsers">Get collection of UWP <see cref="UIModel"/> for all users, otherwise only for the current user.</param>
        Task<List<UIModel>> BuildUwpAppModelsAsync(bool forAllUsers);

        /// <summary>
        /// Get <see cref="UIModel"/> state.
        /// </summary>
        /// <param name="models">A <see cref="UIModel"/> collection.</param>
        Task GetModelsState(ObservableCollection<UIModel> models);

        /// <summary>
        /// Get <see cref="UIModel"/> state.
        /// </summary>
        /// <param name="models">Models collection.</param>
        Task GetModelsStateAsync(ObservableCollection<UIModel> models);

        /// <summary>
        /// Set <see cref="UIModel"/> collection state.
        /// </summary>
        /// <param name="models">Models collection.</param>
        Task SetModelsStateAsync(ObservableCollection<UIModel> models);

        /// <summary>
        /// Returns models in which contain the specified text.
        /// </summary>
        /// <param name="models">Collection of <see cref="UIModel"/> to search.</param>
        /// <param name="text">Text to seek.</param>
        Task<ObservableCollection<UIModel>> GetModelsContainsTextAsync(ObservableCollection<UIModel> models, string text);
    }
}
