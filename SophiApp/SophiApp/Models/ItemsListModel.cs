using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class ItemsListModel : IUIElementModel, IItemsListModel, INotifyPropertyChanged
    {
        private string header;

        public ItemsListModel(JsonDTO json)
        {
            Headers = json.Headers;
            Id = json.Id;
            Tag = json.Tag;
            ChildId = json.ChildId;
            SelectOnce = json.SelectOnce;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public List<int> ChildId { get; set; }
        public string Description { get; set; }

        public Dictionary<UILanguage, string> Descriptions { get; set; }

        public bool HasParent { get; set; }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        public Dictionary<UILanguage, string> Headers { get; set; }
        public int Id { get; set; }
        public bool IsChecked { get; set; }
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
            Header = Headers[language];
        }

        public void SetSystemState()
        {
        }

        public void SetUserState()
        {
        }
    }
}