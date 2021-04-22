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

        public AppVM()
        {
            InitFieldsAndProperties();
            InitTextedElementsAsync();
            //InitUIContainersAsync();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public string AppName => AppData.Name;

        public Localization Localization { get => localizator.Current; }

        public List<string> LocalizationList { get => localizator.GetText(); }

        public List<BaseTextedElement> TextedElements { get; set; }

        public List<BaseTextedElement> UIContainers { get; set; }

        private void InitFieldsAndProperties()
        {
            localizator = new Localizer();
            logger = new Logger();

            logger.AddKeyValueString(LogType.INIT_APP_LOCALIZATION, $"{localizator.Current.Language}");
        }

        private void InitTextedElementsAsync()
        {
            var task = Task.Run(() =>
            {
                logger.AddDateTimeValueString(LogType.INIT_TEXTED_ELEMENT_MODELS);
                var parsedJson = Parser.ParseJson(Properties.Resources.UIData);
                TextedElements = parsedJson.Where(dto => dto.Type == UIType.TextedElement).Select(dto => AppFabric.CreateTextElementModel(dto)).ToList();
                TextedElements.ForEach(model =>
                {
                    model.StateChanged += OnTextedElementStateChanged;
                    model.ErrorOccurred += OnTextedElementErrorOccurred;

                    model.SetLocalization(Localization.Language);
                    model.CurrentStateActionInvoke();
                });
                logger.AddDateTimeValueString(LogType.DONE_INIT_TEXTED_ELEMENT_MODELS);
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