// <copyright file="UIModelDto.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// Data transfer object for UI model.
    /// </summary>
    public class UIModelDto
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIModelDto"/> class.
        /// </summary>
        /// <param name="name">Model name.</param>
        /// <param name="type">Model type.</param>
        /// <param name="tag">Model tag.</param>
        /// <param name="viewId">Model view id.</param>
        /// <param name="windows11LTSC">Model supported Windows 11 LTSC.</param>
        /// <param name="windows11">Model supported Windows 11.</param>
        /// <param name="iconIndex">Icon index for <see cref="UIIconCheckBoxModel"/>.</param>
        public UIModelDto(string name, UIModelType type, UICategoryTag tag, int viewId, bool windows11LTSC, bool windows11, int iconIndex = 0)
        {
            Name = name;
            Type = type;
            Tag = tag;
            IconIndex = iconIndex;
            ViewId = viewId;
            Windows11LTSC = windows11LTSC;
            Windows11 = windows11;
        }

        /// <summary>
        /// Gets a model name.
        /// </summary>
        public string Name { get; init; }

        /// <summary>
        /// Gets a model type.
        /// </summary>
        public UIModelType Type { get; init; }

        /// <summary>
        /// Gets category tag.
        /// </summary>
        public UICategoryTag Tag { get; init; }

        /// <summary>
        /// Gets icon index for <see cref="UIIconCheckBoxModel"/>.
        /// </summary>
        public int IconIndex { get; init; }

        /// <summary>
        /// Gets a model view id.
        /// </summary>
        public int ViewId { get; init; }

        /// <summary>
        /// Gets a value indicating whether Windows 11 LTSC support.
        /// </summary>
        public bool Windows11LTSC { get; init; }

        /// <summary>
        /// Gets a value indicating whether Windows 11 support.
        /// </summary>
        public bool Windows11 { get; init; }
    }
}
