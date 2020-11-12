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
                switch (json.Type)
                {
                    case nameof(ControlsType.SwitchBar):
                        Dictionary<Language, string> localizedHeader = new Dictionary<Language, string> { { Language.RU, json.LocalizedHeader.RU }, { Language.EN, json.LocalizedHeader.EN } };
                        Dictionary<Language, string> localizedDescription = new Dictionary<Language, string> { { Language.RU, json.LocalizedDescription.RU }, { Language.EN, json.LocalizedDescription.EN } };

                        ControlModel model = new ControlModel
                        {
                            Id = json.Id,
                            Tag = json.Tag,
                            Type = ControlsType.SwitchBar,
                            LocalizedHeader = localizedHeader,
                            LocalizedDescription = localizedDescription,
                            Header = localizedHeader[language],
                            Description = localizedDescription[language]
                        };
                        
                        yield return model;
                        break;                    
                }
            }
        }
    }
}
