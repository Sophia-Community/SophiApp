using SophiApp.Models;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IContainer
    {
        List<BaseTextedElement> ChildElements { get; set; }
    }
}