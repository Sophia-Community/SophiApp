using SophiApp.Commons;

namespace SophiApp.Helpers
{
    internal class TextHelper
    {
        internal static string LocalizeCleanupTaskToast(string cleanupTaskToast)
        {
            var toast = cleanupTaskToast.Split('\n');
            toast[6] = toast[6].Replace("<text></text>", $"<text>{LocalizedText.NotificationTaskTitle}</text>");
            toast[9] = toast[9].Replace("hint-wrap=\"true\"></text>", $"hint-wrap=\"true\">{LocalizedText.NotificationTaskEventTitle}</text>");
            toast[14] = toast[14].Replace("hint-wrap=\"true\"></text>", $"hint-wrap=\"true\">{LocalizedText.NotificationTaskEvent}</text>");
            toast[21] = toast[21].Replace("\"\"", $"\"{LocalizedText.NotificationTaskSnoozeInterval}\"");
            toast[22] = toast[22].Replace("\"\"", $"\"{LocalizedText.Minute}\"");
            toast[23] = toast[23].Replace("\"\"", $"\"{LocalizedText.HalfHour}\"");
            toast[24] = toast[24].Replace("\"\"", $"\"{LocalizedText.FourHours}\"");
            toast[27] = toast[27].Replace("\"\"", $"\"{LocalizedText.Run}\"");
            return string.Join("", toast);
        }
    }
}