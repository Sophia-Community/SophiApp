// <copyright file="SystemAnimations.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using Windows.UI.ViewManagement;

    /// <summary>
    /// The "Animation effects" setting of Windows.
    /// </summary>
    public static class SystemAnimations
    {
        /// <summary>
        /// Gets a value indicating whether the user lets the apps play animations.
        /// </summary>
        public static bool AreEnabled => new UISettings().AnimationsEnabled;
    }
}
