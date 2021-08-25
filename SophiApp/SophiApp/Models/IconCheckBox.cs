using SophiApp.Commons;

namespace SophiApp.Models
{
    internal class IconCheckBox : TextedElement
    {
        private string IconName;

        public IconCheckBox(TextedElementDTO dataObject) : base(dataObject)
        {
            Icon = dataObject.Icon;
        }

        public IconCheckBox(TextedChildDTO dataObject) : base(dataObject)
        {
        }

        public string Icon { get; }

    }
}