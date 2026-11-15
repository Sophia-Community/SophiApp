// <copyright file="LGPOScope.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using SophiApp.Contracts.Services;

    /// <summary>
    /// The LGPO scope enumeration using by <see cref="IGroupPolicyService"/> methods.
    /// </summary>
    public enum LGPOScope
    {
        /// <summary>
        /// Policies of the computer.
        /// </summary>
        Computer,

        /// <summary>
        /// Policies of the current user.
        /// </summary>
        User,
    }
}
