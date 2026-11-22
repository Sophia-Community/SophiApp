// <copyright file="Mutators.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Customizations
{
    using Microsoft.Win32;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System;
    using System.Collections.Generic;
    using System.ServiceProcess;
    using System.Text;
    using System.Text.Json.Nodes;
    using TaskScheduler = Microsoft.Win32.TaskScheduler;

    /// <summary>
    /// Set OS settings.
    /// </summary>
    public static partial class Mutators
    {
        private static readonly IAppNotificationService AppNotificationService = App.GetService<IAppNotificationService>();
        private static readonly IAppxPackagesService AppxPackagesService = App.GetService<IAppxPackagesService>();
        private static readonly ICommonDataService DataService = App.GetService<ICommonDataService>();
        private static readonly ICursorsService CursorsService = App.GetService<ICursorsService>();
        private static readonly IDnsService DnsService = App.GetService<IDnsService>();
        private static readonly IRedistributablePackageService RedistributablePackageService = App.GetService<IRedistributablePackageService>();
        private static readonly IFileService FileService = App.GetService<IFileService>();
        private static readonly IFirewallService FirewallService = App.GetService<IFirewallService>();
        private static readonly IGroupPolicyService GroupPolicyService = App.GetService<IGroupPolicyService>();
        private static readonly IInstrumentationService InstrumentationService = App.GetService<IInstrumentationService>();
        private static readonly INetworkAdapterService NetworkAdapterService = App.GetService<INetworkAdapterService>();
        private static readonly IOneDriveService OneDriveService = App.GetService<IOneDriveService>();
        private static readonly IOsService OsService = App.GetService<IOsService>();
        private static readonly IPowerShellService PowerShellService = App.GetService<IPowerShellService>();
        private static readonly IProcessService ProcessService = App.GetService<IProcessService>();
        private static readonly IRegistryService RegistryService = App.GetService<IRegistryService>();
        private static readonly IScheduledTaskService ScheduledTaskService = App.GetService<IScheduledTaskService>();
        private static readonly IUpdateService UpdateService = App.GetService<IUpdateService>();
        private static readonly IWslService WslService = App.GetService<IWslService>();
    }
}
