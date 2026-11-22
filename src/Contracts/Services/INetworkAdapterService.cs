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
        /// Get the connected network adapters that have the "AllowComputerToTurnOffDevice" property to manage: the name, the description and the state of the property.
        /// </summary>
        IReadOnlyList<(string Name, string Title, string Description, bool IsEnabled, bool IsConnected)> GetTurnOffDevice();

        /// <summary>
        /// Set the "AllowComputerToTurnOffDevice" property state of the given network adapters and wait until the network is restored.
        /// </summary>
        /// <param name="adapters">The names of the adapters to change, with the new state of the property.</param>
        void SetTurnOffDevice(IReadOnlyDictionary<string, bool> adapters);
    }
}
