using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class AdvancedRadioGroup : RadioGroup, IParentElements
    {
        private readonly List<TextedChildDto> ChildsDTO;

        public AdvancedRadioGroup(TextedElementDto dataObject) : base(dataObject)
        {
        }
    }
}