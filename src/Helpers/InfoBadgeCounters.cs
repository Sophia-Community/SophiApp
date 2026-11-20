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
        private int uwp = 0;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Gets context menu category info badge counter.
        /// </summary>
        public int ContextMenu
        {
            get
            {
                return contextMenu;
            }

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
            get
            {
                return gaming;
            }

            private set
            {
                gaming = value;
                OnPropertyChanged(nameof(Gaming));
                OnPropertyChanged(nameof(UwpAndGaming));
            }
        }

        /// <summary>
        /// Gets the counter of the UWP apps chosen to be uninstalled. They have a queue of their own,
        /// so the counter is not reset when the chosen settings are applied or canceled.
        /// </summary>
        public int Uwp
        {
            get
            {
                return uwp;
            }

            private set
            {
                uwp = value;
                OnPropertyChanged(nameof(Uwp));
                OnPropertyChanged(nameof(UwpAndGaming));
            }
        }

        /// <summary>
        /// Gets the info badge counter of the menu item that has both the UWP apps and the gaming settings.
        /// </summary>
        public int UwpAndGaming => Uwp + Gaming;

        /// <summary>
        /// Gets personalization category info badge counter.
        /// </summary>
        public int Personalization
        {
            get
            {
                return personalization;
            }

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
            get
            {
                return privacy;
            }

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
            get
            {
                return security;
            }

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
            get
            {
                return system;
            }

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
            get
            {
                return taskScheduler;
            }

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
                    Gaming--;
                    break;
                case UICategoryTag.UWP:
                    Uwp--;
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
                    Gaming++;
                    break;
                case UICategoryTag.UWP:
                    Uwp++;
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
        /// Reset the counter of the UWP apps chosen to be uninstalled to 0.
        /// </summary>
        public void ResetUwp() => Uwp = 0;

        /// <summary>
        /// Reset the info badge counters of the settings to 0. The counter of the UWP apps is kept: the apps are not applied with the settings.
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
