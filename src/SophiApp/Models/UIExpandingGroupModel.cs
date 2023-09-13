// <copyright file="UIExpandingGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    internal class UIExpandingGroupModel : UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIExpandingGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIExpandingGroupModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="items">Child items.</param>
        public UIExpandingGroupModel(UIModelDto dto, string title, string description, List<UIItemModel> items)
            : base(dto, title)
        {
            Description = description;
            Items = items;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets child items.
        /// </summary>
        public List<UIItemModel> Items { get; init; }
    }
}
