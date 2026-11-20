// <copyright file="HttpService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using SophiApp.Contracts.Services;
    using SophiApp.Extensions;
    using SophiApp.Helpers;
    using System.Diagnostics;
    using System.Xml;

    /// <inheritdoc/>
    public class HttpService : IHttpService
    {
        // The time a server has to start answering a download
        private const double DownloadStartTimeout = 100;
        private const double AvailabilityTimeout = 5;

        // One client for the whole app: it keeps the connections and reuses them.
        // The connections are renewed from time to time, because the app changes DNS servers and restarts network adapters.
        // Every request has a timeout of its own, so the client has none
        private static readonly HttpClient Client = new(new SocketsHttpHandler { PooledConnectionLifetime = TimeSpan.FromMinutes(2) })
        {
            Timeout = Timeout.InfiniteTimeSpan,
        };

        /// <inheritdoc/>
        public void DownloadFile(string url, string saveTo)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(saveTo)!);
            Download(url, saveTo);
        }

        /// <inheritdoc/>
        public void DownloadOneDrive(string saveTo)
        {
            App.Status.SetStep("Panel_Step_GettingDownloadLink");
            var xml = new XmlDocument();
            xml.LoadXml(GetString("https://g.live.com/1rewlive5skydrive/OneDriveProductionV2", DownloadStartTimeout));

            // The feed has several installer links: take the second one, or the only one
            var binaries = xml.DocumentElement?.SelectNodes("update/amd64binary");
            var binary = binaries?.Count > 1 ? binaries[1] : binaries?[0];
            var url = binary?.Attributes?.GetNamedItem("url")?.InnerText ?? string.Empty;
            Download(url, saveTo);
        }

        /// <inheritdoc/>
        public T GetFromJson<T>(string url, double timeout)
            where T : class
            => Json.ToObject<T>(GetString(url, timeout));

        /// <inheritdoc/>
        public async Task<T> GetFromJsonAsync<T>(string url, double timeout)
            where T : class
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = await Client.SendAsync(request, cancellation.Token);
            var content = await response.Content.ReadAsStringAsync(cancellation.Token);
            var result = await Json.ToObjectAsync<T>(content);
            return result;
        }

        /// <inheritdoc/>
        public void OpenUrl(string url)
        {
            ExplorerHelper.Open(url);
            App.Logger.LogOpenedUrl(url);
        }

        /// <inheritdoc/>
        public bool UrlIsAvailable(string url)
        {
            try
            {
                using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(AvailabilityTimeout));
                using var request = new HttpRequestMessage(HttpMethod.Head, url);
                using var response = Client.Send(request, cancellation.Token);
                App.Logger.LogUrlIsAvailable(url, response.IsSuccessStatusCode);
                return response.IsSuccessStatusCode;
            }
            catch
            {
                App.Logger.LogUrlIsAvailable(url, false);
                return false;
            }
        }

        // The whole answer has to come within the timeout
        private static string GetString(string url, double timeout)
        {
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(timeout));
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = Client.Send(request, cancellation.Token);
            using var reader = new StreamReader(response.Content.ReadAsStream(cancellation.Token));
            return reader.ReadToEnd();
        }

        private static void Download(string url, string saveTo)
        {
            App.Status.SetStep("Panel_Step_Downloading");

            // The timeout is for the start of the answer only: a large file is downloaded as long as it takes
            using var cancellation = new CancellationTokenSource(TimeSpan.FromSeconds(DownloadStartTimeout));
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            using var response = Client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellation.Token);
            cancellation.CancelAfter(Timeout.InfiniteTimeSpan);
            _ = response.EnsureSuccessStatusCode();
            var totalBytes = response.Content.Headers.ContentLength ?? 0;
            using var urlStream = response.Content.ReadAsStream();
            using var fileStream = new FileStream(saveTo, FileMode.Create);
            var buffer = new byte[81920];
            var downloadedBytes = 0L;
            var shownPercent = 0;
            int read;

            while ((read = urlStream.Read(buffer, 0, buffer.Length)) > 0)
            {
                fileStream.Write(buffer, 0, read);
                downloadedBytes += read;

                // The percent is shown in steps of five, and only if the server tells the file size
                var percent = totalBytes > 0 ? (int)(downloadedBytes * 100 / totalBytes / 5 * 5) : 0;

                if (percent > shownPercent)
                {
                    shownPercent = percent;
                    App.Status.SetStep("Panel_Step_DownloadingPercent", percent);
                }
            }
        }
    }
}
