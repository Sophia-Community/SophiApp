﻿// <copyright file="UIExpandingRadioGroupModel.cs" company="Team Sophia">
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
        /// Gets value returned by the <see cref="accessor"/>.
        /// </summary>
        public int AccessorId { get; private set; }

        /// <summary>
        /// Gets or sets <see cref="UIExpandingRadioGroupModel"/> child items.
        /// </summary>
        public List<UIRadioGroupItemModel> Items { get; set; } = [];

        /// <summary>
        /// Gets or sets selected item id.
        /// </summary>
        public int SelectedId { get; set; }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                AccessorId = SelectedId = accessor.Invoke();
                App.Logger.LogModelState(Name, AccessorId);
                _ = Items.First(item => item.Id == AccessorId).IsChecked = true;
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
            try
            {
                mutator.Invoke(SelectedId);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelSetStateException(ex, Name, SelectedId);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text) || Description.Contains(text, StringComparison.CurrentCultureIgnoreCase) || Items.Exists(i => i.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
