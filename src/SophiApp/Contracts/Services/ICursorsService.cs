// <copyright file="ICursorsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with Windows cursors API.
    /// </summary>
    public interface ICursorsService
    {
        /// <summary>
        /// Set "Windows 11 Cursors Concept v2 from Jepri Creations" dark scheme.
        /// </summary>
        void SetJepriCreationsDarkCursors();

        /// <summary>
        /// Set "Windows 11 Cursors Concept v2 from Jepri Creations" light scheme.
        /// </summary>
        void SetJepriCreationsLightCursors();

        /// <summary>
        /// Set Windows cursors to default scheme.
        /// </summary>
        void SetDefaultCursors();
    }
}
