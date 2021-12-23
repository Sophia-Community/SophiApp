using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class ExpandingGroup : TextedElement, IParentElements
    {
        public ExpandingGroup((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler, EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters) : base(parameters)
        {
            ChildsDTO = parameters.Dto.ChildElements;
        }

        public List<TextedElement> ChildElements { get; set; } = new List<TextedElement>();
        public List<TextedElementDto> ChildsDTO { get; set; }

        public override void ChangeLanguage(UILanguage language)
        {
            base.ChangeLanguage(language);
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }

        public void OnChildErrorOccured(TextedElement child, Exception e) => base.ErrorOccurred(child, e);

        internal override void GetCustomisationStatus()
        {
            try
            {
                Status = CustomisationStatus.Invoke() ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
                ChildElements.ForEach(child => child.GetCustomisationStatus());
            }
            catch (Exception e)
            {
                ChildElements.ForEach(child => child.Status = ElementStatus.UNCHECKED);
                ErrorOccurred?.Invoke(this, e);
            }
        }
    }
}