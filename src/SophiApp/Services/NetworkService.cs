// <copyright file="NetworkService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Net;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class NetworkService : INetworkService
    {
        /// <inheritdoc/>
        public bool IsOnline()
        {
            bool isOnline = false;

            try
            {
                var obtainedIps = Dns.GetHostEntry("dns.msftncsi.com").AddressList;
                var originalIp = new IPAddress(4294929283);
                isOnline = Array.Exists(obtainedIps, ip => ip.Equals(originalIp));
            }
            catch (Exception ex)
            {
                App.Logger.LogIsOnlineException(ex);
            }

            App.Logger.LogIsOnline(isOnline);
            return isOnline;
        }
    }
}
