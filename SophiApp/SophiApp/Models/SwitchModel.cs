using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Models
{
    class SwitchModel : IUIElementModel, INotifyPropertyChanged
    {
        private string description;
        private string header;

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public string Header 
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        public int Id { get; set; }
        public string Tag { get; set; }
        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }
        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }
        
        public SwitchModel(JsonDTO json)
        {
            LocalizedDescriptions = Localizator.GetLocalizedDescriptions(json);
            LocalizedHeaders = Localizator.GetLocalizedHeaders(json);
            Id = json.Id;
            Tag = json.Tag;
        }

        public void SetLocalizationTo(UILanguage language)
        {
            Header = LocalizedHeaders[language];
            Description = LocalizedDescriptions[language];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
