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
        EvenLogBroken,
        FeatureExperiencePackRemoved,
        Is32BitOs,
        MalwareDetected,
        MsDefenderFilesMissing,
        MsDefenderIsBroken,
        MsDefenderServiceStopped,
        MsStoreRemoved,
        RebootRequired,
        RunByNotLoggedUser,
        SecuritySettingsPageHidden,
        Win10EnterpriseSVersion,
        Win10UnsupportedBuild,
        Win10UpdateBuildRevisionLess3448,
        Win11BuildLess22631,
        Win11UbrLess2283,
        WMIBroken,
    }

#pragma warning restore CS1591 // There is no XML comment for an open visible type or member.
}
