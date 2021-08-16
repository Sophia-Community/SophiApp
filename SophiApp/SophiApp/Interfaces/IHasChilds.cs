using SophiApp.Models;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IHasChilds
    {
        List<TextedElement> ChildElements { get; set; }
    }
}