// <copyright file="RequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Security.Principal;
    using System.ServiceProcess;
    using CSharpFunctionalExtensions;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

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
        public Result GetOsBitness()
        {
            return Environment.Is64BitOperatingSystem ? Result.Success() : Result.Failure(nameof(RequirementsFailure.Is32BitOs));
        }

        /// <inheritdoc/>
        public Result GetWmiState()
        {
            try
            {
                var repoState = "chcp 437 | winmgmt /verifyrepository".InvokeAsCmd();
                var serviceIsRun = new ServiceController("Winmgmt").Status == ServiceControllerStatus.Running;
                var repoIsConsistent = repoState.Contains("WMI repository is consistent", StringComparison.Ordinal);
                var captionIsCorrect = !string.IsNullOrEmpty(commonDataService.OsProperties.Caption);

                // TODO: Log WMI state here!
                return serviceIsRun && repoIsConsistent && captionIsCorrect ? Result.Success() : Result.Failure(nameof(RequirementsFailure.WMIBroken));
            }
            catch (Exception)
            {
                // TODO: Log error here!
                return Result.Failure(nameof(RequirementsFailure.WMIBroken));
            }
        }

        /// <inheritdoc/>
        public Result GetOsVersion()
        {
#pragma warning disable S2589 // Boolean expressions should not be gratuitous

            return commonDataService.OsProperties.BuildNumber switch
            {
                var build when commonDataService.IsWindows11 && build < 22000 => Result.Failure(nameof(RequirementsFailure.Win11BuildLess22k)),
                var build when commonDataService.IsWindows11 && build == 22000 => Result.Failure(nameof(RequirementsFailure.Win11BuildEqual22k)),
                var build when commonDataService.IsWindows11 && build == 22621 && commonDataService.OsProperties.UpdateBuildRevision < 2283 => Result.Failure(nameof(RequirementsFailure.Win11UBRLess2283)),
                var build when build == 19045 && commonDataService.OsProperties.UpdateBuildRevision < 3448 => Result.Failure(nameof(RequirementsFailure.Win10UBRLess3448)),
                var build when build < 19045 || build > 19045 => Result.Failure(nameof(RequirementsFailure.Win10WrongBuild)),
                var build when build == 17763 => Result.Failure(nameof(RequirementsFailure.Win10LTSC2k19)),
                var build when build == 19044 && commonDataService.OsProperties.Edition.Contains("EnterpriseS", StringComparison.InvariantCultureIgnoreCase) => Result.Failure(nameof(RequirementsFailure.Win10LTSC2k21)),
                var build when build == 19044 => Result.Failure(nameof(RequirementsFailure.Win10BuildEquals19044)),
                _ => Result.Success()
            };

#pragma warning restore S2589 // Boolean expressions should not be gratuitous
        }

        /// <inheritdoc/>
        public Result HasAdminRights()
        {
            using var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator) ? Result.Success() : Result.Failure(nameof(RequirementsFailure.UserIsNotAdmin));
        }
    }
}
