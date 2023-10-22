// <copyright file="NetService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Net;
    using SophiApp.Contracts.Services;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class NetService : INetService
    {
        /// <inheritdoc/>
        public bool IsOnline()
        {
            try
            {
                var obtainedIps = Dns.GetHostEntry("dns.msftncsi.com").AddressList;
                var originalIp = new IPAddress(4294929283);
                return Array.Exists(obtainedIps, ip => ip.Equals(originalIp));
            }
            catch (Exception)
            {
                // TODO log exception here!
                return false;
            }
        }
    }
}
