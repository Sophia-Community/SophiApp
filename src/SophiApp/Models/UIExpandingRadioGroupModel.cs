// <copyright file="UIExpandingRadioGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UIExpandingRadioGroupModel : UIModel
    {
        private readonly Func<int> accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIExpandingRadioGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that sets the model state.</param>
        /// <param name="items">Model child items.</param>
        public UIExpandingRadioGroupModel(
            UIModelDto dto,
            string title,
            string description,
            Func<int> accessor,
            List<UIRadioGroupItemModel> items)
            : base(dto, title)
        {
            Description = description;
            this.accessor = accessor;
            Items = items;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets <see cref="UIExpandingRadioGroupModel"/> child items.
        /// </summary>
        public List<UIRadioGroupItemModel> Items { get; init; }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                var selectedId = accessor.Invoke();
                Items.ForEach(item => item.IsChecked = item.Id == selectedId);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }
    }
}
