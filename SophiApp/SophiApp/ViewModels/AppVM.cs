using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    internal class AppVM : INotifyPropertyChanged
    {
        private Localizer localizator;
        private Logger logger;
        private IEnumerable<JsonDTO> parsedJson;
        private string visibleViewByTag;
        public const string VisibleViewByTagPropertyName = "VisibleViewByTag";

        public AppVM()
        {
            InitFields();
            InitTextedElementsAsync();
            InitUIContainersAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AppName { get => AppData.Name; }

        public Localization Localization { get => localizator.Current; }

        public List<string> LocalizationList { get => localizator.GetText(); }

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

        private void InitFields()
        {
            logger = new Logger();
            localizator = new Localizer();
            parsedJson = Parser.ParseJson(Properties.Resources.UIData);
            logger.AddKeyValueString(LogType.INIT_APP_LOCALIZATION, $"{localizator.Current.Language}");
            logger.AddKeyValueString(LogType.INIT_VIEW, $"{visibleViewByTag = Tags.ViewSettings}"); //TODO: Change to Privacy
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
    }
}