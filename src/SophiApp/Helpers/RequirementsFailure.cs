// <copyright file="RequirementsFailure.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
#pragma warning disable CS1591 // There is no XML comment for an open visible type or member.

    /// <summary>
    /// Reasons for failure requirements.
    /// </summary>
    public enum RequirementsFailure
    {
        Is32BitOs,
        WMIBroken,
        Win11BuildLess22k,
        Win11BuildEqual22k,
        Win11UBRLess2283,
        Win10UBRLess3448,
        Win10WrongBuild,
        Win10LTSC2k19,
        Win10LTSC2k21,
        Win10BuildEquals19044,
        UserIsNotAdmin,
    }

#pragma warning restore CS1591 // There is no XML comment for an open visible type or member.
}
