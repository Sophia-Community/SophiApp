using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class ExpandingGroup : BaseTextedElement, IContainer
    {
        public ExpandingGroup(JsonDTO json) : base(json)
        {
        }

        public List<BaseTextedElement> Collection { get; set; } = new List<BaseTextedElement>();

        internal override void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            Collection.ForEach(element => element.SetLocalization(language));
        }
    }
}