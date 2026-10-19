// <copyright file="IDisplayService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using Microsoft.UI.Windowing;

    /// <summary>
    /// A service for working with display API.
    /// </summary>
    public interface IDisplayService
    {
        /// <summary>
        /// Get the <see cref="DisplayArea"/> that shows the app <see cref="MainWindow"/>.
        /// </summary>
        Task<DisplayArea?> GetDisplayAreaAsync();
    }
}
