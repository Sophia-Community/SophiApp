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
        private readonly string titleKey;
        private readonly Func<IReadOnlyList<string>>? detailsSource;
        private StartupCheckState state = StartupCheckState.Running;
        private string notice = string.Empty;
        private InfoBarSeverity noticeSeverity = InfoBarSeverity.Error;
        private IReadOnlyList<StartupCheckAction> actions = [];
        private IReadOnlyList<string> details = [];

        /// <summary>
        /// Initializes a new instance of the <see cref="StartupCheck"/> class.
        /// </summary>
        /// <param name="titleKey">The resource key of the text that tells what is checked.</param>
        /// <param name="detailsSource">Returns what the check has found, or null if it has nothing to show.</param>
        public StartupCheck(string titleKey, Func<IReadOnlyList<string>>? detailsSource = null)
        {
            this.titleKey = titleKey;
            this.detailsSource = detailsSource;
        }

        /// <summary>
        /// Gets what is checked. The mark on the right tells that the check is over, so the dots at the end are dropped.
        /// </summary>
        public string Title => titleKey.GetLocalized().TrimEnd('…', '.', ' ');

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
            StartupCheckState.Passed when HasDetails => $"{Title}: {string.Join(", ", details)}",
            StartupCheckState.Passed => $"{Title}: {"StartupCheck_Passed".GetLocalized()}",
            StartupCheckState.Failed or StartupCheckState.Continued => $"{Title}: {"StartupCheck_Failed".GetLocalized()}",
            _ => Title,
        };

        /// <summary>
        /// Gets or sets what the check has found, a line for each thing. The lines stand under the check.
        /// </summary>
        public IReadOnlyList<string> Details
        {
            get
            {
                return details;
            }

            set
            {
                if (SetProperty(ref details, value))
                {
                    OnPropertyChanged(nameof(HasDetails));
                    OnPropertyChanged(nameof(AccessibleName));
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the check has found something to show.
        /// </summary>
        public bool HasDetails => details.Count > 0;

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

        /// <summary>
        /// Shows what the check has found, if it finds anything.
        /// </summary>
        public void ShowDetails() => Details = detailsSource?.Invoke() ?? [];

        /// <summary>
        /// Reads the texts of the line anew, after the language of the app has been changed.
        /// </summary>
        public void Relocalize()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(AccessibleName));

            if (HasDetails)
            {
                ShowDetails();
            }
        }
    }
}
