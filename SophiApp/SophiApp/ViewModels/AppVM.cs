using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private const string AppThemePropertyName = "AppTheme";
        private const string LocalizationPropertyName = "Localization";
        private const string VisibleViewByTagPropertyName = "VisibleViewByTag";
        private Localizer localizator;
        private Logger logger;
        private IEnumerable<JsonDTO> parsedJson;
        private ThemeManager themeManager;
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
                logger.AddDateTimeValueString(LogType.THEME_CHANGED, $"{value.Alias}");
                OnPropertyChanged(AppThemePropertyName);
            }
        }

        public RelayCommand AppThemeChangeCommand { get; private set; }
        public List<string> AppThemeList => themeManager.GetNames();

        public Localization Localization
        {
            get => localizator.Selected;
            private set
            {
                logger.AddDateTimeValueString(LogType.APP_LOCALIZATION_CHANGED, $"{value.Language}");
                OnPropertyChanged(LocalizationPropertyName);
            }
        }

        public RelayCommand LocalizationChangeCommand { get; private set; }

        public RelayCommand SearchClickedCommand { get; private set; }

        public RelayCommand HamburgerClickedCommand { get; private set; }
        public List<string> LocalizationList => localizator.GetNames();

        public List<BaseTextedElement> TextedElements { get; private set; }

        public List<BaseContainer> UIContainers { get; private set; }

        public string VisibleViewByTag
        {
            get => visibleViewByTag;
            set
            {
                visibleViewByTag = value;
                logger.AddDateTimeValueString(LogType.VISIBLE_VIEW_CHANGED, $"{value}");
                OnPropertyChanged(VisibleViewByTagPropertyName);
            }
        }

        private void AppThemeChange(object args)
        {
            //TODO: Show loading panel
            var theme = themeManager.FindName(name: args as string);
            themeManager.Change(theme);
            SetAppThemeProperty(theme);
        }

        private void InitCommands()
        {
            AppThemeChangeCommand = new RelayCommand(new Action<object>(AppThemeChange));
            LocalizationChangeCommand = new RelayCommand(new Action<object>(LocalizationChangeAsync));
            HamburgerClickedCommand = new RelayCommand(new Action<object>(HamburgerClicked));
            SearchClickedCommand = new RelayCommand(new Action<object>(SearchClicked));
            
        }

        private void HamburgerClicked(object args) => SetVisibleViewByTagProperty(args as string);

        private void SetVisibleViewByTagProperty(string tag) => VisibleViewByTag = tag;
        

        private void SearchClicked(object obj)
        {
            //TODO: Search command not Implemented
        }

        private void InitFields()
        {
            logger = new Logger();
            localizator = new Localizer();
            themeManager = new ThemeManager();
            visibleViewByTag = Tags.ViewSettings; //TODO: Change to Privacy
            parsedJson = Parser.ParseJson(Properties.Resources.UIData);

            logger.AddKeyValueString(LogType.INIT_APP_LOCALIZATION, $"{Localization.Language}");
            logger.AddKeyValueString(LogType.INIT_THEME, $"{AppTheme.Alias}");
            logger.AddKeyValueString(LogType.INIT_VIEW, $"{VisibleViewByTag}");
        }

        private void InitTextedElementsAsync()
        {
            var task = Task.Run(() =>
            {
                logger.AddDateTimeValueString(LogType.INIT_TEXTED_ELEMENT_MODELS);
                TextedElements = parsedJson.Where(dto => dto.Type == UIType.TextedElement).Select(dto => AppFabric.CreateTextElementModel(dto)).ToList();
                TextedElements.ForEach(element =>
                {
                    element.StateChanged += OnTextedElementStateChanged;
                    element.ErrorOccurred += OnTextedElementErrorOccurred;

                    element.SetLocalization(Localization.Language);
                    element.CurrentStateActionInvoke();
                });
                logger.AddDateTimeValueString(LogType.DONE_INIT_TEXTED_ELEMENT_MODELS);
            });
            task.Wait();
        }

        private void InitUIContainersAsync()
        {
            var task = Task.Run(() =>
            {
                logger.AddDateTimeValueString(LogType.INIT_CONTAINERS_MODELS);
                UIContainers = parsedJson.Where(dto => dto.Type == UIType.Container).Select(dto => AppFabric.CreateContainerModel(dto)).ToList();
                UIContainers.ForEach(container => container.SetLocalization(Localization.Language));
                logger.AddDateTimeValueString(LogType.DONE_INIT_CONTAINERS_MODELS);
            });
            task.Wait();
        }

        private void LocalizationChangeAsync(object args)
        {
            var task = Task.Run(() =>
            {
                //TODO: Show loading panel
                var localization = localizator.FindName(text: args as string);
                TextedElements.ForEach(element => element.SetLocalization(localization.Language));
                UIContainers.ForEach(container => container.SetLocalization(localization.Language));
                localizator.Change(localization);
                SetLocalizationProperty(localization);
            });
            task.Wait();
        }

        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

        private void OnTextedElementErrorOccurred(uint id, string target, string message)
        {
            //TODO: Implement error handling in the element
            logger.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logger.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_TARGET, target);
            logger.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ERROR_MESSAGE, message);
        }

        private void OnTextedElementStateChanged(uint id, UIElementState state)
        {
            logger.AddDateTimeValueString(LogType.TEXTED_ELEMENT_ID, $"{id}");
            logger.AddDateTimeValueString(LogType.TEXTED_ELEMENT_STATE_CHANGED, $"{state}");
        }

        private void SetAppThemeProperty(Theme theme) => AppTheme = theme;

        private void SetLocalizationProperty(Localization localization) => Localization = localization;
    }
}