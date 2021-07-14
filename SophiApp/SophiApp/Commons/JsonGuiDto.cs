using System.Collections.Generic;

namespace SophiApp.Commons
{
    internal class JsonGuiDto
    {
        public List<JsonGuiChildDto> ChildElements { get; set; }
        public Dictionary<UILanguage, string> Description { get; set; }
        public Dictionary<UILanguage, string> Header { get; set; }
        public uint Id { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; }
    }
}