using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    internal enum UIType
    {
        [EnumMember]
        TextedElement = 0, // The element containing text: Switch, CheckBox, RadioButton, and containers: RadioButtonGroup, ExpandingGroup, etc.
    }
}
