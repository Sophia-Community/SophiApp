using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SophiAppCE.Common
{
    [DataContract]
    public class Header
    {
        [DataMember(Name = "RU")]
        public string RU { get; set; }

        [DataMember(Name = "EN")]
        public string EN { get; set; }
    }

    [DataContract]
    public class Description
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

        [DataMember(Name = "Header")]
        public Header Header { get; set; }

        [DataMember(Name = "Description")]
        public Description Description { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }
    }
}
