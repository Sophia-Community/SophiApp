// <copyright file="IOneDriveService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with Microsoft OneDrive API.
    /// </summary>
    public interface IOneDriveService
    {
        /// <summary>
        /// Get OneDriveSetup.exe file path or default.
        /// </summary>
        string GetSetupFileOrDefault();

        /// <summary>
        /// Install OneDrive.
        /// </summary>
        /// <param name="allUsers">True to install OneDrive for all users of the computer.</param>
        void Install(bool allUsers);

        /// <summary>
        /// Determines whether OneDrive is installed.
        /// </summary>
        bool IsInstalled();

        /// <summary>
        /// Uninstall OneDrive.
        /// </summary>
        void Uninstall();

        /// <summary>
        /// Determines whether user is logged into OneDrive.
        /// </summary>
        bool UserIsLogged();
    }
}
