// <copyright file="ICursorsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using SophiApp.Helpers;

    /// <summary>
    /// A service for working with Windows cursors API.
    /// </summary>
    public interface ICursorsService
    {
        /// <summary>
        /// Reload cursors on-the-fly.
        /// </summary>
        void ReloadCursors();

        /// <summary>
        /// Set Windows cursors to default scheme.
        /// </summary>
        void SetDefaultCursors();

        /// <summary>
        /// Set "Windows 11 Cursors Concept from Jepri Creations" scheme.
        /// </summary>
        /// <param name="theme">Cursor scheme.</param>
        void SetJepriCreationsCursors(JepriCursorsTheme theme);
    }
}
