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
    using System.Diagnostics;
    using System.Globalization;

    /// <summary>
    /// The settings the user chose to change: they are applied together, or the choice is canceled.
    /// </summary>
    public partial class ChosenSettingsViewModel : ObservableObject
    {
        private const double ShadowStorageNoticeGigabytes = 1;

        private readonly AppActivity activity;
        private readonly IGroupPolicyService groupPolicyService;
        private readonly IModelService modelService;
        private readonly IProcessService processService;
        private readonly IRestorePointService restorePointService;
        private readonly ISettingsService settingsService;

        // Read from the settings file when it is asked for the first time: this view model is created before the file is loaded
        private bool? skipRestorePointQuestion;

        [ObservableProperty]
        private bool eventViewerButtonIsVisible;
        [ObservableProperty]
        private bool restartReminderIsVisible;
        [ObservableProperty]
        private bool restorePointQuestionIsVisible;
        [ObservableProperty]
        private bool restorePointFailureIsVisible;
        [ObservableProperty]
        private bool shadowStorageNoticeIsVisible;
        [ObservableProperty]
        private string shadowStorageNotice = string.Empty;
        [ObservableProperty]
        private bool hasChoices;
        [ObservableProperty]
        private bool restorePointCreatedIsVisible;
        [ObservableProperty]
        private bool restorePointNotCreatedIsVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChosenSettingsViewModel"/> class.
        /// </summary>
        /// <param name="activity">What the app is doing now.</param>
        /// <param name="groupPolicyService">A service for working with group policy API.</param>
        /// <param name="modelService">A service for working with UI models using MVVM pattern.</param>
        /// <param name="processService">A service for working with Windows process API.</param>
        /// <param name="restorePointService">A service for working with restore points.</param>
        /// <param name="settingsService">A service for working with app settings.</param>
        public ChosenSettingsViewModel(
            AppActivity activity,
            IGroupPolicyService groupPolicyService,
            IModelService modelService,
            IProcessService processService,
            IRestorePointService restorePointService,
            ISettingsService settingsService)
        {
            this.activity = activity;
            this.groupPolicyService = groupPolicyService;
            this.modelService = modelService;
            this.processService = processService;
            this.restorePointService = restorePointService;
            this.settingsService = settingsService;

            CreateRestorePoint_Command = new AsyncRelayCommand(CreateRestorePointAsync);
            ChoiceChanged_Command = new RelayCommand<UIModel>(model => ChoiceChanged(model!));
            Apply_Command = new RelayCommand(AskAboutRestorePoint);
            ApplyWithRestorePoint_Command = new AsyncRelayCommand(() => ApplyAsync(createRestorePoint: true));
            ApplyWithoutRestorePoint_Command = new AsyncRelayCommand(() => ApplyAsync(createRestorePoint: false));
            Clear_Command = new AsyncRelayCommand(ClearAsync);
            OpenSystemProtection_Command = new RelayCommand(restorePointService.OpenSettings);

            // With nothing left to apply there is nothing to ask about
            ApplicableModels.CollectionChanged += (_, _) => RestorePointQuestionIsVisible &= ApplicableModels.Count > 0;

            // Something is chosen but not applied yet: settings, or apps to uninstall
            ApplicableModels.CollectionChanged += (_, _) => HasChoices = ApplicableModels.Count + InfoBadgeCounters.Uwp > 0;
            InfoBadgeCounters.PropertyChanged += (_, _) => HasChoices = ApplicableModels.Count + InfoBadgeCounters.Uwp > 0;
        }

        /// <summary>
        /// Occurs when the chosen settings have been applied. The window shows the list: what was applied and what was not.
        /// </summary>
        public event EventHandler<IReadOnlyList<AppliedSetting>>? SettingsApplied;

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> that a setting calls when the user chose something in it or took the choice back.
        /// </summary>
        public IRelayCommand<UIModel> ChoiceChanged_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> of the "Apply" button: the app asks about a restore point before it applies anything.
        /// </summary>
        public IRelayCommand Apply_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to create a restore point and apply the chosen settings.
        /// </summary>
        public IAsyncRelayCommand ApplyWithRestorePoint_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to apply the chosen settings without a restore point.
        /// </summary>
        public IAsyncRelayCommand ApplyWithoutRestorePoint_Command { get; }

        /// <summary>
        /// Gets <see cref="IAsyncRelayCommand"/> to create a restore point right now, apart from applying settings.
        /// </summary>
        public IAsyncRelayCommand CreateRestorePoint_Command { get; }

        /// <summary>
        /// Gets <see cref="IRelayCommand"/> to open the window of Windows where restore points are managed.
        /// </summary>
        public IRelayCommand OpenSystemProtection_Command { get; }

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

        /// <summary>
        /// Gets or sets a value indicating whether the settings are applied at once, without the question about a restore point.
        /// </summary>
        public bool SkipRestorePointQuestion
        {
            get
            {
                return skipRestorePointQuestion ??= settingsService.ReadSkipRestorePointQuestion();
            }

            set
            {
                if (value != SkipRestorePointQuestion)
                {
                    skipRestorePointQuestion = value;
                    OnPropertyChanged();
                    _ = settingsService.SaveSkipRestorePointQuestionAsync(value);
                }
            }
        }

        /// <summary>
        /// Forgets what the user chose and what the app told about restore points: the settings are going to be read from scratch.
        /// </summary>
        public void DropChoices()
        {
            ApplicableModels.Clear();
            InfoBadgeCounters.ResetAll();
            InfoBadgeCounters.ResetUwp();
            RestorePointQuestionIsVisible = false;
            RestorePointCreatedIsVisible = false;
            RestorePointNotCreatedIsVisible = false;
            ShadowStorageNoticeIsVisible = false;
            App.Logger.LogApplicableModelsClear();
        }

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

        private void AskAboutRestorePoint()
        {
            // Settings are already being applied or read
            if (!activity.IsIdle || ApplicableModels.Count == 0)
            {
                return;
            }

            // The user chose not to be asked: the settings are applied at once, and no restore point is created
            if (SkipRestorePointQuestion)
            {
                _ = ApplyAsync(createRestorePoint: false);
                return;
            }

            RestorePointQuestionIsVisible = true;
        }

        // A restore point made by hand, with the button of the settings page
        private async Task CreateRestorePointAsync()
        {
            // Settings are already being applied or read
            if (!activity.IsIdle)
            {
                return;
            }

            activity.IsIdle = false;
            RestorePointCreatedIsVisible = false;
            RestorePointNotCreatedIsVisible = false;
            ShadowStorageNoticeIsVisible = false;
            activity.ProgressText = string.Empty;
            activity.ProgressIsVisible = true;
            var created = await TryCreateRestorePointAsync();
            var shadowStorage = created ? await Task.Run(restorePointService.GetShadowStorageGigabytes) : 0;
            activity.ProgressIsVisible = false;
            RestorePointCreatedIsVisible = created;
            RestorePointNotCreatedIsVisible = !created;
            App.Logger.LogUserNotice((created ? "SettingsPage_RestorePointCreated/Message" : "SettingsPage_RestorePointNotCreated/Message").GetLocalized());
            ShowShadowStorageNotice(shadowStorage);
            activity.IsIdle = true;
        }

        // Restore points pile up: the user is told when the shadow copies of the system drive take a gigabyte or more
        private void ShowShadowStorageNotice(double shadowStorage)
        {
            if (shadowStorage >= ShadowStorageNoticeGigabytes)
            {
                ShadowStorageNotice = string.Format("Panel_ShadowStorageNotice".GetLocalized(), shadowStorage.ToString("0.##", CultureInfo.CurrentCulture));
                ShadowStorageNoticeIsVisible = true;
                App.Logger.LogUserNotice(ShadowStorageNotice);
            }
        }

        // The restore point is created first. Without the one the user asked for nothing is applied
        private async Task<bool> TryCreateRestorePointAsync()
        {
            App.Status.SetOperation("Panel_Status_CreatingRestorePoint".GetLocalized());
            var timer = Stopwatch.StartNew();

            try
            {
                await Task.Run(restorePointService.Create);
                App.Logger.LogRestorePointCreated(timer);
                return true;
            }
            catch (Exception ex)
            {
                App.Logger.LogSkippedException("Creating a restore point", ex);

                // The policies of System Restore are already removed from the registry, so they leave the policy editor too
                groupPolicyService.UpdatePolicy();
                return false;
            }
        }

        private async Task ApplyAsync(bool createRestorePoint)
        {
            // Settings are already being applied or read
            if (!activity.IsIdle)
            {
                return;
            }

            activity.IsIdle = false;
            RestorePointQuestionIsVisible = false;
            RestorePointFailureIsVisible = false;
            ShadowStorageNoticeIsVisible = false;
            activity.ProgressText = string.Empty;
            activity.ProgressIsVisible = true;

            if (createRestorePoint && !await TryCreateRestorePointAsync())
            {
                activity.ProgressIsVisible = false;
                RestorePointFailureIsVisible = true;
                App.Logger.LogUserNotice("Panel_RestorePointFailure/Message".GetLocalized());
                activity.IsIdle = true;
                return;
            }

            App.Logger.LogStartApplicableModelsSetState();
            StartMenuHelper.Reset();

            // The button that opens Event Viewer appears only after the custom view was created
            var eventViewCreated = ApplicableModels.OfType<UICheckBoxModel>().Any(model => model.IsChecked && model.Name == nameof(Customizations.Mutators.EventViewerCustomView));

            // The registry is scanned for policies last, after the other settings have written theirs
            if (ApplicableModels.FirstOrDefault(model => model.Name == nameof(Customizations.Mutators.ScanRegistryPolicies)) is UIModel scan)
            {
                ApplicableModels.Move(ApplicableModels.IndexOf(scan), ApplicableModels.Count - 1);
            }

            // File Explorer and the Start menu are restarted only if a setting that is applied needs it
            var restartExplorer = ApplicableModels.Any(model => Customizations.SettingNeeds.RestartsFileExplorer(model.Name));
            App.Logger.LogFileExplorerRestart(restartExplorer);
            await modelService.SetModelsStateAsync(ApplicableModels);
            App.Status.SetOperation("OsRequirements_ReadWindowsSettings".GetLocalized());
            await modelService.GetModelsStateAsync(GetSettingsToRead());

            // What the user is shown when everything is over: every setting with its result
            var applied = ApplicableModels
                .Select(model => new AppliedSetting(model.Title, model.ApplyFailed, model.ApplyFailed ? model.ExceptionText : string.Empty))
                .ToList();
            ApplicableModels.Clear();
            InfoBadgeCounters.ResetAll();
            App.Logger.LogApplicableModelsClear();

            // Open consoles would show outdated data, and an open policy editor could overwrite the changes
            App.Status.SetOperation("Panel_Status_UpdatingPolicies".GetLocalized());
            await Task.Run(() => processService.CloseManagementConsoles("taskschd.msc", "eventvwr.msc", "gpedit.msc"));
            groupPolicyService.UpdatePolicy();

            if (restartExplorer)
            {
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
            }
            else
            {
                // The running programs are told that the settings and the environment variables have changed
                EnvironmentHelper.ForcedRefresh();
            }

            var shadowStorage = await Task.Run(restorePointService.GetShadowStorageGigabytes);
            activity.ProgressIsVisible = false;
            RestartReminderIsVisible = true;
            App.Logger.LogUserNotice("Panel_RestartReminder/Message".GetLocalized());

            ShowShadowStorageNotice(shadowStorage);
            EventViewerButtonIsVisible |= eventViewCreated;
            activity.IsIdle = true;
            SettingsApplied?.Invoke(this, applied);
        }

        private async Task ClearAsync()
        {
            // Settings are already being applied or read
            if (!activity.IsIdle)
            {
                return;
            }

            activity.IsIdle = false;
            RestorePointQuestionIsVisible = false;
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
