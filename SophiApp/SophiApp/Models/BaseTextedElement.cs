using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    //TODO: BaseTextedElement - deprecated !!!
    internal class BaseTextedElement : INotifyPropertyChanged
    {
        private string description;
        private string header;
        private bool isChecked;
        private bool isEnabled;

        internal Action<uint, Exception> ErrorOccurred;
        internal Action<uint, ElementStatus> StateChanged;
        public const string DescriptionPropertyName = "Description";
        public const string HeaderPropertyName = "Header";
        public const string IsCheckedPropertyName = "IsChecked";
        public const string IsEnabledPropertyName = "IsEnabled";

        public Func<bool> CurrentStateAction;
        public Action<bool> SystemStateAction;

        public BaseTextedElement()
        {
        }

        public BaseTextedElement(JsonDto json)
        {
            Id = json.Id;
            Descriptions = json.Description;
            Headers = json.Header;
            Tag = json.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        internal Dictionary<UILanguage, string> Descriptions { get; set; }
        internal Dictionary<UILanguage, string> Headers { get; set; }

        public uint ContainerId { get; set; }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                OnPropertyChanged(DescriptionPropertyName);
            }
        }

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

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(IsCheckedPropertyName);
            }
        }

        public bool IsClicked { get; set; }
        public bool IsContainer { get; set; }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged(IsEnabledPropertyName);
            }
        }

        public ElementStatus Status
        {
            get => GetElementStatus();
            set
            {
                SetElementStatus(value);
                StateChanged?.Invoke(Id, value);
            }
        }

        public string Tag { get; set; }

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

        internal void ChangeState()
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

        internal void GetCurrentState()
        {
            try
            {
                Status = CurrentStateAction?.Invoke() == true ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e);
            }
        }

        internal virtual void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }

        internal void SetSystemState()
        {
            try
            {
                SystemStateAction(Status == ElementStatus.SETTOACTIVE);
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e);
            }
        }
    }
}