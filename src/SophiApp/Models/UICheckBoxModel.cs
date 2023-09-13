// <copyright file="UICheckBoxModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    public class UICheckBoxModel : UIModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UICheckBoxModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UICheckBoxModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        public UICheckBoxModel(UIModelDto dto, string title)
            : base(dto, title)
        {
        }
    }
}
