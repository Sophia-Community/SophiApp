using SophiApp.Commons;
using System.Collections.Generic;

namespace SophiApp.Dto
{
    internal class TextedElementDto
    {
        public Dictionary<UILanguage, string> ChildDescription { get; set; }
        public List<TextedElementDto> ChildElements { get; set; }
        public Dictionary<UILanguage, string> ChildHeader { get; set; }
        public Dictionary<UILanguage, string> Description { get; set; }
        public Dictionary<UILanguage, string> Header { get; set; }
        public uint Id { get; set; }
        public string Tag { get; set; }
        public string Type { get; set; }
    }
}