using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioButtonsGroup : BaseTextedElement, IContainer
    {
        public RadioButtonsGroup(JsonDTO json) : base(json)
        {
            State = UIElementState.UNCHECKED;
            ChildElements = json.ChildElements?.Select(rb => new RadioButton(rb) as BaseTextedElement).ToList();
        }

        internal uint DefaultSelectedId { get; private set; } = default;

        internal bool IsSelected { get; set; } = false;

        public List<BaseTextedElement> ChildElements { get; set; } = new List<BaseTextedElement>();

        internal void SetDefaultSelectedId()
        {
            try
            {
                DefaultSelectedId = ChildElements.First(element => element.State == UIElementState.CHECKED).Id;
            }
            catch (Exception e)
            {
                ErrorOccurred?.Invoke(Id, e);
            }
        }

        internal override void SetLocalization(UILanguage language)
        {
            Header = Headers[language];
            ChildElements.ForEach(element => element.SetLocalization(language));
        }
    }
}