using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IItemsListModel
    {
        bool ArrowIsVisible { get; set; }
        List<int> ChildId { get; set; }
        bool SelectOnce { get; set; }
    }
}