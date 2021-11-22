using SophiApp.Commons;
using SophiApp.Dto;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioGroup : TextedElement, IParentElements
    {
        public RadioGroup((TextedElementDto Dto, Action<TextedElement, Exception> ErrorHandler, EventHandler<TextedElement> StatusHandler, Func<bool> Customisation, UILanguage Language) parameters) : base(parameters)
        {
            ChildsDTO = parameters.Dto.ChildElements;
        }

        internal uint DefaultId { get; private set; }

        public List<TextedElement> ChildElements { get; set; } = new List<TextedElement>();
        public List<TextedElementDto> ChildsDTO { get; set; }

        internal override void GetCustomisationStatus()
        {
            try
            {
                base.GetCustomisationStatus();
                ChildElements.ForEach(child => child.GetCustomisationStatus());
                DefaultId = ChildElements.First(element => element.Status == ElementStatus.CHECKED).Id;
            }
            catch (Exception e)
            {
                ChildElements.ForEach(child => child.Status = ElementStatus.DISABLED);
                ErrorOccurred?.Invoke(this, e);
            }
        }

        public override void ChangeLanguage(UILanguage language)
        {
            base.ChangeLanguage(language);
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }

        public void OnChildErrorOccured(TextedElement element, Exception e) => ErrorOccurred?.Invoke(this, new Exception($"Child with id {element.Id} caused an error: {e.Message}. Method caused an error: {e.TargetSite.DeclaringType.FullName}"));
    }
}