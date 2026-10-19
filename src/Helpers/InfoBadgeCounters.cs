// <copyright file="InfoBadgeCounters.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Helpers
{
    using System.ComponentModel;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Сontains the info badges counters state by <see cref="UICategoryTag"/>.
    /// </summary>
    public class InfoBadgeCounters : INotifyPropertyChanged
    {
        private int contextMenu = 0;
        private int gaming = 0;
        private int personalization = 0;
        private int privacy = 0;
        private int security = 0;
        private int system = 0;
        private int taskScheduler = 0;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets context menu category info badge counter.
        /// </summary>
        public int ContextMenu
        {
            get => contextMenu;
            private set
            {
                contextMenu = value;
                OnPropertyChanged(nameof(ContextMenu));
            }
        }

        /// <summary>
        /// Gets gaming category info badge counter.
        /// </summary>
        public int Gaming
        {
            get => gaming;
            private set
            {
                gaming = value;
                OnPropertyChanged(nameof(Gaming));
            }
        }

        /// <summary>
        /// Gets personalization category info badge counter.
        /// </summary>
        public int Personalization
        {
            get => personalization;
            private set
            {
                personalization = value;
                OnPropertyChanged(nameof(Personalization));
            }
        }

        /// <summary>
        /// Gets privacy category info badge counter.
        /// </summary>
        public int Privacy
        {
            get => privacy;
            private set
            {
                privacy = value;
                OnPropertyChanged(nameof(Privacy));
            }
        }

        /// <summary>
        /// Gets security category info badge counter.
        /// </summary>
        public int Security
        {
            get => security;
            private set
            {
                security = value;
                OnPropertyChanged(nameof(Security));
            }
        }

        /// <summary>
        /// Gets system category info badge counter.
        /// </summary>
        public int System
        {
            get => system;
            private set
            {
                system = value;
                OnPropertyChanged(nameof(System));
            }
        }

        /// <summary>
        /// Gets task scheduler category info badge counter.
        /// </summary>
        public int TaskScheduler
        {
            get => taskScheduler;
            private set
            {
                taskScheduler = value;
                OnPropertyChanged(nameof(TaskScheduler));
            }
        }

        /// <summary>
        /// Decrement the info badge counter using the <see cref="UICategoryTag"/>.
        /// </summary>
        /// <param name="tag">The UI category tag.</param>
        public void DecrementCategory(UICategoryTag tag)
        {
            switch (tag)
            {
                case UICategoryTag.ContextMenu:
                    ContextMenu--;
                    break;
                case UICategoryTag.Gaming:
                case UICategoryTag.UWP:
                    Gaming--;
                    break;
                case UICategoryTag.Personalization:
                    Personalization--;
                    break;
                case UICategoryTag.Privacy:
                    Privacy--;
                    break;
                case UICategoryTag.Security:
                    Security--;
                    break;
                case UICategoryTag.System:
                    System--;
                    break;
                case UICategoryTag.TaskScheduler:
                    TaskScheduler--;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Increment the info badge counter using the <see cref="UICategoryTag"/>.
        /// </summary>
        /// <param name="tag">The UI category tags.</param>
        public void IncrementCategory(UICategoryTag tag)
        {
            switch (tag)
            {
                case UICategoryTag.ContextMenu:
                    ContextMenu++;
                    break;
                case UICategoryTag.Gaming:
                case UICategoryTag.UWP:
                    Gaming++;
                    break;
                case UICategoryTag.Personalization:
                    Personalization++;
                    break;
                case UICategoryTag.Privacy:
                    Privacy++;
                    break;
                case UICategoryTag.Security:
                    Security++;
                    break;
                case UICategoryTag.System:
                    System++;
                    break;
                case UICategoryTag.TaskScheduler:
                    TaskScheduler++;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Reset all info badge counter values to 0.
        /// </summary>
        public void ResetAll()
        {
            ContextMenu = 0;
            Gaming = 0;
            Personalization = 0;
            Privacy = 0;
            Security = 0;
            System = 0;
            TaskScheduler = 0;
        }

        private void OnPropertyChanged([CallerMemberName] string? name = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
