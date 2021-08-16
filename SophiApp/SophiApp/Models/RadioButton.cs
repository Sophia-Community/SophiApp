using SophiApp.Commons;

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