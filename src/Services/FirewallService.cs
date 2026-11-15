// <copyright file="FirewallService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Collections.Generic;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class FirewallService : IFirewallService
    {
        private readonly INetFwPolicy2 firewallPolicy;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirewallService"/> class.
        /// </summary>
        public FirewallService()
        {
            var netCfg = Type.GetTypeFromProgID("HNetCfg.FwPolicy2")!;
            firewallPolicy = (INetFwPolicy2)Activator.CreateInstance(netCfg)!;
        }

        /// <inheritdoc/>
        public IEnumerable<INetFwRule> GetGroupRules(string name)
        {
            // The group may have no rules, and it is not an error
            return firewallPolicy.Rules.OfType<INetFwRule>().Where(rule => rule.Grouping == name);
        }
    }
}
