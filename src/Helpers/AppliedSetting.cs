// <copyright file="AppliedSetting.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// A line of the list that is shown after the chosen settings are applied.
    /// </summary>
    /// <param name="Title">The title of the setting.</param>
    /// <param name="Failed">The setting could not be applied.</param>
    /// <param name="Notice">Why it could not be applied, or an empty text.</param>
    public sealed record AppliedSetting(string Title, bool Failed, string Notice)
    {
        /// <summary>
        /// Gets a value indicating whether the setting was applied.
        /// </summary>
        public bool IsApplied => !Failed;

        /// <summary>
        /// Gets a value indicating whether there is a reason to show.
        /// </summary>
        public bool HasNotice => Notice.Length > 0;
    }
}
