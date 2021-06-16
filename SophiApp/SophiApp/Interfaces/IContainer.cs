using SophiApp.Models;
using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IContainer
    {
        List<BaseTextedElement> Collection { get; set; }
    }
}