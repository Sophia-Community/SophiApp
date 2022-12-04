using System.Windows;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace SophiApp.Helpers
{
    internal class ToastHelper
    {
        internal static void ShowUpdateToast(string currentVersion, string newVersion)
        {
            var header = Application.Current.FindResource("Localization.Toast.Update.Header");
            var download = Application.Current.FindResource("Localization.Toast.Update.Download");
            var xml = $"<toast duration=\"Long\" scenario=\"reminder\">\r\n\t\t\t\t<visual>\r\n\t\t\t\t\t<binding template=\"ToastGeneric\">\r\n\t\t\t\t\t\t<text>{header}</text>\r\n\t\t\t\t\t\t<group>\r\n\t\t\t\t\t\t\t<subgroup>\r\n\t\t\t\t\t\t\t\t<text hint-style=\"body\" hint-wrap=\"true\">{currentVersion} {'\u2192'} {newVersion}</text>\r\n\t\t\t\t\t\t\t</subgroup>\r\n\t\t\t\t\t\t</group>\r\n\t\t\t\t\t</binding>\r\n\t\t\t\t</visual>\r\n\t\t\t\t<audio src=\"ms-winsoundevent:notification.default\" />\r\n\t\t\t\t<actions>\r\n\t\t\t\t\t<action arguments=\"{AppHelper.GitHubReleasesPage}\" content=\"{download}\" activationType=\"protocol\"/>\r\n\t\t\t\t\t<action arguments=\"dismiss\" content=\"\" activationType=\"system\"/>\r\n\t\t\t\t</actions>\r\n\t\t\t</toast>";
            var toastXml = new XmlDocument();
            toastXml.LoadXml(xml);
            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier("Sophia").Show(toast);
        }
    }
}