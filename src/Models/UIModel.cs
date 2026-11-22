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
        // Other names some functions are known by. They are found by the search too
        private static readonly Dictionary<string, string> OtherNames = new()
        {
            ["Cursors"] = "InstallCursors",
            ["InstallVisualC_x86"] = "InstallVCRedist",
            ["InstallVisualC_x64"] = "InstallVCRedist",
            ["InstallDotNetRuntime_10"] = "InstallDotNetRuntimes",
            ["InstallDotNetRuntime_11"] = "InstallDotNetRuntimes",
            ["UserShellFolderLocation"] = "SetUserShellFolderLocation WinPrtScrFolder",
        };

        private bool isEnabled = true;
        private bool isSelected = false;
        private string title;
        private string applyFailure = string.Empty;

        // What the setting showed before another setting locked it
        private object? choiceBeforeLock;
        private string textBeforeLock = string.Empty;

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
            Group = dto.Group;
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
        /// Gets the group of settings the model belongs to on its page, or an empty string.
        /// </summary>
        public string Group { get; private set; }

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
                return isEnabled && !IsLocked;
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
        /// Gets a value indicating whether the choice is made by another chosen setting, so the user cannot change it here.
        /// </summary>
        public bool IsLocked { get; private set; }

        /// <summary>
        /// Gets the name of the setting that locked this one, or an empty string.
        /// </summary>
        public string LockedBy { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets what is chosen: true or false for a check box, the number of the option for a list.
        /// Null for the settings whose choice another setting cannot decide.
        /// </summary>
        public virtual object? Choice
        {
            get => null;
            set => _ = value;
        }

        /// <summary>
        /// Gets a value indicating whether the user chose something that differs from the state of Windows.
        /// A setting with such a choice is applied with the others; a choice that is taken back makes it false again.
        /// </summary>
        public abstract bool IsChanged { get; }

        /// <summary>
        /// Gets what the user chose, as a text for the log.
        /// </summary>
        public abstract string ChosenValue { get; }

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
        /// Makes the choice for the user and keeps it until the lock is taken off.
        /// </summary>
        /// <param name="value">The value to choose.</param>
        /// <param name="owner">The name of the setting that decides.</param>
        /// <param name="reason">A localized text that says why the setting cannot be changed.</param>
        public void LockChoice(object value, string owner, string reason)
        {
            if (!IsLocked)
            {
                choiceBeforeLock = Choice;
                textBeforeLock = ExceptionText;
            }

            // The value goes first: a check box that changes while it is disabled can keep the old check mark
            Choice = value;
            IsLocked = true;
            LockedBy = owner;
            OnPropertyChanged(nameof(IsLocked));
            OnPropertyChanged(nameof(IsEnabled));
            SetExceptionText(reason);
        }

        /// <summary>
        /// Takes the lock off.
        /// </summary>
        /// <param name="restoreChoice">True to bring back what was chosen before the lock.</param>
        public void UnlockChoice(bool restoreChoice)
        {
            if (!IsLocked)
            {
                return;
            }

            IsLocked = false;
            LockedBy = string.Empty;
            OnPropertyChanged(nameof(IsLocked));
            OnPropertyChanged(nameof(IsEnabled));

            if (restoreChoice && choiceBeforeLock is not null)
            {
                Choice = choiceBeforeLock;
            }

            SetExceptionText(textBeforeLock);
        }

        /// <summary>
        /// Returns a value indicating whether a specified text occurs within this model.
        /// </summary>
        /// <param name="text">The text to seek.</param>
        public virtual bool ContainsText(string text)
            => Title.Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || ExceptionText.Contains(text, StringComparison.CurrentCultureIgnoreCase)
            || NameContains(text);

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

        // The name of the function is compared without hyphens and underscores, so "Install-WSL" finds "InstallWSL"
        private bool NameContains(string text)
        {
            var wanted = text.Replace("-", string.Empty).Replace("_", string.Empty).Trim();

            if (wanted.Length == 0)
            {
                return false;
            }

            return Name.Replace("_", string.Empty).Contains(wanted, StringComparison.OrdinalIgnoreCase)
                || (OtherNames.TryGetValue(Name, out var otherName) && otherName.Contains(wanted, StringComparison.OrdinalIgnoreCase));
        }

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
