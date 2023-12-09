// <copyright file="UICheckBoxGroupItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Child element of a <see cref="UIExpandingCheckBoxGroupModel"/>.
    /// </summary>
    /// <param name="Title">Item title.</param>
    public record UICheckBoxGroupItemModel(string Title)
    {
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
