using Newtonsoft.Json;

namespace SophiApp.Dto
{
    internal class WinTerminalSettingsDto
    {
        [JsonProperty("profiles")]
        public Profiles Profiles { get; set; }
    }

    public class Profiles
    {
        [JsonProperty("defaults")]
        public Defaults Defaults { get; set; }
    }

    public class Defaults
    {
        [JsonProperty("elevate")]
        public bool Elevate { get; set; }
    }
}