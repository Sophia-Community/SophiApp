// <copyright file="UIRadioButtonsGroup3Model.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    /// <inheritdoc/>
    public class UIRadioButtonsGroup3Model : UIModel
    {
        private readonly Func<int> accessor;
        private readonly Action<int> mutator;
        private string selectedId = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIRadioButtonsGroup3Model"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="title_1">First radio button title.</param>
        /// <param name="title_2">Second radio button title.</param>
        /// <param name="title_3">Third radio button title.</param>
        /// <param name="accessor">Method that sets the model state.</param>
        /// <param name="mutator">Method that changes Windows settings.</param>
        public UIRadioButtonsGroup3Model(
            UIModelDto dto,
            string title,
            string description,
            string title_1,
            string title_2,
            string title_3,
            Func<int> accessor,
            Action<int> mutator)
            : base(dto, title)
        {
            Description = description;
            Title_1 = title_1;
            Title_2 = title_2;
            Title_3 = title_3;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; init; }

        /// <summary>
        /// Gets <see cref="accessor"/> returned value.
        /// </summary>
        public int DefaultId { get; private set; }

        /// <summary>
        /// Gets or sets selected item id.
        /// </summary>
        public string SelectedId
        {
            get => selectedId;
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
        /// Gets first radio button title.
        /// </summary>
        public string Title_1 { get; init; }

        /// <summary>
        /// Gets second radio button title.
        /// </summary>
        public string Title_2 { get; init; }

        /// <summary>
        /// Gets third radio button title.
        /// </summary>
        public string Title_3 { get; init; }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                DefaultId = accessor.Invoke();
                SelectedId = DefaultId.ToString();
                App.Logger.LogModelState(Name, DefaultId);
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
            catch (Exception ex)
            {
                IsEnabled = false;
                App.Logger.LogModelSetStateException(ex, Name, id);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text) || Description.Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }
}
