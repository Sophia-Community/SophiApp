// <copyright file="AssociationItem.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.ComponentModel;
    using Microsoft.UI.Xaml;
    using SophiApp.Extensions;

    /// <summary>
    /// A program the user wants to open an extension or a protocol with.
    /// </summary>
    public class AssociationItem : INotifyPropertyChanged
    {
        private string program = string.Empty;
        private string extension = string.Empty;
        private string icon = string.Empty;
        private string currentApp = string.Empty;

        // What was applied last, so the same program is not applied twice
        private string applied = string.Empty;

        /// <summary>
        /// Property change event.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets the path to the program, the whole command with it, or the ProgId of a registered app.
        /// </summary>
        public string Program
        {
            get => program;
            internal set => Set(ref program, value, nameof(Program));
        }

        /// <summary>
        /// Gets the extension with the dot, or the name of the protocol.
        /// </summary>
        public string Extension
        {
            get => extension;
            internal set => Set(ref extension, value, nameof(Extension));
        }

        /// <summary>
        /// Gets the path to the icon for the files, or an empty string.
        /// </summary>
        public string Icon
        {
            get => icon;
            internal set => Set(ref icon, value, nameof(Icon));
        }

        /// <summary>
        /// Gets the name of the app that opens the extension in Windows now, or an empty string.
        /// </summary>
        public string CurrentApp
        {
            get
            {
                return currentApp;
            }

            internal set
            {
                Set(ref currentApp, value, nameof(CurrentApp));
                OnPropertyChanged(nameof(CurrentAppText));
                OnPropertyChanged(nameof(CurrentAppVisibility));
            }
        }

        /// <summary>
        /// Gets the text that names the app that opens the extension in Windows now.
        /// </summary>
        public string CurrentAppText => string.Format("Association_Current".GetLocalized(), CurrentApp);

        /// <summary>
        /// Gets the visibility of <see cref="CurrentAppText"/>.
        /// </summary>
        public Visibility CurrentAppVisibility => CurrentApp.Length > 0 ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Gets a value indicating whether there is a program and an extension to apply.
        /// </summary>
        public bool IsFilled => Program.Length > 0 && Extension.Length > 0;

        /// <summary>
        /// Gets a value indicating whether what is typed now has already been applied.
        /// </summary>
        public bool IsApplied => applied == Signature;

        private string Signature => $"{Program}|{Extension}|{Icon}";

        /// <summary>
        /// Remembers that what is typed now has been applied.
        /// </summary>
        internal void MarkApplied() => applied = Signature;

        private void Set(ref string field, string value, string name)
        {
            if (field != value)
            {
                field = value;
                OnPropertyChanged(name);
            }
        }

        // The state is read on background threads
        private void OnPropertyChanged(string name)
            => App.MainWindow.DispatcherQueue.TryEnqueue(() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name)));
    }
}
