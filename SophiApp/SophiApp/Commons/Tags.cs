using System.Windows;

namespace SophiApp.Commons
{
    internal struct Tags
    {
        internal static readonly string ViewContextMenu = Application.Current.FindResource("Tags.View.ContextMenu") as string;
        internal static readonly string ViewGames = Application.Current.FindResource("Tags.View.Games") as string;
        internal static readonly string ViewLoading = Application.Current.FindResource("Tags.View.Loading") as string;
        internal static readonly string ViewPersonalization = Application.Current.FindResource("Tags.View.Personalization") as string;
        internal static readonly string ViewPrivacy = Application.Current.FindResource("Tags.View.Privacy") as string;
        internal static readonly string ViewSecurity = Application.Current.FindResource("Tags.View.Security") as string;
        internal static readonly string ViewSettings = Application.Current.FindResource("Tags.View.Settings") as string;
        internal static readonly string ViewStartMenu = Application.Current.FindResource("Tags.View.StartMenu") as string;
        internal static readonly string ViewSystem = Application.Current.FindResource("Tags.View.System") as string;
        internal static readonly string ViewTaskSheduler = Application.Current.FindResource("Tags.View.TaskSheduler") as string;
        internal static readonly string ViewUwpApps = Application.Current.FindResource("Tags.View.UwpApps") as string;
        internal static readonly string ConditionOSBitness = Application.Current.FindResource("Tags.Conditions.OSBitness") as string;
        internal static readonly string ConditionSomethingWrong = Application.Current.FindResource("Tags.Conditions.SomethingWrong") as string;
        internal static readonly string ConditionOsBuildVersion = Application.Current.FindResource("Tags.Conditions.OsBuildVersion") as string;
    }
}