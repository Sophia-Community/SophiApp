// <copyright file="StartupCheck.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using Microsoft.UI.Xaml.Controls;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// A line of the startup page: a check of Windows, its result and what the user is told when it fails.
    /// </summary>
    public class StartupCheck : ObservableObject
    {
        private StartupCheckState state = StartupCheckState.Running;
        private string notice = string.Empty;
        private InfoBarSeverity noticeSeverity = InfoBarSeverity.Error;
        private IReadOnlyList<StartupCheckAction> actions = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupCheck"/> class.
        /// </summary>
        /// <param name="title">What is checked.</param>
        public StartupCheck(string title)
        {
            // The mark on the right tells that the check is over, so the dots at the end are dropped
            Title = title.TrimEnd('…', '.', ' ');
        }

        /// <summary>
        /// Gets what is checked.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets or sets what the check has come to.
        /// </summary>
        public StartupCheckState State
        {
            get
            {
                return state;
            }

            set
            {
                if (SetProperty(ref state, value))
                {
                    OnPropertyChanged(nameof(IsRunning));
                    OnPropertyChanged(nameof(IsPassed));
                    OnPropertyChanged(nameof(IsFailed));
                    OnPropertyChanged(nameof(IsContinued));
                    OnPropertyChanged(nameof(AccessibleName));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the check is in progress.
        /// </summary>
        public bool IsRunning => state == StartupCheckState.Running;

        /// <summary>
        /// Gets a value indicating whether the check has passed.
        /// </summary>
        public bool IsPassed => state == StartupCheckState.Passed;

        /// <summary>
        /// Gets a value indicating whether the check has failed and the app cannot go on.
        /// </summary>
        public bool IsFailed => state == StartupCheckState.Failed;

        /// <summary>
        /// Gets a value indicating whether the check has found something the user can pass by.
        /// </summary>
        public bool IsContinued => state == StartupCheckState.Continued;

        /// <summary>
        /// Gets the line as a screen reader says it: the mark is a picture, so the result is added in words.
        /// </summary>
        public string AccessibleName => state switch
        {
            StartupCheckState.Passed => $"{Title}: {"StartupCheck_Passed".GetLocalized()}",
            StartupCheckState.Failed or StartupCheckState.Continued => $"{Title}: {"StartupCheck_Failed".GetLocalized()}",
            _ => Title,
        };

        /// <summary>
        /// Gets or sets what the user is told when the check fails. An empty text hides the notice.
        /// </summary>
        public string Notice
        {
            get
            {
                return notice;
            }

            set
            {
                if (SetProperty(ref notice, value))
                {
                    OnPropertyChanged(nameof(NoticeIsOpen));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the notice is shown.
        /// </summary>
        public bool NoticeIsOpen => notice.Length > 0;

        /// <summary>
        /// Gets or sets how serious the notice is: a warning can be passed by, an error cannot.
        /// </summary>
        public InfoBarSeverity NoticeSeverity
        {
            get => noticeSeverity;
            set => SetProperty(ref noticeSeverity, value);
        }

        /// <summary>
        /// Gets or sets the buttons of the notice.
        /// </summary>
        public IReadOnlyList<StartupCheckAction> Actions
        {
            get
            {
                return actions;
            }

            set
            {
                if (SetProperty(ref actions, value))
                {
                    OnPropertyChanged(nameof(HasActions));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the notice has buttons.
        /// </summary>
        public bool HasActions => actions.Count > 0;
    }
}
