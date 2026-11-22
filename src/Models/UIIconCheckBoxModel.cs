// <copyright file="UIIconCheckBoxModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <inheritdoc/>
    public class UIIconCheckBoxModel : UIModel
    {
        private readonly Func<bool> accessor;
        private readonly Action<bool> mutator;
        private bool isChecked;

        // The state read from Windows
        private bool readState;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIIconCheckBoxModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIIconCheckBoxModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that sets the IsEnabled state.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UIIconCheckBoxModel(UIModelDto dto, string title, string description, Func<bool> accessor, Action<bool> mutator)
            : base(dto, title)
        {
            Description = description;
            IconIndex = dto.IconIndex;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets icon index.
        /// </summary>
        public int IconIndex { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether model checked state.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                isChecked = value;
                App.Logger.LogModelState(Name, IsChecked);
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override bool IsChanged => IsChecked != readState;

        /// <inheritdoc/>
        public override string ChosenValue => IsChecked.ToString();

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                readState = accessor.Invoke();
                IsChecked = readState;
                ShowNotice(string.Empty);
            }
            catch (CustomizationException ex)
            {
                IsEnabled = false;
                ShowBlockingReason(ex.GetText());
                App.Logger.LogModelGetStateException(Name, ex);
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
            catch (CustomizationException ex)
            {
                // The setting stays available: the user can try again when the cause is gone
                SetApplyFailure(ex.GetText());
                App.Logger.LogModelSetStateException(ex, Name, IsChecked);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                SetApplyFailure(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, IsChecked);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text) => base.ContainsText(text) || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }
}
