using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    [DataContract]
    public class JsonDTO
    {
        [DataMember(Name = "CheckBoxIsVisible")]
        public bool CheckBoxIsVisible { get; set; }

        [DataMember(Name = "Childrens")]
        public List<JsonDTO> Childrens { get; set; }

        [DataMember(Name = "ExpanderIsVisible")]
        public bool ExpanderIsVisible { get; set; }

        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "LocalizedDescriptions")]
        public LocalizedDescriptions LocalizedDescriptions { get; set; }

        [DataMember(Name = "LocalizedHeaders")]
        public LocalizedHeaders LocalizedHeaders { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }
    }

    [DataContract]
    public class LocalizedDescriptions
    {
        [DataMember(Name = "EN")]
        public string EN { get; set; }

        [DataMember(Name = "RU")]
        public string RU { get; set; }
    }

    [DataContract]
    public class LocalizedHeaders
    {
        [DataMember(Name = "EN")]
        public string EN { get; set; }

        [DataMember(Name = "RU")]
        public string RU { get; set; }
    }
}