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
        public string HeaderEn { get; set; }
        public string HeaderRu { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionRu { get; set; }
        public string Type { get; set; }
        public string Sha256 { get; set; }
        public string Tag { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
