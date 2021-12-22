using System.Windows;

namespace SophiApp.Commons
{
    internal struct LocalizedText
    {
        internal static readonly string CleanupTaskDescription = Application.Current.FindResource("Localization.CleanupTask.Description") as string;
        internal static readonly string NotificationTaskDescription = Application.Current.FindResource("Localization.NotificationTask.Description") as string;
        internal static readonly string NotificationTaskTitle = Application.Current.FindResource("Localization.NotificationTask.Title") as string;
        internal static readonly string NotificationTaskEventTitle = Application.Current.FindResource("Localization.NotificationTask.EventTitle") as string;
        internal static readonly string NotificationTaskEvent = Application.Current.FindResource("Localization.NotificationTask.Event") as string;
        internal static readonly string NotificationTaskSnoozeInterval = Application.Current.FindResource("Localization.NotificationTask.SnoozeInterval") as string;
        internal static readonly string Minute = Application.Current.FindResource("Localization.Time.Minute") as string;
        internal static readonly string HalfHour = Application.Current.FindResource("Localization.Time.HalfHour") as string;
        internal static readonly string FourHours = Application.Current.FindResource("Localization.Time.FourHours") as string;
        internal static readonly string Run = Application.Current.FindResource("Localization.Run") as string;
    }
}