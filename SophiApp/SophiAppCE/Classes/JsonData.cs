using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Classes
{
    public class List
    {
        public string SecurityEn { get; set; }

        public string SecurityRu { get; set; }

        public string BasicEn { get; set; }

        public string BasicRu { get; set; }

        public string ElevateEn { get; set; }

        public string ElevateRu { get; set; }

        public string NonElevateEn { get; set; }

        public string NonElevateRu { get; set; }

        public string AutoEn { get; set; }

        public string AutoRu { get; set; }

        public string AskEn { get; set; }

        public string AskRu { get; set; }
    }

    [DataContract]
    public class JsonData
    {
        [DataMember(Name = "Id")]
        public string Id { get; set; }

        [DataMember(Name = "Path")]
        public string Path { get; set; }

        [DataMember(Name = "HeaderEn")]
        public string HeaderEn { get; set; }

        [DataMember(Name = "HeaderRu")]
        public string HeaderRu { get; set; }

        [DataMember(Name = "DescriptionEn")]
        public string DescriptionEn { get; set; }

        [DataMember(Name = "DescriptionRu")]
        public string DescriptionRu { get; set; }

        [DataMember(Name = "Type")]
        public string Type { get; set; }

        [DataMember(Name = "Sha256")]
        public string Sha256 { get; set; }

        [DataMember(Name = "Tag")]
        public string Tag { get; set; }

        [DataMember(Name = "List")]
        public List<List> List { get; set; }

        [DataMember(Name = "DescriptionSecurityEn")]
        public string DescriptionSecurityEn { get; set; }

        [DataMember(Name = "DescriptionSecurityRu")]
        public string DescriptionSecurityRu { get; set; }

        [DataMember(Name = "DescriptionBasicEn")]
        public string DescriptionBasicEn { get; set; }

        [DataMember(Name = "DescriptionBasicRu")]
        public string DescriptionBasicRu { get; set; }
    }
}
