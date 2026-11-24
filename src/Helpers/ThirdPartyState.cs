// <copyright file="ThirdPartyState.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    /// <summary>
    /// What Windows is set to when it is none of the options of a setting: something set by another app or by hand.
    /// </summary>
    /// <param name="Details">What exactly is set: a name or addresses. It is the second line of the list item.</param>
    /// <param name="IconFile">The path of a cursor file to show as the icon, or an empty string.</param>
    public sealed record ThirdPartyState(string Details, string IconFile)
    {
        /// <summary>
        /// The number an accessor returns for such a state.
        /// </summary>
        public const int Id = -1;
    }
}
