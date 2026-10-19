// <copyright file="IRequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with app requirements.
    /// </summary>
    public interface IRequirementsService
    {
        /// <summary>
        /// Gets <see cref="RequirementAction"/> collection.
        /// </summary>
        List<RequirementAction> GetActions();

        /// <summary>
        /// Initialize <see cref="IRequirementsService"/> data.
        /// </summary>
        void Initialize();
    }
}
