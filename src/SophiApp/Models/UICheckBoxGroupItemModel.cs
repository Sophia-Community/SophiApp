// <copyright file="UICheckBoxGroupItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <summary>
    /// Child element of a <see cref="UIExpandingCheckBoxGroupModel"/>.
    /// </summary>
    public class UICheckBoxGroupItemModel
    {
        private readonly Func<bool> accessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="UICheckBoxGroupItemModel"/> class.
        /// </summary>
        /// <param name="accessor">Method that sets the IsEnabled state.</param>
        /// <param name="title">Model title.</param>
        public UICheckBoxGroupItemModel(Func<bool> accessor, string title)
        {
            this.accessor = accessor;
            Title = title;
        }

        /// <summary>
        /// Gets model title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets a value indicating whether model is checked.
        /// </summary>
        public bool IsChecked { get; private set; } = false;

        /// <summary>
        /// Gets a value indicating whether model enabled state.
        /// </summary>
        public bool IsEnabled { get; private set; } = true;

        /// <summary>
        /// Get the model state.
        /// </summary>
        public void GetState()
        {
            try
            {
                IsChecked = accessor.Invoke();
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelGetStateException(Title, ex);
            }
        }
    }
}
