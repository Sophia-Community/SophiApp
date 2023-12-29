// <copyright file="UIRadioGroupItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// The <see cref="UIRadioGroupItemModel"/> child item model.
    /// </summary>
    public class UIRadioGroupItemModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UIRadioGroupItemModel"/> class.
        /// </summary>
        /// <param name="title">A model title.</param>
        /// <param name="groupName">A model group name.</param>
        /// <param name="id">A model id.</param>
        public UIRadioGroupItemModel(string title, string groupName, int id)
        {
            Title = title;
            GroupName = groupName;
            Id = id;
        }

        /// <summary>
        /// Gets model title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets model group name.
        /// </summary>
        public string GroupName { get; init; }

        /// <summary>
        /// Gets model id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether model is checked.
        /// </summary>
        public bool IsChecked { get; set; } = false;
    }
}
