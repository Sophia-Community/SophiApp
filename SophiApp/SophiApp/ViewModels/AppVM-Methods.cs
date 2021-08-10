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
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Debugger = SophiApp.Helpers.Debugger;
using IContainer = SophiApp.Interfaces.IContainer;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void AdvancedSettingsClicked(object args) => AdvancedSettingsVisibility = !AdvancedSettingsVisibility;

        private async void ApplyingSettingsAsync(object args)
        {
            //TODO: AppVM ApplyingSettingsAsync - need testing and refactoring.
            debugger.Write(DebuggerRecord.INIT_APPLYING_SETTINGS);
            debugger.Write(DebuggerRecord.CHANGED_ELEMENTS, $"{TextedElementsChangedCounter}");
            SetHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetTextedElementsChangedCounter();
            SetLoadingPanelVisibility();
            await ApplyingSettingsAsync();
            debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS_RESET);
            await ResetTextedElementsStateAsync();
            OsHelper.PostMessage();
            OsHelper.Refresh();
            SetLoadingPanelVisibility();
            SetHitTest();
            debugger.Write(DebuggerRecord.DONE_APPLYING_SETTINGS);
        }

        private async Task ApplyingSettingsAsync()
        {
            await Task.Run(() =>
            {
                TextedElements.ForEach(element =>
                {
                    if (element is IContainer container)
                    {
                        container.ChildElements.ForEach(e =>
                        {
                            if (e.Status == ElementStatus.SETTOACTIVE || e.Status == ElementStatus.SETTODEFAULT)
                            {
                                e.SetSystemState();
                            }
                        });

                        return;
                    }

                    //if (element.State == UIElementState.SETTOACTIVE || element.State == UIElementState.SETTODEFAULT)
                    //{
                    //    element.SetSystemState();
                    //}
                });
            });
        }

        private async void AppThemeChangeAsync(object args) => await AppThemeChangeAsync(args as string);

        private async Task AppThemeChangeAsync(string name)
        {
            await Task.Run(() =>
            {
                var theme = themesHelper.Find(name);
                themesHelper.ChangeTheme(theme);
                SetAppSelectedThemeProperty(theme);
            });
        }

        private async void ExpandingGroupClickedAsync(object args)
        {
            //TODO: AppVM ExpandingGroupClickedAsync - need testing and refactoring.
            var list = args as List<uint>;
            await ExpandingGroupClickedAsync(elementId: list.First(), containerId: list.Last());
        }

        private async Task ExpandingGroupClickedAsync(uint elementId, uint containerId)
        {
            await Task.Run(() =>
            {
                var group = TextedElements.First(container => container.Id == containerId) as IContainer;
                var element = group.ChildElements.First(e => e.Id == elementId);
                element.ChangeState();
                SetTextedElementsChangedCounter(element.Status);
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
            debugger.Write(DebuggerRecord.LOCALIZATION, $"{Localization.Language}");
            debugger.Write(DebuggerRecord.THEME, $"{AppSelectedTheme.Alias}");
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS);

                //TODO: TextedElements - for UIData.json modify.

                try
                {
                    var file = File.ReadAllText("UIData.json");
                    //var bytes = Encoding.UTF8.GetBytes(file);
                    TextedElements = JsonConvert.DeserializeObject<IEnumerable<JsonGuiDto>>(file)
                                                .Select(dto => ElementsFabric.CreateTextedElement(dto))
                                                .ToList();

                    TextedElements.ForEach(element =>
                    {
                        element.StatusChanged += OnTextedElementStateChanged;
                        element.ErrorOccurred += OnTextedElementErrorOccurredAsync;
                        element.ChangeLanguage(Localization.Language);
                        Thread.Sleep(50); //TODO: AppVM - Thread.Sleep for randomize element state.
                        element.GetCustomisation();
                    });
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "SophiApp has error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                debugger.Write(DebuggerRecord.DONE_TEXTED_ELEMENTS);
            });
        }

        private bool IsNewVersion(ReleaseDto dto) => new Version(dto.tag_name) > AppData.Version && dto.prerelease == false && dto.draft == false;

        private async void LocalizationChangeAsync(object args) => await LocalizationChangeAsync(args as string);

        private async Task LocalizationChangeAsync(string localizationName)
        {
            await Task.Run(() =>
            {
                var localization = localizationsHelper.FindName(localizationName);
                TextedElements.ForEach(element => element.ChangeLanguage(localization.Language));
                localizationsHelper.Change(localization);
                SetLocalizationProperty(localization);
                OnPropertyChanged(AppSelectedThemePropertyName);
                OnPropertyChanged(AppThemesPropertyName);
            });
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void OnRadioButtonsGroupErrorOccurredAsync(uint id, Exception e)
        {
            //TODO: AppVM OnRadioButtonsGroupErrorOccurredAsync - need testing and refactoring.
            debugger.Write(DebuggerRecord.ELEMENT_HAS_ERROR, $"{id}");
            debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
            debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
            await OnRadioButtonsGroupErrorOccurredAsync(id);
        }

        private async Task OnRadioButtonsGroupErrorOccurredAsync(uint id)
        {
            await Task.Run(() =>
            {
                var group = TextedElements.First(element => element.Id == id);
                //group.State = UIElementState.DISABLED;
            });
        }

        private async void OnTextedElementErrorOccurredAsync(TextedElement element, Exception e)
        {
            await Task.Run(() =>
            {
                element.Status = ElementStatus.DISABLED;
                debugger.Write(DebuggerRecord.ELEMENT_HAS_ERROR, $"{element.Id}");
                debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
            });
        }

        private void OnTextedElementStateChanged(object sender, TextedElement e) => debugger.Write(DebuggerRecord.ELEMENT_STATE, $"{e.Id}", $"{e.Status}");

        private async void RadioButtonGroupClickedAsync(object args)
        {
            var list = args as List<uint>;
            await RadioButtonGroupClickedAsync(elementId: list.First(), groupId: list.Last());
        }

        private async Task RadioButtonGroupClickedAsync(uint elementId, uint groupId)
        {
            await Task.Run(() =>
            {
                //TODO: AppVM RadioButtonGroupClickedAsync - need testing and refactoring.
                //var group = TextedElements.First(g => g.Id == groupId) as RadioButtonsGroup;
                //var element = group.ChildElements.First(e => e.Id == elementId);
                //group.ChildElements.ForEach(e => e.State = e.Id == elementId ? UIElementState.SETTOACTIVE : UIElementState.UNCHECKED);

                //if (element.Id != group.DefaultSelectedId && group.IsSelected == false)
                //{
                //    SetTextedElementsChangedCounter(UIElementState.SETTOACTIVE);
                //    group.IsSelected = true;
                //}

                //if (element.Id == group.DefaultSelectedId)
                //{
                //    SetTextedElementsChangedCounter(UIElementState.UNCHECKED);
                //    group.IsSelected = false;
                //}
            });
        }

        private async void ResetTextedElementsStateAsync(object args)
        {
            debugger.Write(DebuggerRecord.INIT_TEXTED_ELEMENTS_RESET);
            SetHitTest(hamburgerHitTest: false, viewsHitTest: false, windowCloseHitTest: false);
            SetTextedElementsChangedCounter();
            SetLoadingPanelVisibility();
            await ResetTextedElementsStateAsync();
            SetLoadingPanelVisibility();
            SetHitTest();
            debugger.Write(DebuggerRecord.DONE_TEXTED_ELEMENTS_RESET);
        }

        private async Task ResetTextedElementsStateAsync()
        {
            //TODO: AppVM ResetTextedElementsStateAsync - need testing and refactoring.
            await Task.Run(() =>
            {
                //TextedElements.ForEach(element =>
                //{
                //    if (element is IContainer container)
                //    {
                //        container.ChildElements
                //                 .Where(e => e.State != UIElementState.DISABLED)
                //                 .ToList()
                //                 .ForEach(e => e.GetCurrentState());

                //        if (element is RadioButtonsGroup group)
                //        {
                //            //group.SetDefaultSelectedId();
                //        }

                //        return;
                //    }

                //    if (element.State != UIElementState.DISABLED)
                //    {
                //        element.GetCurrentState();
                //    }
                //});
            });
        }

        private async void SaveDebugLogAsync(object args) => await SaveDebugLogAsync();

        private async Task SaveDebugLogAsync()
        {
            await Task.Run(() =>
            {
                try
                {
                    FileHelper.Save(list: debugger.GetLog(), path: AppData.DebugFilePath);
                }
                catch (Exception e)
                {
                    debugger.Write(DebuggerRecord.DEBUG_SAVE_HAS_ERROR);
                    debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
                    debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
                    debugger.Write(DebuggerRecord.ERROR_METHOD, $"{e.TargetSite.Name}");
                }

                Thread.Sleep(5000);
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

        private void SetAppSelectedThemeProperty(Theme theme) => AppSelectedTheme = theme;

        private void SetHitTest(bool hamburgerHitTest = true, bool viewsHitTest = true, bool windowCloseHitTest = true)
        {
            HamburgerHitTest = hamburgerHitTest;
            ViewsHitTest = viewsHitTest;
            WindowCloseHitTest = windowCloseHitTest;
        }

        private void SetLoadingPanelVisibility() => LoadingPanelVisibility = !LoadingPanelVisibility;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetTextedElementsChangedCounter(ElementStatus elementState)
        {
            switch (elementState)
            {
                case ElementStatus.SETTOACTIVE:
                case ElementStatus.SETTODEFAULT:
                    TextedElementsChangedCounter++;
                    break;

                case ElementStatus.CHECKED:
                case ElementStatus.UNCHECKED:
                    TextedElementsChangedCounter--;
                    break;

                default:
                    break;
            }
        }

        private void SetTextedElementsChangedCounter() => TextedElementsChangedCounter = 0;

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;

        private async void TextedElementClickedAsync(object args) => await TextedElementClickedAsync(id: Convert.ToUInt32(args));

        private async Task TextedElementClickedAsync(uint id)
        {
            await Task.Run(() =>
            {
                //TODO: AppVM TextedElementClickedAsync need testing and refactoring.
                var element = TextedElements.First(e => e.Id == id);
                //element.ChangeState();
                //SetTextedElementsChangedCounter(element.State);
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