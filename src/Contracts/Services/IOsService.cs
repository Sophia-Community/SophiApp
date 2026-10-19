// <copyright file="IOsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using System.ServiceProcess;

    /// <summary>
    /// A service for working with Windows services API.
    /// </summary>
    public interface IOsService
    {
        /// <summary>
        /// Get service <paramref name="name"/> state.
        /// </summary>
        /// <param name="name">Service name.</param>
        ServiceControllerStatus GetStatus(string name);

        /// <summary>
        /// Sets the startup mode of the Windows service.
        /// </summary>
        /// <param name="service">Represents a Windows service and allows you to connect to a running or stopped.</param>
        /// <param name="mode">Indicates the start mode of the service.</param>
        void SetStartMode(ServiceController service, ServiceStartMode mode);

        /// <summary>
        /// Returns true if the service exists.
        /// </summary>
        /// <param name="name">Service name.</param>
        bool Exist(string name);

        /// <summary>
        /// Try set, without any exceptions, service startup mode.
        /// </summary>
        /// <param name="name">Service name.</param>
        /// <param name="mode">Indicates the start mode of the service.</param>
        void TrySetStartMode(string name, ServiceStartMode mode);

        /// <summary>
        /// Try start, without any exceptions, service.
        /// </summary>
        /// <param name="name">Service name.</param>
        bool TryStart(string name);
    }
}
