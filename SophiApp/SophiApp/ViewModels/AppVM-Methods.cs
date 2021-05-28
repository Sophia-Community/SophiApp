using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using IContainer = SophiApp.Interfaces.IContainer;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void InitCommands()
        {
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync));
            TextedElementClickedCommand = new RelayCommand(new Action<object>(TextedElementClickedAsync));
        }

        private void InitProps()
        {
            debugger = new Debugger();
            localizationsHelper = new LocalizationsHelper();
            themesHelper = new ThemesHelper();
            IsHitTestVisible = false;
            VisibleViewByTag = Tags.ViewLoading;

            debugger.AddRecord(DebuggerRecord.LOCALIZATION, $"{Localization.Language}");
            debugger.AddRecord(DebuggerRecord.THEME, $"{AppTheme.Alias}");
            debugger.AddRecord();
        }

        private async Task InitTextedElementsAsync()
        {
            await Task.Run(() =>
            {
                debugger.AddRecord(DebuggerRecord.INIT_TEXTED_ELEMENT);
                TextedElements = Parser.ParseJson(Properties.Resources.UIData).Where(dto => dto.Type == UIType.TextedElement)
                                                                              .Select(dto => ElementsFabric.Create(dto))
                                                                              .ToList();

                TextedElements.ForEach(element =>
                {
                    if (element.IsContainer == false)
                    {
                        element.ErrorOccurred += OnTextedElementErrorOccurredAsync;
                        element.StateChanged += OnTextedElementStateChanged;
                        element.CurrentStateAction = ElementsFabric.GetCurrentStateAction(element.Id);
                        element.SystemStateAction = ElementsFabric.GetSystemStateAction(element.Id);
                        element.GetCurrentState();
                    }

                    if (element.ContainerId > 0)
                    {
                        var container = TextedElements.First(c => c.Id == element.ContainerId);
                        container.Collection.Add(element);
                    }
                });

                TextedElements.RemoveAll(element => element.ContainerId > 0);
                debugger.AddRecord();
            });
        }

        private bool IsNewVersion(Version currentVersion, string outsideVersion, bool outsidePrerelease, bool outsideDraft)
        {
            return new Version(outsideVersion) > currentVersion && outsidePrerelease == false && outsideDraft == false;
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void OnTextedElementErrorOccurredAsync(uint id, string target, string message)
        {
            debugger.AddRecord();
            debugger.AddRecord(DebuggerRecord.ELEMENT_HAS_ERROR, $"{id}");
            debugger.AddRecord(DebuggerRecord.ELEMENT_ERROR_TARGET, $"{target}");
            debugger.AddRecord(DebuggerRecord.ELEMENT_ERROR_MESSAGE, $"{message}");
            debugger.AddRecord();
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

        private void OnTextedElementStateChanged(uint id, UIElementState state) => debugger.AddRecord(DebuggerRecord.ELEMENT_STATE, $"{id}", $"{state}");

        private async void SearchClickedAsync(object args) => await SearchClickedAsync(args as string);

        private async Task SearchClickedAsync(string search)
        {
            await Task.Run(() =>
            {
                //TODO: Search not implemented
            });
        }

        private void SetIsHitTestVisible(bool state) => IsHitTestVisible = state;

        private async Task SetPause(int milliseconds) => await Task.Run(() => Thread.Sleep(milliseconds));

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
                    debugger.AddRecord(response is null ? DebuggerRecord.UPDATE_RESPONSE_NULL : DebuggerRecord.UPDATE_RESPONSE_OK);
                    using (Stream dataStream = response.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(dataStream);
                        string responseFromServer = reader.ReadToEnd();
                        debugger.AddRecord(DebuggerRecord.UPDATE_RESPONSE_LENGTH, $"{responseFromServer.Length}");
                        var release = Parser.ParseJson(responseFromServer).First();
                        debugger.AddRecord(DebuggerRecord.UPDATE_VERSION_FOUND, $"{release.Tag_Name}");
                        debugger.AddRecord(DebuggerRecord.UPDATE_VERSION_IS_PRERELEASE, $"{release.Prerelease}");
                        debugger.AddRecord(DebuggerRecord.UPDATE_VERSION_IS_DRAFT, $"{release.Draft}");
                        var updateRequired = IsNewVersion(currentVersion: AppData.Version, outsideVersion: release.Tag_Name,
                                                          outsidePrerelease: release.Prerelease, outsideDraft: release.Draft);

                        if (updateRequired)
                        {
                            debugger.AddRecord(DebuggerRecord.UPDATE_VERSION_REQUIRED);
                            debugger.AddRecord();
                            SetUpdateAvailableProperty(true);
                            Toaster.ShowUpdateToast(currentVersion: AppData.Version.ToString(), newVersion: release.Tag_Name);
                            return;
                        }

                        debugger.AddRecord(DebuggerRecord.UPDATE_VERSION_NOT_REQUIRED);
                        debugger.AddRecord();
                    }
                }
                catch (Exception e)
                {
                    debugger.AddRecord(DebuggerRecord.UPDATE_HAS_ERROR, e.Message);
                    debugger.AddRecord();
                }
            });
        }

        internal async void InitData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            //TODO: Remove before release !!!
            //await UpdateIsAvailableAsync();
            await InitTextedElementsAsync();
            await SetTextedElementsLocalizationAsync(Localization.Language);
            await SetPause(2000);
            SetVisibleViewByTagProperty(Tags.ViewPrivacy);
            SetIsHitTestVisible(true);
            Mouse.OverrideCursor = null;
        }
    }
}