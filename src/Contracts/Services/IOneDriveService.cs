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
        /// Get OneDrive user data folder path.
        /// </summary>
        string GetUserDataFolderOrDefault();

        /// <summary>
        /// Install OneDrive.
        /// </summary>
        void Install();

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
