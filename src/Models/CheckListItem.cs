// <copyright file="CheckListItem.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using Microsoft.UI.Xaml;
    using SophiApp.Helpers;

    /// <summary>
    /// An item of <see cref="UICheckListModel"/>: its state in Windows and the state the user chose for it.
    /// </summary>
    public sealed class CheckListItem : INotifyPropertyChanged
    {
        private readonly bool readState;
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="CheckListItem"/> class.
        /// </summary>
        /// <param name="item">The item as it is read from Windows.</param>
        public CheckListItem(SettingCheckItem item)
        {
            Id = item.Id;
            Title = item.Title;
            Subtitle = item.Subtitle;
            readState = item.IsChecked;
            isChecked = item.IsChecked;
        }

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the id the item is changed by.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the item.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the second line under the name, or an empty string.
        /// </summary>
        public string Subtitle { get; }

        /// <summary>
        /// Gets the visibility of the second line.
        /// </summary>
        public Visibility SubtitleVisibility => Subtitle.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets or sets a value indicating whether the item is chosen to be turned on.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return isChecked;
            }

            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(IsChecked)));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the chosen state differs from the one in Windows.
        /// </summary>
        public bool IsChanged => IsChecked != readState;
    }
}
