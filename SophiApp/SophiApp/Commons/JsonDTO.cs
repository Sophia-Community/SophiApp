using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SophiApp.Commons
{
    [DataContract]
    internal class JsonDTO
    {
        [DataMember(Name = "ContainerId")]
        public uint ContainerId { get; set; }

        [DataMember(Name = "Descriptions")]
        public Dictionary<UILanguage, string> Descriptions { get; set; }

        [DataMember(Name = "Headers")]
        public Dictionary<UILanguage, string> Headers { get; set; }

        [DataMember(Name = "Id")]
        public uint Id { get; set; }

        [DataMember(Name = "IsContainer")]
        public bool IsContainer { get; set; }

        [DataMember(Name = "Model")]
        public string Model { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "Type")]
        public UIType Type { get; set; }
    }
}