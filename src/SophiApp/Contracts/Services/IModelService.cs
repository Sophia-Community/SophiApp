// <copyright file="IModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Collections.Concurrent;
    using SophiApp.Models;

    /// <summary>
    /// A service for working with <see cref="UIModel"/> using MVVM pattern.
    /// </summary>
    public interface IModelService
    {
        /// <summary>
        /// Using the file "UIMarkup.json" creates a collection of <see cref="UIModel"/> types.
        /// </summary>
        Task<List<UIModel>> BuildJsonModelsAsync();

        /// <summary>
        /// Using the <see cref="IAppxPackagesService"/> creates a UWP <see cref="UIModel"/> collection.
        /// </summary>
        /// <param name="forAllUsers">Get collection of UWP <see cref="UIModel"/> for all users, otherwise only for the current user.</param>
        Task<List<UIModel>> BuildUwpAppModelsAsync(bool forAllUsers);

        /// <summary>
        /// Returns models in which contain the specified text.
        /// </summary>
        /// <param name="models">A collection of <see cref="UIModel"/> to search.</param>
        /// <param name="text">The text to seek.</param>
        Task<List<UIModel>> GetModelsContainsAsync(ConcurrentBag<UIModel> models, string text);

        /// <summary>
        /// Using multiple threads to get the <see cref="UIModel"/> state.
        /// </summary>
        /// <param name="models"><see cref="UIModel"/> collection.</param>
        Task GetStateAsync(ConcurrentBag<UIModel> models);

        /// <summary>
        /// Using multiple threads to get the models state.
        /// </summary>
        /// <param name="enumerable"><see cref="UIModel"/> collection.</param>
        /// <param name="getStateCallback">Action to be performed after invoke get state of each model.</param>
        Task GetStateAsync(IEnumerable<UIModel> enumerable, Action getStateCallback);

        /// <summary>
        /// Using multiple threads to set the models state.
        /// </summary>
        /// <param name="enumerable"><see cref="UIModel"/> collection.</param>
        /// <param name="setStateCallback">Action to be performed after invoke set state of each model.</param>
        /// <param name="token">Propagates notification that operations should be canceled.</param>
        Task SetStateAsync(IEnumerable<UIModel> enumerable, Action setStateCallback, CancellationToken token);
    }
}
