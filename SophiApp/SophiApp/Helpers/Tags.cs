using System.Windows;

namespace SophiApp.Helpers
{
    internal struct Tags
    {
        internal static readonly string ContextMenu = Application.Current.FindResource("Tags.ContextMenu") as string;
        internal static readonly string Games = Application.Current.FindResource("Tags.Games") as string;
        internal static readonly string Personalization = Application.Current.FindResource("Tags.Personalization") as string;
        internal static readonly string Privacy = Application.Current.FindResource("Tags.Privacy") as string;
        internal static readonly string Security = Application.Current.FindResource("Tags.Security") as string;
        internal static readonly string StartMenu = Application.Current.FindResource("Tags.StartMenu") as string;
        internal static readonly string System = Application.Current.FindResource("Tags.System") as string;
        internal static readonly string TaskSheduler = Application.Current.FindResource("Tags.TaskSheduler") as string;
        internal static readonly string UwpApps = Application.Current.FindResource("Tags.UwpApps") as string;
    }
}