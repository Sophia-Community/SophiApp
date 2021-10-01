using SophiApp.Commons;

namespace SophiApp.Models
{
    internal class RadioButton : TextedElement
    {
        public RadioButton(TextedElementDto dataObject) : base(dataObject)
        {
        }

        public RadioButton(TextedChildDto dataObject) : base(dataObject)
        {
        }

        internal uint ParentId { get; set; }
    }
}