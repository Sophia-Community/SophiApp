using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    [DataContract]
    public class JsonDTO
    {
        [DataMember(Name = "ArrowIsVisible")]
        public bool ArrowIsVisible { get; set; }

        [DataMember(Name = "ChildId")]
        public List<int> ChildId { get; set; }

        [DataMember(Name = "Descriptions")]
        public Dictionary<UILanguage, string> Descriptions { get; set; }

        [DataMember(Name = "HasParent")]
        public bool HasParent { get; set; }

        [DataMember(Name = "Headers")]
        public Dictionary<UILanguage, string> Headers { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "SelectOnce")]
        public bool SelectOnce { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }
    }
}