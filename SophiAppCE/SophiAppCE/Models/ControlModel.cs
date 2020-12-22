using SophiAppCE.Common;
using SophiAppCE.Helpers;
using SophiAppCE.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiAppCE.Models
{
    class ControlModel : INotifyPropertyChanged
    {
        private bool state;
        private bool actualState = false;
        private string header;
        private string description;
        private bool isChanged = false;

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

        public IAction Action { get; set; }

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

        public bool IsChanged
        {
            get => isChanged;
            set
            {
                isChanged = value;
                OnPropertyChanged("IsChanged");
            }
        }

        public UInt16 Id { get; set; }
        
        public string Tag { get; set; }
        
        public Dictionary<Language, string> LocalizedHeader { get; set; }

        public Dictionary<Language, string> LocalizedDescription { get; set; }

        public void ChangeState()
        {
            ActualState = !ActualState;
            IsChanged = !IsChanged;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
