using System;
using System.Net.Http;

namespace SophiApp.Helpers
{
    internal class HttpHelper
    {
        private const string PROBE_URL = "https://www.google.com";
        private static bool online = isOnline();
        internal static bool IsOnline { get => online; }

        private static bool isOnline()
        {
            bool result;
            var client = new HttpClient();

            client.DefaultRequestHeaders.ConnectionClose = true;

            try
            {
                using (var response = client.GetAsync(PROBE_URL).Result)
                {
                    result = response.StatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch (Exception)
            {
                result = false;
            }

            return result;
        }
    }
}