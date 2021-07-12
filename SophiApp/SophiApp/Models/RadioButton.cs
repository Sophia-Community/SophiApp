using SophiApp.Commons;

namespace SophiApp.Models
{
    internal class RadioButton : BaseTextedElement
    {
        public RadioButton(JsonDTO json) : base(json)
        {
        }

        public RadioButton(RadioButtonJsonDTO radioButtonJsonDTO)
        {
            Id = radioButtonJsonDTO.ChildId;
            Descriptions = radioButtonJsonDTO.ChildDescription;
            Headers = radioButtonJsonDTO.ChildHeader;
            State = UIElementState.UNCHECKED;
        }
    }
}