using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Localization = SophiApp.Commons.Localization;

namespace SophiApp.ViewModels
{
    class AppVM : INotifyPropertyChanged
    {
        private Localizer localizator;
        private Logger logger;

        public string AppName => AppData.Name;

        public Localization Localization { get => localizator.Current; }
        
        public List<string> LocalizationList { get => localizator.GetText(); }

        public List<BaseElement> UIElements { get; set; }

        public List<BaseElement> UIContainers { get; set; }


        public AppVM()
        {
            InitFieldsAndProperties();            
            InitUIElementsAsync();            
            //InitUIContainersAsync();



        }

        private void InitUIElementsAsync()
        {            
            var task = Task.Run(() =>
            {
                logger.DateString(LogType.INIT_MODELS);
                var parsedJson = Parser.ParseJson(Properties.Resources.UIData);
                UIElements = parsedJson.Where(dto => dto.Type == UIType.Element).Select(dto => AppFabric.CreateElementModel(dto)).ToList();
                //TODO: Logger element state
                UIElements.ForEach(model => model.SetLocalization(Localization.Language));
                logger.DateString(LogType.DONE_INIT_MODELS);
            });
            task.Wait();            
        }

        private void InitFieldsAndProperties()
        {
            localizator = new Localizer();
            logger = new Logger();

            logger.ValueString(LogType.INIT_LOCALIZATION, $"{localizator.Current.Language}");
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

    }
}
