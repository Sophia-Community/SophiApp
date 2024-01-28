// <copyright file="IModelService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Collections.Concurrent;
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
        List<UIModel> BuildModels();

        /// <summary>
        /// Using multiple threads to get the models state.
        /// </summary>
        /// <param name="models"><see cref="UIModel"/> collection.</param>
        Task GetStateAsync(ConcurrentBag<UIModel> models);

        /// <summary>
        /// Using another thread get the models state.
        /// </summary>
        /// <param name="models"><see cref="UIModel"/> collection.</param>
        Task GetStateAsync(ObservableCollection<UIModel> models);
    }
}
