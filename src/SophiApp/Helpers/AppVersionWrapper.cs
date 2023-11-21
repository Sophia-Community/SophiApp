// <copyright file="AppVersionWrapper.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// App version wrapper.
    /// </summary>
    public record AppVersionWrapper
    {
        /// <summary>
        /// Gets or sets app pre-release version.
        /// </summary>
        public Version? SophiApp_pre_release { get; set; }

        /// <summary>
        /// Gets or sets app release version.
        /// </summary>
        public Version? SophiApp_release { get; set; }
    }
}
