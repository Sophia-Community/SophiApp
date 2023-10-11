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
        /// Get the state of Windows Management Instrumentation.
        /// </summary>
        Result GetWmiState();

        /// <summary>
        /// Get Windows version.
        /// </summary>
        Result GetOsVersion();
    }
}
