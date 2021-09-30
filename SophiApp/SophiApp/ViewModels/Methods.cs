using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Debugger = SophiApp.Helpers.DebugHelper;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void AdvancedSettingsClicked(object args) => AdvancedSettingsVisibility = !AdvancedSettingsVisibility;

        private async void ApplyingSettingsAsync(object args)
        {
            await Task.Run(() =>
            {
                debugger.StatusEntry($"Started applying {customActions.Count} setting(s)");
                var stopwatch = Stopwatch.StartNew();
                SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
                SetLoadingPanelVisibility();

                customActions.ForEach(action =>
                    {
                        try
                        {
                            action.Invoke();
                            debugger.ActionEntry(action.Id, action.Parameter);
                        }
                        catch (Exception e)
                        {
                            debugger.Exception($"Customization action {action.Id} with parameter {action.Parameter} caused an error", e);
                        }
                    });

                customActions.Clear();
                OnPropertyChanged(CustomActionsCounterPropertyName);
                OsHelper.PostMessage();
                OsHelper.Refresh();
                TextedElements.ForEach(element => element.GetCustomisationStatus());
                SetLoadingPanelVisibility();
                SetControlsHitTest();
                stopwatch.Stop();
                debugger.StopApplying(stopwatch);
            });
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

        private void DebugModeClicked(object args) => DebugMode = !DebugMode;

        private void HamburgerClicked(object args) => SetVisibleViewTag(args as string);

        private async void HyperLinkClickedAsync(object args)
        {
            MouseHelper.ShowWaitCursor(show: true);
            await Task.Run(() =>
            {
                var link = args as string;
                debugger.StatusEntry($"Clicked link: \"{link}\"");
                Process.Start(link);
            });
            MouseHelper.ShowWaitCursor(show: false);
        }

        private void InitCommands()
        {
            DebugModeClickedCommand = new RelayCommand(new Action<object>(DebugModeClicked));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            TextedElementClickedCommand = new RelayCommand(new Action<object>(TextedElementClickedAsync));
            RadioGroupClickedCommand = new RelayCommand(new Action<object>(RadioGroupClickedAsync));
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChangeAsync));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            HyperLinkClickedCommand = new RelayCommand(new Action<object>(HyperLinkClickedAsync));
            AdvancedSettingsClickedCommand = new RelayCommand(new Action<object>(AdvancedSettingsClicked));
            SaveDebugLogCommand = new RelayCommand(new Action<object>(SaveDebugLogAsync));
            ResetTextedElementsStateCommand = new RelayCommand(new Action<object>(ResetTextedElementsStateAsync));
            ApplyingSettingsCommand = new RelayCommand(new Action<object>(ApplyingSettingsAsync));
        }

        private void InitProperties()
        {
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
            debugger = new Debugger(language: $"{ Localization.Language}", theme: $"{ AppSelectedTheme.Alias}");
            DebugMode = true;
            LoadingPanelVisibility = false;
            HamburgerHitTest = false;
            ViewsHitTest = true;
            WindowCloseHitTest = true;
            AdvancedSettingsVisibility = false;
            customActions = new List<CustomActionDTO>();
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.StatusEntry("Started initialization texted elements");
                var stopwatch = Stopwatch.StartNew();
                TextedElements = JsonConvert.DeserializeObject<IEnumerable<TextedElementDTO>>(Encoding.UTF8.GetString(Properties.Resources.UIData))
                                            .Select(dto => FabricHelper.CreateTextedElement(dataObject: dto,
                                                                                            errorHandler: OnTextedElementErrorAsync,
                                                                                            statusHandler: OnTextedElementStatusChanged,
                                                                                            language: Localization.Language)).ToList();
                stopwatch.Stop();
                debugger.StopInit(stopwatch);
            });
        }

        private bool IsNewVersion(ReleaseDTO dto) => new Version(dto.tag_name) > DataHelper.Version && !dto.prerelease && !dto.draft;

        private bool IsSupportedOs() => OsHelper.GetProductName().Contains(WINDOWS_10) && OsHelper.GetBuild() >= MinimalOsBuild;

        private async void LocalizationChangeAsync(object args)
        {
            await Task.Run(() =>
            {
                var localization = localizationsHelper.FindName(args as string);
                TextedElements.ForEach(element => element.ChangeLanguage(localization.Language));
                localizationsHelper.Change(localization);
                SetLocalizationProperty(localization);
                OnPropertyChanged(AppSelectedThemePropertyName);
                OnPropertyChanged(AppThemesPropertyName);
            });
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void OnTextedElementErrorAsync(TextedElement element, Exception e)
        {
            await Task.Run(() =>
            {
                debugger.Exception($"An error occured in element: {element.Id}", e);
                element.Status = ElementStatus.DISABLED;
            });
        }

        private void OnTextedElementStatusChanged(object sender, TextedElement element) => debugger.ElementChanged(element.Id, element.Status);

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
            debugger.StatusEntry("Started reset texted elements status");
            var stopwatch = Stopwatch.StartNew();
            SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetLoadingPanelVisibility();
            await Task.Run(() =>
            {
                customActions.Clear();
                OnPropertyChanged(CustomActionsCounterPropertyName);
                TextedElements.ForEach(element => element.GetCustomisationStatus());
            });
            SetLoadingPanelVisibility();
            SetControlsHitTest();
            stopwatch.Stop();
            debugger.StopInit(stopwatch);
        }

        private async void SaveDebugLogAsync(object args)
        {
            await Task.Run(() =>
            {
                try
                {
                    debugger.Save(DataHelper.DebugFile);
                }
                catch (Exception)
                {
                }
            });
        }

        private void SetAppSelectedTheme(Theme theme) => AppSelectedTheme = theme;

        private void SetControlsHitTest(bool hamburgerHitTest = true, bool viewsHitTest = true, bool windowCloseHitTest = true)
        {
            HamburgerHitTest = hamburgerHitTest;
            ViewsHitTest = viewsHitTest;
            WindowCloseHitTest = windowCloseHitTest;
        }

        private void SetCustomAction(RadioGroup group)
        {
            group.ChildElements.ForEach(child =>
            {
                if (customActions.ContainsId(child.Id))
                {
                    customActions.RemoveDataObject(child.Id);
                    OnPropertyChanged(CustomActionsCounterPropertyName);
                    return;
                }

                if (child.Status == ElementStatus.CHECKED && child.Id != group.DefaultId)
                {
                    customActions.AddDataObject(child.Id, CustomisationsHelper.GetCustomisationOs(child.Id), true);
                    OnPropertyChanged(CustomActionsCounterPropertyName);
                }
            });
        }

        private void SetCustomAction(TextedElement element)
        {
            if (customActions.ContainsId(element.Id))
            {
                customActions.RemoveDataObject(element.Id);
                return;
            }

            customActions.AddDataObject(element.Id, CustomisationsHelper.GetCustomisationOs(element.Id), element.Status == ElementStatus.SETTOACTIVE);
        }

        private void SetLoadingPanelVisibility() => LoadingPanelVisibility = !LoadingPanelVisibility;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetVisibleViewTag(string tag) => VisibleViewByTag = tag;

        private async void TextedElementClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var element = args as TextedElement;
                element.ChangeStatus();
                SetCustomAction(element);
                OnPropertyChanged(CustomActionsCounterPropertyName);
            });
        }

        private async Task UpdateIsAvailableAsync()
        {
            await Task.Run(() =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(DataHelper.GitHubApiReleases);
                request.UserAgent = DataHelper.UserAgent;

                try
                {
                    var response = request.GetResponse();
                    debugger.UpdateResponseIsNull(response is null);
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        var serverResponse = reader.ReadToEnd();
                        var release = JsonConvert.DeserializeObject<List<ReleaseDTO>>(serverResponse).FirstOrDefault();
                        debugger.HasRelease(release.tag_name, release.prerelease, release.draft);

                        if (IsNewVersion(release))
                        {
                            debugger.UpdateEntry("The update can be done");
                            SetUpdateAvailableProperty(true);
                            ToastHelper.ShowUpdateToast(currentVersion: DataHelper.Version.ToString(), newVersion: release.tag_name);
                            return;
                        }

                        debugger.UpdateEntry("No update required");
                    }
                }
                catch (Exception e)
                {
                    debugger.Exception("An error occurred while checking for an update", e);
                }
            });
        }

        internal async void InitData()
        {
            if (IsSupportedOs())
            {
                MouseHelper.ShowWaitCursor(show: true);
                //await UpdateIsAvailableAsync();
                await InitTextedElementsAsync();
                SetVisibleViewTag(Tags.ViewPrivacy);
                SetControlsHitTest(hamburgerHitTest: true);
                MouseHelper.ShowWaitCursor(show: false);
                return;
            }

            SetVisibleViewTag(Tags.OsNotSupported);
        }
    }
}