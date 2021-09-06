using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Extensions;
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
using Debugger = SophiApp.Helpers.Debugger;
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
                debugger.AddRecord($"Started applying {customActions.Count} setting(s)");
                var stopwatch = Stopwatch.StartNew();
                SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
                SetLoadingPanelVisibility();

                try
                {
                    customActions.ForEach(action =>
                    {
                        action.Action.Invoke(action.Parameter);
                    });
                }
                catch (Exception e)
                {
                    debugger.AddRecord($"Applying customization action caused an error");
                    debugger.AddRecord($"Error information \"{e.Message}\"");
                }

                customActions.Clear();
                OnPropertyChanged(CustomActionsCounterPropertyName);
                OsHelper.PostMessage();
                OsHelper.Refresh();
                SetLoadingPanelVisibility();
                SetControlsHitTest();
                stopwatch.Stop();
                debugger.AddRecord($"It took {string.Format("{0:N0}", stopwatch.Elapsed.TotalSeconds)} seconds to apply the settings");
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

        private void HamburgerClicked(object args) => SetVisibleViewTag(args as string);

        private async void HyperLinkClickedAsync(object args)
        {
            MouseHelper.ShowWaitCursor(show: true);
            await Task.Run(() =>
            {
                var link = args as string;
                debugger.AddRecord($"Clicked link: \"{link}\"");
                Process.Start(link);
            });
            MouseHelper.ShowWaitCursor(show: false);
        }

        private void InitCommands()
        {
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
            loadingPanelVisibility = false;
            HamburgerHitTest = false;
            ViewsHitTest = true;
            WindowCloseHitTest = true;
            advancedSettingsVisibility = false;
            VisibleViewByTag = Tags.ViewLoading;
            customActions = new List<CustomActionDTO>();
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.AddRecord("Started initialization texted elements");
                var stopwatch = Stopwatch.StartNew();
                TextedElements = JsonConvert.DeserializeObject<IEnumerable<TextedElementDTO>>(Encoding.UTF8.GetString(Properties.Resources.UIData))
                                            .Select(dto => ElementsFabric.CreateTextedElement(dataObject: dto, errorHandler: OnTextedElementErrorAsync,
                                                        statusHandler: OnTextedElementStatusChanged, language: Localization.Language))
                                            .ToList();
                stopwatch.Stop();
                debugger.AddRecord($"The collection initialization took {string.Format("{0:N0}", stopwatch.Elapsed.TotalSeconds)} seconds");
            });
        }

        private bool IsNewVersion(ReleaseDTO dto) => new Version(dto.tag_name) > AppData.Version && !dto.prerelease && !dto.draft;

        private bool IsSupportedOs() => OsHelper.GetProductName().Contains("Windows 10") && OsHelper.GetBuild() >= MinimalOsBuild;

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
                debugger.AddRecord($"An error occured in element {element.Id}");
                debugger.AddRecord($"Error information \"{e.Message}\"");
                debugger.AddRecord($"The class that caused the error \"{e.TargetSite.DeclaringType.FullName}\"");
                debugger.AddRecord($"The method that caused the error \"{e.TargetSite.Name}\"");
                element.Status = ElementStatus.DISABLED;
            });
        }

        private void OnTextedElementStatusChanged(object sender, TextedElement element) => debugger.AddRecord($"The element {element.Id} has changed status to {element.Status}");

        private async void RadioGroupClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var rbutton = args as RadioButton;
                var group = TextedElements.FirstOrDefault(element => element.Id == rbutton.ParentId) as RadioGroup;
                group?.ChildElements.ForEach(child => child.Status = child.Id == rbutton.Id ? ElementStatus.CHECKED : ElementStatus.UNCHECKED);
                SetCustomAction(group);
            });
        }

        private async void ResetTextedElementsStateAsync(object args)
        {
            debugger.AddRecord("Started reset texted elements status");
            var stopwatch = Stopwatch.StartNew();
            SetControlsHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetLoadingPanelVisibility();
            await Task.Run(() =>
            {
                customActions.Clear();
                OnPropertyChanged(CustomActionsCounterPropertyName);
                TextedElements.ForEach(element =>
                {
                    element.GetCustomisationStatus();
                });
            });
            SetLoadingPanelVisibility();
            SetControlsHitTest();
            stopwatch.Stop();
            debugger.AddRecord($"The collection resetting took {string.Format("{0:N0}", stopwatch.Elapsed.TotalSeconds)} seconds");
        }

        private async void SaveDebugLogAsync(object args)
        {
            await Task.Run(() =>
            {
                try
                {
                    debugger.Save(AppData.DebugFilePath);
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
                HttpWebRequest request = WebRequest.CreateHttp(AppData.GitHubApiReleases);
                request.UserAgent = AppData.UserAgent;

                try
                {
                    var response = request.GetResponse();
                    debugger.AddRecord(response is null ? "When checking for an update, no response was received from the update server" : "When checking for an update, a response was received from the update server");
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        var serverResponse = reader.ReadToEnd();
                        var release = JsonConvert.DeserializeObject<List<ReleaseDTO>>(serverResponse).FirstOrDefault();
                        debugger.AddRecord($"New version {release.tag_name} is available");
                        debugger.AddRecord($"Version {release.tag_name} is prerelease: {release.prerelease}");
                        debugger.AddRecord($"Version {release.tag_name} is draft: {release.draft}");

                        if (IsNewVersion(release))
                        {
                            debugger.AddRecord("The update can be done");
                            SetUpdateAvailableProperty(true);
                            Toaster.ShowUpdateToast(currentVersion: AppData.Version.ToString(), newVersion: release.tag_name);
                            return;
                        }

                        debugger.AddRecord("No update required");
                    }
                }
                catch (Exception e)
                {
                    debugger.AddRecord("An error occurred while checking for an update");
                    debugger.AddRecord($"Error information \"{e.Message}\"");
                    debugger.AddRecord($"The class that caused the error \"{e.TargetSite.DeclaringType.FullName}\"");
                    debugger.AddRecord($"The method that caused the error \"{e.TargetSite.Name}\"");
                }
            });
        }

        internal async void InitData()
        {
            if (IsSupportedOs())
            {
                MouseHelper.ShowWaitCursor(show: true);
                //TODO: UpdateIsAvailableAsync - comment before debug, uncomment before release.
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