// <copyright file="UIModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// The UI element model.
    /// </summary>
    public class UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        public UIModel(UIModelDto dto, string title)
        {
            Title = title;
            (Name, Type, Tag, Windows10Support, Windows11Support, _) = dto;
        }

        /// <summary>
        /// Gets model unique name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets model type.
        /// </summary>
        public UIModelType Type { get; init; }

        /// <summary>
        /// Gets model category tag.
        /// </summary>
        public UICategoryTag Tag { get; init; }

        /// <summary>
        /// Gets a value indicating whether model supported windows 10.
        /// </summary>
        public bool Windows10Support { get; init; }

        /// <summary>
        /// Gets a value indicating whether model supported windows 11.
        /// </summary>
        public bool Windows11Support { get; init; }

        /// <summary>
        /// Gets model title.
        /// </summary>
        public string Title { get; init; }
    }
}
