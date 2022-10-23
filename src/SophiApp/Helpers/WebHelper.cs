using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

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

        internal static void Download(string url, string file, bool deleteIsExisting)
        {
            if (deleteIsExisting && File.Exists(file))
                File.Delete(file);

            Download(url, file);
        }

        internal static T GetJsonResponse<T>(string url)
        {
            var webRequest = WebRequest.CreateHttp(url);
            webRequest.UserAgent = AppHelper.UserAgent;
            var webResponse = webRequest.GetResponse();

            using (var dataStream = webResponse.GetResponseStream())
            {
                var reader = new StreamReader(dataStream);
                var parsedJson = JsonConvert.DeserializeObject<T>(reader.ReadToEnd());
                return parsedJson;
            }
        }

        internal static async Task<string> GetPostResponse(string uri, Dictionary<string, string> parameters)
        {
            using (var client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(parameters);
                var response = await client.PostAsync(uri, content);
                return await response.Content.ReadAsStringAsync();
            }
        }

        internal static XmlDocument GetXmlResponse(string url)
        {
            XmlDocument myXmlDocument = new XmlDocument();
            myXmlDocument.Load(url);
            return myXmlDocument;
        }
    }
}