using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private const string AppThemePropertyName = "AppTheme";
        private const string LocalizationPropertyName = "Localization";
        private const string VisibleInfoPanelByTagPropertyName = "VisibleInfoPanelByTag";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private LocalizationManager localizationManager;
        private LogManager logManager;
        private IEnumerable<JsonDTO> parsedJson;
        private ThemeManager themeManager;
        private string visibleInfoPanelByTag;
        private string visibleViewByTag;

        public AppVM()
        {
            InitFields();
            InitCommands();
            InitTextedElementsAsync();
            InitUIContainersAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AppName { get => AppData.Name; }

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

        public RelayCommand HamburgerClickedCommand { get; private set; }

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
        public RelayCommand SearchClickedCommand { get; private set; }
        public List<BaseTextedElement> TextedElements { get; private set; }

        public List<BaseContainer> UIContainers { get; private set; }

        public string VisibleInfoPanelByTag
        {
            get => visibleInfoPanelByTag;
            set
            {
                visibleInfoPanelByTag = value;
                var logType = value != Tags.Empty ? LogType.VISIBLE_INFOPANEL : LogType.HIDE_INFOPANEL;
                logManager.AddDateTimeValueString(logType, $"{value}");
                OnPropertyChanged(VisibleInfoPanelByTagPropertyName);
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

        private void AppThemeChangeAsync(object args)
        {
            SetVisibleInfoPanelByTagProperty(Tags.InfoPanelLoading);
            var task = Task.Run(() =>
            {
                var theme = themeManager.FindName(name: args as string);
                themeManager.Change(theme);
                SetAppThemeProperty(theme);
            });
            task.Wait();
            SetVisibleInfoPanelByTagProperty(Tags.Empty);
        }

        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void InitCommands()
        {
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChangeAsync));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClicked));
        }

        private void InitFields()
        {
            logManager = new LogManager();
            localizationManager = new LocalizationManager();
            themeManager = new ThemeManager();
            visibleViewByTag = Tags.ViewSettings; //TODO: Change to Privacy
            visibleInfoPanelByTag = string.Empty;
            parsedJson = Parser.ParseJson(Properties.Resources.UIData);

            logManager.AddKeyValueString(LogType.INIT_APP_LOCALIZATION, $"{Localization.Language}");
            logManager.AddKeyValueString(LogType.INIT_THEME, $"{AppTheme.Alias}");
            logManager.AddKeyValueString(LogType.INIT_VIEW, $"{VisibleViewByTag}");
        }

        private void InitTextedElementsAsync()
        {
            var task = Task.Run(() =>
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
            });
            task.Wait();
        }

        private void InitUIContainersAsync()
        {
            var task = Task.Run(() =>
            {
                logManager.AddDateTimeValueString(LogType.INIT_CONTAINERS_MODELS);
                UIContainers = parsedJson.Where(dto => dto.Type == UIType.Container).Select(dto => AppFabric.CreateContainerModel(dto)).ToList();
                UIContainers.ForEach(container => container.SetLocalization(Localization.Language));
                logManager.AddDateTimeValueString(LogType.DONE_INIT_CONTAINERS_MODELS);
            });
            task.Wait();
        }

        private void LocalizationChangeAsync(object args)
        {
            SetVisibleInfoPanelByTagProperty(Tags.InfoPanelLoading);
            var task = Task.Run(() =>
            {
                var localization = localizationManager.FindName(text: args as string);
                TextedElements.ForEach(element => element.SetLocalization(localization.Language));
                UIContainers.ForEach(container => container.SetLocalization(localization.Language));
                localizationManager.Change(localization);
                SetLocalizationProperty(localization);
            });
            task.Wait();
            SetVisibleInfoPanelByTagProperty(Tags.Empty);
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void OnTextedElementErrorOccurred(uint id, string target, string message)
        {
            //TODO: Implement error handling in the element
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_TARGET, target);
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_MESSAGE, message);
        }

        private void OnTextedElementStateChanged(uint id, UIElementState state)
        {
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logManager.AddDateTimeValueString(LogType.TEXTED_ELEMENT_STATE_CHANGED, $"{state}");
        }

        private void SearchClicked(object obj)
        {
            //TODO: Search command not Implemented
        }

        private void SetAppThemeProperty(Theme theme) => AppTheme = theme;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;

        private void SetVisibleInfoPanelByTagProperty(string infoPanelTag) => VisibleInfoPanelByTag = infoPanelTag;

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;
    }
}