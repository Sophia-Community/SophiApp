// <copyright file="IFirewallService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using NetFwTypeLib;

    /// <summary>
    /// A service for working with Windows firewall API.
    /// </summary>
    public interface IFirewallService
    {
        /// <summary>
        /// Gets firewall group rules using the group name.
        /// </summary>
        /// <param name="groupName">The name of group to search rules.</param>
        IEnumerable<INetFwRule> GetGroupRules(string groupName);
    }
}
