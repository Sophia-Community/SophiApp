using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;

namespace SophiApp.Commons
{
    internal class Parser
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
                                var jsonSerializer = new DataContractJsonSerializer(typeof(JsonDTO), new DataContractJsonSerializerSettings() { UseSimpleDictionaryFormat = true });
                                dto = (JsonDTO)jsonSerializer.ReadObject(memoryStream);
                            }

                            return dto;
                        });
        }
    }
}