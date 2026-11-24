// <copyright file="UIIconRadioButtonsModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// A choice of any number of options with icons. None of the options can be selected.
    /// The reset button returns to the Windows default; it can be pressed in any state, because Windows may be set to something that is not among the options.
    /// </summary>
    public class UIIconRadioButtonsModel : UIModel
    {
        /// <summary>
        /// The id of the state in which no option is selected.
        /// </summary>
        public const int NoOptionId = 0;

        private readonly Func<int> accessor;
        private readonly Action<int> mutator;
        private string selectedId = NoOptionId.ToString();
        private bool resetRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIIconRadioButtonsModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="options">Titles of the options.</param>
        /// <param name="links">Addresses of the web pages about the options. An option without a page has an empty address.</param>
        /// <param name="accessor">Method that returns the number of the selected option, starting from 1, or 0.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UIIconRadioButtonsModel(UIModelDto dto, string title, string description, IReadOnlyList<string> options, IReadOnlyList<string> links, Func<int> accessor, Action<int> mutator)
            : base(dto, title)
        {
            Description = description;
            Options = options;
            Links = links;
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
        /// Gets addresses of the web pages about the options, in the order of the options.
        /// </summary>
        public IReadOnlyList<string> Links { get; }

        /// <summary>
        /// Gets the method that tells what Windows is set to when it is none of the options, or null.
        /// </summary>
        public Func<ThirdPartyState>? ThirdPartyAccessor { get; init; }

        /// <summary>
        /// Gets the title of the list item that stands for a state that is none of the options.
        /// </summary>
        public string ThirdPartyTitle { get; init; } = string.Empty;

        /// <summary>
        /// Gets what Windows is set to when it is none of the options. Null when it is one of them or nothing is set.
        /// The list shows it as one more item, so a choice made instead of it can be taken back.
        /// </summary>
        public ThirdPartyState? ThirdParty { get; private set; }

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

        /// <summary>
        /// Gets or sets a value indicating whether the user chose to return to the Windows default.
        /// It is a choice like an option: it is applied together with the other chosen settings.
        /// </summary>
        public bool ResetRequested
        {
            get
            {
                return resetRequested;
            }

            set
            {
                if (resetRequested != value)
                {
                    resetRequested = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the user chose something that differs from the current state of Windows.
        /// </summary>
        public override bool IsChanged => ResetRequested || SelectedId != DefaultId.ToString();

        /// <inheritdoc/>
        public override string ChosenValue => SelectedId;

        /// <summary>
        /// Gets the address of the icon of an option. The icons are the files "Name_1.svg", "Name_2.svg" and so on in the Assets folder.
        /// </summary>
        /// <param name="number">The number of the option, starting from 1.</param>
        public Uri GetOptionIcon(int number) => new($"ms-appx:///Assets/{Name}_{number}.svg");

        /// <inheritdoc/>
        public override void GetState()
        {
            // The choice is either applied or canceled by now
            ResetRequested = false;

            try
            {
                DefaultId = accessor.Invoke();

                // The list gets the item before the item is selected
                ThirdParty = DefaultId == ThirdPartyState.Id ? ThirdPartyAccessor?.Invoke() ?? new(string.Empty, string.Empty) : null;
                OnPropertyChanged(nameof(ThirdParty));
                SelectedId = DefaultId.ToString();
                App.Logger.LogModelState(Name, DefaultId);
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
            var id = int.Parse(SelectedId);

            // What is set by another app is only shown, it cannot be applied
            if (id == ThirdPartyState.Id)
            {
                return;
            }

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
