using System.Windows;

namespace SophiAppCE.Managers
{
    struct TagManager
    {
        internal static readonly string Privacy = Application.Current.TryFindResource("Tag.Privacy") as string;
        internal static readonly string Ui  = Application.Current.TryFindResource("Tag.Ui") as string;
        internal static readonly string ContextMenu = Application.Current.TryFindResource("Tag.ContextMenu") as string;
        internal static readonly string StartMenu = Application.Current.TryFindResource("Tag.StartMenu") as string;
        internal static readonly string System = Application.Current.TryFindResource("Tag.System") as string;
        internal static readonly string TaskSheduler = Application.Current.TryFindResource("Tag.TaskSheduler") as string;
        internal static readonly string Security = Application.Current.TryFindResource("Tag.Security") as string;
        internal static readonly string Game = Application.Current.TryFindResource("Tag.Game") as string;
        internal static readonly string Uwp = Application.Current.TryFindResource("Tag.Uwp") as string;
        internal static readonly string OneDrive = Application.Current.TryFindResource("Tag.OneDrive") as string;
        internal static readonly string Loading = Application.Current.TryFindResource("Tag.Loading") as string;
    }
}
