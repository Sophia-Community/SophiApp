using System.Windows;

namespace SophiApp.Helpers
{
    internal class ClipboardHelper
    {
        internal static void CopyText(string text) => Clipboard.SetText(text);
    }
}