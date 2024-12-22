// <copyright file="UIRadioGroupItemModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using SophiApp.ViewModels;

    /// <summary>
    /// The <see cref="UIRadioGroupItemModel"/> child item model.
    /// </summary>
    public class UIRadioGroupItemModel : INotifyPropertyChanged
    {
        private bool isChecked = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIRadioGroupItemModel"/> class.
        /// </summary>
        /// <param name="title">A model title.</param>
        /// <param name="groupName">A model group name.</param>
        /// <param name="id">A model id.</param>
        /// <param name="parentId">A parent <see cref="UIExpandingRadioGroupModel"/> model id.</param>
        public UIRadioGroupItemModel(string title, string groupName, int id, int parentId)
        {
            Title = title;
            GroupName = groupName;
            Id = id;
            ParentId = parentId;
        }

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets model title.
        /// </summary>
        public string Title { get; init; }

        /// <summary>
        /// Gets model group name.
        /// </summary>
        public string GroupName { get; init; }

        /// <summary>
        /// Gets model id.
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// Gets parent <see cref="UIExpandingRadioGroupModel"/> model id.
        /// </summary>
        public int ParentId { get; init; }

        /// <summary>
        /// Gets or sets a value indicating whether model is checked.
        /// </summary>
        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(nameof(IsChecked));
            }
        }

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
