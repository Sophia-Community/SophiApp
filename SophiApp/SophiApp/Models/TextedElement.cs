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
        private ElementStatus status;

        public TextedElement((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler,
                                EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters)
        {
            CustomisationStatus = parameters.Customisation;
            Descriptions = parameters.Dto.Description ?? parameters.Dto.ChildDescription;
            ErrorOccurred = parameters.ErrorHandler;
            Headers = parameters.Dto.Header ?? parameters.Dto.ChildHeader;
            Id = parameters.Dto.Id;
            Language = parameters.Language;
            StatusChanged = parameters.StatusHandler;
            Tag = parameters.Dto.Tag;
            ViewId = parameters.Dto.ViewId;
            Windows10Supported = parameters.Dto.Windows10Supported;
            Windows11Supported = parameters.Dto.Windows11Supported;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public event EventHandler<TextedElement> StatusChanged;

        protected Dictionary<UILanguage, string> Descriptions { get; set; }

        internal Func<bool> CustomisationStatus { get; set; }

        internal Action<TextedElement, Exception> ErrorOccurred { get; set; }
        internal UILanguage Language { get; set; }
        internal bool Windows10Supported { get; private set; }
        internal bool Windows11Supported { get; private set; }

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

        public ElementStatus Status
        {
            get => status;
            set
            {
                status = value;
                OnPropertyChanged("Status");
                StatusChanged?.Invoke(null, this);
            }
        }

        public string Tag { get; }
        public uint ViewId { get; }

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        internal void ChangeStatus() => Status = Status == ElementStatus.UNCHECKED ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;

        internal virtual bool ContainsText(string text)
        {
            var desiredText = text.ToLower();
            return Header.ToLower().Contains(desiredText) || Description.ToLower().Contains(desiredText);
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

        internal virtual void Initialize()
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