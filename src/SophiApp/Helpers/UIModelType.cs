// <copyright file="UIModelType.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using SophiApp.Models;

#pragma warning disable SA1602 // An item within a C# enumeration is missing an Xml documentation header.
#pragma warning disable CS1591 // There is no XML comment for an open visible type or member.

    /// <summary>
    /// <see cref="UIModelDto"/> type.
    /// </summary>
    public enum UIModelType
    {
        CheckBox,
        ExpandingCheckBoxGroup,
        ExpandingGroup,
        ExpandingRadioGroup,
        RadioGroup,
    }

#pragma warning restore SA1602 // An item within a C# enumeration is missing an Xml documentation header.
#pragma warning restore CS1591 // There is no XML comment for an open visible type or member.
}
