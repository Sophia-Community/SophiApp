// <copyright file="Accessors.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using Microsoft.Win32.TaskScheduler;
    using NetFwTypeLib;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.ServiceProcess;
    using System.Text;
    using Windows.ApplicationModel;

    /// <summary>
    /// Get OS settings.
    /// </summary>
    public static partial class Accessors
    {
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService DataService = App.GetService<ICommonDataService>();
        private static readonly IDnsService DnsService = App.GetService<IDnsService>();
        private static readonly IRedistributablePackageService PackageService = App.GetService<IRedistributablePackageService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IGroupPolicyService GroupPolicyService = App.GetService<IGroupPolicyService>();
        private static readonly IHttpService HttpService = App.GetService<IHttpService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly INetworkAdapterService NetworkAdapterService = App.GetService<INetworkAdapterService>();
        private static readonly IOneDriveService OneDriveService = App.GetService<IOneDriveService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IUpdateService UpdateService = App.GetService<IUpdateService>();
        private static readonly IWslService WslService = App.GetService<IWslService>();

        // The logos of the families of Linux distributions in the Assets folder
        private static readonly Dictionary<string, string> WslLogos = new(StringComparer.OrdinalIgnoreCase)
        {
            ["AlmaLinux"] = "WSL_AlmaLinux.svg",
            ["archlinux"] = "WSL_archlinux.svg",
            ["AzureLinux"] = "WSL_AzureLinux.svg",
            ["Debian"] = "WSL_Debian.svg",
            ["eLxr"] = "WSL_eLxr.svg",

            ["Fedora"] = "WSL_Fedora.svg",
            ["kali"] = "WSL_kali.svg",
            ["openSUSE"] = "WSL_openSUSE.svg",
            ["RLCPlus"] = "WSL_RLCPlus.svg",
            ["SUSE"] = "WSL_SUSE.svg",
            ["Ubuntu"] = "WSL_Ubuntu.svg",
        };

        private static readonly IXmlService XmlService = App.GetService<IXmlService>();

        private static bool VirtualizationIsAvailable()
            => (InstrumentationService.CPUVirtualizationIsEnabled() ?? false) || (InstrumentationService.HypervisorIsPresent() ?? false);
    }
}
