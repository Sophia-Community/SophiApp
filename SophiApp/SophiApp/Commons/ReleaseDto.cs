using System.Collections.Generic;

namespace SophiApp.Commons
{
    internal class ReleaseDto
    {
        public Dictionary<string, string> assests { get; set; }

        public string assets_url { get; set; }

        public Dictionary<string, string> author { get; set; }

        public string body { get; set; }

        public string created_at { get; set; }

        public bool draft { get; set; }

        public string html_url { get; set; }

        public string id { get; set; }

        public string name { get; set; }

        public string node_id { get; set; }

        public bool prerelease { get; set; }

        public string published_at { get; set; }

        public string tag_name { get; set; }

        public string tarball_url { get; set; }

        public string target_commitish { get; set; }

        public string upload_url { get; set; }

        public string url { get; set; }

        public string zipball_url { get; set; }
    }
}