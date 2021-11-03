using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class FirewallHelper
    {
        private static INetFwPolicy2 firewallPolicy = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")) as INetFwPolicy2;

        internal static List<INetFwRule> GetGroupRule(string groupName) => firewallPolicy.Rules.OfType<INetFwRule>().Where(rule => rule.Grouping == groupName).ToList();

        internal static bool IsRuleGroupEnabled(int profileBitMask, string groupName) => firewallPolicy.IsRuleGroupEnabled(profileBitMask, groupName);
    }
}