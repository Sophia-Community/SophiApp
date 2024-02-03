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

        /// <summary>
        /// Initializes a new instance of the <see cref="UIModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        protected UIModel(UIModelDto dto, string title)
        {
            Title = title;
            (Name, Type, Tag, ViewId, Windows10Support, Windows11Support, _) = dto;
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
        /// Gets a value indicating whether model supported windows 10.
        /// </summary>
        public bool Windows10Support { get; private set; }

        /// <summary>
        /// Gets a value indicating whether model supported windows 11.
        /// </summary>
        public bool Windows11Support { get; private set; }

        /// <summary>
        /// Gets model title.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether model enabled state.
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
        /// Gets the model state.
        /// </summary>
        public abstract void GetState();

        /// <summary>
        /// Sets the model state.
        /// </summary>
        public abstract void SetState();

        /// <summary>
        /// <see cref="PropertyChanged"/> event handler.
        /// </summary>
        /// <param name="name">Property name.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            App.MainWindow.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
        }
    }
}
