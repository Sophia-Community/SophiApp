using SophiAppCE.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class Parser
    {
        internal static IEnumerable<JsonData> ParseJson()
        {
            return Regex.Matches(Encoding.UTF8.GetString(Properties.Resources.ControlsData), @"\{\r\n(.*?)\""\r\n  },", RegexOptions.Compiled | RegexOptions.Singleline)
                        .Cast<Match>()
                        .Select(m =>
                        {
                            JsonData json = new JsonData();

                            using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(m.Value)))
                            {
                                DataContractJsonSerializer jsonSerializer = new DataContractJsonSerializer(typeof(JsonData));
                                json = (JsonData)jsonSerializer.ReadObject(memoryStream);
                            }

                            return json;
                        });
        }
    }
}
