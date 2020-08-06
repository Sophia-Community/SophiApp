using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Classes
{
    [DataContract]
    internal class JsonData
    {
        [DataMember]
        internal string Id { get; set; }

        [DataMember]
        internal string Path { get; set; }

        [DataMember]
        internal string HeaderEn { get; set; }

        [DataMember]
        internal string HeaderRu { get; set; }

        [DataMember]
        internal string DescriptionEn { get; set; }

        [DataMember]
        internal string DescriptionRu { get; set; }

        [DataMember]
        internal string Type { get; set; }

        [DataMember]
        internal string Sha256 { get; set; }

        [DataMember]
        internal string Tag { get; set; }        
    }
}
