using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
