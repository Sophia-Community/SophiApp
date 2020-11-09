using SophiAppCE.Common;
using SophiAppCE.Helpers;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.ViewModel
{
    class AppViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<BaseModel> ControlsModelsCollection { get; set; }
        public AppViewModel()
        {
            ControlsModelsCollectionFilling();
        }

        private void ControlsModelsCollectionFilling()
        {
            IEnumerable<JsonData> jsonData = Parser.ParseJson();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyChanged)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyChanged));
        }
    }
}
