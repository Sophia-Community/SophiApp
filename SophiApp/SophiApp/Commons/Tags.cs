using System.Windows;

namespace SophiApp.Commons
{
    internal enum ConditionsTag
    {
        DefenderCorrupted,
        NewVersion,
        OsBuildVersion,
        OsFilesCorrupted,
        OsVersion,
        RebootRequired,
        SingleAdminSession,
        SingleInstance,
        SomethingWrong,
        SycnexScript,
        Win10Tweaker,
    }

    internal enum InfoPanelVisibility
    {
        HideAll,
        Loading,
        RestartNecessary
    }

    internal struct Tags
    {
        internal static readonly string ApplyingException = Application.Current.FindResource("Tags.View.ApplyingException") as string;
        internal static readonly string ViewContextMenu = Application.Current.FindResource("Tags.View.ContextMenu") as string;
        internal static readonly string ViewGames = Application.Current.FindResource("Tags.View.Games") as string;
        internal static readonly string ViewLoading = Application.Current.FindResource("Tags.View.Loading") as string;
        internal static readonly string ViewPersonalization = Application.Current.FindResource("Tags.View.Personalization") as string;
        internal static readonly string ViewPrivacy = Application.Current.FindResource("Tags.View.Privacy") as string;
        internal static readonly string ViewSearch = Application.Current.FindResource("Tags.View.Search") as string;
        internal static readonly string ViewSecurity = Application.Current.FindResource("Tags.View.Security") as string;
        internal static readonly string ViewSettings = Application.Current.FindResource("Tags.View.Settings") as string;
        internal static readonly string ViewStartMenu = Application.Current.FindResource("Tags.View.StartMenu") as string;
        internal static readonly string ViewSystem = Application.Current.FindResource("Tags.View.System") as string;
        internal static readonly string ViewTaskScheduler = Application.Current.FindResource("Tags.View.TaskScheduler") as string;
        internal static readonly string ViewUwpApps = Application.Current.FindResource("Tags.View.UwpApps") as string;
    }
}