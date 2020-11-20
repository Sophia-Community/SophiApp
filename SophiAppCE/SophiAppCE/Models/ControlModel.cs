using SophiAppCE.Common;
using SophiAppCE.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Models
{
    class ControlModel : INotifyPropertyChanged
    {
        private bool state = false;
        private bool actualState = false;
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

        public bool ActualState
        {
            get => actualState;
            set
            {
                actualState = value;
                OnPropertyChanged("ActualState");
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
        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public UInt16 Id { get; set; }
        public ControlsType Type { get; set; }
        public string Tag { get; set; }

        public Dictionary<Language, string> LocalizedHeader { get; set; }

        public Dictionary<Language, string> LocalizedDescription { get; set; }

        public void ChangeActualState()
        {
            ActualState = !ActualState;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
