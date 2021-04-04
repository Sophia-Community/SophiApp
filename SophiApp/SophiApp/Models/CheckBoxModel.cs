using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace SophiApp.Models
{
    internal class CheckBoxModel : IUIElementModel, INotifyPropertyChanged
    {
        private string description;
        private string header;
        private bool userState = default;
        private bool systemState = default;
        private bool isChecked = default;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        public bool SystemState
        {
            get => systemState;
            set
            {
                systemState = value;
                OnPropertyChanged("SystemState");
            }
        }

        public bool UserState
        {
            get => userState;
            set
            {
                userState = value;
                OnPropertyChanged("UserState");
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

        public CheckBoxModel(JsonDTO json)
        {
            LocalizedDescriptions = Localizator.GetLocalizedDescriptions(json);
            LocalizedHeaders = Localizator.GetLocalizedHeaders(json);
            Id = json.Id;
            Tag = json.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

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

        private void OnPropertyChanged(string propertyName)
        {
            //TODO: Remove before Release !!!
#if DEBUG
            Debug.WriteLine(DateTime.Now);
            Debug.WriteLine($"Id: {Id}");
            Debug.WriteLine($"SystemState: {SystemState}");
            Debug.WriteLine($"UserState: {UserState}");
            Debug.WriteLine($"IsChecked: {IsChecked}");
            Debug.WriteLine(string.Empty);
#endif
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}