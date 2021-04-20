using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Localization = SophiApp.Helpers.Localization;


namespace SophiApp.ViewModels
{
    class AppVM : INotifyPropertyChanged
    {
        private Localizer localizator;

        public string AppName => AppData.Name;

        public Localization Localization { get => localizator.Current; }
        
        public List<string> LocalizationList { get; }


        public AppVM()
        {
            localizator = new Localizer();
            LocalizationList = localizator.GetText();            
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyChanged) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));

    }
}
