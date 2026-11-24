// <copyright file="IDnsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    /// <summary>
    /// A service for working with DNS servers of the network adapter in use.
    /// </summary>
    public interface IDnsService
    {
        /// <summary>
        /// Gets the number of the DNS-over-HTTPS provider whose servers the network adapter in use is set to.
        /// </summary>
        /// <returns>
        /// The number of the provider, starting from 1. 0 if the adapter does not encrypt the queries.
        /// <see cref="Helpers.ThirdPartyState.Id"/> if it encrypts them with other servers.
        /// </returns>
        int GetProvider();

        /// <summary>
        /// Gets the DNS servers of the network adapter in use that are queried over HTTPS.
        /// </summary>
        /// <returns>The addresses of the servers.</returns>
        IReadOnlyList<string> GetEncryptedServers();

        /// <summary>
        /// Sets the DNS servers of a provider for the network adapter in use and turns DNS-over-HTTPS on for them.
        /// If the default servers of the network answered before and the new ones do not, the default servers are returned and the method fails with the reason.
        /// </summary>
        /// <param name="provider">The number of the provider, starting from 1. 0 returns the DNS servers the network gives.</param>
        void SetProvider(int provider);
    }
}
