using System.Collections.Generic;

namespace SophiApp.Interfaces
{
    internal interface IItemsListModel
    {
        List<int> ChildIds { get; set; }

        bool SelectOnce { get; set; }
    }
}