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
        private bool isChecked;
        private bool isClicked;
        private bool isEnabled;

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
            Id = dto.Id;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TextedElement> StatusChanged;

        protected Dictionary<UILanguage, string> Descriptions { get; set; }
        internal Action<bool?> CustomizeOs { get; set; }
        internal Action<TextedElement, Exception> ErrorOccurred { get; set; }

        internal virtual ElementStatus Status
        {
            get => GetElementStatus();
            set
            {
                SetElementStatus(value);
                StatusChanged?.Invoke(null, this);
            }
        }

        public Func<bool?> CustomisationStatus { get; set; }

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

        public Dictionary<UILanguage, string> Headers { get; set; }

        public uint Id { get; }

        public virtual bool IsChecked
        {
            get => isChecked;
            private set
            {
                isChecked = value;
                OnPropertyChanged("IsChecked");
            }
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

        public virtual bool IsEnabled
        {
            get => isEnabled;
            private set
            {
                isEnabled = value;
                OnPropertyChanged("IsEnabled");
            }
        }

        public string Tag { get; }

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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

        internal void ChangeStatus()
        {
            switch (Status)
            {
                case ElementStatus.CHECKED:
                    Status = ElementStatus.SETTODEFAULT;
                    break;

                case ElementStatus.UNCHECKED:
                    Status = ElementStatus.SETTOACTIVE;
                    break;

                case ElementStatus.SETTODEFAULT:
                    Status = ElementStatus.CHECKED;
                    break;

                case ElementStatus.SETTOACTIVE:
                    Status = ElementStatus.UNCHECKED;
                    break;
            }
        }

        internal virtual void GetCustomisationStatus()
        {
            try
            {
                Status = CustomisationStatus?.Invoke() == true ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
            }
        }

        public virtual void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }
    }
}