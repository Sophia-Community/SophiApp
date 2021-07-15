using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class ExpandingGroup : TextedElement, IHasChilds
    {
        public ExpandingGroup(JsonGuiDto dto) : base(dto)
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