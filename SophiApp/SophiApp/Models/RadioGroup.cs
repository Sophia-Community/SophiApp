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
        private readonly List<TextedChildDto> ChildsDTO;

        public RadioGroup(TextedElementDto dataObject) : base(dataObject)
        {
            ChildsDTO = dataObject.ChildElements;
        }

        internal uint DefaultId { get; private set; }

        public List<TextedElement> ChildElements { get; set; }

        private void OnChildErrorOccured(TextedElement element, Exception e) => ErrorOccurred?.Invoke(this, new Exception($"Child with id {element.Id} caused an error: {e.Message}. Method caused an error: {e.TargetSite.DeclaringType.FullName}"));

        internal override void GetCustomisationStatus()
        {
            try
            {
                Status = CustomisationStatus.Invoke() ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
                ChildElements.ForEach(child => child.GetCustomisationStatus());
                DefaultId = ChildElements.First(element => element.Status == ElementStatus.CHECKED).Id;
            }
            catch (Exception e)
            {
                ChildElements.ForEach(child => child.Status = ElementStatus.UNCHECKED);
                ErrorOccurred?.Invoke(this, e);
            }
        }

        internal override void Init(Action<TextedElement, Exception> errorHandler, EventHandler<TextedElement> statusHandler,
                                                    UILanguage language, Func<bool> customisationStatus)
        {
            ErrorOccurred = errorHandler;
            StatusChanged += statusHandler;
            CustomisationStatus = customisationStatus;
            ChildElements = ChildsDTO.Select(child => FabricHelper.GetTextedElementChild(child, OnChildErrorOccured, statusHandler, language)).ToList();
            ChildElements.ForEach(child => (child as RadioButton).ParentId = Id);
            ChangeLanguage(language);
            GetCustomisationStatus();
        }

        public override void ChangeLanguage(UILanguage language)
        {
            base.ChangeLanguage(language);
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }
    }
}