using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace SophiApp.ViewModels
{
    internal partial class AppVM
    {
        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void InitCommands()
        {
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync));
        }

        private async Task InitDataAsync()
        {
            await Task.Run(() =>
            {
                //TODO: Remove before release !!!
                //UpdateIsAvailable();
                InitTextedElements();
                Thread.Sleep(3000);
            });
        }

        private void InitTextedElements()
        {
            debugger.AddRecord(DebuggerRecord.INIT_TEXTED_ELEMENT);
            TextedElements = Parser.ParseJson(Properties.Resources.UIData).Where(dto => dto.Type == UIType.TextedElement)
                                                                          .Select(dto => ElementsFabric.Create(dto))
                                                                          .ToList();

            TextedElements.Where(element => element.IsContainer is false)
                          .ToList()
                          .ForEach(element => 
                          {
                              element.StateChanged += OnElementStateChanged;
                              element.ErrorOccurred += OnElementErrorOccurredAsync;
                              //TODO: element.SetLocalization()
                              element.GetCurrentState();
                          });


            debugger.AddRecord();
        }

        private async void OnElementErrorOccurredAsync(uint id, string target, string message)
        {
            debugger.AddRecord();
            debugger.AddRecord(DebuggerRecord.ELEMENT_HAS_ERROR, $"{id}");
            debugger.AddRecord(DebuggerRecord.ELEMENT_ERROR_TARGET, $"{target}"); //TODO: Test this !!!
            debugger.AddRecord(DebuggerRecord.ELEMENT_ERROR_MESSAGE, $"{message}");
            debugger.AddRecord();
            await ElementErrorOccurredAsync(id);
        }

        private async Task ElementErrorOccurredAsync(uint id)
        {
            await Task.Run(() =>
           {
               
           });
        }

        private void OnElementStateChanged(uint id, UIElementState state) => debugger.AddRecord(DebuggerRecord.ELEMENT_STATE, $"{id}", $"{state}");        

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

        private bool IsNewVersion(Version currentVersion, string outsideVersion, bool outsidePrerelease, bool outsideDraft)
        {
            return new Version(outsideVersion) > currentVersion && outsidePrerelease == false && outsideDraft == false;
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private async void SearchClickedAsync(object args) => await SearchClickedAsync(args as string);

        private async Task SearchClickedAsync(string search)
        {
            await Task.Run(() =>
            {
                //TODO: Search not implemented
            });
        }

        private void SetIsHitTestVisible(bool state) => IsHitTestVisible = state;

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;

        private void UpdateIsAvailable()
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
        }

        internal async void InitData()
        {
            Mouse.OverrideCursor = Cursors.Wait;
            await InitDataAsync();
            SetIsHitTestVisible(true);
            SetVisibleViewByTagProperty(Tags.ViewPrivacy);
            Mouse.OverrideCursor = null;
        }
    }
}