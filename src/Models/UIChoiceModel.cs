// <copyright file="UIChoiceModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// A choice of one item from a list that is known only when the app runs. If the list tells which item is set in Windows,
    /// that item is selected; otherwise nothing is chosen until the user chooses, and the first item of the list stands for that.
    /// </summary>
    public class UIChoiceModel : UIModel
    {
        /// <summary>
        /// The id of the item that stands for "nothing is chosen".
        /// </summary>
        public const string NoChoiceId = "";

        private readonly Func<IReadOnlyList<SettingChoice>> accessor;
        private readonly Action<string> mutator;
        private readonly SettingChoice noChoice;
        private IReadOnlyList<SettingChoice> choices;
        private string selectedId = NoChoiceId;

        // The id of the item that is set in Windows, if the list has such an item
        private string currentId = NoChoiceId;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIChoiceModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="noChoiceTitle">The title of the item that stands for "nothing is chosen".</param>
        /// <param name="accessor">Method that returns the items to choose from.</param>
        /// <param name="mutator">Method that applies the id of the chosen item.</param>
        public UIChoiceModel(UIModelDto dto, string title, string description, string noChoiceTitle, Func<IReadOnlyList<SettingChoice>> accessor, Action<string> mutator)
            : base(dto, title)
        {
            Description = description;
            IconIndex = dto.IconIndex;
            noChoice = new SettingChoice(NoChoiceId, noChoiceTitle, string.Empty, true);
            choices = new[] { noChoice };
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the index of the card icon.
        /// </summary>
        public int IconIndex { get; }

        /// <summary>
        /// Gets the items to choose from. The first one stands for "nothing is chosen".
        /// </summary>
        public IReadOnlyList<SettingChoice> Choices
        {
            get
            {
                return choices;
            }

            private set
            {
                choices = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the id of the chosen item.
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
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user chose an item to apply.
        /// </summary>
        public override bool IsChanged => SelectedId != currentId;

        /// <inheritdoc/>
        public override string ChosenValue => SelectedId;

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                var items = accessor.Invoke();

                // A list that tells what is set in Windows has no item for "nothing is chosen"
                var current = items.FirstOrDefault(item => item.IsCurrent);
                Choices = current is null ? items.Prepend(noChoice).ToArray() : items.ToArray();

                // The choice is either applied or canceled by now
                currentId = current?.Id ?? NoChoiceId;
                SelectedId = currentId;
                App.Logger.LogModelState(Name, items.Count);
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
            var id = SelectedId;

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
                SetApplyFailure(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, id);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text)
            || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || Choices.Any(choice => choice.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase) || choice.Subtitle.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
