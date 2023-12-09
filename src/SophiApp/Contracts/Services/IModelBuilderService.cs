// <copyright file="IModelBuilderService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;
    using SophiApp.Models;

    /// <summary>
    /// MVVM pattern model builder service.
    /// </summary>
    public interface IModelBuilderService
    {
        /// <summary>
        /// Using the file "UIMarkup.json" creates a collection of <see cref="UIModel"/> types.
        /// </summary>
        Task BuildModelsAsync();

        /// <summary>
        /// Gets models using tags.
        /// </summary>
        /// <param name="tag">Returned models tag.</param>
        List<UIModel> GetModelsByTag(UICategoryTag tag);
    }
}
