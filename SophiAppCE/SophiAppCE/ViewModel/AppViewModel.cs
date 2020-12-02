using SophiAppCE.Common;
using SophiAppCE.Helpers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace SophiAppCE.ViewModel
{
    class AppViewModel : INotifyPropertyChanged
    {
        private readonly Dictionary<Language, ResourceDictionary> LocalizedDictionaries = new Dictionary<Language, ResourceDictionary>()
        {
            { Language.RU, new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/RU.xaml", UriKind.Absolute)} },
            { Language.EN, new ResourceDictionary() { Source = new Uri("pack://application:,,,/Localization/EN.xaml", UriKind.Absolute)} }
        };
        private RelayCommand selectAllCommand;
        private RelayCommand controlClickedCommand;
        private RelayCommand changePageCommand;
        private Language UILanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == nameof(Language.RU) ? Language.RU : Language.EN;
        private UInt16 activeControlsCounter = default(UInt16);
        private string activePage = Tags.Privacy;

        public ObservableCollection<ControlModel> ControlsModelsCollection { get; set; }

        public AppViewModel()
        {
            SetUiLanguageTo(UILanguage);
            ControlsModelsCollectionFilling();
        }

        private void SetUiLanguageTo(Language language)
        {
            Application.Current.Resources.MergedDictionaries.Add(LocalizedDictionaries[language]);
        }

        public UInt16 ActiveControlsCounter
        {           
            get => activeControlsCounter;
            private set 
            {
                activeControlsCounter = value;
                OnPropertyChanged("ActiveControlsCounter");
            }
        }

        public string ActivePage
        {
            get => activePage;
            private set
            {
                activePage = value;
                OnPropertyChanged("ActivePage");
            }
        }
        
        private void ChangeActiveControlsCounter(bool value)
        {
            if (value)
                ActiveControlsCounter++;
            else
                ActiveControlsCounter--;
        }

        private void ControlsModelsCollectionFilling()
        {
            IEnumerable<JsonData> jsons = Parser.ParseJson();
            IEnumerable<ControlModel> models = ControlsFabric.Create(jsonData: jsons, language: UILanguage);
            ControlsModelsCollection = new ObservableCollection<ControlModel>(models);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }

        public RelayCommand SelectAllCommand => selectAllCommand ?? new RelayCommand(SelectAll);
        public RelayCommand ControlClickedCommand => controlClickedCommand ?? new RelayCommand(ControlClicked);

        public RelayCommand ChangePageCommand => changePageCommand ?? new RelayCommand(ChangePage);

        private void ChangePage(object args) => ActivePage = args as string;

        private void ControlClicked(object args)
        {
            UInt16 id = Convert.ToUInt16(args);
            ControlModel controlModel = ControlsModelsCollection.Where(m => m.Id == id).First();
            controlModel.ChangeActualState();            
            ChangeActiveControlsCounter(controlModel.ActualState);            
        }

        private void SelectAll(object args)
        {
            object[] arg = args as object[];
            string tag = arg.First() as string;
            bool state = Convert.ToBoolean(arg.Last());
            ControlsModelsCollection.Where(m => m.Tag == tag && m.State != true && m.ActualState != state)
                                    .ToList()
                                    .ForEach(m =>
                                    {
                                        m.ChangeActualState();
                                        ChangeActiveControlsCounter(m.ActualState);
                                    });                                             
        }
    }
}
