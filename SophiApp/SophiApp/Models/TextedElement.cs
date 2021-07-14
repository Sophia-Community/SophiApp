using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class TextedElement : IElement
    {
        private string description;
        private string header;

        public TextedElement(JsonGuiDto dto)
        {
            Headers = dto.Header;
            Descriptions = dto.Description;
            Id = dto.Id;
            Tag = dto.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private Dictionary<UILanguage, string> Descriptions { get; }
        private Dictionary<UILanguage, string> Headers { get; }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged("Description");
            }
        }

        public Action<Exception> ErrorOccurred { get; set; }

        public string Header
        {
            get => header;
            set
            {
                header = value;
                OnPropertyChanged("Header");
            }
        }

        public uint Id { get; }

        public string Tag { get; }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }
    }
}