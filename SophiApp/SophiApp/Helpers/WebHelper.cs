using Newtonsoft.Json;
using System.IO;
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

        internal static T GetJsonRequest<T>(string url, T dto)
        {
            var request = WebRequest.CreateHttp(url);
            request.UserAgent = AppHelper.UserAgent;
            var response = request.GetResponse();

            using (var dataStream = response.GetResponseStream())
            {
                var reader = new StreamReader(dataStream);
                var parsedJson = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                return parsedJson;
            }
        }
    }
}