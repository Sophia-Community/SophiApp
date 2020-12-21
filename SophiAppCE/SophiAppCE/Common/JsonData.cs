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
        public ushort Id { get; set; }

        [DataMember(Name = "LocalizedHeader")]
        public LocalizedHeader LocalizedHeader { get; set; }

        [DataMember(Name = "LocalizedDescription")]        
        public LocalizedDescription LocalizedDescription { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "StateClass")]
        public string StateClass { get; set; }

        [DataMember(Name = "StateMethod")]
        public string StateMethod { get; set; }

        [DataMember(Name = "StateArg")]
        public string StateArg { get; set; }
    }
}
