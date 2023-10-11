// <copyright file="RequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.ServiceProcess;
    using CSharpFunctionalExtensions;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.ViewModels;

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class RequirementsService : IRequirementsService
    {
        private readonly ICommonDataService commonDataService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsService"/> class.
        /// </summary>
        /// <param name="commonDataService">A service for working with common app data.</param>
        public RequirementsService(ICommonDataService commonDataService)
        {
            this.commonDataService = commonDataService;
        }

        /// <inheritdoc/>
        public Result GetWmiState()
        {
            var mgmtService = new ServiceController("Winmgmt");
            var mgmtRepository = "chcp 437 | winmgmt /verifyrepository".InvokeAsPoweShell();

            try
            {
                return mgmtService.Status == ServiceControllerStatus.Running && string.Equals(mgmtRepository[0].BaseObject as string, "WMI repository is consistent", StringComparison.InvariantCultureIgnoreCase)
                ? Result.Success()
                : Result.Failure(typeof(WmiStateViewModel).FullName!);
            }
            catch (Exception)
            {
                // TODO: Log error here!
                return Result.Failure(typeof(WmiStateViewModel).FullName!);
            }
        }

        /// <inheritdoc/>
        public Result GetOsVersion()
        {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous
            return commonDataService.OsProperties.BuildNumber switch
            {
                var build when commonDataService.IsWindows11 && build < 22000 => Result.Failure(typeof(Win11BuildLess22KViewModel).FullName!),
                var build when commonDataService.IsWindows11 && build == 22000 => Result.Failure(typeof(Win11Build22KViewModel).FullName!),
                var build when commonDataService.IsWindows11 && build == 22621 && commonDataService.OsProperties.UpdateBuildRevision < 2283 => Result.Failure(typeof(Win11UbrLess2283ViewModel).FullName!),
                _ => Result.Success()
            };
#pragma warning restore S2589 // Boolean expressions should not be gratuitous
        }
    }
}
