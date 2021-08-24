using SophiApp.Commons;
using SophiApp.Helpers;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class ExpandingGroup : TextedElement, IParentElements
    {
        private readonly List<TextedChildDTO> ChildsDTO;

        public ExpandingGroup(TextedElementDTO dataObject) : base(dataObject)
        {
            ChildsDTO = dataObject.ChildElements;
        }

        public List<TextedElement> ChildElements { get; set; }

        internal override void GetCustomisationStatus() => ChildElements.ForEach(child => child.GetCustomisationStatus());

        internal override void Init(Action<TextedElement, Exception> errorHandler, EventHandler<TextedElement> statusHandler,
                                            UILanguage language, Func<bool> customisationStatus)
        {
            ErrorOccurred = errorHandler;
            StatusChanged += statusHandler;
            base.ChangeLanguage(language);
            ChildElements = ChildsDTO.Select(child => ElementsFabric.CreateChildElement(child, errorHandler, statusHandler, language)).ToList();
            Status = ElementStatus.UNCHECKED;
        }

        public override void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }
    }
}