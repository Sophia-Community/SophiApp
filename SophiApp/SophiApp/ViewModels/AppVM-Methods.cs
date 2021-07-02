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
                        container.Collection.ForEach(e =>
                        {
                            if (e.State == UIElementState.SETTOACTIVE || e.State == UIElementState.SETTODEFAULT)
                            {
                                e.SetSystemState();
                            }
                        });

                        return;
                    }

                    if (element.State == UIElementState.SETTOACTIVE || element.State == UIElementState.SETTODEFAULT)
                    {
                        element.SetSystemState();
                    }
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
            var list = args as List<uint>;
            await ExpandingGroupClickedAsync(elementId: list.First(), containerId: list.Last());
        }

        private async Task ExpandingGroupClickedAsync(uint elementId, uint containerId)
        {
            await Task.Run(() =>
            {
                var group = TextedElements.First(container => container.Id == containerId) as IContainer;
                var element = group.Collection.First(e => e.Id == elementId);
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

        private void InitProps()
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
                    var bytes = Encoding.UTF8.GetBytes(file);
                    TextedElements = Parser.ParseJson(bytes)
                                           .Where(dto => dto.Type == UIType.TextedElement)
                                           .Select(dto => ElementsFabric.Create(dto))
                                           .ToList();
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "SophiApp has error", MessageBoxButton.OK, MessageBoxImage.Error);
                }

                //TextedElements = Parser.ParseJson(Properties.Resources.UIData).Where(dto => dto.Type == UIType.TextedElement)
                //                                                              .Select(dto => ElementsFabric.Create(dto))
                //                                                              .ToList();

                TextedElements.ForEach(element =>
                {
                    element.SetLocalization(Localization.Language);

                    if (!(element is IContainer))
                    {
                        element.ErrorOccurred += OnTextedElementErrorOccurredAsync;
                        element.StateChanged += OnTextedElementStateChanged;
                        element.CurrentStateAction = ElementsFabric.SetCurrentStateAction(element.Id);
                        element.SystemStateAction = ElementsFabric.SetSystemStateAction(element.Id);
                        element.GetCurrentState();

                        if (element.ContainerId > 0)
                        {
                            var container = TextedElements.First(c => c.Id == element.ContainerId) as IContainer;
                            container.Collection.Add(element);
                        }
                    }
                });

                TextedElements.RemoveAll(element => element.ContainerId > 0);
                TextedElements.Where(element => element is RadioButtonsGroup)
                              .Cast<RadioButtonsGroup>()
                              .ToList()
                              .ForEach(group =>
                              {
                                  group.ErrorOccurred += OnRadioButtonsGroupErrorOccurredAsync;
                                  group.SetDefaultSelectedId();
                              });

                debugger.Write(DebuggerRecord.DONE_TEXTED_ELEMENTS);
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
                TextedElements.ForEach(element => element.SetLocalization(localization.Language));
                localizationsHelper.Change(localization);
                SetLocalizationProperty(localization);
                OnPropertyChanged(AppSelectedThemePropertyName);
                OnPropertyChanged(AppThemesPropertyName);
            });
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void OnRadioButtonsGroupErrorOccurredAsync(uint id, Exception e)
        {
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
                group.State = UIElementState.DISABLED;
            });
        }

        private async void OnTextedElementErrorOccurredAsync(uint id, Exception e)
        {
            debugger.Write(DebuggerRecord.ELEMENT_HAS_ERROR, $"{id}");
            debugger.Write(DebuggerRecord.ERROR_MESSAGE, $"{e.Message}");
            debugger.Write(DebuggerRecord.ERROR_CLASS, $"{e.TargetSite.DeclaringType.FullName}");
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
            await RadioButtonGroupClickedAsync(elementId: list.First(), groupId: list.Last());
        }

        private async Task RadioButtonGroupClickedAsync(uint elementId, uint groupId)
        {
            await Task.Run(() =>
            {
                var group = TextedElements.First(g => g.Id == groupId) as RadioButtonsGroup;
                var element = group.Collection.First(e => e.Id == elementId);
                group.Collection.ForEach(e => e.State = e.Id == elementId ? UIElementState.SETTOACTIVE : UIElementState.UNCHECKED);

                if (element.Id != group.DefaultSelectedId && group.IsSelected == false)
                {
                    SetTextedElementsChangedCounter(UIElementState.SETTOACTIVE);
                    group.IsSelected = true;
                }

                if (element.Id == group.DefaultSelectedId)
                {
                    SetTextedElementsChangedCounter(UIElementState.UNCHECKED);
                    group.IsSelected = false;
                }
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
            await Task.Run(() =>
            {
                TextedElements.ForEach(element =>
                {
                    if (element is IContainer container)
                    {
                        container.Collection
                                 .Where(e => e.State != UIElementState.DISABLED)
                                 .ToList()
                                 .ForEach(e => e.GetCurrentState());

                        if (element is RadioButtonsGroup group)
                        {
                            group.SetDefaultSelectedId();
                        }

                        return;
                    }

                    if (element.State != UIElementState.DISABLED)
                    {
                        element.GetCurrentState();
                    }
                });
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

        private void SetTextedElementsChangedCounter() => TextedElementsChangedCounter = 0;

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
                            SetUpdateAvailableProperty(true);
                            Toaster.ShowUpdateToast(currentVersion: AppData.Version.ToString(), newVersion: release.Tag_Name);
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
            //TODO: UpdateIsAvailableAsync - uncomment update function before release.
            //await UpdateIsAvailableAsync();
            await InitTextedElementsAsync();
            SetVisibleViewByTagProperty(Tags.ViewPrivacy);
            SetHitTest(hamburgerHitTest: true);
            MouseHelper.ShowWaitCursor(show: false);
        }
    }
}