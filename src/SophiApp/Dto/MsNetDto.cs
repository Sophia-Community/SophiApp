using Newtonsoft.Json;
using System;

namespace SophiApp.Dto
{
    internal class MsNetDto
    {
        [JsonProperty("latest-release")]
        public Version LatestRelease { get; set; }

        [JsonProperty("releases")]
        public MsNetRelease[] Releases { get; set; }
    }

    internal class MsNetRelease
    {
        [JsonProperty("release-date")]
        public DateTime ReleaseDate { get; set; }

        [JsonProperty("release-version")]
        public string ReleaseVersion { get; set; }

        [JsonProperty("windowsdesktop")]
        public MsNetWindowsDesktop WindowsDesktop { get; set; }
    }

    internal class MsNetWindowsDesktop
    {
        [JsonProperty("files")]
        public MsNetWindowsDesktopFiles[] Files { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("version-display")]
        public string VersionDisplay { get; set; }
    }

    internal class MsNetWindowsDesktopFiles
    {
        [JsonProperty("hash")]
        public string Hash { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("rid")]
        public string Rid { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}