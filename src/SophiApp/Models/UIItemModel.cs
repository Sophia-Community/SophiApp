// <copyright file="UIItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Child element of a composite UI object.
    /// </summary>
    /// <param name="Title">Item title.</param>
    public record UIItemModel(string Title)
    {
    }

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
