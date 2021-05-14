using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    internal enum UIType
    {
        [EnumMember]
        TextedElement = 0, // Element containing text: Switch, CheckBox, RadioButton, etc.

        [EnumMember]
        RadioButtonGroup = 1, // RadioButtonGroup container

        [EnumMember]
        ExpandingGroup = 2 // ExpandingGroup container
    }
}