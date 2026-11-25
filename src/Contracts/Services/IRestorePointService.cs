// <copyright file="IRestorePointService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with restore points of the system drive.
    /// </summary>
    public interface IRestorePointService
    {
        /// <summary>
        /// Creates a restore point for the system drive. System protection is turned on if it is off.
        /// </summary>
        void Create();

        /// <summary>
        /// Gets how much space the shadow copies of the system drive take up. Restore points are shadow copies,
        /// and Windows does not tell their size apart from the other ones.
        /// </summary>
        /// <returns>The space in gigabytes rounded to hundredths, or 0 if there is no shadow storage or it cannot be read.</returns>
        double GetShadowStorageGigabytes();

        /// <summary>
        /// Opens the window of Windows where restore points are managed.
        /// </summary>
        void OpenSettings();
    }
}
