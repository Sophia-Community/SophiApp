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
        private RelayCommand switchBarClickedCommand;
        private Language UILanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper() == nameof(Language.RU) ? Language.RU : Language.EN;

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
        public RelayCommand SwitchBarClickedCommand => switchBarClickedCommand ?? new RelayCommand(SwitchBarClicked);
        private void SwitchBarClicked(object args)
        {
            UInt16 id = Convert.ToUInt16(args);            
            ControlsModelsCollection.Where(m => m.Id == id)
                                    .Select(m => m.ActualState = !m.ActualState)
                                    .ToList();
        }

        private void SelectAll(object args)
        {
            object[] arg = args as object[];
            string tag = arg.First() as string;
            bool state = Convert.ToBoolean(arg.Last());
            ControlsModelsCollection.Where(m => m.Tag == tag && m.State != true && m.ActualState != state)
                                    .Select(m => m.ActualState = state)
                                    .ToList();            
        }
    }
}
