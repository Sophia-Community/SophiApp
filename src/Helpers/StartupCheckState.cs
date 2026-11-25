// <copyright file="StartupCheckState.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// What a check of the startup page has come to.
    /// </summary>
    public enum StartupCheckState
    {
        /// <summary>
        /// The check is in progress.
        /// </summary>
        Running,

        /// <summary>
        /// The check has passed.
        /// </summary>
        Passed,

        /// <summary>
        /// The check has failed, and the app cannot go on.
        /// </summary>
        Failed,

        /// <summary>
        /// The check has found something the user can pass by: the app asks, or the user has already chosen to go on.
        /// </summary>
        Continued,
    }
}
