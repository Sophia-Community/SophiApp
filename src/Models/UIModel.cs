// <copyright file="UIModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using SophiApp.Helpers;

    /// <summary>
    /// The UI element model.
    /// </summary>
    public abstract class UIModel : INotifyPropertyChanged
    {
        private bool isEnabled = true;
        private bool isSelected = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        protected UIModel(UIModelDto dto, string title)
        {
            Title = title;
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
        /// Gets model title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether model is enabled.
        /// </summary>
        public bool IsEnabled
        {
            get => isEnabled;
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
            get => isSelected;
            set
            {
                isSelected = value;
                OnPropertyChanged();
            }
        }

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
        public virtual bool ContainsText(string text) => Title.Contains(text, StringComparison.CurrentCultureIgnoreCase);

        /// <summary>
        /// <see cref="PropertyChanged"/> event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
            => App.MainWindow.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
    }
}
