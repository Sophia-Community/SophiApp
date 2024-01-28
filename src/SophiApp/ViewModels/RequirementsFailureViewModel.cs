// <copyright file="RequirementsFailureViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// Implements the <see cref="RequirementsFailureViewModel"/> class.
    /// </summary>
    public partial class RequirementsFailureViewModel : ObservableRecipient
    {
        private readonly IUpdateService updateService;
        private readonly ICommonDataService commonDataService;

        [ObservableProperty]
        private string statusText = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="RequirementsFailureViewModel"/> class.
        /// </summary>
        /// <param name="updateService">A service for dealing with OS updates.</param>
        /// <param name="commonDataService">A service for transferring app data between layers of DI.</param>
        public RequirementsFailureViewModel(IUpdateService updateService, ICommonDataService commonDataService)
        {
            this.updateService = updateService;
            this.commonDataService = commonDataService;
        }

        /// <summary>
        /// Prepares the ViewModel for display in UI.
        /// </summary>
        /// <param name="reason">Reason for failure requirements.</param>
        public void PrepareForNavigation(RequirementsFailure reason)
        {
            StatusText = LocalizeFailureReason(reason);

            if (IsNeedUpdate(reason))
            {
                updateService.RunOsUpdate();
            }
        }

        private string LocalizeFailureReason(RequirementsFailure reason)
        {
            return reason switch
            {
                RequirementsFailure.Is32BitOs => "OsRequirementsFailure_Is32BitOs".GetLocalized(),
                RequirementsFailure.WMIBroken => "OsRequirementsFailure_WmiBroken".GetLocalized(),
                RequirementsFailure.Win11BuildLess22631 => string.Format("OsRequirementsFailure_Win11UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
                RequirementsFailure.Win11UbrLess2283 => string.Format("OsRequirementsFailure_Win11UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
                RequirementsFailure.Win10EnterpriseSVersion => "OsRequirementsFailure_Win10EnterpriseSVersion".GetLocalized(),
                RequirementsFailure.Win10UnsupportedBuild => string.Format("OsRequirementsFailure_Win10UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
                RequirementsFailure.Win10UpdateBuildRevisionLess3448 => string.Format("OsRequirementsFailure_Win10UnsupportedBuild".GetLocalized(), commonDataService.OsProperties.BuildNumber, commonDataService.OsProperties.UpdateBuildRevision),
                RequirementsFailure.RunByNotLoggedUser => "OsRequirementsFailure_RunByNotLoggedUser".GetLocalized(),
                RequirementsFailure.MalwareDetected => string.Format("OsRequirementsFailure_MalwareDetected".GetLocalized(), commonDataService.DetectedMalware),
                RequirementsFailure.FeatureExperiencePackRemoved => "OsRequirementsFailure_FeatureExperiencePackRemoved".GetLocalized(),
                RequirementsFailure.RebootRequired => "OsRequirementsFailure_RebootRequired".GetLocalized(),
                RequirementsFailure.MsDefenderFilesMissing => string.Format("OsRequirementsFailure_MsDefenderFilesMissing".GetLocalized(), commonDataService.MsDefenderFileMissing),
                RequirementsFailure.MsDefenderServiceStopped => commonDataService.MsDefenderServiceStopped.GetLocalized(),
                RequirementsFailure.MsDefenderIsBroken => "OsRequirementsFailure_MsDefenderIsBroken".GetLocalized(),
                _ => throw new ArgumentOutOfRangeException(paramName: nameof(reason), message: $"Value: {reason} is not found in {typeof(RequirementsFailure).FullName} enumeration.")
            };
        }

        private bool IsNeedUpdate(RequirementsFailure reason)
        {
            switch (reason)
            {
                case RequirementsFailure.Win11BuildLess22631:
                case RequirementsFailure.Win11UbrLess2283:
                case RequirementsFailure.Win10UnsupportedBuild:
                    return true;

                default:
                    return false;
            }
        }
    }
}
