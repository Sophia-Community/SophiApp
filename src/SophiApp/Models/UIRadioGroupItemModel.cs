// <copyright file="UIRadioGroupItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Child element of a composite UI object.
    /// </summary>
    /// <param name="Title">Item title.</param>
    /// <param name="GroupName">Radio button group name.</param>
    public record UIRadioGroupItemModel(string Title, string GroupName)
    {
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
