// <copyright file="UserFolderItem.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using Microsoft.UI.Xaml;
    using SophiApp.Helpers;

    /// <summary>
    /// A user folder with its location in Windows and the location chosen for it.
    /// </summary>
    public class UserFolderItem : INotifyPropertyChanged
    {
        private string path = string.Empty;
        private bool isEnabled = true;
        private bool isTyped;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserFolderItem"/> class.
        /// </summary>
        /// <param name="folder">The user folder.</param>
        /// <param name="title">The name of the folder to show.</param>
        /// <param name="glyph">The icon of the folder: a symbol of the icon font.</param>
        public UserFolderItem(UserFolder folder, string title, string glyph)
        {
            Folder = folder;
            Title = title;
            Glyph = glyph;
        }

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the user folder.
        /// </summary>
        public UserFolder Folder { get; }

        /// <summary>
        /// Gets the name of the folder to show.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the icon of the folder: a symbol of the icon font.
        /// </summary>
        public string Glyph { get; }

        /// <summary>
        /// Gets the location of the folder in Windows.
        /// </summary>
        public string CurrentPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets the location the folder has in a new Windows user profile.
        /// </summary>
        public string DefaultPath { get; private set; } = string.Empty;

        /// <summary>
        /// Gets or sets the location to show: the chosen one, or the one in Windows if nothing is chosen.
        /// </summary>
        public string Path
        {
            get
            {
                return path;
            }

            set
            {
                path = value;
                OnPropertyChanged(nameof(Path));
                OnPropertyChanged(nameof(CanBeReset));
                OnPropertyChanged(nameof(DefaultIsChosen));
                OnPropertyChanged(nameof(DefaultIsAvailable));
                OnPropertyChanged(nameof(MenuMarkVisibility));
                OnPropertyChanged(nameof(TypedMarkVisibility));
            }
        }

        /// <summary>
        /// Gets the visibility of the mark of the "Change" button: a new location is chosen with it.
        /// </summary>
        public Visibility MenuMarkVisibility => IsChanged && !DefaultIsChosen && !isTyped ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets the visibility of the mark of the location box: a new location is typed into it.
        /// </summary>
        public Visibility TypedMarkVisibility => IsChanged && isTyped ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets or sets a value indicating whether a new location can be chosen for the folder.
        /// </summary>
        public bool IsEnabled
        {
            get
            {
                return isEnabled;
            }

            set
            {
                isEnabled = value;
                OnPropertyChanged(nameof(IsEnabled));
                OnPropertyChanged(nameof(DefaultIsAvailable));
            }
        }

        /// <summary>
        /// Gets a value indicating whether a new location is chosen for the folder.
        /// </summary>
        public bool IsChanged => !UserFolderHelper.IsSamePath(Path, CurrentPath);

        /// <summary>
        /// Gets a value indicating whether the shown location differs from the default one.
        /// </summary>
        public bool CanBeReset => !UserFolderHelper.IsSamePath(Path, DefaultPath);

        /// <summary>
        /// Gets a value indicating whether the default location is chosen instead of the one in Windows.
        /// </summary>
        public bool DefaultIsChosen => IsChanged && !CanBeReset;

        /// <summary>
        /// Gets a value indicating whether the default location can be chosen, or the choice of it can be taken back.
        /// </summary>
        public bool DefaultIsAvailable => IsEnabled && (CanBeReset || DefaultIsChosen);

        /// <summary>
        /// Takes the location read from Windows. The choice is dropped.
        /// </summary>
        /// <param name="currentPath">The location of the folder in Windows.</param>
        /// <param name="defaultPath">The location the folder has in a new Windows user profile.</param>
        internal void SetLocation(string currentPath, string defaultPath)
        {
            CurrentPath = currentPath;
            DefaultPath = defaultPath;
            isTyped = false;
            Path = currentPath;
        }

        /// <summary>
        /// Takes the location the user chose.
        /// </summary>
        /// <param name="newPath">The chosen location.</param>
        /// <param name="typed">The location was typed, not chosen with a button.</param>
        internal void SetChosenLocation(string newPath, bool typed)
        {
            isTyped = typed;
            Path = newPath;
        }

        // The locations are read on background threads
        private void OnPropertyChanged(string name)
            => App.MainWindow.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
    }
}
