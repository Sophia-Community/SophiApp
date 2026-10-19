// <copyright file="HttpService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using System.Diagnostics;
    using System.Xml;

    /// <inheritdoc/>
    public class HttpService : IHttpService
    {
        /// <inheritdoc/>
        public void DownloadFile(string url, string saveTo)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveTo) !);
            using var client = new HttpClient();
            using var urlStream = client.GetStreamAsync(url).Result;
            using var fileStream = new FileStream(saveTo, FileMode.Create);
            urlStream.CopyTo(fileStream);
        }

        /// <inheritdoc/>
        public void DownloadOneDrive(string saveTo)
        {
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://g.live.com/1rewlive5skydrive/OneDriveProductionV2");
            using var response = client.SendAsync(request).Result;
            var result = response.Content.ReadAsStringAsync().Result;
            var xml = new XmlDocument();
            xml.LoadXml(result);
            var url = xml?.DocumentElement?.SelectSingleNode("update/amd64binary")?.Attributes?.GetNamedItem("url")?.InnerText ?? string.Empty;
            using var urlStream = client.GetStreamAsync(url).Result;
            using var fileStream = new FileStream(saveTo, FileMode.Create);
            urlStream.CopyTo(fileStream);
        }

        /// <inheritdoc/>
        public async Task<T> GetFromJsonAsync<T>(string url, double timeout)
            where T : class
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(timeout);
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await client.SendAsync(request);
            var content = await response.Content.ReadAsStringAsync();
            var result = await Json.ToObjectAsync<T>(content);
            return result;
        }

        /// <inheritdoc/>
        public void OpenUrl(string url)
        {
            Process.Start("explorer.exe", url);
            App.Logger.LogOpenedUrl(url);
        }

        /// <inheritdoc/>
        public bool UrlIsAvailable(string url)
        {
            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = client.Send(request);
                App.Logger.LogUrlIsAvailable(url, response.IsSuccessStatusCode);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                App.Logger.LogUrlIsAvailable(url, false);
                return false;
            }
        }
    }
}
