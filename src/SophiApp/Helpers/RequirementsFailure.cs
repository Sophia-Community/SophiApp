// <copyright file="RequirementsFailure.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
#pragma warning disable CS1591 // There is no XML comment for an open visible type or member.

    /// <summary>
    /// Reasons for failure requirements.
    /// </summary>
    public enum RequirementsFailure
    {
        FeatureExperiencePackRemoved,
        Is32BitOs,
        MalwareDetected,
        MsDefenderFilesMissing,
        MsDefenderIsBroken,
        MsDefenderServiceStopped,
        RebootRequired,
        RunByNotLoggedUser,
        Win10EnterpriseSVersion,
        Win10UnsupportedBuild,
        Win10UpdateBuildRevisionLess3448,
        Win11BuildEqual22631,
        Win11BuildLess22631,
        WMIBroken,
    }

#pragma warning restore CS1591 // There is no XML comment for an open visible type or member.
}
