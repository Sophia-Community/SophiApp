using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Debugger = SophiApp.Helpers.Debugger;
using IContainer = SophiApp.Interfaces.IContainer;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void AdvancedSettingsClicked(object args) => AdvancedSettingsVisibility = !AdvancedSettingsVisibility;

        private async void AppThemeChangeAsync(object args) => await AppThemeChangeAsync(args as string);

        private async Task AppThemeChangeAsync(string themeName)
        {
            await Task.Run(() =>
            {
                var theme = themesHelper.FindName(themeName);
                themesHelper.Change(theme);
                SetAppThemeProperty(theme);
            });
        }

        private async void ExpandingGroupClickedAsync(object args)
        {
            var list = args as List<uint>;
            await ExpandingGroupClickedAsync(elementId: list.First(), containerId: list.Last());
        }

        private async Task ExpandingGroupClickedAsync(uint elementId, uint containerId)
        {
            await Task.Run(() =>
            {
                var element = TextedElements.First(container => container.Id == containerId)
                              .Collection
                              .First(e => e.Id == elementId);
                
                element.ChangeState();
                SetTextedElementsChangedCounter(element.State);
            });
        }

        private async void ExportSettingsAsync(object args) => await ExportSettingsAsync();

        private async Task ExportSettingsAsync()
        {
            //TODO: ExportSettingsAsync Not Implemented
            await Task.Run(() =>
            {
                debugger.Write(DebuggerRecord.INIT_EXPORT_SETTINGS);
                // ...
                debugger.Write();
            });
        }

        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private async void HyperLinkClickedAsync(object args)
        {
            MouseHelper.ShowWaitCursor(show: true);
            await HyperLinkClickedAsync(args as string);
            MouseHelper.ShowWaitCursor(show: false);
        }

        private async Task HyperLinkClickedAsync(string link)
        {
            await Task.Run(() =>
            {
                debugger.Write(DebuggerRecord.HYPERLINK_OPEN, link);
                Process.Start(link);
            });
        }

        private async void ImportSettingsAsync(object args) => await ImportSettingsAsync();

        private async Task ImportSettingsAsync()
        {
            //TODO: ImportSettingsAsync Not Implemented
            await Task.Run(() =>
            {
                debugger.Write(DebuggerRecord.INIT_IMPORT_SETTINGS);
                // ...
                debugger.Write();
            });
        }

        private void InitCommands()
        {
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync));
            TextedElementClickedCommand = new RelayCommand(new Action<object>(TextedElementClickedAsync));
            RadioButtonGroupClickedCommand = new RelayCommand(new Action<object>(RadioButtonGroupClickedAsync));
            ExpandingGroupClickedCommand = new RelayCommand(new Action<object>(ExpandingGroupClickedAsync));
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChangeAsync));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            HyperLinkClickedCommand = new RelayCommand(new Action<object>(HyperLinkClickedAsync));
            ExportSettingsCommand = new RelayCommand(new Action<object>(ExportSettingsAsync));
            ImportSettingsCommand = new RelayCommand(new Action<object>(ImportSettingsAsync));
            AdvancedSettingsClickedCommand = new RelayCommand(new Action<object>(AdvancedSettingsClicked));
            ResetOsToDefaultStateCommand = new RelayCommand(new Action<object>(ResetOsToDefaultStateAsync));
            SaveDebugLogCommand = new RelayCommand(new Action<object>(SaveDebugLogAsync));
        }

        private void InitProps()
        {
            debugger = new Debugger();
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
            IsHitTestVisible = false;
            VisibleViewByTag = Tags.ViewLoading;
            advancedSettingsVisibility = false;

            debugger.Write(DebuggerRecord.LOCALIZATION, $"{Localization.Language}");
            debugger.Write(DebuggerRecord.THEME, $"{AppTheme.Alias}");
            debugger.Write();
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS);
                TextedElements = Parser.ParseJson(Properties.Resources.UIData).Where(dto => dto.Type == UIType.TextedElement)
                                                                              .Select(dto => ElementsFabric.Create(dto))
                                                                              .ToList();

                TextedElements.ForEach(element =>
                {
                    if (element.IsContainer == false)
                    {
                        element.ErrorOccurred += OnTextedElementErrorOccurredAsync;
                        element.StateChanged += OnTextedElementStateChanged;
                        element.CurrentStateAction = ElementsFabric.SetCurrentStateAction(element.Id);
                        element.SystemStateAction = ElementsFabric.SetSystemStateAction(element.Id);
                        element.GetCurrentState();
                    }

                    if (element.ContainerId > 0)
                    {
                        var container = TextedElements.First(c => c.Id == element.ContainerId);
                        container.Collection.Add(element);
                    }
                });

                TextedElements.RemoveAll(element => element.ContainerId > 0);
                debugger.Write();
            });
        }

        private bool IsNewVersion(Version currentVersion, string outsideVersion, bool outsidePrerelease, bool outsideDraft)
        {
            return new Version(outsideVersion) > currentVersion && outsidePrerelease == false && outsideDraft == false;
        }

        private async void LocalizationChangeAsync(object args) => await LocalizationChangeAsync(args as string);

        private async Task LocalizationChangeAsync(string localizationName)
        {
            await Task.Run(() =>
            {
                var localization = localizationsHelper.FindName(localizationName);
                TextedElements.ForEach(element =>
                {
                    if (element is IContainer)
                    {
                        (element as IContainer).SetLocalization(localization.Language);
                        return;
                    }

                    element.SetLocalization(localization.Language);
                });
                localizationsHelper.Change(localization);
                SetLocalizationProperty(localization);
            });
        }

        //TODO: SetLoadingPanelVisibilityProperty(isVisible: true);
        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void OnTextedElementErrorOccurredAsync(uint id, Exception e)
        {
            debugger.Write();
            debugger.Write(DebuggerRecord.ELEMENT_HAS_ERROR, $"{id}");
            debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
            debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
            debugger.Write();
            await OnTextedElementErrorOccurredAsync(id);
        }

        private async Task OnTextedElementErrorOccurredAsync(uint id)
        {
            await Task.Run(() =>
           {
               var element = TextedElements.First(e => e.Id == id);
               element.State = UIElementState.DISABLED;
           });
        }

        private void OnTextedElementStateChanged(uint id, UIElementState state) => debugger.Write(DebuggerRecord.ELEMENT_STATE, $"{id}", $"{state}");

        private async void RadioButtonGroupClickedAsync(object args)
        {
            var list = args as List<uint>;
            await RadioButtonGroupClickedAsync(elementId: list.First(), containerId: list.Last());
        }

        private async Task RadioButtonGroupClickedAsync(uint elementId, uint containerId)
        {
            await Task.Run(() =>
            {
                TextedElements.First(container => container.Id == containerId)
                              .Collection
                              .ForEach(element => element.State = element.Id == elementId
                                                                ? UIElementState.SETTOACTIVE
                                                                : UIElementState.UNCHECKED);
            });
        }

        private async void ResetOsToDefaultStateAsync(object args) => await ResetOsToDefaultStateAsync();

        private async Task ResetOsToDefaultStateAsync()
        {
            await Task.Run(() =>
            {
                //TODO: ResetOsToDefaultStateAsync Not Implemented
                debugger.Write(DebuggerRecord.INIT_RESET_TO_DEFAULT);
                // ...
                debugger.Write();
            });
        }

        private async void SaveDebugLogAsync(object args)
        {
            //TODO: SetLoadingPanelVisibilityProperty(isVisible: true);
            await SaveDebugLogAsync();
            //debugger.Write(isSaved ? DebuggerRecord.DEBUG_SAVED : DebuggerRecord.DEBUG_SAVE_HAS_ERROR);
            //TODO: SetLoadingPanelVisibilityProperty(isVisible: false);
        }

        private async Task SaveDebugLogAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    FileHelper.Save(list: debugger.GetLog(), path: AppData.DebugFilePath);
                    debugger.Write(DebuggerRecord.DEBUG_SAVED);
                }
                catch (Exception e)
                {
                    debugger.Write();
                    debugger.Write(DebuggerRecord.DEBUG_SAVE_HAS_ERROR);
                    debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                    debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
                    debugger.Write(DebuggerRecord.ERROR_METHOD, $"{e.TargetSite.Name}");
                    debugger.Write();
                }
            });
        }

        private async void SearchClickedAsync(object args) => await SearchClickedAsync(args as string);

        private async Task SearchClickedAsync(string search)
        {
            await Task.Run(() =>
            {
                //TODO: SearchClickedAsync not implemented
            });
        }

        //TODO: SetLoadingPanelVisibilityProperty(isVisible: true);
        private void SetAppThemeProperty(Theme theme) => AppTheme = theme;

        private void SetIsHitTestVisible(bool state) => IsHitTestVisible = state;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        //TODO: Clicked command increment or decrement counter ??
        private void SetTextedElementsChangedCounter(UIElementState elementState)
        {
            switch (elementState)
            {
                case UIElementState.SETTOACTIVE:
                case UIElementState.SETTODEFAULT:
                    TextedElementsChangedCounter++;
                    break;

                case UIElementState.CHECKED:
                case UIElementState.UNCHECKED:
                    TextedElementsChangedCounter--;
                    break;

                default:
                    break;
            }
        }

        private async Task SetTextedElementsLocalizationAsync(UILanguage language)
        {
            await Task.Run(() =>
            {
                TextedElements.ForEach(element =>
                {
                    if (element is IContainer)
                    {
                        (element as IContainer).SetLocalization(language);
                        return;
                    }

                    element.SetLocalization(language);
                });
            });
        }

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;

        private async void TextedElementClickedAsync(object args) => await TextedElementClickedAsync(id: Convert.ToUInt32(args));

        private async Task TextedElementClickedAsync(uint id)
        {
            await Task.Run(() =>
            {
                var element = TextedElements.First(e => e.Id == id);
                element.ChangeState();
                SetTextedElementsChangedCounter(element.State);
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
                    debugger.Write(response is null ? DebuggerRecord.UPDATE_RESPONSE_NULL : DebuggerRecord.UPDATE_RESPONSE_OK);
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        debugger.Write(DebuggerRecord.UPDATE_RESPONSE_LENGTH, $"{responseFromServer.Length}");
                        var release = Parser.ParseJson(responseFromServer).First();
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_FOUND, $"{release.Tag_Name}");
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_IS_PRERELEASE, $"{release.Prerelease}");
                        debugger.Write(DebuggerRecord.UPDATE_VERSION_IS_DRAFT, $"{release.Draft}");
                        var updateRequired = IsNewVersion(currentVersion: AppData.Version, outsideVersion: release.Tag_Name,
                                                          outsidePrerelease: release.Prerelease, outsideDraft: release.Draft);

                        if (updateRequired)
                        {
                            debugger.Write(DebuggerRecord.UPDATE_VERSION_REQUIRED);
                            debugger.Write();
                            SetUpdateAvailableProperty(true);
                            Toaster.ShowUpdateToast(currentVersion: AppData.Version.ToString(), newVersion: release.Tag_Name);
                            return;
                        }

                        debugger.Write(DebuggerRecord.UPDATE_VERSION_NOT_REQUIRED);
                        debugger.Write();
                    }
                }
                catch (Exception e)
                {
                    debugger.Write(DebuggerRecord.UPDATE_HAS_ERROR);
                    debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                    debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
                    debugger.Write(DebuggerRecord.ERROR_METHOD, $"{e.TargetSite.Name}");
                    debugger.Write();
                }
            });
        }

        internal async void InitData()
        {
            MouseHelper.ShowWaitCursor(show: true);
            //TODO: Uncomment update function before release.
            //await UpdateIsAvailableAsync();
            await InitTextedElementsAsync();
            await SetTextedElementsLocalizationAsync(Localization.Language);
            SetVisibleViewByTagProperty(Tags.ViewPrivacy);
            SetIsHitTestVisible(true);
            MouseHelper.ShowWaitCursor(show: false);
        }
    }
}