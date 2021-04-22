using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    internal enum UIType
    {
        [EnumMember]
        TextedElement, //Element containing text: Switch, CheckBox, RadioButton, etc.

        [EnumMember]
        Container //Element containing other elements:ExpandingList, RadioButtonsGroup, etc. Nesting containers into each other, for example a list into a list, is not possible.
    }
}