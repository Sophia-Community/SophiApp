// <copyright file="UIExpandingRadioGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UIExpandingRadioGroupModel : UIModel
    {
        private readonly Func<int> accessor;
        private readonly Action<int> mutator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIExpandingRadioGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that sets the model state.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UIExpandingRadioGroupModel(
            UIModelDto dto,
            string title,
            string description,
            Func<int> accessor,
            Action<int> mutator)
            : base(dto, title)
        {
            Description = description;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets or sets <see cref="UIExpandingRadioGroupModel"/> child items.
        /// </summary>
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
        public List<UIRadioGroupItemModel> Items { get; set; } = [];
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                var selectedId = accessor.Invoke();
                App.Logger.LogModelState(Name, selectedId);
                _ = Items.First(item => item.Id == selectedId).IsChecked = true;
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }

        /// <inheritdoc/>
        public override void SetState()
        {
            var checkedItem = Items.First(item => item.IsChecked);

            try
            {
                mutator.Invoke(checkedItem.Id);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelSetStateException(ex, Name, checkedItem.Id);
            }
        }
    }
}
