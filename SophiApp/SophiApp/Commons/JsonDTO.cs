using System.Collections.Generic;

namespace SophiApp.Commons
{
    //[DataContract]
    internal class JsonDTO
    {
        public List<RadioButtonJsonDTO> ChildElements { get; set; }

        //[DataMember(Name = "Descriptions")]
        public Dictionary<UILanguage, string> Description { get; set; }

        public bool HasChild { get; set; }

        //[DataMember(Name = "Headers")]
        public Dictionary<UILanguage, string> Header { get; set; }

        //[DataMember(Name = "ContainerId")]
        public uint Id { get; set; }

        //[DataMember(Name = "Id")]
        //public uint Id { get; set; }

        //[DataMember(Name = "IsContainer")]
        //public bool IsContainer { get; set; }

        //[DataMember(Name = "Model")]
        public string Model { get; set; }

        //[DataMember(Name = "Tag")]
        public string Tag { get; set; }

        //[DataMember(Name = "Type")]
        public UIType Type { get; set; }
    }

    internal class RadioButtonJsonDTO
    {
        public Dictionary<UILanguage, string> ChildDescription { get; set; }
        public Dictionary<UILanguage, string> ChildHeader { get; set; }
        public uint ChildId { get; set; }
        public string Model { get; set; }
    }
}