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
    class ItemsListModel : IUIElementModel, INotifyPropertyChanged
    {
        private string description;
        private string header;        
        private bool systemState = default;
        private bool userState = default;

        public ItemsListModel(JsonDTO json)
        {
            Childrens = json.Childrens.Select(dto => Fabric.CreateElementModel(dto)).ToList();
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

        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }
        public List<IUIElementModel> Childrens { get; set; }

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

        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }

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
            SystemState = true;
        }

        public void SetUserState()
        {
            UserState = !UserState;
        }
    }
}
