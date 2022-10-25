using NetFwTypeLib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Helpers
{
    internal class FirewallHelper
    {
        // https://docs.microsoft.com/en-us/windows/win32/api/icftypes/ne-icftypes-net_fw_profile_type2
        // NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_DOMAIN = 1;
        // NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PRIVATE = 2;
        // NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_PUBLIC = 4;
        // NET_FW_PROFILE_TYPE2_.NET_FW_PROFILE2_ALL = int.MaxValue;

        private static INetFwPolicy2 firewallPolicy = Activator.CreateInstance(Type.GetTypeFromProgID("HNetCfg.FwPolicy2")) as INetFwPolicy2;

        internal static List<INetFwRule> GetGroupRule(string group) => firewallPolicy.Rules.OfType<INetFwRule>().Where(rule => rule.Grouping == group).ToList();

        internal static bool IsRuleGroupEnabled(string group) => firewallPolicy.IsRuleGroupCurrentlyEnabled[group];

        internal static void SetGroupRule(int profileMask, bool enable, params string[] groups)
        {
            foreach (var group in groups)
            {
                GetGroupRule(group).ForEach(rule =>
                {
                    rule.Enabled = enable;
                    rule.Profiles = profileMask;
                });
            }
        }
    }
}