using SophiApp.Commons;
using SophiApp.Interfaces;

namespace SophiApp.Models
{
    internal class RadioButtonsGroup : BaseTextedElement, IContainer
    {
        public RadioButtonsGroup(JsonDTO json) : base(json)
        {
        }

        void IContainer.SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            Collection.ForEach(element => element.SetLocalization(language));
        }
    }
}