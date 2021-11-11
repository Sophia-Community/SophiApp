using System.Net;

namespace SophiApp.Helpers
{
    internal class WebHelper
    {
        internal static void Download(string url, string file)
        {
            using (var client = new WebClient())
            {
                client.DownloadFile(url, file);
            }
        }
    }
}