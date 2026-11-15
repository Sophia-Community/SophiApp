// <copyright file="INetworkAdapterService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with network adapters.
    /// </summary>
    public interface INetworkAdapterService
    {
        /// <summary>
        /// Get the "AllowComputerToTurnOffDevice" property state of the connected network adapters, or null if no adapter has this property to manage.
        /// </summary>
        bool? GetTurnOffDevice();

        /// <summary>
        /// Set the "AllowComputerToTurnOffDevice" property state of the connected network adapters and wait until the network is restored.
        /// </summary>
        /// <param name="enable">Network adapters "AllowComputerToTurnOffDevice" property state.</param>
        void SetTurnOffDevice(bool enable);
    }
}
