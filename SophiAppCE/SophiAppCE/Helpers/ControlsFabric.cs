using SophiAppCE.Common;
using SophiAppCE.Controls;
using SophiAppCE.Models;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SophiAppCE.Helpers
{
    internal static class ControlsFabric
    {
        internal static IEnumerable<ControlModel> Create(IEnumerable<JsonData> jsonData, Language language)
        {
            foreach (JsonData json in jsonData)
            {
                Dictionary<Language, string> localizedHeader = new Dictionary<Language, string> { { Language.RU, json.LocalizedHeader.RU }, { Language.EN, json.LocalizedHeader.EN } };
                Dictionary<Language, string> localizedDescription = new Dictionary<Language, string> { { Language.RU, json.LocalizedDescription.RU }, { Language.EN, json.LocalizedDescription.EN } };

                yield return new ControlModel
                {
                    Action = ControlModelActions.SetAction(json.Tag, json.Id),
                    Id = json.Id,
                    Tag = json.Tag,
                    Type = SetType(json.Type),
                    LocalizedHeader = localizedHeader,
                    LocalizedDescription = localizedDescription,
                    Header = localizedHeader[language],
                    Description = localizedDescription[language],
                    State = ControlModelStates.GetState(json.Id)
                };
            }
        }

        private static ControlsType SetType(string type)
        {
            return (ControlsType)Enum.Parse(typeof(ControlsType), type);
        }
    }
}
