using System.Windows;

namespace SophiApp.Helpers
{
    internal class TextHelper
    {
        private static readonly char delimiter = '\n';
        private static readonly char placeholder = '*';

        internal static string LocalizeCleanupTaskToast(string cleanupTaskToast)
        {
            var toast = cleanupTaskToast.Split(delimiter);
            toast[6] = toast[6].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.CleanupTask.NotificationTask.Title")}");
            toast[9] = toast[9].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.CleanupTask.NotificationTask.EventTitle")}");
            toast[16] = toast[16].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.CleanupTask.NotificationTask.Run")}");
            return string.Join("", toast);
        }

        internal static string LocalizeClearTempTaskToast(string clearTempTaskToast)
        {
            var toast = clearTempTaskToast.Split(delimiter);
            toast[7] = toast[7].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.Toast.Title.Notificaton")}");
            toast[10] = toast[10].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.ClearTempTask.Event")}");
            return string.Join("", toast);
        }

        internal static string LocalizeEventViewerCustomXml(string eventViewerCustomXml)
        {
            var securityString = "*[System[(EventID=4688)]]";
            var xml = eventViewerCustomXml.Split(delimiter);
            xml[6] = xml[6].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.EventViewer.CustomView.Name")}");
            xml[7] = xml[7].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.EventViewer.CustomView.Description")}");
            xml[10] = xml[10].Replace($"{placeholder}", securityString);
            return string.Join("", xml);
        }

        internal static string LocalizeSoftwareDistributionTaskToast(string softwareDistributionToast)
        {
            var toast = softwareDistributionToast.Split(delimiter);
            toast[8] = toast[8].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.Toast.Title.Notificaton")}");
            toast[11] = toast[11].Replace($"{placeholder}", $"{Application.Current.FindResource("Localization.SoftwareDistributionTask.Event")}");
            return string.Join("", toast);
        }
    }
}