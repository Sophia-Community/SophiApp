using SophiApp.Commons;

namespace SophiApp.Models
{
    internal class CheckBox : TextedElement
    {
        public CheckBox(JsonGuiDto dto) : base(dto)
        {
        }

        public CheckBox(JsonGuiChildDto dto) : base(dto)
        {

        }
    }
}