// <copyright file="ServiceController.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Extensions
{
    using System.ServiceProcess;

    /// <summary>
    /// Implements ServiceController extensions.
    /// </summary>
    public static class ServiceController
    {
        /// <summary>
        /// Try start the service.
        /// </summary>
        /// <param name="service">Represents a Windows service and allows you to connect to a running or stopped.</param>
        public static void TryStart(this System.ServiceProcess.ServiceController service)
        {
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                service.Start();
            }
        }

        /// <summary>
        /// Try stop the service.
        /// </summary>
        /// <param name="service">Represents a Windows service and allows you to connect to a running or stopped.</param>
        public static void TryStop(this System.ServiceProcess.ServiceController service)
        {
            if (service.Status == ServiceControllerStatus.Running)
            {
                service.Stop();
            }
        }
    }
}
