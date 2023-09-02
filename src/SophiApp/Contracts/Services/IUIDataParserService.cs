// <copyright file="IUIDataParserService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// Service for working with "UIData.json" file.
    /// </summary>
    public interface IUIDataParserService
    {
        /// <summary>
        /// Parses the file "UIData.json" and returns a collection of <see cref="UIControlDto"/>.
        /// </summary>
        Task ParseAsync();
    }
}
