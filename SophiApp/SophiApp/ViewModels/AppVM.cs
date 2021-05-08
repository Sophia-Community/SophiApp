using Microsoft.Win32;
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
using System.Threading.Tasks;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        //TODO: Check all controls IsEnabled property!

        private const string AdvancedSettingsVisibilityPropertyName = "AdvancedSettingsVisibility";
        private const string TextedElementsChangedCounterPropertyName = "TextedElementsChangedCounter";
        private const string AppThemePropertyName = "AppTheme";
        private const string LocalizationPropertyName = "Localization";
        private const string UpdateAvailablePropertyName = "UpdateAvailable";
        private const string LoadingPanelVisibilityPropertyName = "LoadingPanelVisibility";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private bool advancedSettingsVisibility;
        private LocalizationManager localizationManager;
        private LogManager logManager;
        private IEnumerable<JsonDTO> parsedJson;
        private uint textedElementsChangedCounter;
        private ThemeManager themeManager;
        private bool updateAvailable;
        private bool loadingPanelVisibility;
        private string visibleViewByTag;

        public AppVM()
        {
            InitFields();
            InitCommands();
            //TODO: Uncomment before release
            //UpdateIsAvailability();
            InitTextedElements();
            InitUIContainers();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public RelayCommand AdvancedSettingsClickedCommand { get; private set; }

        public bool AdvancedSettingsVisibility
        {
            get => advancedSettingsVisibility;
            set
            {
                advancedSettingsVisibility = value;
                logManager.AddDateTimeValueString(LogType.ADVANCED_SETTINGS_IS_VISIBLE, $"{value}");
                OnPropertyChanged(AdvancedSettingsVisibilityPropertyName);
            }
        }

        public Theme AppTheme
        {
            get => themeManager.Selected;
            private set
            {
                logManager.AddDateTimeValueString(LogType.THEME_CHANGED, $"{value.Alias}");
                OnPropertyChanged(AppThemePropertyName);
            }
        }

        public RelayCommand AppThemeChangeCommand { get; private set; }
        public List<string> AppThemeList => themeManager.GetNames();
        public RelayCommand ContainerElementClickedCommand { get; private set; }
        public RelayCommand ExportSettingsCommand { get; private set; }
        public RelayCommand HamburgerClickedCommand { get; private set; }
        public RelayCommand HyperLinkClickedCommand { get; private set; }
        public RelayCommand ImportSettingsCommand { get; private set; }

        public Localization Localization
        {
            get => localizationManager.Selected;
            private set
            {
                logManager.AddDateTimeValueString(LogType.APP_LOCALIZATION_CHANGED, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public RelayCommand LocalizationChangeCommand { get; private set; }
        public List<string> LocalizationList => localizationManager.GetNames();
        public RelayCommand SaveDebugLogCommand { get; private set; }
        public RelayCommand SearchClickedCommand { get; private set; }

        public RelayCommand TextedElementClickedCommand { get; private set; }

        public List<BaseTextedElement> TextedElements { get; private set; }

        public List<BaseContainer> UIContainers { get; private set; }

        public uint TextedElementsChangedCounter 
        { 
            get => textedElementsChangedCounter; 
            set
            {
                textedElementsChangedCounter = value;
                OnPropertyChanged(TextedElementsChangedCounterPropertyName);
            }
        }

        public bool UpdateAvailable
        {
            get => updateAvailable;
            set
            {
                updateAvailable = value;
                logManager.AddDateTimeValueString(LogType.UPDATE_AVAILABLE_CHANGED, $"{value}");
                OnPropertyChanged(UpdateAvailablePropertyName);
            }
        }

        public bool LoadingPanelVisibility
        {
            get => loadingPanelVisibility;
            set
            {
                loadingPanelVisibility = value;
                OnPropertyChanged(LoadingPanelVisibilityPropertyName);
            }
        }

        public string VisibleViewByTag
        {
            get => visibleViewByTag;
            set
            {
                visibleViewByTag = value;
                logManager.AddDateTimeValueString(LogType.VISIBLE_VIEW_CHANGED, $"{value}");
                OnPropertyChanged(VisibleViewByTagPropertyName);
            }
        }

        private void AdvancedSettingsClicked(object args) => AdvancedSettingsVisibility = !AdvancedSettingsVisibility;

        private async void AppThemeChangeAsync(object args)
        {
            SetLoadingPanelVisibilityProperty(isVisible: true);
            await ChangeThemeAsync(args as string);
            SetLoadingPanelVisibilityProperty(isVisible: false);
        }

        private async Task ChangeLocalizationAsync(string localizationName)
        {
            await Task.Run(() =>
            {
                var localization = localizationManager.FindName(localizationName);
                TextedElements.ForEach(element => element.SetLocalization(localization.Language));
                UIContainers.ForEach(container => container.SetLocalization(localization.Language));
                localizationManager.Change(localization);
                SetLocalizationProperty(localization);
            });
        }

        private async Task ChangeThemeAsync(string themeName)
        {
            await Task.Run(() =>
            {
                var theme = themeManager.FindName(themeName);
                themeManager.Change(theme);
                SetAppThemeProperty(theme);
            });
        }

        private async void ContainerElementClickedAsync(object args) => await ContainerElementClickedAsync(id: Convert.ToUInt32(args));

        private async Task ContainerElementClickedAsync(uint id)
        {
            await Task.Run(() =>
            {
                var containerId = TextedElements.Where(element => element.Id == id).First().ContainerId;
                TextedElements.Where(element => element.ContainerId == containerId)
                              .ToList()
                              .ForEach(element => element.State = element.Id == id ? UIElementState.SETTOACTIVE : UIElementState.UNCHECKED);
            });
        }

        private void ExportSettings(object args)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDataManager.StartupFolder;
            openFileDialog.ShowDialog();
            //SetVisibleInfoPanelByTagProperty(Tags.InfoPanelLoading);
            //TODO: Export Settings not implemented
            //TODO: Allow export is ChangedElementsCounter > 0
        }

        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void HyperLinkClicked(object args)
        {
            var link = args as string;
            logManager.AddDateTimeValueString(LogType.HYPERLINK_OPEN, link);
            Process.Start(link);
        }

        private void ImportSettings(object args)
        {
            //TODO: Import Settings not implemented
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.InitialDirectory = AppDataManager.StartupFolder;
            openFileDialog.ShowDialog();
        }

        private void InitCommands()
        {
            AdvancedSettingsClickedCommand = new RelayCommand(new Action<object>(AdvancedSettingsClicked));
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChangeAsync));
            ContainerElementClickedCommand = new RelayCommand(new Action<object>(ContainerElementClickedAsync));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClickedAsync));
            HyperLinkClickedCommand = new RelayCommand(new Action<object>(HyperLinkClicked));
            ImportSettingsCommand = new RelayCommand(new Action<object>(ImportSettings));
            ExportSettingsCommand = new RelayCommand(new Action<object>(ExportSettings));
            SaveDebugLogCommand = new RelayCommand(new Action<object>(SaveDebugLogAsync));
            TextedElementClickedCommand = new RelayCommand(new Action<object>(TextedElementClickedAsync));
        }

        private void InitFields()
        {
            logManager = new LogManager();
            localizationManager = new LocalizationManager();
            textedElementsChangedCounter = default;
            themeManager = new ThemeManager();
            visibleViewByTag = Tags.ViewPrivacy;
            loadingPanelVisibility = default;
            updateAvailable = false;
            advancedSettingsVisibility = false;
            parsedJson = Parser.ParseJson(Properties.Resources.UIData);

            logManager.AddKeyValueString(LogType.INIT_APP_LOCALIZATION, $"{Localization.Language}");
            logManager.AddKeyValueString(LogType.INIT_THEME, $"{AppTheme.Alias}");
            logManager.AddKeyValueString(LogType.INIT_VIEW, $"{VisibleViewByTag}");
            logManager.AddSeparator();
        }

        private void InitTextedElements()
        {
            logManager.AddDateTimeValueString(LogType.INIT_TEXTED_ELEMENT_MODELS);
            TextedElements = parsedJson.Where(dto => dto.Type == UIType.TextedElement).Select(dto => AppFabric.CreateTextElementModel(dto)).ToList();
            TextedElements.ForEach(element =>
            {
                element.StateChanged += OnTextedElementStateChanged;
                element.ErrorOccurred += OnTextedElementErrorOccurred;

                element.SetLocalization(Localization.Language);
                element.CurrentStateActionInvoke();
            });
            logManager.AddDateTimeValueString(LogType.DONE_INIT_TEXTED_ELEMENT_MODELS);
            logManager.AddSeparator();
        }

        private void InitUIContainers()
        {
            logManager.AddDateTimeValueString(LogType.INIT_CONTAINERS_MODELS);
            UIContainers = parsedJson.Where(dto => dto.Type == UIType.Container).Select(dto => AppFabric.CreateContainerModel(dto)).ToList();
            UIContainers.ForEach(container =>
            {
                container.Collection = TextedElements.Where(element => element.ContainerId == container.Id).ToList();
                container.SetLocalization(Localization.Language);
                logManager.AddDateTimeValueString(LogType.CONTAINERS_ELEMENT_ID, $"{container.Id}");
            });
            logManager.AddDateTimeValueString(LogType.DONE_INIT_CONTAINERS_MODELS);
            logManager.AddSeparator();
        }

        private bool IsNewVersion(Version currentVersion, string outsideVersion, bool outsidePrerelease, bool outsideDraft)
        {
            return new Version(outsideVersion) > currentVersion && outsidePrerelease == false && outsideDraft == false;
        }

        private async void LocalizationChangeAsync(object args)
        {
            SetLoadingPanelVisibilityProperty(isVisible: true);
            await ChangeLocalizationAsync(args as string);
            SetLoadingPanelVisibilityProperty(isVisible: false);
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void OnTextedElementErrorOccurred(uint id, string target, string message)
        {
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_TARGET, target);
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_MESSAGE, message);
            logManager.AddSeparator();

            var containerId = TextedElements.Where(element => element.Id == id).First().ContainerId;

            if (containerId > 0)
            {
                TextedElements.Where(element => element.ContainerId == containerId).ToList().ForEach(element => element.State = UIElementState.DISABLED);
            }
            else
            {
                TextedElements.Where(element => element.Id == id).First().State = UIElementState.DISABLED;
            }
        }

        private void OnTextedElementStateChanged(uint id, UIElementState state)
        {
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_STATE_CHANGED, $"{state}");
        }

        private async void SaveDebugLogAsync(object args)
        {
            SetLoadingPanelVisibilityProperty(isVisible: true);
            var isSaved = await SaveDebugLogAsync();
            logManager.AddDateTimeValueString(isSaved ? LogType.DEBUG_SAVE_OK : LogType.DEBUG_SAVE_HAS_ERROR);
            SetLoadingPanelVisibilityProperty(isVisible: false);
        }

        private async Task<bool> SaveDebugLogAsync() => await Task<bool>.Run(() => FileManager.Save(logManager.GetLog(), AppDataManager.DebugLogPath));

        private void SearchClickedAsync(object obj)
        {
            //TODO: Search not implemented
        }

        private void SetAppThemeProperty(Theme theme) => AppTheme = theme;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetUpdateAvailableProperty(bool state) => UpdateAvailable = state;

        private void SetLoadingPanelVisibilityProperty(bool isVisible) => LoadingPanelVisibility = isVisible;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;

        private async void TextedElementClickedAsync(object args) => await TextedElementClickedAsync(id: Convert.ToUInt32(args));

        private async Task TextedElementClickedAsync(uint id)
        {
            await Task.Run(() =>
            {
                var element = TextedElements.Where(e => e.Id == id).First();
                element.ChangeState();
                SetTextedElementsChangedCounter(element.State);                
            });
        }

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

        private void UpdateIsAvailability()
        {
            HttpWebRequest request = WebRequest.CreateHttp(AppDataManager.GitHubReleases);
            request.UserAgent = AppDataManager.UserAgent;

            try
            {
                var response = request.GetResponse();
                logManager.AddDateTimeValueString(response is null ? LogType.UPDATE_RESPONSE_NULL : LogType.UPDATE_RESPONSE_OK);
                using (Stream dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    logManager.AddDateTimeValueString(LogType.UPDATE_RESPONSE_LENGTH, $"{responseFromServer.Length}");
                    var release = Parser.ParseJson(responseFromServer).First();
                    logManager.AddDateTimeValueString(LogType.UPDATE_VERSION_FOUND, $"{release.Tag_Name}");
                    logManager.AddDateTimeValueString(LogType.UPDATE_VERSION_IS_PRERELEASE, $"{release.Prerelease}");
                    logManager.AddDateTimeValueString(LogType.UPDATE_VERSION_IS_DRAFT, $"{release.Draft}");
                    var updateRequired = IsNewVersion(currentVersion: AppDataManager.Version, outsideVersion: release.Tag_Name,
                                                             outsidePrerelease: release.Prerelease, outsideDraft: release.Draft);

                    if (updateRequired)
                    {
                        logManager.AddDateTimeValueString(LogType.UPDATE_VERSION_REQUIRED);
                        SetUpdateAvailableProperty(true);
                        return;
                    }

                    logManager.AddDateTimeValueString(LogType.UPDATE_VERSION_NOT_REQUIRED);
                }
            }
            catch (Exception e)
            {
                logManager.AddDateTimeValueString(LogType.UPDATE_HAS_ERROR, e.Message);
            }
        }
    }
}