using SophiAppCE.Managers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Models
{
    public class SwitchBarModel : INotifyPropertyChanged
    {
        private bool state = default(bool);
        private string header;
        private string description;

        public bool State
        {
            get => state;
            set
            {
                state = value;
                OnPropertyChanged("State");
            }
        }
        public string Id { get; set; }
        public string Path { get; set; }
        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }        
        public string Type { get; set; }
        public string Sha256 { get; set; }
        public string Tag { get; set; }
        public Dictionary<UiLanguage, string> LocalizedHeader { get; set; }
        public Dictionary<UiLanguage, string> LocalizedDescription { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
