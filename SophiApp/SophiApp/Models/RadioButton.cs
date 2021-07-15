using SophiApp.Commons;
using SophiApp.Interfaces;
using System.Collections.Generic;

namespace SophiApp.Models
{
    internal class RadioButton : TextedElement
    {
        public RadioButton(JsonGuiDto dto) : base(dto)
        {
        }

        public RadioButton(JsonGuiChildDto dto) : base(dto)
        {
        }
    }
}