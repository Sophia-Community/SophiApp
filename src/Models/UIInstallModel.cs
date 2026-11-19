// <copyright file="UIInstallModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Models
{
    using System.Globalization;
    using SophiApp.Extensions;
    using SophiApp.Helpers;

    /// <summary>
    /// A package that is installed or updated when the chosen settings are applied. Some packages can be uninstalled too.
    /// </summary>
    public class UIInstallModel : UIModel
    {
        private const string ActionFailedResourceKey = "UIModel_Exception_InstallFailed";
        private readonly Func<InstallState> accessor;
        private readonly Action<bool> mutator;
        private InstallState state;
        private bool installRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="UIInstallModel"/> class.
        /// </summary>
        /// <param name="dto">Dto for <see cref="UIInstallModel"/> initialization.</param>
        /// <param name="title">Model title.</param>
        /// <param name="description">Model description.</param>
        /// <param name="accessor">Method that returns the package state.</param>
        /// <param name="mutator">Method that installs or updates the package if it gets true, and uninstalls it otherwise.</param>
        public UIInstallModel(UIModelDto dto, string title, string description, Func<InstallState> accessor, Action<bool> mutator)
            : base(dto, title)
        {
            this.accessor = accessor;
            this.mutator = mutator;
            Description = description;
            IconIndex = dto.IconIndex;
        }

        /// <summary>
        /// Gets model description.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the index of the card icon.
        /// </summary>
        public int IconIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the package can be uninstalled.
        /// </summary>
        public bool CanUninstall { get; init; }

        /// <summary>
        /// Gets a method that installs the package for all users of the computer. Null if the package has no such option.
        /// </summary>
        public Action<bool>? AllUsersMutator { get; init; }

        /// <summary>
        /// Gets a value indicating whether the package can be installed for all users.
        /// </summary>
        public bool HasAllUsersOption => AllUsersMutator is not null;

        /// <summary>
        /// Gets or sets a value indicating whether the user chose to install the package for all users.
        /// </summary>
        public bool AllUsers { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether <see cref="SetState"/> installs the package. It uninstalls the package otherwise.
        /// </summary>
        public bool InstallRequested
        {
            get
            {
                return installRequested;
            }

            set
            {
                installRequested = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets what <see cref="SetState"/> is going to do, for the status line: "Installing OneDrive".
        /// </summary>
        public string ActionTitle
        {
            get
            {
                var installKey = State == InstallState.UpdateAvailable ? "Panel_Install_Updating" : "Panel_Install_Installing";
                var key = InstallRequested ? installKey : "Panel_Install_Uninstalling";
                return string.Format(CultureInfo.CurrentCulture, key.GetLocalized(), Title);
            }
        }

        /// <summary>
        /// Gets the package state.
        /// </summary>
        public InstallState State
        {
            get
            {
                return state;
            }

            private set
            {
                state = value;
                App.Logger.LogModelState(Name, State);
                OnPropertyChanged();
            }
        }

        /// <inheritdoc/>
        public override void GetState()
        {
            try
            {
                State = accessor.Invoke();
                IsEnabled = true;

                // After a failed action the card stays available, and the user is told to try again
                ShowNotice(string.Empty);
            }
            catch (CustomizationException ex)
            {
                State = ex.InstallState ?? State;
                IsEnabled = false;
                ShowBlockingReason(ex.GetText());
                App.Logger.LogModelGetStateException(Name, ex);
            }
            catch (Exception ex)
            {
                IsEnabled = false;
                ShowBlockingReason(string.Empty);
                App.Logger.LogModelGetStateException(Name, ex);
            }
        }

        /// <inheritdoc/>
        public override void SetState()
        {
            try
            {
                var action = InstallRequested && AllUsers && AllUsersMutator is not null ? AllUsersMutator : mutator;
                action.Invoke(InstallRequested);
            }
            catch (Exception ex)
            {
                SetApplyFailure(ex is CustomizationException reason ? reason.GetText() : ActionFailedResourceKey.GetLocalized());
                App.Logger.LogModelSetStateException(ex, Name, InstallRequested);
            }
        }

        /// <inheritdoc/>
        public override bool ContainsText(string text) => base.ContainsText(text) || FormattedText.ToPlainText(Description).Contains(text, StringComparison.CurrentCultureIgnoreCase);
    }
}
