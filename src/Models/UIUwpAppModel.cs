// <copyright file="UIUwpAppModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UIUwpAppModel : UIModel
    {
        private bool isChecked = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIUwpAppModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UICheckBoxModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="image">The path to model logo.</param>
        public UIUwpAppModel(UIModelDto dto, string title, Uri image)
            : base(dto, title)
        {
            Image = image;
        }

        /// <summary>
        /// Gets path to model logo.
        /// </summary>
        public Uri Image { get; init; }

        /// <summary>
        /// Gets or sets the method to be executed when the <see cref="UIUwpAppModel"/> set state.
        /// </summary>
        public Action<string, bool>? Mutator { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the appx package will be removed for all users.
        /// </summary>
        public bool ForAllUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether model checked state.
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                App.Logger.LogModelState(Name, IsChecked);
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void GetState()
        {
            IsChecked = false;
        }

        /// <inheritdoc/>
        public override void SetState()
        {
            try
            {
                Mutator?.Invoke(Title, ForAllUsers);
            }
            catch (Exception ex)
            {
                App.Logger.LogModelSetStateException(ex, Name, IsChecked);
            }
            finally
            {
                IsEnabled = false;
            }
        }
    }
}
