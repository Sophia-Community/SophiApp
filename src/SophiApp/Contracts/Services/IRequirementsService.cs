// <copyright file="IRequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using CSharpFunctionalExtensions;

    /// <summary>
    /// A service for working with app requirements.
    /// </summary>
    public interface IRequirementsService
    {
        /// <summary>
        /// Get os bitness.
        /// </summary>
        Result GetOsBitness();

        /// <summary>
        /// Get the Windows Management Instrumentation state.
        /// </summary>
        Result GetWmiState();

        /// <summary>
        /// Get os version.
        /// </summary>
        Result GetOsVersion();

        /// <summary>
        /// Detect that the app is run by a logged-in user.
        /// </summary>
        Result AppRunFromLoggedUser();

        /// <summary>
        /// Detect that Windows was not broken by 3rd party harmful tweakers and trojans.
        /// </summary>
        Result MalwareDetection();

        /// <summary>
        /// Get the Windows Feature Experience Pack state.
        /// </summary>
        Result GetFeatureExperiencePackState();

        /// <summary>
        /// Get a pending reboot state.
        /// </summary>
        Result GetPendingRebootState();

        /// <summary>
        /// Detect latest version of the app.
        /// </summary>
        Result AppUpdateDetection();

        /// <summary>
        /// Detect that Microsoft Defender files exist.
        /// </summary>
        Result GetMsDefenderFilesExist();

        /// <summary>
        /// Get Microsoft Defender services state.
        /// </summary>
        Result GetMsDefenderServicesState();

        /// <summary>
        /// Get a Microsoft Defender state.
        /// </summary>
        Result GetMsDefenderState();
    }
}
