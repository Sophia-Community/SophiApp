using SophiApp.Models;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    //TODO: IContainer - deprecated !!!
    internal interface IContainer
    {
        List<BaseTextedElement> ChildElements { get; set; }
    }
}