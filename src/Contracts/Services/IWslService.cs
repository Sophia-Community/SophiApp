// <copyright file="IWslService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with Windows Subsystem for Linux (WSL).
    /// </summary>
    public interface IWslService
    {
        /// <summary>
        /// Gets the Linux distributions that can be installed: the family, such as "Ubuntu", the name for "wsl --install" and the name to show.
        /// The list is taken from the WSL repository, so an Internet connection is required.
        /// </summary>
        IReadOnlyList<(string Family, string Name, string FriendlyName)> GetDistributions();

        /// <summary>
        /// Starts downloading the list of the Linux distributions in the background, so the first <see cref="GetDistributions"/> does not wait for the network.
        /// </summary>
        void BeginGetDistributions();

        /// <summary>
        /// Gets the names of the distributions that are already installed for the current user.
        /// </summary>
        IReadOnlyList<string> GetInstalledDistributions();

        /// <summary>
        /// Installs WSL, if it is not installed yet, and a Linux distribution. Returns when the installer window is closed.
        /// </summary>
        /// <param name="distribution">The name of the distribution.</param>
        void Install(string distribution);
    }
}
