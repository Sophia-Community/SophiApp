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
        /// Starts a service, if it is not running, and waits until it runs. Fails if the service cannot be started.
        /// </summary>
        /// <param name="name">Service name.</param>
        /// <param name="startMode">The start mode the service gets before it is started, if it has another one.</param>
        void Start(string name, ServiceStartMode? startMode = null);
    }
}
