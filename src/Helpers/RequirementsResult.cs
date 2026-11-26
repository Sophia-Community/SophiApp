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
        /// <summary>
        /// The requirement is met.
        /// </summary>
        AllCorrect,

        /// <summary>
        /// A newer version of the app is available.
        /// </summary>
        NewAppVersionFound,

        /// <summary>
        /// The app was run on behalf of a user who is not the logged-in one.
        /// </summary>
        LoggedUserNotAdmin,

        /// <summary>
        /// Windows was broken by a harmful tweaker.
        /// </summary>
        HarmfulTweakerFound,

        /// <summary>
        /// Some addresses the app gets its data from did not answer.
        /// </summary>
        ExternalDataUnavailable,

        /// <summary>
        /// The hosts file blocks resources that the app uses.
        /// </summary>
        HostsEntriesFound,

        /// <summary>
        /// Required UWP components are missing.
        /// </summary>
        UWPComponentsMissing,

        /// <summary>
        /// Microsoft Defender files are missing.
        /// </summary>
        DefenderComponentsMissing,

        /// <summary>
        /// A Windows component does not work.
        /// </summary>
        WindowsComponentStabilityDisrupted,

        /// <summary>
        /// Windows PowerShell is not allowed to start, most likely by an antivirus.
        /// </summary>
        PowerShellBlocked,

        /// <summary>
        /// Controlled folder access is turned on and has to be turned off.
        /// </summary>
        DisableControlledFolderAccess,

        /// <summary>
        /// Windows is waiting for a restart.
        /// </summary>
        RebootPending,

        /// <summary>
        /// The system drive is encrypted, but BitLocker protection is off.
        /// </summary>
        SystemDriveEncryptedBitLockerDisabled,

        /// <summary>
        /// UEFI certificates have to be updated.
        /// </summary>
        UpdateUEFICertificates,

        /// <summary>
        /// Secure Boot has to be enabled.
        /// </summary>
        EnableSecureBoot,

        /// <summary>
        /// The CPU architecture is not supported.
        /// </summary>
        UnsupportedArchitecture,

        /// <summary>
        /// The Windows version is not supported.
        /// </summary>
        WrongWindowsVersion,

        /// <summary>
        /// The Windows build is too old and has to be updated.
        /// </summary>
        UpdateWindowsBuild,
    }
}
