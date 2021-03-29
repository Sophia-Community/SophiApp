using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SophiApp.Commons
{
    class Parser
    {
        internal static IEnumerable<JsonDTO> ParseJson(byte[] jsonData)
        {
            var matchPattern = @"\n    {(.*?)\n    }";
            return Regex.Matches(Encoding.UTF8.GetString(jsonData), matchPattern, RegexOptions.Compiled | RegexOptions.Singleline)
                        .Cast<Match>()
                        .Select(match =>
                        {
                            var dto = new JsonDTO();

                            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(match.Value)))
                            {
                                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(JsonDTO));
                                dto = (JsonDTO)jsonSerializer.ReadObject(memoryStream);
                            }

                            return dto;
                        });
        }
    }
}
