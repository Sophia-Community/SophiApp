using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    internal enum UIType
    {
        [EnumMember]
        TextedElement = 0, //Element containing text: Switch, CheckBox, RadioButton, etc.

        [EnumMember]
        Container = 1 //Element containing other elements:ExpandingList, RadioButtonsList, etc. Nesting containers into each other, for example a list into a list, is not possible.
    }
}