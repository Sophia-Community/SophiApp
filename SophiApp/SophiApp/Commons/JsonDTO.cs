using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    [DataContract]
    public class LocalizedHeaders
    {
        [DataMember(Name = "RU")]
        public string RU { get; set; }

        [DataMember(Name = "EN")]
        public string EN { get; set; }
    }

    [DataContract]
    public class LocalizedDescriptions
    {
        [DataMember(Name = "RU")]
        public string RU { get; set; }

        [DataMember(Name = "EN")]
        public string EN { get; set; }
    }

    [DataContract]
    public class JsonDTO
    {
        [DataMember(Name = "Id")]
        public int Id { get; set; }

        [DataMember(Name = "LocalizedHeaders")]
        public LocalizedHeaders LocalizedHeaders { get; set; }

        [DataMember(Name = "LocalizedDescriptions")]
        public LocalizedDescriptions LocalizedDescriptions { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }
    }
}
