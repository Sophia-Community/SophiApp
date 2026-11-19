// <copyright file="UIModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// The UI element model.
    /// </summary>
    public abstract class UIModel : INotifyPropertyChanged
    {
        private bool isEnabled = true;
        private bool isSelected = false;
        private string title;
        private string applyFailure = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        protected UIModel(UIModelDto dto, string title)
        {
            this.title = title;
            Name = dto.Name;
            Type = dto.Type;
            Tag = dto.Tag;
            ViewId = dto.ViewId;
            Windows11LTSC = dto.Windows11LTSC;
            Windows11 = dto.Windows11;
        }

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets model unique name.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets model type.
        /// </summary>
        public UIModelType Type { get; private set; }

        /// <summary>
        /// Gets model category tag.
        /// </summary>
        public UICategoryTag Tag { get; private set; }

        /// <summary>
        /// Gets a value that determines the order in which the model is displayed in the View.
        /// </summary>
        public int ViewId { get; private set; }

        /// <summary>
        /// Gets a value indicating whether model supported Windows 11 LTSC.
        /// </summary>
        public bool Windows11LTSC { get; private set; }

        /// <summary>
        /// Gets a value indicating whether model supported Windows 11.
        /// </summary>
        public bool Windows11 { get; private set; }

        /// <summary>
        /// Gets or sets model title.
        /// </summary>
        public string Title
        {
            get
            {
                return title;
            }

            protected set
            {
                if (title != value)
                {
                    title = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether model is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            protected set
            {
                isEnabled = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether model is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }

            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets the localized text shown under the setting: why it is unavailable, why it could not be applied, or a notice.
        /// </summary>
        public string ExceptionText { get; private set; } = string.Empty;

        /// <summary>
        /// Gets a value indicating whether <see cref="ExceptionText"/> is not empty and can be visible.
        /// </summary>
        public bool ExceptionTextIsVisible { get; private set; }

        /// <summary>
        /// Gets the text for a setting that could not be applied for an unknown reason.
        /// </summary>
        protected static string ApplyFailedText => "UIModel_Exception_ApplyFailed".GetLocalized();

        /// <summary>
        /// Gets the model state.
        /// </summary>
        public abstract void GetState();

        /// <summary>
        /// Sets the model state.
        /// </summary>
        public abstract void SetState();

        /// <summary>
        /// Returns a value indicating whether a specified text occurs within this model.
        /// </summary>
        /// <param name="text">The text to seek.</param>
        public virtual bool ContainsText(string text)
            => Title.Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || ExceptionText.Contains(text, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// Shows why the setting is unavailable.
        /// </summary>
        /// <param name="reason">A localized text.</param>
        protected void ShowBlockingReason(string reason)
        {
            applyFailure = string.Empty;
            SetExceptionText(reason);
        }

        /// <summary>
        /// Shows a text under a setting whose state has just been read: the reason of a failed apply if there is one, a notice otherwise.
        /// </summary>
        /// <param name="notice">A localized notice, or an empty string.</param>
        protected void ShowNotice(string notice)
        {
            var text = applyFailure.Length > 0 ? applyFailure : notice;
            applyFailure = string.Empty;
            SetExceptionText(text);
        }

        /// <summary>
        /// Remembers why the setting could not be applied. The text is shown when the state is read again.
        /// </summary>
        /// <param name="reason">A localized text.</param>
        protected void SetApplyFailure(string reason) => applyFailure = reason;

        /// <summary>
        /// <see cref="PropertyChanged"/> event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => App.MainWindow.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));

        private void SetExceptionText(string text)
        {
            // The log gets the same text as the user sees under the setting
            if (text.Length > 0 && text != ExceptionText)
            {
                App.Logger.LogModelNotice(Name, text);
            }

            ExceptionText = text;
            ExceptionTextIsVisible = text.Length > 0;
            OnPropertyChanged(nameof(ExceptionText));
            OnPropertyChanged(nameof(ExceptionTextIsVisible));
        }
    }
}
