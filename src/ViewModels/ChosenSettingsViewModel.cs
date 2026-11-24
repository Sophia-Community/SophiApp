// <copyright file="ChosenSettingsViewModel.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.ViewModels
{
    using CommunityToolkit.Mvvm.ComponentModel;
    using CommunityToolkit.Mvvm.Input;
    using SophiApp.Contracts.Services;
    using SophiApp.Customizations;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using SophiApp.Models;
    using System.Collections.ObjectModel;

    /// <summary>
    /// The settings the user chose to change: they are applied together, or the choice is canceled.
    /// </summary>
    public partial class ChosenSettingsViewModel : ObservableObject
    {
        private readonly AppActivity activity;
        private readonly IGroupPolicyService groupPolicyService;
        private readonly IModelService modelService;
        private readonly IProcessService processService;

        [ObservableProperty]
        private bool eventViewerButtonIsVisible;
        [ObservableProperty]
        private bool restartReminderIsVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChosenSettingsViewModel"/> class.
        /// </summary>
        /// <param name="activity">What the app is doing now.</param>
        /// <param name="groupPolicyService">A service for working with group policy API.</param>
        /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        public ChosenSettingsViewModel(AppActivity activity, IGroupPolicyService groupPolicyService, IModelService modelService, IProcessService processService)
        {
            this.activity = activity;
            this.groupPolicyService = groupPolicyService;
            this.modelService = modelService;
            this.processService = processService;
            ChoiceChanged_Command = new RelayCommand<UIModel>(model => ChoiceChanged(model!));
            Apply_Command = new AsyncRelayCommand(ApplyAsync);
            Clear_Command = new AsyncRelayCommand(ClearAsync);
        }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> that a setting calls when the user chose something in it or took the choice back.
        /// </summary>
        public IRelayCommand<UIModel> ChoiceChanged_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to apply the chosen settings.
        /// </summary>
        public IAsyncRelayCommand Apply_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to cancel the choice of all settings.
        /// </summary>
        public IAsyncRelayCommand Clear_Command { get; }

        /// <summary>
        /// Gets the settings that are going to be applied.
        /// </summary>
        public ObservableCollection<UIModel> ApplicableModels { get; } = [];

        /// <summary>
        /// Gets or sets all settings of the app. A choice in one of them can decide the value of another.
        /// </summary>
        public IReadOnlyList<UIModel> Settings { get; set; } = [];

        /// <summary>
        /// Gets the info badges counters state by <see cref="UICategoryTag"/>.
        /// </summary>
        public InfoBadgeCounters InfoBadgeCounters { get; } = new();

        private static async Task CloseSearchWindowAsync()
        {
            var closed = await ForegroundHelper.CloseSearchAsync(WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow)).ConfigureAwait(false);
            App.Logger.LogSearchWindowWatch(closed, ForegroundHelper.LastWatch);
        }

        private void ChoiceChanged(UIModel model)
        {
            Queue(model);
            UpdateLinkedSettings(model);
        }

        // A choice can decide the value of the settings that write the same place in Windows.
        // Such a setting shows that value and is locked until the choice is taken back
        private void UpdateLinkedSettings(UIModel model)
        {
            foreach (var links in SettingLinks.All.Where(link => link.Setting == model.Name).GroupBy(link => link.Dependent))
            {
                if (Settings.FirstOrDefault(setting => setting.Name == links.Key) is not UIModel dependent)
                {
                    continue;
                }

                var link = model.IsChanged ? links.FirstOrDefault(link => link.Choice is null || link.Choice.Equals(model.Choice)) : null;

                if (link is not null && (dependent.IsEnabled || dependent.IsLocked))
                {
                    dependent.LockChoice(link.Value, model.Name, string.Format("UIModel_Notice_SetByAnotherSetting".GetLocalized(), model.Title));
                    Queue(dependent);
                }
                else if (link is null && dependent.LockedBy == model.Name)
                {
                    dependent.UnlockChoice(restoreChoice: true);
                    Queue(dependent);
                }
            }
        }

        // The applied or canceled settings are read again, and so are the settings that share a place in Windows with them.
        // The locks are taken off first: nothing is chosen anymore
        private ObservableCollection<UIModel> GetSettingsToRead()
        {
            foreach (var locked in Settings.Where(setting => setting.IsLocked))
            {
                locked.UnlockChoice(restoreChoice: false);
            }

            var chosen = ApplicableModels.Select(model => model.Name).ToHashSet();
            var linked = SettingLinks.All
                .Where(link => chosen.Contains(link.Setting) || chosen.Contains(link.Dependent))
                .SelectMany(link => new[] { link.Setting, link.Dependent })
                .ToHashSet();
            return new(ApplicableModels.Concat(Settings.Where(setting => linked.Contains(setting.Name))).Distinct());
        }

        // A setting with a choice is applied with the others, and one whose choice is taken back is not
        private void Queue(UIModel model)
        {
            var isChanged = model.IsChanged;
            var isQueued = ApplicableModels.Contains(model);
            model.IsSelected = isChanged;

            if (isChanged && isQueued)
            {
                App.Logger.LogApplicableModelChanged(model.Name, model.ChosenValue);
                return;
            }

            if (isChanged)
            {
                ApplicableModels.Add(model);
                InfoBadgeCounters.IncrementCategory(model.Tag);
                App.Logger.LogApplicableModelAdded(model.Name, model.ChosenValue);
                return;
            }

            if (isQueued)
            {
                _ = ApplicableModels.Remove(model);
                InfoBadgeCounters.DecrementCategory(model.Tag);
                App.Logger.LogApplicableModelRemoved(model.Name);
            }
        }

        private async Task ApplyAsync()
        {
            // Settings are already being applied or read
            if (!activity.IsIdle)
            {
                return;
            }

            activity.IsIdle = false;
            activity.ProgressText = string.Empty;
            activity.ProgressIsVisible = true;
            App.Logger.LogStartApplicableModelsSetState();
            StartMenuHelper.Reset();

            // The button that opens Event Viewer appears only after the custom view was created
            var eventViewCreated = ApplicableModels.OfType<UICheckBoxModel>().Any(model => model.IsChecked && model.Name == nameof(Customizations.Mutators.EventViewerCustomView));

            // The registry is scanned for policies last, after the other settings have written theirs
            if (ApplicableModels.FirstOrDefault(model => model.Name == nameof(Customizations.Mutators.ScanRegistryPolicies)) is UIModel scan)
            {
                ApplicableModels.Move(ApplicableModels.IndexOf(scan), ApplicableModels.Count - 1);
            }

            await modelService.SetModelsStateAsync(ApplicableModels);
            App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
            await modelService.GetModelsStateAsync(GetSettingsToRead());
            ApplicableModels.Clear();
            InfoBadgeCounters.ResetAll();
            App.Logger.LogApplicableModelsClear();

            // Open consoles would show outdated data, and an open policy editor could overwrite the changes
            App.Status.SetOperation("Panel_Status_UpdatingPolicies".GetLocalized());
            await Task.Run(() => processService.CloseManagementConsoles("taskschd.msc", "eventvwr.msc", "gpedit.msc"));
            groupPolicyService.UpdatePolicy();

            // Save all opened folders in order to restore them after File Explorer restart
            App.Status.SetOperation("Panel_Status_RestartingExplorer".GetLocalized());
            var openedFolders = ExplorerHelper.GetOpenedFolders();
            EnvironmentHelper.RefreshUserDesktop();
            EnvironmentHelper.ForcedRefresh();
            processService.KillProcessByName("StartMenuExperienceHost");
            processService.KillProcessByName("explorer");
            await ExplorerHelper.RestoreAsync(openedFolders);

            // Windows Search is restarted too, a little later, so it is watched for after the settings are applied
            _ = CloseSearchWindowAsync();
            activity.ProgressIsVisible = false;
            RestartReminderIsVisible = true;
            App.Logger.LogUserNotice("Panel_RestartReminder/Message".GetLocalized());
            EventViewerButtonIsVisible |= eventViewCreated;
            activity.IsIdle = true;
        }

        private async Task ClearAsync()
        {
            // Settings are already being applied or read
            if (!activity.IsIdle)
            {
                return;
            }

            activity.IsIdle = false;
            App.Logger.LogApplicableModelsCanceled();
            App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
            activity.ProgressIsVisible = true;

            // Reading the state again drops the choice of every setting
            await modelService.GetModelsStateAsync(GetSettingsToRead());
            ApplicableModels.Clear();
            InfoBadgeCounters.ResetAll();
            App.Logger.LogApplicableModelsClear();
            activity.ProgressIsVisible = false;
            activity.IsIdle = true;
        }
    }
}
