using Newtonsoft.Json;
using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
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
            //TODO: AppVM ApplyingSettingsAsync - need testing and refactoring.
            //debugger.Write(DebuggerRecord.INIT_APPLYING_SETTINGS);
            //debugger.Write(DebuggerRecord.CHANGED_ELEMENTS, $"{TextedElementsChangedCounter}");
            SetHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetLoadingPanelVisibility();
            await ApplyingSettingsAsync();
            //debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS_RESET);
            await ResetTextedElementsStateAsync();
            OsHelper.PostMessage();
            OsHelper.Refresh();
            SetLoadingPanelVisibility();
            SetHitTest();
            //debugger.Write(DebuggerRecord.DONE_APPLYING_SETTINGS);
        }

        private async Task ApplyingSettingsAsync()
        {
            await Task.Run(() =>
            {
                TextedElements.ForEach(element =>
                {
                    //if (element is IContainer container)
                    //{
                    //    container.ChildElements.ForEach(e =>
                    //    {
                    //        if (e.Status == ElementStatus.SETTOACTIVE || e.Status == ElementStatus.SETTODEFAULT)
                    //        {
                    //            e.SetSystemState();
                    //        }
                    //    });

                    //    return;
                    //}

                    //if (element.State == UIElementState.SETTOACTIVE || element.State == UIElementState.SETTODEFAULT)
                    //{
                    //    element.SetSystemState();
                    //}
                });
            });
        }

        private async void AppThemeChangeAsync(object args)
        {
            await Task.Run(() =>
            {
                var name = args as string;
                var theme = themesHelper.Find(name);
                themesHelper.ChangeTheme(theme);
                SetAppSelectedThemeProperty(theme);
            });
        }

        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private async void HyperLinkClickedAsync(object args)
        {
            MouseHelper.ShowWaitCursor(show: true);
            await Task.Run(() =>
            {
                var link = args as string;
                debugger.Write("HYPERLINK_OPEN", link);
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
            debugger = new Debugger();
            localizationsHelper = new LocalizationsHelper();
            loadingPanelVisibility = false;
            themesHelper = new ThemesHelper();
            HamburgerHitTest = false;
            ViewsHitTest = true;
            WindowCloseHitTest = true;
            VisibleViewByTag = Tags.ViewLoading;
            advancedSettingsVisibility = false;
            debugger.InitWrite("LOCALIZATION", $"{ Localization.Language}");
            debugger.InitWrite("THEME", $"{ AppSelectedTheme.Alias}");
            debugger.InitWrite();
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.Write("INIT_ELEMENTS");

                //TODO: TextedElements - for UIData.json modify.

                var file = File.ReadAllText("UIData.json");
                //var bytes = Encoding.UTF8.GetBytes(file);
                TextedElements = JsonConvert.DeserializeObject<IEnumerable<TextedElementDTO>>(file)
                                            .Select(dto => ElementsFabric.CreateTextedElement(dataObject: dto, errorHandler: OnTextedElementErrorAsync,
                                                                                              statusHandler: OnTextedElementStatusChanged, language: Localization.Language))
                                            .ToList();

                debugger.Write("INIT_ELEMENTS_DONE");
                debugger.InitWrite();
            });
        }

        private bool IsNewVersion(ReleaseDto dto) => new Version(dto.tag_name) > AppData.Version && !dto.prerelease && !dto.draft;

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
                debugger.Write("ELEMENT_HAS_ERROR", $"{element.Id}", "ERROR_MESSAGE", $"{e.Message}", "ERROR_CLASS", $"{e.TargetSite.DeclaringType.FullName}");
                element.Status = ElementStatus.DISABLED;
            });
        }

        private void OnTextedElementStatusChanged(object sender, TextedElement element) => debugger.Write("ELEMENT_CHANGE_STATUS", $"{element.Id}", $"{element.Status}");

        private async void RadioGroupClickedAsync(object args)
        {
            await Task.Run(() =>
            {
                var rbutton = args as RadioButton;
                var group = TextedElements.FirstOrDefault(element => element.Id == rbutton.Parent) as RadioGroup;
                group?.ChildElements.ForEach(child => child.Status = child.Id == rbutton.Id ? ElementStatus.CHECKED : ElementStatus.UNCHECKED);
            });
        }

        private async void ResetTextedElementsStateAsync(object args)
        {
            //TODO: ResetTextedElementsStateAsync need refactoring.
            //debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS_RESET);
            SetHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetLoadingPanelVisibility();
            await ResetTextedElementsStateAsync();
            SetLoadingPanelVisibility();
            SetHitTest();
            //debugger.Write(DebuggerRecord.DONE_TEXTED_ELEMENTS_RESET);
        }

        private async Task ResetTextedElementsStateAsync()
        {
            await Task.Run(() =>
            {
                TextedElements.ForEach(element =>
                {
                    if (element is IParentElements parent)
                    {
                        parent.ChildElements.ForEach(child => child.GetCustomisationStatus());

                        if (element is RadioGroup group)
                        {
                            group.SetDefaultId();
                        }
                    }
                    else
                    {
                        element.GetCustomisationStatus();
                    }

                    Thread.Sleep(100); //TODO: AppVM - Thread.Sleep for randomize element state.
                });
            });
        }

        private async void SaveDebugLogAsync(object args)
        {
            await Task.Run(() =>
            {
                try
                {
                    debugger.Save(AppData.DebugFilePath);
                }
                catch (Exception e)
                {
                    //TODO: SaveDebugLogAsync - need refactoring.

                    //debugger.Write(DebuggerRecord.DEBUG_SAVE_HAS_ERROR);
                    //debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                    //debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
                    //debugger.Write(DebuggerRecord.ERROR_METHOD, $"{e.TargetSite.Name}");
                }

                Thread.Sleep(5000);
            });
        }

        private void SetAppSelectedThemeProperty(Theme theme) => AppSelectedTheme = theme;

        private void SetHitTest(bool hamburgerHitTest = true, bool viewsHitTest = true, bool windowCloseHitTest = true)
        {
            HamburgerHitTest = hamburgerHitTest;
            ViewsHitTest = viewsHitTest;
            WindowCloseHitTest = windowCloseHitTest;
        }

        private void SetLoadingPanelVisibility() => LoadingPanelVisibility = !LoadingPanelVisibility;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;

        private async void TextedElementClickedAsync(object args) => await Task.Run(() => (args as TextedElement).ChangeStatus());

        private async Task UpdateIsAvailableAsync()
        {
            await Task.Run(() =>
            {
                HttpWebRequest request = WebRequest.CreateHttp(AppData.GitHubApiReleases);
                request.UserAgent = AppData.UserAgent;

                try
                {
                    var response = request.GetResponse();
                    debugger.Write(response is null ? DebuggerRecord.UPDATE_RESPONSE_NULL : DebuggerRecord.UPDATE_RESPONSE_OK);
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        var serverResponse = reader.ReadToEnd();
                        debugger.Write(DebuggerRecord.UPDATE_RESPONSE_LENGTH, $"{serverResponse.Length}");
                        var release = JsonConvert.DeserializeObject<List<ReleaseDto>>(serverResponse).FirstOrDefault();
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_FOUND, $"{release.tag_name}");
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_IS_PRERELEASE, $"{release.prerelease}");
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_IS_DRAFT, $"{release.draft}");

                        if (IsNewVersion(release))
                        {
                            debugger.Write(DebuggerRecord.UPDATE_VERSION_REQUIRED);
                            SetUpdateAvailableProperty(true);
                            Toaster.ShowUpdateToast(currentVersion: AppData.Version.ToString(), newVersion: release.tag_name);
                            return;
                        }

                        debugger.Write(DebuggerRecord.UPDATE_VERSION_NOT_REQUIRED);
                    }
                }
                catch (Exception e)
                {
                    debugger.Write(DebuggerRecord.UPDATE_HAS_ERROR);
                    debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                    debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
                    debugger.Write(DebuggerRecord.ERROR_METHOD, $"{e.TargetSite.Name}");
                }
            });
        }

        internal async void InitData()
        {
            MouseHelper.ShowWaitCursor(show: true);
            //TODO: UpdateIsAvailableAsync - uncomment before release.
            //await UpdateIsAvailableAsync();
            await InitTextedElementsAsync();
            SetVisibleViewByTagProperty(Tags.ViewPrivacy);
            SetHitTest(hamburgerHitTest: true);
            MouseHelper.ShowWaitCursor(show: false);
        }
    }
}