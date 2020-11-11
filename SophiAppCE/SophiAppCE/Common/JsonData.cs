using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using SophiAppCE.Helpers;

namespace SophiAppCE.Common
{
    [DataContract]
    public class LocalizedHeader
    {
        [DataMember(Name = "RU")]
        public string RU { get; set; }

        [DataMember(Name = "EN")]
        public string EN { get; set; }
    }

    [DataContract]
    public class LocalizedDescription
    {
        [DataMember(Name = "RU")]
        public string RU { get; set; }

        [DataMember(Name = "EN")]
        public string EN { get; set; }
    }

    [DataContract]
    public class JsonData
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "LocalizedHeader")]
        public LocalizedHeader LocalizedHeader { get; set; }

        [DataMember(Name = "LocalizedDescription")]        
        public LocalizedDescription LocalizedDescription { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }
    }
}
