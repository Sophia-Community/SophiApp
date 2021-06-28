using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class FirewallHelper
    {
        internal static List<INetFwRule> GetGroupRule(string groupName)
        {
            var firewallPolicy = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")) as INetFwPolicy2;
            return firewallPolicy.Rules.OfType<INetFwRule>().Where(rule => rule.Grouping == groupName).ToList();
        }
    }
}