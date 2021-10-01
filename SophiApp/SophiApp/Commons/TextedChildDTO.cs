using System.Collections.Generic;

namespace SophiApp.Commons
{
    internal class TextedChildDto
    {
        public Dictionary<UILanguage, string> ChildDescription { get; set; }
        public Dictionary<UILanguage, string> ChildHeader { get; set; }
        public uint Id { get; set; }
        public string Type { get; set; }
    }
}