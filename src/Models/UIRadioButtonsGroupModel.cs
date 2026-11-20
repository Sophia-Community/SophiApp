// <copyright file="UIRadioButtonsGroupModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// A choice of one option out of several. One of the options is always selected.
    /// </summary>
    public class UIRadioButtonsGroupModel : UIModel
    {
        private readonly Func<int> accessor;
        private readonly Action<int> mutator;
        private string selectedId = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIRadioButtonsGroupModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="options">Titles of the options.</param>
        /// <param name="accessor">Method that returns the number of the selected option, starting from 1.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UIRadioButtonsGroupModel(UIModelDto dto, string title, string description, IReadOnlyList<string> options, Func<int> accessor, Action<int> mutator)
            : base(dto, title)
        {
            Description = description;
            Options = options;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets titles of the options.
        /// </summary>
        public IReadOnlyList<string> Options { get; }

        /// <summary>
        /// Gets <see cref="accessor"/> returned value.
        /// </summary>
        public int DefaultId { get; private set; }

        /// <summary>
        /// Gets or sets selected item id.
        /// </summary>
        public string SelectedId
        {
            get
            {
                return selectedId;
            }

            set
            {
                if (selectedId != value)
                {
                    selectedId = value;
                    OnPropertyChanged(nameof(SelectedId));
                }
            }
        }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                DefaultId = accessor.Invoke();
                SelectedId = DefaultId.ToString();
                App.Logger.LogModelState(Name, DefaultId);
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
            var id = int.Parse(SelectedId);

            try
            {
                mutator.Invoke(id);
            }
            catch (CustomizationException ex)
            {
                // The setting stays available: the user can try again when the cause is gone
                SetApplyFailure(ex.GetText());
                App.Logger.LogModelSetStateException(ex, Name, id);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                SetApplyFailure(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, id);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text)
            || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || Options.Any(option => option.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
