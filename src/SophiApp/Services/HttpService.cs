// <copyright file="HttpService.cs" company="Team Sophia">
// Copyright (c) Team Sophia. All rights reserved.
// </copyright>

namespace SophiApp.Services
{
    using System.Text.RegularExpressions;
    using SophiApp.Contracts.Services;

    /// <inheritdoc/>
    public class HttpService : IHttpService
    {
        private readonly Regex hrefPattern = new (@"(?inx)
<a \s [^>]*
    href \s* = \s*
        (?<q> ['""] )
            (?<url> [^""]+ )
        \k<q>
[^>]* >");

        #pragma warning disable S1075 // URIs should not be hardcoded

        /// <inheritdoc/>
        public async Task DownloadHEVCAppxAsync(string fileName)
        {
            var content = new List<KeyValuePair<string, string>>
            {
                new ("type", "url"), new ("url", "https://apps.microsoft.com/detail/9N4WGH0Z6VHQ"), new ("ring", "Retail"), new ("lang", "en-US"),
            };
            using var client = new HttpClient();
            using var request = new HttpRequestMessage(HttpMethod.Post, "https://store.rg-adguard.net/api/GetFiles");
            request.Content = new FormUrlEncodedContent(content);
            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var appxLink = hrefPattern.Matches(result).Last().Value.Replace("<a href=\"", null).Replace("\" rel=\"noreferrer\">", null);
            using var stream = await client.GetStreamAsync(appxLink);
            using var file = File.Create(fileName);
            await stream.CopyToAsync(file);
        }
    }
        #pragma warning restore S1075 // URIs should not be hardcoded
}
