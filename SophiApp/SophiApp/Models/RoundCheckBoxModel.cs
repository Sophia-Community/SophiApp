using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class RoundCheckBoxModel : IUIElementModel, ICheckable, INotifyPropertyChanged
    {
        private string description;
        private string header;
        private bool isChecked = default;
        private bool systemState = default;
        private bool userState = default;

        public RoundCheckBoxModel(JsonDTO json)
        {
            LocalizedDescriptions = Localizator.GetLocalizedDescriptions(json);
            LocalizedHeaders = Localizator.GetLocalizedHeaders(json);
            Id = json.Id;
            Tag = json.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }

        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }

        public bool SystemState
        {
            get => systemState;
            set
            {
                systemState = value;
                OnPropertyChanged("SystemState");
            }
        }

        public string Tag { get; set; }

        public bool UserState
        {
            get => userState;
            set
            {
                userState = value;
                OnPropertyChanged("UserState");
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetLocalizationTo(UILanguage language)
        {
            Header = LocalizedHeaders[language];
            Description = LocalizedDescriptions[language];
        }

        public void SetSystemState()
        {
            SystemState = true;
            IsChecked = true;
        }

        public void SetUserState()
        {
            UserState = !UserState;
            IsChecked = !IsChecked;
        }
    }
}