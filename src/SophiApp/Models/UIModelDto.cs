// <copyright file="UIModelDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Data transfer object for <see cref="UIModel"/>.
    /// </summary>
    public record UIModelDto(
        string Name,
        UIModelType Type,
        UICategoryTag Tag,
        int ViewId,
        bool Windows10Support,
        bool Windows11Support,
        int NumberOfItems);

#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
}
