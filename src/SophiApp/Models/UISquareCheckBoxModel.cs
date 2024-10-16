// <copyright file="UISquareCheckBoxModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UISquareCheckBoxModel : UIModel
    {
        private readonly Func<bool> accessor;
        private readonly Action<bool> mutator;
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="UISquareCheckBoxModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UICheckBoxModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="accessor">Method that sets the IsEnabled state.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UISquareCheckBoxModel(UIModelDto dto, string title, Func<bool> accessor, Action<bool> mutator)
            : base(dto, title)
        {
            this.accessor = accessor;
            this.mutator = mutator;
        }

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

        /// <inheritdoc/>
        public override void SetState()
        {
            try
            {
                mutator.Invoke(IsChecked);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelSetStateException(ex, Name, IsChecked);
            }
        }
    }
}
