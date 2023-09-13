// <copyright file="IModelBuilderService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.Collections.ObjectModel;
    using SophiApp.Models;

    /// <summary>
    /// MVVM pattern model builder service.
    /// </summary>
    public interface IModelBuilderService
    {
        /// <summary>
        /// Parses "UIMarkup.json" file and build the <see cref="UIModel"/> classes.
        /// </summary>
        ObservableCollection<UIModel> BuildUIModels();
    }
}
