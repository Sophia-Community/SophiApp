using SophiApp.Commons;
using SophiApp.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Models
{
    class BaseElement : INotifyPropertyChanged
    {
        private string header;
        private string description;
        private bool isChecked;
        private bool isEnabled;

        public const string DescriptionPropertyName = "Description";
        public const string HeaderPropertyName = "Header";
        public const string IsEnabledPropertyName = "IsEnabled";
        public const string IsCheckedPropertyName = "IsChecked";

        private Dictionary<UILanguage, string> Descriptions { get; set; }
        private Dictionary<UILanguage, string> Headers { get; set; }

        public Func<bool> CurrentStateInvoke;

        public uint ContainerId { get; set; }

        public uint Id { get; set; }

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

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnPropertyChanged(IsCheckedPropertyName);
            }
        }

        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                isEnabled = value;
                OnPropertyChanged(IsEnabledPropertyName);
            }
        }

        public bool IsClicked { get; set; }

        public string Model { get; set; }

        public UIElementState State
        {
            get => GetState();
            set => SetState(value);
        }

        private void SetState(UIElementState value)
        {
            switch (value)
            {
                case UIElementState.DISABLED:
                    IsEnabled = IsChecked = IsClicked = false;
                    break;
                case UIElementState.CHECKED:
                    IsEnabled = IsChecked = true;
                    IsClicked = false;
                    break;
                case UIElementState.UNCHECKED:
                    IsEnabled = true;
                    IsChecked = IsClicked = false;
                    break;
                case UIElementState.SETTODEFAULT:
                    IsEnabled = IsClicked = true;
                    IsChecked = false;
                    break;
                case UIElementState.SETTOACTIVE:
                    IsEnabled = IsChecked = IsClicked = true;
                    break;
            }
        }

        private UIElementState GetState()
        {
            if (IsEnabled & IsChecked & IsClicked == false)
                return UIElementState.CHECKED;

            if (IsEnabled & IsChecked == false & IsClicked == false)
                return UIElementState.UNCHECKED;

            if (IsEnabled & IsChecked == false & IsClicked)
                return UIElementState.SETTODEFAULT;

            if (IsEnabled & IsChecked & IsChecked)
                return UIElementState.SETTOACTIVE;

            return UIElementState.DISABLED;
        }

        public string Tag { get; set; }

        public BaseElement(JsonDTO json)
        {
            ContainerId = json.ContainerId;
            Id = json.Id;
            Descriptions = json.Descriptions;
            Headers = json.Headers;
            Model = json.Model;
            Tag = json.Tag;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }

        internal void ChangeState()
        {
            switch (State)
            {                
                case UIElementState.CHECKED:
                    State = UIElementState.SETTODEFAULT;
                    break;
                case UIElementState.UNCHECKED:
                    State = UIElementState.SETTOACTIVE;
                    break;
                case UIElementState.SETTODEFAULT:
                    State = UIElementState.CHECKED;
                    break;
                case UIElementState.SETTOACTIVE:
                    State = UIElementState.UNCHECKED;
                    break;
            }

        }

    }
}
