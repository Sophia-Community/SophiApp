// <copyright file="UIRadioGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UIRadioGroupModel : UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIRadioGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIRadioGroupModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="items">Child items.</param>
        public UIRadioGroupModel(UIModelDto dto, string title, List<UIRadioGroupItemModel> items)
            : base(dto, title)
        {
            Items = items;
        }

        /// <summary>
        /// Gets child items.
        /// </summary>
        public List<UIRadioGroupItemModel> Items { get; init; }
    }
}
