// <copyright file="IRequirementsService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Contracts.Services
{
    using CSharpFunctionalExtensions;

    /// <summary>
    /// Service for working with OS requirements.
    /// </summary>
    public interface IRequirementsService
    {
        /// <summary>
        /// Get os bitness.
        /// </summary>
        Result GetOsBitness();

        /// <summary>
        /// Get the state of Windows Management Instrumentation.
        /// </summary>
        Result GetWmiState();

        /// <summary>
        /// Get os version.
        /// </summary>
        Result GetOsVersion();

        /// <summary>
        /// Determines whether the logged in user has administrator privileges.
        /// </summary>
        Result HasAdminRights();
    }
}
