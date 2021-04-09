using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IItemsListModel
    {
        List<int> ChildId { get; set; }

        bool SelectOnce { get; set; }
    }
}