using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

//using System.Xml;

namespace SophiApp.Helpers
{
    internal class Toaster
    {
        internal static void ShowUpdateToast()
        {
            var APP_ID = "windows.immersivecontrolpanel_cw5n1h2txyewy!microsoft.windows.immersivecontrolpanel";
            var xml = @"<toast duration=""Long"" scenario=""reminder"">
							<visual>
								<binding template=""ToastGeneric"">
									<text>$($Localization.TelegramTitle)</text>
									<group>
										<subgroup>
											<text hint-style=""body"" hint-wrap=""true"">https://t.me/sophia_chat</text>
										</subgroup>
									</group>
								</binding>
							</visual>
							<audio src=""ms-winsoundevent:notification.default"" />
							<actions>
								<action arguments=""https://t.me/sophia_chat"" content=""$($Localization.Open)"" activationType=""protocol""/>
								<action arguments=""dismiss"" content="""" activationType=""system""/>
							</actions>
						</toast>";
            var toastXml = new XmlDocument();
            toastXml.LoadXml(xml);
            var toast = new ToastNotification(toastXml);
            ToastNotificationManager.CreateToastNotifier(APP_ID).Show(toast);
        }
    }
}