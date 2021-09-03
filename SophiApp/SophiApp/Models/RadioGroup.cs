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

        internal uint DefaultId { get; private set; }

        public List<TextedElement> ChildElements { get; set; }

        private void OnChildErrorOccured(TextedElement element, Exception e) => ErrorOccurred?.Invoke(this, new Exception($"Child with id {element.Id} caused an error: {e.Message}. Method caused an error: {e.TargetSite.DeclaringType.FullName}"));

        internal override void GetCustomisationStatus()
        {
            try
            {
                Status = base.CustomisationStatus.Invoke() ? ElementStatus.CHECKED : ElementStatus.UNCHECKED;
                ChildElements.ForEach(child => child.GetCustomisationStatus());
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(this, e);
            }
        }

        internal override void Init(Action<TextedElement, Exception> errorHandler, EventHandler<TextedElement> statusHandler,
                                                    UILanguage language, Func<bool> customisationStatus)
        {
            ErrorOccurred = errorHandler;
            StatusChanged += statusHandler;
            CustomisationStatus = customisationStatus;
            base.ChangeLanguage(language);
            ChildElements = ChildsDTO.Select(child => ElementsFabric.CreateChildElement(child, OnChildErrorOccured, statusHandler, language)).ToList();
            ChildElements.ForEach(child => (child as RadioButton).ParentId = Id);
            GetCustomisationStatus();
            SetDefaultId();
        }

        internal void SetDefaultId()
        {
            try
            {
                DefaultId = ChildElements.First(element => element.Status == ElementStatus.CHECKED).Id;
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