using SophiApp.Commons;

namespace SophiApp.Models
{
    internal class RadioButton : TextedElement
    {
        public RadioButton(TextedElementDTO dataObject) : base(dataObject)
        {
        }

        public RadioButton(TextedChildDTO dataObject) : base(dataObject)
        {
        }

        internal uint Parent { get; set; }
    }
}