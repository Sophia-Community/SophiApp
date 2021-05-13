using SophiApp.Commons;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class BaseContainer : INotifyPropertyChanged
    {
        private string header;
        public const string HeaderPropertyName = "Header";        

        public BaseContainer(JsonDTO json)
        {
            Id = json.Id;
            Headers = json.Headers;
            Model = json.Model;
            Tag = json.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<UILanguage, string> Headers { get; set; }
        public List<BaseTextedElement> Collection { get; set; } = new List<BaseTextedElement>();

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged(HeaderPropertyName);
            }
        }

        public uint Id { get; set; }

        public string Model { get; set; }

        public string Tag { get; set; }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void SetLocalization(UILanguage language) => Header = Headers[language];
    }
}