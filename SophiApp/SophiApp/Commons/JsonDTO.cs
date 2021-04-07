using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    [DataContract]
    public class JsonDTO
    {
        [DataMember(Name = "ChildIds")]
        public List<int> ChildIds { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "InContainer")]
        public bool InContainer { get; set; }

        [DataMember(Name = "LocalizedDescriptions")]
        public Dictionary<UILanguage, string> LocalizedDescriptions { get; set; }

        [DataMember(Name = "LocalizedHeaders")]
        public Dictionary<UILanguage, string> LocalizedHeaders { get; set; }

        [DataMember(Name = "SelectOnce")]
        public bool SelectOnce { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }
    }
}