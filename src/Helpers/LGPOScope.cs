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
        Computer,
        User,
    }
}
