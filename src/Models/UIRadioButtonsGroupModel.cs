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
        // The options whose icon is a symbol of the icon font. It takes the color of the text, like the icons of Windows Terminal settings
        private static readonly Dictionary<string, string> OptionGlyphs = new()
        {
            ["DefaultTerminalApp_2"] = "\uE756",
        };

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
        /// Gets a value indicating whether the options have icons.
        /// </summary>
        public bool HasOptionIcons { get; init; }

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
        public override bool IsChanged => SelectedId != DefaultId.ToString();

        /// <inheritdoc/>
        public override string ChosenValue => SelectedId;

        /// <inheritdoc/>
        public override object? Choice
        {
            get => int.TryParse(SelectedId, out var id) ? id : null;
            set => SelectedId = value is int id ? id.ToString() : SelectedId;
        }

        /// <summary>
        /// Gets the address of the icon of an option, if the options have icons. The icons are the files "Name_1.svg", "Name_2.svg" and so on in the Assets folder.
        /// An option that has a symbol instead has no file.
        /// </summary>
        /// <param name="number">The number of the option, starting from 1.</param>
        public Uri? GetOptionIcon(int number)
            => HasOptionIcons && GetOptionGlyph(number).Length == 0 ? new($"ms-appx:///Assets/{Name}_{number}.svg") : null;

        /// <summary>
        /// Gets the symbol of the icon font that is the icon of an option, or an empty string.
        /// </summary>
        /// <param name="number">The number of the option, starting from 1.</param>
        public string GetOptionGlyph(int number)
            => HasOptionIcons && OptionGlyphs.TryGetValue($"{Name}_{number}", out var glyph) ? glyph : string.Empty;

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
