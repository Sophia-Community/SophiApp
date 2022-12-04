using Microsoft.Win32;
using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Conditions;
using SophiApp.Customisations;
using SophiApp.Dto;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using SophiApp.Models;
using SophiApp.Watchers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shell;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void AdvancedSettingsClicked(object args) => AdvancedSettingsVisibility = AdvancedSettingsVisibility.Invert();

        private async void ApplyingSettingsAsync(object args)
        {
            SetTaskbarItemInfoProgress();

            await Task.Run(async () =>
            {
                var applyingCancellationSource = new CancellationTokenSource();
                var token = applyingCancellationSource.Token;

                DebugHelper.StartApplyingSettings(CustomActions.Count);
                SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
                SetInfoPanelVisibility(InfoPanelVisibility.Loading);

                foreach (var action in CustomActions)
                {
                    try
                    {
                        if (action is UwpCustomisation customisation)
                        {
                            customisation.Invoke();
                        }
                        else
                        {
                            action.Invoke();
                            DebugHelper.ActionTaken(action);
                        }
                    }
                    catch (Exception e)
                    {
                        DebugHelper.HasException($"Customization action {action.Id} with parameter {action.Parameter} caused an error", e);
                        applyingCancellationSource.Cancel();
                        CustomActions.Clear();
                        OnPropertyChanged(CustomActionsPropertyName);
                        var faultyElement = FindFaultyElement(action.Id);
                        SetApplyingSettingsError(faultyElement.Header);
                        SetVisibleViewTag(Tags.ApplyingException);
                        SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false);
                        break;
                    }
                }

                if (token.IsCancellationRequested.Invert())
                {
                    var totalStopWatch = Stopwatch.StartNew();
                    OnPropertyChanged(CustomActionsPropertyName);
                    await GetTextedElementsStatusAsync();
                    GetUwpElements();
                    OsHelper.PostMessage();
                    OsHelper.RefreshEnvironment();
                    OsHelper.SafelyRestartExplorerProcess();
                    RunPostRestartExplorerActions();
                    CustomActions.Clear();
                    SetInfoPanelVisibility(InfoPanelVisibility.RestartNecessary);
                    SetControlsHitTest();
                    totalStopWatch.Stop();
                    DebugHelper.StopApplyingSettings(totalStopWatch.Elapsed.TotalSeconds);
                }
            });

            SetTaskbarItemInfoProgress();
        }

        private async void AppThemeChangeAsync(object args)
        {
            await Task.Run(() =>
            {
                var name = args as string;
                var theme = themesHelper.Find(name);
                themesHelper.ChangeTheme(theme);
                SetAppSelectedTheme(theme);
            });
        }

        private async void AppThemeChangeAsync(Theme theme)
        {
            await Task.Run(() =>
            {
                themesHelper.ChangeTheme(theme);
                SetAppSelectedTheme(theme);
            });
        }

        private void DebugModeClicked(object args) => DebugMode = DebugMode.Invert();

        private async Task DeserializeTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                var deserializedElements = JsonConvert.DeserializeObject<IEnumerable<TextedElementDto>>(Encoding.UTF8.GetString(Properties.Resources.UIData))
                                                      .Where(dto => IsWindows11 ? dto.Windows11Supported : dto.Windows10Supported)
                                                      .Select(dto => FabricHelper.CreateTextedElement(dto: dto,
                                                                                                      errorHandler: OnTextedElementErrorAsync,
                                                                                                      statusHandler: OnTextedElementStatusChanged,
                                                                                                      language: Localization.Language))
                                                      .OrderByDescending(element => element.ViewId);

                TextedElements = new ConcurrentBag<TextedElement>(deserializedElements);
            });
        }

        private TextedElement FindFaultyElement(uint id)
        {
            return TextedElements.Where(element => element.Id == id).FirstOrDefault()
                   ?? TextedElements.Where(element => element is IParentElements parent && parent.ChildElements.Any(child => child.Id == id))
                                    .First();
        }

        private Task GetTextedElementsStatus(string tag)
        {
            return Task.Factory.StartNew(() => TextedElements.Where(element => element.Tag == tag && element.Status != ElementStatus.DISABLED)
                                                      .ToList()
                                                      .ForEach(element => element.GetCustomisationStatus()));
        }

        private async Task GetTextedElementsStatusAsync()
        {
            var task = new Task[] { GetTextedElementsStatus("Privacy"), GetTextedElementsStatus("Personalization"), GetTextedElementsStatus("System"),
                                    GetTextedElementsStatus("StartMenu"), GetTextedElementsStatus("UwpApps"), GetTextedElementsStatus("Games"),
                                    GetTextedElementsStatus("TaskScheduler"), GetTextedElementsStatus("Security"), GetTextedElementsStatus("ContextMenu") };

            await Task.WhenAll(task);
        }

        private void GetUwpElements()
        {
            DebugHelper.StartInitUwpApps();
            var stopwatch = Stopwatch.StartNew();
            UwpElementsCurrentUser = UwpHelper.GetPackagesDto(forAllUsers: false)
                                              .Select(dto => FabricHelper.CreateUwpElement(dto))
                                              .OrderBy(uwp => uwp.DisplayName)
                                              .ToList();

            UwpElementsAllUsers = UwpHelper.GetPackagesDto(forAllUsers: true)
                                           .Select(dto => FabricHelper.CreateUwpElement(dto))
                                           .OrderBy(uwp => uwp.DisplayName)
                                           .ToList();
            stopwatch.Stop();
            DebugHelper.StopInitUwpApps(stopwatch.Elapsed.TotalSeconds);
        }

        private void HamburgerClicked(object args)
        {
            var tag = args as string;

            if (VisibleViewByTag != tag)
                SetVisibleViewTag(tag);
        }

        private async void HyperLinkClickedAsync(object args)
        {
            MouseHelper.ShowWaitCursor(show: true);
            await Task.Run(() =>
            {
                var link = args as string;
                DebugHelper.LinkClicked(link);
                Process.Start(link);
            });
            MouseHelper.ShowWaitCursor(show: false);
        }

        private void InitializeCommands()
        {
            TaskSchedulerOpenClickedCommand = new RelayCommand(new Action<object>(TaskSchedulerOpenClicked));
            AdvancedSettingsClickedCommand = new RelayCommand(new Action<object>(AdvancedSettingsClicked));
            ApplyingSettingsCommand = new RelayCommand(new Action<object>(ApplyingSettingsAsync));
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChangeAsync));
            DebugModeClickedCommand = new RelayCommand(new Action<object>(DebugModeClicked));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            HyperLinkClickedCommand = new RelayCommand(new Action<object>(HyperLinkClickedAsync));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            OpenUpdateCenterWindowCommand = new RelayCommand(new Action<object>(OpenUpdateCenterWindowAsync));
            RadioGroupClickedCommand = new RelayCommand(new Action<object>(RadioGroupClickedAsync));
            ResetTextedElementsStateCommand = new RelayCommand(new Action<object>(ResetTextedElementsStateAsync));
            RiskAgreeClickedCommand = new RelayCommand(new Action<object>(RiskAgreeClickedAsync));
            SaveDebugLogCommand = new RelayCommand(new Action<object>(SaveDebugLogAsync));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync), new Predicate<object>(SearchClicked_CanExecute));
            SwitchUwpForAllUsersClickedCommand = new RelayCommand(new Action<object>(SwitchUwpForAllUsersClicked));
            TextedElementClickedCommand = new RelayCommand(new Action<object>(TextedElementClickedAsync));
            UwpButtonClickedCommand = new RelayCommand(new Action<object>(UwpButtonClickedAsync));
        }

        private async Task InitializeDataAsync()
        {
            SetVisibleViewTag(Tags.ViewLoading);
            SetTaskbarItemInfoProgress();
            MouseHelper.ShowWaitCursor(show: true);
            await DeserializeTextedElementsAsync();
            await InitializeTextedElementsAsync();
            await InitializeUwpElementsAsync();
            await InitializeWatchersAsync();
            await InitializeWindowsDefenderState();
            await PrepareOsData();
            SetVisibleViewTag(Tags.ViewPrivacy);
            SetControlsHitTest(hamburgerHitTest: true);
            MouseHelper.ShowWaitCursor(show: false);
            SetTaskbarItemInfoProgress();
        }

        private void InitializeProperties()
        {
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
            DebugMode = true;
            buildName = Application.Current.TryFindResource("Localization.Build.Name") as string;
            HamburgerHitTest = false;
            ViewsHitTest = true;
            VisibleViewByTag = Tags.ViewLoading;
            WindowCloseHitTest = true;
            AdvancedSettingsVisibility = false;
            InfoPanelVisibility = InfoPanelVisibility.HideAll;
            CustomActions = new List<Customisation>();
            FoundTextedElement = new List<TextedElement>();
            Search = SearchState.Stopped;
            UwpForAllUsersState = ElementStatus.UNCHECKED;
            DebugHelper.AppLanguage($"{Localization.Language}");
            DebugHelper.AppTheme(AppSelectedTheme.Alias);
        }

        private async Task InitializeTextedElements(string tag) => await Task.Run(() => TextedElements.Where(element => element.Tag == tag)
        .ToList().ForEach(element => element.Initialize()));

        private async Task InitializeTextedElementsAsync()
        {
            DebugHelper.StartInitTextedElements();
            var stopwatch = Stopwatch.StartNew();

            var task = new Task[] { InitializeTextedElements("Privacy"), InitializeTextedElements("Personalization"), InitializeTextedElements("System"),
                                    InitializeTextedElements("StartMenu"), InitializeTextedElements("UwpApps"), InitializeTextedElements("Games"),
                                    InitializeTextedElements("TaskScheduler"), InitializeTextedElements("Security"), InitializeTextedElements("ContextMenu") };

            await Task.WhenAll(task);
            stopwatch.Stop();
            DebugHelper.StopInitTextedElements(stopwatch.Elapsed.TotalSeconds);
        }

        private async Task InitializeUwpElementsAsync() => await Task.Run(() => GetUwpElements());

        private async Task InitializeWatchersAsync()
        {
            await Task.Run(() =>
            {
                var regWatcher = RegistryWatcher.GetInstance();
                regWatcher.SystemThemeChangedEvent += OnSystemThemeChanged;
                _ = regWatcher.Start();
            });
        }

        private async Task InitializeWindowsDefenderState()
        {
            var elementsToDisableId = new List<uint>() { 800, 801, 802, 808 };
            await Task.Run(() =>
            {
                if (WindowsDefenderHelper.IsValid().Invert())
                {
                    TextedElements.Where(element => elementsToDisableId.Contains(element.Id))
                    .ToList().ForEach(element => element.Status = ElementStatus.DISABLED);
                }
            });
        }

        private async void LocalizationChangeAsync(object args)
        {
            await Task.Run(() =>
            {
                var localization = localizationsHelper.FindName(args as string);

                foreach (var element in TextedElements)
                {
                    element.ChangeLanguage(localization.Language);
                }

                localizationsHelper.Change(localization);
                SetLocalizationProperty(localization);
                OnPropertyChanged(AppSelectedThemePropertyName);
                OnPropertyChanged(AppThemesPropertyName);
            });
        }

        private void OnConditionsHasError(object sender, Exception e)
        {
            DebugHelper.HasException("An error occurred during the startup OS condition check", e);
            ConditionsHelperError = e.Message;
            SetVisibleViewTag($"{ConditionsTag.SomethingWrong}");
        }

        private void OnConditionsHasProblem(object sender, IStartupCondition e)
        {
            SetVisibleViewTag($"{e.Tag}");

            if (e is OsVersionCondition)
            {
                try
                {
                    ComObjectHelper.EnableUpdateForOtherProducts();
                    Thread.Sleep(1000);
                    PowerShellHelper.GetUwpAppsUpdates();
                    Thread.Sleep(1000);
                    ComObjectHelper.SetWindowsUpdateDetectNow();
                }
                catch (Exception)
                {
                }
            }
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void OnSystemThemeChanged(object sender, byte value) => AppThemeChangeAsync(themesHelper.Themes[value]);

        private async void OnTextedElementErrorAsync(TextedElement element, Exception e)
        {
            await Task.Run(() =>
            {
                DebugHelper.HasException($"An error occured in element: {element.Id}", e);
                element.Status = ElementStatus.DISABLED;
            });
        }

        private void OnTextedElementStatusChanged(object sender, TextedElement element) => DebugHelper.TextedElementChanged(element.Id, element.Status);

        private async void OpenUpdateCenterWindowAsync(object obj)
        {
            await Task.Run(() =>
            {
                const string UPDATE_CENTER_WINDOW = "ms-settings:windowsupdate-action";
                _ = ProcessHelper.Start(UPDATE_CENTER_WINDOW, null, ProcessWindowStyle.Normal);
            });
        }

        private async Task PrepareOsData()
        {
            await Task.Run(() =>
            {
                // Persist Sophia notifications to prevent to immediately disappear from Action Center
                RegHelper.SetValue(RegistryHive.CurrentUser, @"Software\Microsoft\Windows\CurrentVersion\Notifications\Settings\Sophia", "ShowInActionCenter", 1, RegistryValueKind.DWord);
                RegHelper.SetValue(RegistryHive.ClassesRoot, @"AppUserModelId\Sophia", "DisplayName", "Sophia", RegistryValueKind.String);
                RegHelper.SetValue(RegistryHive.ClassesRoot, @"AppUserModelId\Sophia", "ShowInSettings", 0, RegistryValueKind.DWord);
            });
        }

        private async void RadioGroupClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var rButton = args as RadioButton;
                var rGroup = TextedElements.FirstOrDefault(element => element.Id == rButton.ParentId) as RadioGroup;
                rGroup?.ChildElements.ForEach(child => child.Status = child.Id == rButton.Id ? ElementStatus.CHECKED : ElementStatus.UNCHECKED);
                SetCustomAction(rGroup);
            });
        }

        private async void ResetTextedElementsStateAsync(object args)
        {
            DebugHelper.StartResetTextedElements();
            var stopwatch = Stopwatch.StartNew();
            SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetInfoPanelVisibility(InfoPanelVisibility.Loading);

            await Task.Run(() =>
            {
                CustomActions.Clear();
                OnPropertyChanged(CustomActionsPropertyName);

                foreach (var element in TextedElements)
                {
                    element.GetCustomisationStatus();
                }

                GetUwpElements();
            });

            SetInfoPanelVisibility(InfoPanelVisibility.HideAll);
            SetControlsHitTest();
            stopwatch.Stop();
            DebugHelper.StopResetTextedElements(stopwatch.Elapsed.TotalSeconds);
        }

        private async void RiskAgreeClickedAsync(object obj)
        {
            DebugHelper.RiskAgreed();
            await InitializeDataAsync();
        }

        private async void SaveDebugLogAsync(object args)
        {
            await Task.Run(() =>
            {
                try
                {
                    DebugHelper.Save(AppHelper.DebugFile);
                }
                catch (Exception)
                {
                }
            });
        }

        private bool SearchClicked_CanExecute(object arg) => string.IsNullOrWhiteSpace(arg as string).Invert();

        private async void SearchClickedAsync(object arg)
        {
            await Task.Run(() =>
            {
                var stopwatch = Stopwatch.StartNew();
                var searchString = arg as string;
                FoundTextedElement.Clear();
                Search = SearchState.Running;
                SetVisibleViewTag(Tags.ViewSearch);
                FoundTextedElement = TextedElements.Where(element => element.Status != ElementStatus.DISABLED
                                                                        && element.ContainsText(searchString)).ToList();
                Search = SearchState.Stopped;
                stopwatch.Stop();
                DebugHelper.StopSearch(searchString, stopwatch.Elapsed.TotalSeconds, foundTextedElement.Count);
            });
        }

        private void SetApplyingSettingsError(string name)
        {
            var applyingString = Application.Current.FindResource("Localization.ViewApplyingException.InApplying") as string;
            var exceptionString = Application.Current.FindResource("Localization.ViewApplyingException.HasException") as string;

            ApplyingSettingsError = name;
            ApplyingSettingsErrorInApplying = applyingString;
            ApplyingSettingsErrorHasException = exceptionString;
        }

        private void SetAppSelectedTheme(Theme theme) => AppSelectedTheme = theme;

        private void SetControlsHitTest(bool hamburgerHitTest = true, bool viewsHitTest = true, bool windowCloseHitTest = true)
        {
            HamburgerHitTest = hamburgerHitTest;
            ViewsHitTest = viewsHitTest;
            WindowCloseHitTest = windowCloseHitTest;
        }

        private void SetCustomAction(UwpElement uwp)
        {
            SetInfoPanelVisibility(InfoPanelVisibility.HideAll);
            if (CustomActions.ContainsId(uwp.PackageFullName))
            {
                CustomActions.RemoveAction(uwp.PackageFullName);
                OnPropertyChanged(CustomActionsPropertyName);
                return;
            }

            CustomActions.AddAction(uwp.PackageFullName, UwpHelper.RemovePackage, UwpForAllUsersState == ElementStatus.CHECKED);
            OnPropertyChanged(CustomActionsPropertyName);
        }

        private void RunPostRestartExplorerActions()
        {
            var meetAction = CustomActions.Find(action => action.Id == 239);

            if (meetAction?.Parameter ?? false)
                meetAction.Invoke();
        }

        private void SetCustomAction(RadioGroup group)
        {
            SetInfoPanelVisibility(InfoPanelVisibility.HideAll);
            group.ChildElements.ForEach(child =>
            {
                if (CustomActions.ContainsId(child.Id))
                {
                    CustomActions.RemoveAction(child.Id);
                    OnPropertyChanged(CustomActionsPropertyName);
                    return;
                }

                if (child.Status == ElementStatus.CHECKED && child.Id != group.DefaultId)
                {
                    CustomActions.AddAction(child.Id, CustomisationsHelper.GetCustomisationOs(child.Id), true);
                    OnPropertyChanged(CustomActionsPropertyName);
                }
            });
        }

        private void SetCustomAction(TextedElement element)
        {
            SetInfoPanelVisibility(InfoPanelVisibility.HideAll);

            if (CustomActions.ContainsId(element.Id))
            {
                CustomActions.RemoveAction(element.Id);
                OnPropertyChanged(CustomActionsPropertyName);
                return;
            }

            CustomActions.AddAction(element.Id, CustomisationsHelper.GetCustomisationOs(element.Id), element.Status == ElementStatus.CHECKED);
            OnPropertyChanged(CustomActionsPropertyName);
        }

        private void SetInfoPanelVisibility(InfoPanelVisibility visibility) => InfoPanelVisibility = visibility;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetTaskbarItemInfoProgress()
        {
            var taskbarInfo = Application.Current.MainWindow.FindName("TaskBarItemInfo") as TaskbarItemInfo;
            taskbarInfo.ProgressState = taskbarInfo.ProgressState == TaskbarItemProgressState.None ? TaskbarItemProgressState.Indeterminate : TaskbarItemProgressState.None;
        }

        private void SetVisibleViewTag(string tag) => VisibleViewByTag = tag;

        private void SwitchUwpForAllUsersClicked(object args) => UwpForAllUsersState = UwpForAllUsersState == ElementStatus.UNCHECKED ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;

        private void TaskSchedulerOpenClicked(object args) => ProcessHelper.Start(MSC_TASK_SHEDULER, null, ProcessWindowStyle.Normal);

        private async void TextedElementClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var element = args as TextedElement;
                element.ChangeStatus();
                SetCustomAction(element);
            });
        }

        private async void UwpButtonClickedAsync(object args) => await Task.Run(() => SetCustomAction(args as UwpElement));

        internal async void InitializeStartupConditionsAsync()
        {
            DebugHelper.StartStartupConditions();
            var stopwatch = Stopwatch.StartNew();
            var conditionsHelper = new StartupConditionsHelper(errorHandler: OnConditionsHasError, resultHandler: OnConditionsHasProblem);
            await conditionsHelper.CheckAsync();
            stopwatch.Stop();
            DebugHelper.StopStartupConditions(stopwatch.Elapsed.TotalSeconds);

            if (conditionsHelper.HasProblem.Invert())
                await InitializeDataAsync();
        }

        internal async Task RemoveFrameworkLog() => await Task.Run(() => FileHelper.TryDeleteFile(AppHelper.AppFrameworkLog));
    }
}