using System;
using System.Net.Http;

namespace SophiApp.Helpers
{
    internal class HttpHelper
    {
        private const string PROBE_URL = "https://www.google.com";
        private static readonly HttpClient client = new HttpClient();

        internal static bool IsOnline()
        {
            bool result;
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