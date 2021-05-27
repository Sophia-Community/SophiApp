using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    internal enum UIType
    {
        [EnumMember]
        TextedElement = 0, // The element containing text: Switch, CheckBox, RadioButton, and containers: RadioButtonGroup, ExpandingGroup, etc.
    }
}