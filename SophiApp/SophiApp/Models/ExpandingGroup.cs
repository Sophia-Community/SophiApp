using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class ExpandingGroup : BaseTextedElement, IContainer
    {
        public ExpandingGroup(JsonDTO json) : base(json)
        {
            State = UIElementState.UNCHECKED;
        }

        public List<BaseTextedElement> ChildElements { get; set; } = new List<BaseTextedElement>();

        internal override void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            ChildElements.ForEach(element => element.SetLocalization(language));
        }
    }
}