// <copyright file="UICheckListModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using SophiApp.Helpers;

    /// <summary>
    /// A list of things that are turned on or off one by one, such as scheduled tasks or network adapters.
    /// The list is known only when the app runs.
    /// </summary>
    public class UICheckListModel : UIModel
    {
        private readonly Func<IReadOnlyList<SettingCheckItem>> accessor;
        private readonly Action<IReadOnlyDictionary<string, bool>> mutator;
        private IReadOnlyList<CheckListItem> items = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="UICheckListModel"/> class.
        /// </summary>
        /// <param name="dto">Data transfer object for <see cref="UIModel"/>.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that returns the items with their state.</param>
        /// <param name="mutator">Method that applies the new state of the changed items, by their ids.</param>
        public UICheckListModel(UIModelDto dto, string title, string description, Func<IReadOnlyList<SettingCheckItem>> accessor, Action<IReadOnlyDictionary<string, bool>> mutator)
            : base(dto, title)
        {
            Description = description;
            this.accessor = accessor;
            this.mutator = mutator;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the items. They are read anew every time the state is read.
        /// </summary>
        public IReadOnlyList<CheckListItem> Items
        {
            get
            {
                return items;
            }

            private set
            {
                items = value;
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override bool IsChanged => Items.Any(item => item.IsChanged);

        /// <inheritdoc/>
        public override string ChosenValue => string.Join(", ", Items.Where(item => item.IsChanged).Select(item => $"{item.Title}: {item.IsChecked}"));

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                Items = accessor.Invoke().Select(item => new CheckListItem(item)).ToArray();
                // The log gets the number of the items that are turned on
                App.Logger.LogModelState(Name, Items.Count(item => item.IsChecked));
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
            var changed = Items.Where(item => item.IsChanged).ToDictionary(item => item.Id, item => item.IsChecked);

            try
            {
                mutator.Invoke(changed);
            }
            catch (CustomizationException ex)
            {
                // The setting stays available: the user can try again when the cause is gone
                SetApplyFailure(ex.GetText(), ex.Action);
                App.Logger.LogModelSetStateException(ex, Name, ChosenValue);
            }
            catch (Exception ex)
            {
                SetApplyFailure(ApplyFailedText);
                App.Logger.LogModelSetStateException(ex, Name, ChosenValue);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text)
            => base.ContainsText(text)
            || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || Items.Any(item => item.Title.Contains(text, StringComparison.CurrentCultureIgnoreCase) || item.Subtitle.Contains(text, StringComparison.CurrentCultureIgnoreCase));
    }
}
