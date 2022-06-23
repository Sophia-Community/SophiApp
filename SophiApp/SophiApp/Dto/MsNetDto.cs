using Newtonsoft.Json;
using System;

namespace SophiApp.Dto
{
    internal class MsNetDto
    {
        [JsonProperty("latest-release")]
        public Version LatestRelease { get; set; }
    }
}