using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class TextedElement : IElement
    {
        private bool isEnabled;
        private bool isChecked;
        private bool isClicked;
        private string description;
        private string header;

        internal TextedElement(JsonGuiDto dto)
        {
            Headers = dto.Header;
            Descriptions = dto.Description;
            Id = dto.Id;
            Tag = dto.Tag;
        }

        internal TextedElement(JsonGuiChildDto dto)
        {
            Headers = dto.ChildHeader;
            Descriptions = dto.ChildDescription;
            Id = dto.ChildId;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<TextedElement> StatusChanged;

        protected Dictionary<UILanguage, string> Descriptions { get; set; }
        public Dictionary<UILanguage, string> Headers { get; set; }

        public virtual bool IsEnabled
        {
            get => isEnabled;
            private set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public virtual bool IsChecked
        {
            get => isChecked;
            private set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
        }

        internal virtual ElementStatus Status
        {
            get => GetElementStatus();
            set
            {
                SetElementStatus(value);
                StatusChanged?.Invoke(null, this);
            }
        }

        private void SetElementStatus(ElementStatus value)
        {
            switch (value)
            {
                case ElementStatus.DISABLED:
                    IsEnabled = IsChecked = IsClicked = false;
                    break;

                case ElementStatus.CHECKED:
                    IsEnabled = IsChecked = true;
                    IsClicked = false;
                    break;

                case ElementStatus.UNCHECKED:
                    IsEnabled = true;
                    IsChecked = IsClicked = false;
                    break;

                case ElementStatus.SETTODEFAULT:
                    IsEnabled = IsClicked = true;
                    IsChecked = false;
                    break;

                case ElementStatus.SETTOACTIVE:
                    IsEnabled = IsChecked = IsClicked = true;
                    break;
                default:
                    break;
            }
        }

        private ElementStatus GetElementStatus()
        {
            if (IsEnabled & IsChecked & IsClicked == false)
                return ElementStatus.CHECKED;

            if (IsEnabled & IsChecked == false & IsClicked == false)
                return ElementStatus.UNCHECKED;

            if (IsEnabled & IsChecked == false & IsClicked)
                return ElementStatus.SETTODEFAULT;

            if (IsEnabled & IsChecked & IsChecked)
                return ElementStatus.SETTOACTIVE;

            return ElementStatus.DISABLED;
        }

        public virtual bool IsClicked
        {
            get => isClicked;
            private set
            {
                isClicked = value;
                OnPropertyChanged("IsClicked");
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

        internal Action<TextedElement, Exception> ErrorOccurred { get; set; }

        public Func<bool> CustomisationState { get; set; }

        internal Action<bool> CustomizeOs { get; set; }

        internal void GetCustomisation()
        {
            try
            {
                Status = CustomisationState.Invoke() ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
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

        public uint Id { get; }

        public string Tag { get; }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public virtual void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }
    }
}