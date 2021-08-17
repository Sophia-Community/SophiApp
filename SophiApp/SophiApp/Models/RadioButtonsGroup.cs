using SophiApp.Commons;
using SophiApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SophiApp.Models
{
    internal class RadioButtonsGroup : TextedElement, IHasChilds
    {
        public RadioButtonsGroup(JsonGuiDto dto) : base(dto)
        {
        }

        internal uint DefaultSelectedId { get; private set; } = default;
        internal bool IsSelected { get; set; } = false;
        public List<TextedElement> ChildElements { get; set; }

        internal void OnChildStatusChanged(object sender, TextedElement e)
        {
            ChildElements.ForEach(child => child.Status = child.Id == e.Id ? ElementStatus.CHECKED : ElementStatus.UNCHECKED);
        }

        internal void SetDefaultSelectedId()
        {
            try
            {
                DefaultSelectedId = ChildElements.First(element => element.Status == ElementStatus.CHECKED).Id;
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