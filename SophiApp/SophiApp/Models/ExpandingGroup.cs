using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class ExpandingGroup : TextedElement, IParentElements
    {
        public ExpandingGroup(TextedElementDTO dataObject) : base(dataObject)
        {
        }

        public List<TextedElement> ChildElements { get; set; }

        public override void ChangeLanguage(UILanguage language)
        {
            Header = Headers[language];
            Description = Descriptions[language];
            ChildElements.ForEach(child => child.ChangeLanguage(language));
        }
    }
}