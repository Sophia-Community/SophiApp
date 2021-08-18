using SophiApp.Models;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IParentElements
    {
        List<TextedElement> ChildElements { get; set; }
    }
}