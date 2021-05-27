using SophiApp.Commons;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace SophiApp.Models
{
    internal class BaseTextedElement : INotifyPropertyChanged
    {
        private string description;
        private string header;
        private bool isChecked;
        private bool isEnabled;

        public const string DescriptionPropertyName = "Description";
        public const string HeaderPropertyName = "Header";
        public const string IsCheckedPropertyName = "IsChecked";
        public const string IsEnabledPropertyName = "IsEnabled";

        public Func<bool> CurrentStateAction;
        public Action<bool> SystemStateAction;

        public BaseTextedElement(JsonDTO json)
        {
            ContainerId = json.ContainerId;
            Id = json.Id;
            Descriptions = json.Descriptions;
            Headers = json.Headers;
            Tag = json.Tag;
            IsContainer = json.IsContainer;
        }

        public delegate void TextedElementErrorOccurred(uint id, string target, string message);

        public delegate void TextedElementStateHandler(uint id, UIElementState state);

        public event TextedElementErrorOccurred ErrorOccurred;

        public event PropertyChangedEventHandler PropertyChanged;

        public event TextedElementStateHandler StateChanged;

        internal Dictionary<UILanguage, string> Descriptions { get; set; }
        internal Dictionary<UILanguage, string> Headers { get; set; }
        public List<BaseTextedElement> Collection { get; set; } = new List<BaseTextedElement>();
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

        public UIElementState State
        {
            get => GetState();
            set
            {
                SetState(value);
                StateChanged?.Invoke(Id, value);
            }
        }

        public string Tag { get; set; }

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

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

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

        internal void GetCurrentState()
        {
            try
            {
                State = CurrentStateAction?.Invoke() == true ? UIElementState.CHECKED : UIElementState.UNCHECKED;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e.TargetSite.Name, e.Message);
            }
        }

        internal void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }

        internal void SetSystemState(bool state)
        {
            try
            {
                SystemStateAction(state);
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e.TargetSite.Name, e.Message);
            }
        }
    }
}