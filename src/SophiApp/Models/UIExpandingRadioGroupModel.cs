// <copyright file="UIExpandingRadioGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UIExpandingRadioGroupModel : UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIExpandingRadioGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title"><see cref="UIExpandingRadioGroupModel"/> title.</param>
        /// <param name="items"><see cref="UIExpandingRadioGroupModel"/> items.</param>
        public UIExpandingRadioGroupModel(UIModelDto dto, string title, List<UIRadioGroupItemModel> items)
            : base(dto, title)
        {
            Items = items;
        }

        /// <summary>
        /// Gets <see cref="UIExpandingRadioGroupModel"/> items.
        /// </summary>
        public List<UIRadioGroupItemModel> Items { get; init; }
    }
}
