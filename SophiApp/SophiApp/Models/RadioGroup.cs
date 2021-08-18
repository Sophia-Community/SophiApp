using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioGroup : TextedElement, IParentElements
    {
        private readonly List<TextedChildDTO> ChildsDTO;

        public RadioGroup(TextedElementDTO dataObject) : base(dataObject)
        {
            ChildsDTO = dataObject.ChildElements;
        }

        internal uint DefaultSelected { get; private set; }
        internal bool IsSelected { get; set; } = false;
        public List<TextedElement> ChildElements { get; set; }

        internal override void Init(Action<TextedElement, Exception> errorHandler, EventHandler<TextedElement> statusHandler, UILanguage language, Func<bool> customisationStatus)
        {
            ChildElements = ChildsDTO.Select(child => ElementsFabric.CreateChildElement(child, errorHandler, statusHandler, language)).ToList();
            ChildElements.ForEach(child => (child as RadioButton).Parent = Id);
            ErrorOccurred = errorHandler;
            StatusChanged += statusHandler;
            base.Init(errorHandler, statusHandler, language, customisationStatus);
            SetDefaultSelected();
        }

        internal void SetDefaultSelected()
        {
            try
            {
                DefaultSelected = ChildElements.First(element => element.Status == ElementStatus.CHECKED).Id;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
            }
        }

        public override void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }
    }
}