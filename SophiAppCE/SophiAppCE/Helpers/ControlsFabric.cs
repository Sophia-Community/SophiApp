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
        internal static IEnumerable<ControlModel> CreateAll(IEnumerable<JsonData> jsonData, LanguageFamily language)
        {
            foreach (JsonData json in jsonData)
            {
                Dictionary<LanguageFamily, string> localizedHeader = new Dictionary<LanguageFamily, string> { { LanguageFamily.RU, json.LocalizedHeader.RU }, { LanguageFamily.EN, json.LocalizedHeader.EN } };
                Dictionary<LanguageFamily, string> localizedDescription = new Dictionary<LanguageFamily, string> { { LanguageFamily.RU, json.LocalizedDescription.RU }, { LanguageFamily.EN, json.LocalizedDescription.EN } };

                yield return new ControlModel
                {
                    //TODO: Одинаковые заголовки страниц !!!
                    //HACK: Delete Before Release !!!

                    //Action = ControlModelActions.SetAction(json.Tag, json.Id),
                    Id = json.Id,
                    Tag = json.Tag,
                    Type = SetType(json.Type),
                    LocalizedHeader = localizedHeader,
                    LocalizedDescription = localizedDescription,
                    Header = localizedHeader[language],
                    Description = localizedDescription[language]
                    //State = ControlModelStates.GetState(json.Id)
                };
            }
        }

        private static ControlsType SetType(string type)
        {
            return (ControlsType)Enum.Parse(typeof(ControlsType), type);
        }
    }
}
