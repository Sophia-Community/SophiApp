// <copyright file="RequirementsResult.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// Result of requirement action.
    /// </summary>
    public enum RequirementsResult
    {
        AllCorrect,
        UnsupportedArchitecture,
        NewAppVersionFound,
        LoggedInUserNotAdmin,
        HarmfulTweakerFound,
        HostsEntriesFound,
        UWPComponentsMissing,
        DefenderComponentsMissing,
        WindowsComponentStabilityDisrupted,
        DisableControlledFolderAccess,
        RebootPending,
        SystemDriveEncryptedBitLockerDisabled,
        UpdateUEFICertificates,
        WrongWindowsVersion,
        UpdateWindowsBuild,
    }
}
