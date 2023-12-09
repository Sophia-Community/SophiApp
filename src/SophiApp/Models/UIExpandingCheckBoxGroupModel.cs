// <copyright file="UIExpandingCheckBoxGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UIExpandingCheckBoxGroupModel : UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIExpandingCheckBoxGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title"><see cref="UIExpandingCheckBoxGroupModel"/> title.</param>
        /// <param name="description"><see cref="UIExpandingCheckBoxGroupModel"/> description.</param>
        /// <param name="items"><see cref="UIExpandingCheckBoxGroupModel"/> child items.</param>
        public UIExpandingCheckBoxGroupModel(UIModelDto dto, string title, string description, List<UICheckBoxGroupItemModel> items)
            : base(dto, title)
        {
            Items = items;
            Description = description;
        }

        /// <summary>
        /// Gets <see cref="UIExpandingCheckBoxGroupModel"/> child items.
        /// </summary>
        public List<UICheckBoxGroupItemModel> Items { get; init; }

        /// <summary>
        /// Gets <see cref="UIExpandingCheckBoxGroupModel"/> description.
        /// </summary>
        public string Description { get; init; }
    }
}
