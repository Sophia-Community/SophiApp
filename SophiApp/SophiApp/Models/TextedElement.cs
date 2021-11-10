using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace SophiApp.Models
{
    internal class TextedElement : IElement
    {
        private string description;
        private string header;
        private bool isChecked;
        private bool isClicked;
        private bool isEnabled;

        public TextedElement((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler,
                                EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters)
        {
            Headers = parameters.Dto.Header ?? parameters.Dto.ChildHeader;
            Descriptions = parameters.Dto.Description ?? parameters.Dto.ChildDescription;
            Id = parameters.Dto.Id;
            Tag = parameters.Dto.Tag;
            ErrorOccurred = parameters.ErrorHandler;
            StatusChanged = parameters.StatusHandler;
            Language = parameters.Language;
            CustomisationStatus = parameters.Customisation;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TextedElement> StatusChanged;

        protected Dictionary<UILanguage, string> Descriptions { get; set; }
        internal Func<bool> CustomisationStatus { get; set; }
        internal Action<TextedElement, Exception> ErrorOccurred { get; set; }
        internal UILanguage Language { get; set; }

        internal ElementStatus Status
        {
            get => GetStatus();
            set
            {
                SetStatus(value);
                StatusChanged?.Invoke(null, this);
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

        private ElementStatus GetStatus()
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

        private void SetStatus(ElementStatus status)
        {
            switch (status)
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
                Status = CustomisationStatus.Invoke() ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
            }
        }

        internal virtual void Init()
        {
            var stopwatch = Stopwatch.StartNew();
            ChangeLanguage(Language);
            GetCustomisationStatus();
            stopwatch.Stop();
            DebugHelper.TextedElementInit(Id, stopwatch.Elapsed.TotalSeconds);
        }

        public virtual void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
        }
    }
}