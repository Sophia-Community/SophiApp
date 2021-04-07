using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class ItemsListModel : IUIElementModel, IItemsListModel, INotifyPropertyChanged
    {
        private string description;
        private string header;
        private bool isChecked = default;
        private bool systemState = default;
        private bool userState = default;

        public ItemsListModel(JsonDTO json)
        {
            LocalizedHeaders = json.LocalizedHeaders;
            Id = json.Id;
            Tag = json.Tag;
            ChildIds = json.ChildIds;
            SelectOnce = json.SelectOnce;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> ChildIds { get; set; }
        public string Description { get; set; }

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

        public bool InContainer { get; set; }

        public bool IsChecked { get; set; }

        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }

        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }

        public bool SelectOnce { get; set; }
        public bool SystemState { get; set; }

        public string Tag { get; set; }

        public bool UserState { get; set; }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetLocalizationTo(UILanguage language)
        {
            Header = LocalizedHeaders[language];
        }

        public void SetSystemState()
        {
        }

        public void SetUserState()
        {
        }
    }
}