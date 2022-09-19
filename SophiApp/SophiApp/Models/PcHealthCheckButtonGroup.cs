using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class PcHealthCheckButtonGroup : TextedElement, IParentElements
    {
        public PcHealthCheckButtonGroup((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler, EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters) : base(parameters)
        {
            ChildsDTO = parameters.Dto.ChildElements;
        }

        public List<TextedElement> ChildElements { get; set; } = new List<TextedElement>();
        public List<TextedElementDto> ChildsDTO { get; set; }

        internal override bool ContainsText(string text)
        {
            if (base.ContainsText(text))
                return true;

            var desiredText = text.ToLower();
            return ChildElements.Any(child => child.Header.ToLower().Contains(desiredText) || child.Description.ToLower().Contains(desiredText));
        }

        internal override void GetCustomisationStatus()
        {
            try
            {
                base.GetCustomisationStatus();
                ChildElements.ForEach(child => child.GetCustomisationStatus());
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
            }
        }

        public override void ChangeLanguage(UILanguage language)
        {
            base.ChangeLanguage(language);
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }

        public void OnChildErrorOccured(TextedElement element, Exception e) => ErrorOccurred?.Invoke(this, e);
    }
}