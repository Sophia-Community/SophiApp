// <copyright file="UIActionModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// An action that has no state to read: it is either chosen with its button and done when the chosen settings are applied, or not.
    /// </summary>
    public class UIActionModel : UIModel
    {
        private readonly Func<bool> accessor;
        private readonly Action<bool> mutator;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIActionModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIActionModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="buttonText">The caption of the button that chooses the action.</param>
        /// <param name="accessor">Method that fails with the reason if the action is unavailable. Its result is not used.</param>
        /// <param name="mutator">Method that does the action if it gets true.</param>
        public UIActionModel(UIModelDto dto, string title, string description, string buttonText, Func<bool> accessor, Action<bool> mutator)
            : base(dto, title)
        {
            Description = description;
            ButtonText = buttonText;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the caption of the button that chooses the action.
        /// </summary>
        public string ButtonText { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the action is chosen to be done.
        /// </summary>
        public bool IsRequested { get; set; }

        /// <inheritdoc/>
        public override bool IsChanged => IsRequested;

        /// <inheritdoc/>
        public override string ChosenValue => ButtonText;

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                // The choice is either applied or canceled by now
                IsRequested = false;

                // There is no state, only the reason why the action is unavailable
                _ = accessor.Invoke();
                IsEnabled = true;
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
                mutator.Invoke(true);
            }
            catch (CustomizationException ex)
            {
                // The action stays available: the user can try again when the cause is gone
                SetApplyFailure(ex.GetText());
                App.Logger.LogModelSetStateException(ex, Name, true);
            }
            catch (Exception ex)
            {
                SetApplyFailure(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, true);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text)
            || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || ButtonText.Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }
}
