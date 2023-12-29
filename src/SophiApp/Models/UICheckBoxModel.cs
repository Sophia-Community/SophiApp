// <copyright file="UICheckBoxModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UICheckBoxModel : UIModel
    {
        private readonly Func<bool> accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UICheckBoxModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UICheckBoxModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that sets the IsEnabled state.</param>
        public UICheckBoxModel(UIModelDto dto, string title, string description, Func<bool> accessor)
            : base(dto, title)
        {
            Description = description;
            this.accessor = accessor;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets a value indicating whether model checked state.
        /// </summary>
        public bool IsChecked { get; private set; }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                IsChecked = accessor.Invoke();
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }
    }
}
